using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// 物品结构
public class ItemManager : MonoBehaviour
{
    public ItemData item;
    // 背包容量
    public int backpackCapacity = 100;

    // 物品栏字典，使用物品的唯一标识符作为键
    private Dictionary<string, ItemData> id = new Dictionary<string, ItemData>();

    public bool itemUsed;
    public ItemReader itemReader;

    // 物品预制体
    public GameObject woodPrefab;
    public GameObject stonePrefab;
    public GameObject healthPotionPrefab;

    // 显示物品数量的文本
    public Text woodCountText;
    public Text goldCoinCountText;
    public Text stoneCountText;
    public Text healthPotionCountText;

    // 提供一个只读的方式访问 id
    public IReadOnlyDictionary<string, ItemData> GetInventoryReadOnly()
    {
        return id;
    }

    // 拾取物品逻辑
    public void PickupItem(ItemData itemData)
    {
        // 使用 itemData 的 id
        ItemData newItem = new ItemData { type = itemData.type, quantity = 1, id = itemData.id };

        var existingItem = id.Values.FirstOrDefault(item => item.id == itemData.id && item.quantity < item.stack);
        if (existingItem != null) // 使用 != null 判断
        {
            existingItem.quantity++;
            id[existingItem.id] = existingItem;
        }
        else
        {
            id.Add(newItem.id, newItem); // 新物品添加在 else 中
        }

        UpdateUI();

        Debug.Log($"Picked up item: {itemData.name}");
    }

    // 任务提交消耗物品逻辑
    public bool ConsumeForTask(ItemData itemData, int quantity)
    {
        bool itemConsumed = false; // 标记物品是否被消耗
        foreach (var itemPair in id.Keys.ToList()) // 使用 Keys.ToList() 避免修改时出错
        {
            if (id[itemPair].id == itemData.id && id[itemPair].quantity >= quantity)
            {
                id[itemPair].quantity -= quantity;

                // 检查数量是否为0并移除
                if (id[itemPair].quantity == 0)
                {
                    id.Remove(itemPair);
                }
                itemConsumed = true; // 标记为已消耗
                break; // 找到则退出循环
            }
        }

        if (itemConsumed)
        {
            UpdateUI();
        }
        
        return itemConsumed;
    }

    // 使用物品逻辑
    public void UseItem(ItemData itemData)
    {
        itemUsed = false;

        foreach (var itemPair in id.Keys.ToList())
        {
            if (id[itemPair].id == itemData.id && id[itemPair].quantity > 0)
            {
                id[itemPair].quantity--; // 直接更新原始字典

                if (id[itemPair].quantity == 0)
                {
                    id.Remove(itemPair);
                }
                itemUsed = true;
                break;
            }
        }

        UpdateUI(); // 确保UI更新
    }

    // 物品堆叠逻辑
    public void StackItems()
    {
        var itemGroups = id.Values.GroupBy(item => item.type);
        var stackedInventory = new Dictionary<string, ItemData>();
        foreach (var group in itemGroups)
        {
            int totalQuantity = group.Sum(item => item.quantity);
            int remainingQuantity = totalQuantity % group.First().stack; // 使用堆叠尺寸
            int stackedQuantity = totalQuantity - remainingQuantity;

            if (stackedQuantity > 0)
            {
                stackedInventory.Add(group.First().id, new ItemData { type = group.Key, quantity = stackedQuantity, id = group.First().id });
            }

            if (remainingQuantity > 0)
            {
                string newItemId = GenerateItemId(); // 生成新ID
                stackedInventory.Add(newItemId, new ItemData { type = group.Key, quantity = remainingQuantity, id = newItemId });
            }
        }

        id = stackedInventory;
        UpdateUI();
    }

    // 物品拆分逻辑
    public void SplitItem(ItemData itemToSplitData, int splitQuantity)
    {
        if (splitQuantity <= 0)
        {
            Debug.LogError("拆分数量必须大于0。");
            return;
        }

        // 找到可以拆分的物品
        var itemToSplit = id.Values.FirstOrDefault(item => item.id == itemToSplitData.id && item.quantity >= splitQuantity);

        if (itemToSplit != null)
        {
            // 减少原物品的数量
            itemToSplit.quantity -= splitQuantity;
            id[itemToSplit.id] = itemToSplit; // 更新字典中的物品

            // 生成新物品的ID，确保是唯一的
            string newItemId = GenerateItemId();

            // 创建新物品并赋值
            ItemData newItem = new ItemData
            {
                name = itemToSplit.name,
                type = itemToSplit.type,
                id = newItemId,
                quality = itemToSplit.quality,
                price = itemToSplit.price,
                stack = itemToSplit.stack,
                description = itemToSplit.description,
                attributes = new List<string>(itemToSplit.attributes),
                quantity = splitQuantity
            };

            // 将新物品添加到字典中
            id.Add(newItemId, newItem);

            UpdateUI();
        }
        else
        {
            Debug.LogError("找不到可以拆分的物品或拆分数量不够。");
        }
    }

    // 丢弃物品逻辑
    public void DropItem(ItemData itemData, int quantity)
    {
        var newInventory = new Dictionary<string, ItemData>(id);
        foreach (var itemPair in newInventory.Keys.ToList())
        {
            if (newInventory[itemPair].id == itemData.id && newInventory[itemPair].quantity >= quantity)
            {
                ItemData tempItem = newInventory[itemPair];
                tempItem.quantity -= quantity;
                if (tempItem.quantity == 0)
                {
                    newInventory.Remove(itemPair);
                }
                else
                {
                    newInventory[itemPair] = tempItem;
                }
            }
        }

        if (newInventory != id)
        {
            id = newInventory;
            UpdateUI();
        }
    }

    // 更新物品栏 UI 的方法
    private void UpdateUI()
    {
        int woodCount = id.Values.Where(item1 => item1.name == "Wood").Sum(item => item.quantity);
        int goldCoinCount = id.Values.Where(item1 => item1.name == "GoldCoin").Sum(item => item.quantity);
        int stoneCount = id.Values.Where(item1 => item1.name == "Stone").Sum(item => item.quantity);
        int healthPotionCount = id.Values.Where(item1 => item1.name == "HealthPotion").Sum(item => item.quantity);
        
        woodCountText.text = $"Wood: {woodCount}";
        goldCoinCountText.text = $"Gold Coins: {goldCoinCount}";
        stoneCountText.text = $"Stone: {stoneCount}";
        healthPotionCountText.text = $"Health Potions: {healthPotionCount}";
    }

    // 添加方法用于检查是否可以制造特定物品
    public bool CanManufacture(ItemData itemData)
    {
        switch (itemData.name)
        {
            case ("WoodenSword"):
                return id.Values.Where(i => i.name == "Wood").Sum(i => i.quantity) >= 2;

            case ("StoneSword"):
                return id.Values.Where(i => i.name == "Stone").Sum(i => i.quantity) >= 1 &&
                       id.Values.Where(i => i.name == "Wood").Sum(i => i.quantity) >= 2;
            default:
                return false;
        }
    }

  
    private string GenerateItemId()
    {
        return System.Guid.NewGuid().ToString(); 
    }
}
