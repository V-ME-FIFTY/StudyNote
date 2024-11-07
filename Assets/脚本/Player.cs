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
    public float jumpForce = 5f; // ������Ծ����
    public float maxJumpTime = 1f; // �����Ծʱ��
    private float jumpTime = 0f; // ��ǰ��Ծʱ��
     public float AttackInterval = 0.5f; 
    public float pickupRange = 2f; // ʰȡ��Χ
    public bool canBePickedUp = false; // �Ƿ���Ա�ʰȡ
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
        if (currentHealth <= 0) return; // ��ֹ������˺�����

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
        float moveHorizontal = Input.GetAxis("Horizontal"); // ��ȡˮƽ����
        float moveVertical = Input.GetAxis("Vertical"); // ��ȡ��ֱ����

        Vector3 movement = new Vector2(moveHorizontal, moveVertical).normalized; // �����ƶ���������һ��

        // ʹ�� Translate �����ƶ���ɫ
        transform.Translate(movement * MoveSpeed * Time.deltaTime, Space.World);

        // �������ת�߼��������ƶ����������ɫ����
        if (movement.magnitude > 0)
        {
            // ȡ���ƶ�����
            Vector3 flippedDirection = -movement;

            Quaternion targetRotation = Quaternion.LookRotation(flippedDirection); // ʹ�÷�ת��ķ��򴴽�Ŀ����ת
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotateSpeed * Time.deltaTime); // ƽ����ת
        }

    }

    public void Jump()
    {
        if (isJumping) return; // ���������Ծ����ֹ��ε���
        isJumping = true; // ��ǽ�ɫΪ������Ծ
        float JumpMultiplier = jumpTime / maxJumpTime;
        Vector2 jumpVector = new Vector2(0, jumpForce * JumpMultiplier); // ������Ծ����
        rb.AddForce(jumpVector, ForceMode.Impulse); // ʩ����Ծ����
        isJumping = false; // ��Ծ�������ָ�״̬
    }

    public void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.CompareTag("Interactable")) // ����ǩ
                {
                    hit.collider.GetComponent<IInteractable>().Interact();
                }
            }
        }
    }

    public void TryPickup()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange); // ��ⷶΧ�ڵ�����
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Pickupable")) // ����ǩ
            {
                // �õ���Ʒ���������ʰȡ
                PickupItem pickupItem = collider.GetComponent<PickupItem>();
                if (pickupItem != null && canBePickedUp ==true) // ȷ����ȡ����PickupItem���
                {
                    // ������Ե��� ItemManager ���Լ��ķ���������ʰȡ�߼�
                  
                  itemManager.PickupItem(pickupItem.itemType); // �����������������������Ʒ���߼�
                   
                    Destroy(collider.gameObject); // ʰȡ��������Ʒ����
                  canBePickedUp = false; // �����Ʒ���Ա�ʰȡ
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
            if (Input.GetKeyDown(KeyCode.Space) && !isJumping) // ���¿ո���Ҳ�����Ծ״̬
            {
                jumpTime = 0f; // ������Ծʱ��
                Jump(); // ֱ�ӵ�����Ծ
            }

           
            if (isJumping)
            {
                if (Input.GetKey(KeyCode.Space) && jumpTime < maxJumpTime)
                {
                    jumpTime += Time.deltaTime; 
                }

                // ���ͷ���Ծ�����ߴﵽ�����Ծʱ��ʱ��ִ����Ծ
                if (Input.GetKeyUp(KeyCode.Space) || jumpTime >= maxJumpTime)
                {
                    Jump();
                }
            }

            Move(); // ��ɫ�ƶ�
            Interact(); // ��ɫ����
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true; // ����Ϊ���ڹ���״̬
        
        yield return new WaitForSeconds(AttackInterval); 
        isAttacking = false; // �ָ�״̬
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

