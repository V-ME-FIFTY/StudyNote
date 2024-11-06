using Assets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI; // 添加此行以使用 UI 相关组件

public class Soil : MonoBehaviour, IDiggable, IHealth, IDeath, IDroppable, ICollectable
{
    public bool IsDiggable { get; set; } = true;
    private float digInterval = 0.5f; 
    private bool isDigging = false;
    private int takeDamage = 5;
    private int currentHealth = 100; // 健康值
    public GameObject droppedItem; // 掉落物体
    public GameObject itemPrefab;
    public Sprite itemIcon; // 添加图标字段
    private ItemManager itemManager;

    private void Start()
    {
        // 假设 itemManager 是通过查找方式初始化的，可以根据需求调整
        itemManager = FindObjectOfType<ItemManager>();
        if (itemManager == null)
        {
            Debug.LogError("ItemManager 未找到，请确保场景中有该组件。");
        }
    }

    public void Health()
    {
        // 更新土壤的健康值
        Debug.Log("土壤的健康值为：" + currentHealth);
        if (currentHealth > 0)
        {
            currentHealth -= takeDamage; // 每次调用 Health 时减少生命值
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0; // 确保健康值不低于 0
            Death(); // 直接调用死亡逻辑
        }
    }

    public void Death()
    {
        if (currentHealth <= 0 && gameObject != null)
        {
            Drop();
            Destroy(this.gameObject);
            Debug.Log("土壤已被破坏。");
        }
    }

    public void Drop()
    {
        Vector3 dropPosition = transform.position;
        GameObject droppedItem = Instantiate(itemPrefab, dropPosition, Quaternion.identity);
        
        // 如果掉落物体具有某个组件用于显示图标，则设置图标
        if (droppedItem.TryGetComponent(out Image iconImage)) // 确保掉落物体上有 Image 组件
        {
            iconImage.sprite = itemIcon; // 设置掉落物体的图标
        }

        Debug.Log("土壤掉落。");
    }

    public void Dig()
    {
        Debug.Log("正在挖掘土壤。");
        Health(); // 在挖掘时自动更新健康值
    }

    public void Collect()
    {
        Debug.Log("正在收集土壤。");
        itemManager.PickupItem(ItemType.Soil); // 收集物品
        Destroy( droppedItem); // 直接销毁当前土壤对象
        Debug.Log("土壤已被收集。");
    }

    void Update()
    {
        if (IsDiggable && Input.GetMouseButton(0)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.CompareTag("Diggable") && !isDigging) // 检查标签并且不在挖掘状态
                {
                    StartCoroutine(PerformDigging());//xiecheng
                }
            }
        }
    }

    private IEnumerator PerformDigging()
    {
        isDigging = true; // 设置为正在挖掘状态
        Dig(); // 执行挖掘并更新健康
        yield return new WaitForSeconds(digInterval); // 等待挖掘间隔
        isDigging = false; // 恢复状态
    }
}
