using Assets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI; // 添加此行以使用 UI 相关组件

public class Stone : MonoBehaviour, IDiggable, IHealth, IDeath, IDroppable, ICollectable
{
    public bool IsDiggable { get; set; } = true;
    private float digInterval = 0.5f;
    private bool isDigging = false;
    private int takeDamage = 5;
    private int currentHealth = 500; // 健康值
    public GameObject droppedItem; // 掉落物体
    public GameObject itemPrefab;
    public Sprite itemIcon; // 添加图标字段
    private ItemManager itemManager;

    private void Start()
    {
        // 初始化 ItemManager
        itemManager = FindObjectOfType<ItemManager>();
        if (itemManager == null)
        {
            Debug.LogError("ItemManager 未找到，请确保场景中有该组件。");
        }
    }

    public void Health()
    {
        // 更新土壤的健康值
        if (currentHealth > 0)
        {
            currentHealth -= takeDamage; // 每次调用 Health 时减少生命值
            Debug.Log("stone的健康值为：" + currentHealth);
        }

        // 处理死亡逻辑
        if (currentHealth <= 0)
        {
            currentHealth = 0; // 确保健康值不低于 0
            Death(); // 直接调用死亡逻辑
        }
    }

    public void Death()
    {
        if (currentHealth <= 0)
        {
            Drop();
            Destroy(gameObject); // 删除当前游戏对象
            Debug.Log("stone已被破坏。");
        }
    }

    public void Drop()
    {
        Vector3 dropPosition = transform.position;
        GameObject newDroppedItem = Instantiate(itemPrefab, dropPosition, Quaternion.identity);

        // 如果掉落物体具有某个组件用于显示图标，则设置图标
        if (newDroppedItem.TryGetComponent(out Image iconImage)) // 确保掉落物体上有 Image 组件
        {
            iconImage.sprite = itemIcon; // 设置掉落物体的图标
        }

      
    }

    public void Dig()
    {
        if (currentHealth > 0)
        {
            
            Health(); // 在挖掘时自动更新健康值
        }
    }

    public void Collect()
    {
       
        itemManager.PickupItem(ItemType.Soil); // 收集物品
        if (droppedItem != null) // 确保不为 Null
        {
            Destroy(droppedItem); // 直接销毁当前的掉落物
            droppedItem = null; // 清空引用
        }
        Destroy(gameObject); // 直接销毁当前土壤对象
       
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
                    StartCoroutine(PerformDigging()); // 启动挖掘协程
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
