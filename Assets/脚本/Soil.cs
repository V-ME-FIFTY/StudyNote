using Assets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI; // ��Ӵ�����ʹ�� UI ������

public class Soil : MonoBehaviour, IDiggable, IHealth, IDeath, IDroppable, ICollectable
{
    public bool IsDiggable { get; set; } = true;
    private float digInterval = 0.5f; 
    private bool isDigging = false;
    private int takeDamage = 5;
    private int currentHealth = 100; // ����ֵ
    public GameObject droppedItem; // ��������
    public GameObject itemPrefab;
    public Sprite itemIcon; // ���ͼ���ֶ�
    private ItemManager itemManager;

    private void Start()
    {
        // ���� itemManager ��ͨ�����ҷ�ʽ��ʼ���ģ����Ը����������
        itemManager = FindObjectOfType<ItemManager>();
        if (itemManager == null)
        {
            Debug.LogError("ItemManager δ�ҵ�����ȷ���������и������");
        }
    }

    public void Health()
    {
        // ���������Ľ���ֵ
        Debug.Log("�����Ľ���ֵΪ��" + currentHealth);
        if (currentHealth > 0)
        {
            currentHealth -= takeDamage; // ÿ�ε��� Health ʱ��������ֵ
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0; // ȷ������ֵ������ 0
            Death(); // ֱ�ӵ��������߼�
        }
    }

    public void Death()
    {
        if (currentHealth <= 0 && gameObject != null)
        {
            Drop();
            Destroy(this.gameObject);
            Debug.Log("�����ѱ��ƻ���");
        }
    }

    public void Drop()
    {
        Vector3 dropPosition = transform.position;
        GameObject droppedItem = Instantiate(itemPrefab, dropPosition, Quaternion.identity);
        
        // ��������������ĳ�����������ʾͼ�꣬������ͼ��
        if (droppedItem.TryGetComponent(out Image iconImage)) // ȷ�������������� Image ���
        {
            iconImage.sprite = itemIcon; // ���õ��������ͼ��
        }

        Debug.Log("�������䡣");
    }

    public void Dig()
    {
        Debug.Log("�����ھ�������");
        Health(); // ���ھ�ʱ�Զ����½���ֵ
    }

    public void Collect()
    {
        Debug.Log("�����ռ�������");
        itemManager.PickupItem(ItemType.Soil); // �ռ���Ʒ
        Destroy( droppedItem); // ֱ�����ٵ�ǰ��������
        Debug.Log("�����ѱ��ռ���");
    }

    void Update()
    {
        if (IsDiggable && Input.GetMouseButton(0)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.CompareTag("Diggable") && !isDigging) // ����ǩ���Ҳ����ھ�״̬
                {
                    StartCoroutine(PerformDigging());//xiecheng
                }
            }
        }
    }

    private IEnumerator PerformDigging()
    {
        isDigging = true; // ����Ϊ�����ھ�״̬
        Dig(); // ִ���ھ򲢸��½���
        yield return new WaitForSeconds(digInterval); // �ȴ��ھ���
        isDigging = false; // �ָ�״̬
    }
}
