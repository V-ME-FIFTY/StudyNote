using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDeath, IAttack, IInteractable, IJumpable
{
 
    public bool IsUsingStoneAxe { get; set; } 
    public bool IsUsingHealingPotion { get; set; }
    public bool IsUsingStonePickaxe { get; set; }

    [Header("Player State")]
    
    public bool isDead = false;
    public bool isMoving = false;
    public bool isAttacking = false;
    public bool isInteracting = false;
       private bool isJumping = false;
    [Header("Player Stats")]
    public float MoveSpeed = 2f;public  static int maxHealth = 100;
    public int currentHealth = maxHealth;
    public float RotateSpeed = 10f; 
    public float jumpForce = 5f; // 基础跳跃力度
    public float maxJumpTime = 1f; // 最大跳跃时间
    private float jumpTime = 0f; // 当前跳跃时间
     public float AttackInterval = 0.5f; 
    public float pickupRange = 2f; // 拾取范围
    public bool canBePickedUp = false; // 是否可以被拾取
    [Header("Player Components")]
    private Rigidbody rb; 
    public ItemManager itemManager; 
    private Animator animator;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Renderer playerRenderer;
    private Color originalColor;
    public void ChangeHealth(int damage,Vector2 position, GameObject BeingAttacked)
    {
        if (currentHealth <= 0) return; // 阻止死后的伤害处理

        currentHealth -= damage;
        if (currentHealth<= 0)
        {
            Death();
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

       
        StartCoroutine(FlashRed());
    }

   

    public void Death()
    {
        if (!isDead)
        {
       
            Destroy(gameObject); 
            isDead = false;
        }
    }

    public void Attack()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.CompareTag("Destructible") && !isAttacking) 
            {
                StartCoroutine(PerformAttack());
            }
        }
    }

    public void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); // 获取水平输入
        float moveVertical = Input.GetAxis("Vertical"); // 获取垂直输入

        Vector3 movement = new Vector2(moveHorizontal, moveVertical).normalized; // 计算移动向量并归一化

        // 使用 Translate 方法移动角色
        transform.Translate(movement * MoveSpeed * Time.deltaTime, Space.World);

        // 额外的旋转逻辑，根据移动方向调整角色朝向
        if (movement.magnitude > 0)
        {
            // 取反移动向量
            Vector3 flippedDirection = -movement;

            Quaternion targetRotation = Quaternion.LookRotation(flippedDirection); // 使用翻转后的方向创建目标旋转
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotateSpeed * Time.deltaTime); // 平滑旋转
        }

    }

    public void Jump()
    {
        if (isJumping) return; // 如果正在跳跃，防止多次调用
        isJumping = true; // 标记角色为正在跳跃
        float JumpMultiplier = jumpTime / maxJumpTime;
        Vector2 jumpVector = new Vector2(0, jumpForce * JumpMultiplier); // 计算跳跃力度
        rb.AddForce(jumpVector, ForceMode.Impulse); // 施加跳跃力度
        isJumping = false; // 跳跃结束，恢复状态
    }

    public void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.CompareTag("Interactable")) // 检查标签
                {
                    hit.collider.GetComponent<IInteractable>().Interact();
                }
            }
        }
    }

    public void TryPickup()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange); // 检测范围内的物体
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Pickupable")) // 检查标签
            {
                // 拿到物品组件并进行拾取
                PickupItem pickupItem = collider.GetComponent<PickupItem>();
                if (pickupItem != null && canBePickedUp ==true) // 确保获取到了PickupItem组件
                {
                    // 这里可以调用 ItemManager 或自己的方法来处理拾取逻辑
                  
                  itemManager.PickupItem(pickupItem.itemType); // 假设你有这个方法来处理物品的逻辑
                   
                    Destroy(collider.gameObject); // 拾取后销毁物品对象
                  canBePickedUp = false; // 标记物品可以被拾取
                }
            }
        }
    }

    void Start()
    {
        originalColor = playerRenderer.material.color;
        rb = GetComponent<Rigidbody>();
        itemManager = FindObjectOfType<ItemManager>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!isDead) 
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                TryPickup();
            }
            if (Input.GetMouseButton(0))
            {
                Attack();
            }
            if (Input.GetKeyDown(KeyCode.Space) && !isJumping) // 按下空格键且不在跳跃状态
            {
                jumpTime = 0f; // 重置跳跃时间
                Jump(); // 直接调用跳跃
            }

           
            if (isJumping)
            {
                if (Input.GetKey(KeyCode.Space) && jumpTime < maxJumpTime)
                {
                    jumpTime += Time.deltaTime; 
                }

                // 当释放跳跃键或者达到最大跳跃时间时，执行跳跃
                if (Input.GetKeyUp(KeyCode.Space) || jumpTime >= maxJumpTime)
                {
                    Jump();
                }
            }

            Move(); // 角色移动
            Interact(); // 角色交互
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true; // 设置为正在攻击状态
        
        yield return new WaitForSeconds(AttackInterval); 
        isAttacking = false; // 恢复状态
    }
    private IEnumerator FlashRed()
    {
        if (playerRenderer != null)
        {
            playerRenderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            playerRenderer.material.color = originalColor;
        }
    }
}

