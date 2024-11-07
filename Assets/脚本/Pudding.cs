using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;
namespace Assets
{
    public class Pudding : MonoBehaviour, IDamageable, ITargetable
    {
        public GameObject goldPrefab;
        public GameObject potionPrefab;
        public int damageAmount = 10;
        public int health = 30;
        public float detectionRadius = 15f;
        public float moveSpeed = 3.5f;
        public float hateDecayRate = 0.1f;
        public float patrolRadius = 5f;
        public float patrolWaitTime = 2f;

        private GameObject target;
        private float hateValue;
        private Vector3 patrolTarget;
        private float waitTimer;
        private bool isWaiting;
        private Animator animator;

        public enum EnemyMovementDirection
        {
            Up,
            Down,
            Left,
            Right
        }

        private EnemyMovementDirection currentMovementDirection;
        private Renderer enemyRenderer;
        private Color originalColor;
        private ItemManager itemManager;
        private Rigidbody2D rb;

        void Start()
        {
            animator = GetComponent<Animator>();
            SetRandomPatrolTarget();
            enemyRenderer = GetComponent<Renderer>();
            if (enemyRenderer != null)
            {
                originalColor = enemyRenderer.material.color;
            }
            rb = GetComponent<Rigidbody2D>();
            GameObject itemManagerObject = GameObject.FindWithTag("ItemManager");
            itemManager = itemManagerObject.GetComponent<ItemManager>();
        }

        void Update()
        {
            GameObject closestPlayer = FindClosestPlayer();
            if (closestPlayer != null && (target == null || hateValue <= 0))
            {
                SetTarget(closestPlayer);
            }

            ManagePatrolAndChase();
            UpdateAnimation();
        }

        private GameObject FindClosestPlayer()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            GameObject closestPlayer = null;
            float closestDistance = detectionRadius;

            foreach (GameObject player in players)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                if (distanceToPlayer < closestDistance)
                {
                    closestPlayer = player;
                    closestDistance = distanceToPlayer;
                }
            }

            return closestPlayer;
        }

        public void SetTarget(GameObject newTarget)
        {
            target = newTarget;
            hateValue = 10f; // 重置仇恨值
        }

        public GameObject GetTarget()
        {
            return target;
        }

        private void ManagePatrolAndChase()
        {
            if (target == null || hateValue <= 0)
            {
                if (isWaiting)
                {
                    waitTimer -= Time.deltaTime;
                    if (waitTimer <= 0)
                    {
                        isWaiting = false;
                    }
                }
                else
                {
                    Patrol();
                }

                if (hateValue > 0)
                {
                    hateValue -= hateDecayRate * Time.deltaTime;
                }
            }
            else
            {
                ChaseTarget();
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && target == null) // 只在没有目标的情况下设置
            {
                SetTarget(other.gameObject);
                Player player = target.GetComponent<Player>();
                if (player != null)
                {
                    AudioSource enemyAudioSource = GetComponent<AudioSource>();
                    if (enemyAudioSource != null && enemyAudioSource.clip != null)
                    {
                        enemyAudioSource.Play();
                    }

                    player.ChangeHealth(-damageAmount,player. transform.position, gameObject);
                }
            }
        }

        void UpdateAnimation()
        {
            if (health > 0) // 确保敌人未死亡时才更新动画
            {
                if (target != null && hateValue > 0)
                {
                    animator.SetBool("IsChasing", true);
                    animator.SetInteger("MovementDirection", (int)currentMovementDirection);
                }
                else
                {
                    animator.SetBool("IsChasing", false);
                }
            }
            else
            {
                animator.SetBool("IsChasing", false); // 确保死亡时动画停止
            }
        }

        public void TakeDamage(int damage)
        {
            if (health <= 0) return; 

            health -= damage;
            if (health <= 0)
            {
                Die();
                return; // 直接返回，避免后续处理死后状态
            }

            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }

            ApplyKnockback();
            StartCoroutine(FlashRed());
        }

        private void ApplyKnockback()
        {
            Vector2 knockbackDirection = (target != null)
                ? (transform.position - target.transform.position).normalized
                : Vector2.up;

            if (rb != null)
            {
                rb.AddForce(knockbackDirection * 2f, ForceMode2D.Impulse);
            }
        }

        private IEnumerator FlashRed()
        {
            if (enemyRenderer != null)
            {
                enemyRenderer.material.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                enemyRenderer.material.color = originalColor;
            }
        }

        private void Die()
        {
            int randomDrop = Random.Range(0, 100);
            if (randomDrop < 30)
            {
                itemManager.PickupItem(ItemType.GoldCoin);
            }
            else if (randomDrop < 100)
            {
                itemManager.PickupItem(ItemType.HealthPotion);
            }

            if (animator != null)
            {
                animator.SetTrigger("Die");
                Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
            }
           
        }

        void ChaseTarget()
        {
            if (target != null)
            {
                Vector3 directionToTarget = target.transform.position - transform.position;

                if (Mathf.Abs(directionToTarget.x) > Mathf.Abs(directionToTarget.y))
                {
                    currentMovementDirection = directionToTarget.x > 0 ? EnemyMovementDirection.Right : EnemyMovementDirection.Left;
                }
                else
                {
                    currentMovementDirection = directionToTarget.y > 0 ? EnemyMovementDirection.Up : EnemyMovementDirection.Down;
                }
                transform.Translate(directionToTarget.normalized * moveSpeed * Time.deltaTime, Space.World);
            }
        }

        void Patrol()
        {
            if ((transform.position - patrolTarget).magnitude < 0.1f)
            {
                isWaiting = true;
                waitTimer = patrolWaitTime;
                SetRandomPatrolTarget();
            }
            else
            {
                Vector3 directionToPatrolTarget = patrolTarget - transform.position;
                if (Mathf.Abs(directionToPatrolTarget.x) > Mathf.Abs(directionToPatrolTarget.y))
                {
                    currentMovementDirection = directionToPatrolTarget.x > 0 ? EnemyMovementDirection.Right : EnemyMovementDirection.Left;
                }
                else
                {
                    currentMovementDirection = directionToPatrolTarget.y > 0 ? EnemyMovementDirection.Up : EnemyMovementDirection.Down;
                }
                transform.Translate(directionToPatrolTarget.normalized * moveSpeed * Time.deltaTime, Space.World);
            }
        }

        void SetRandomPatrolTarget()
        {
            float randomAngle = Random.Range(0, 2 * Mathf.PI);
            patrolTarget = transform.position + new Vector3(Mathf.Sin(randomAngle), Mathf.Cos(randomAngle), 0) * patrolRadius;
        }
    }
}