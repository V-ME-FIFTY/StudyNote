using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackManager : MonoBehaviour
{
    private ItemData item;
    public GameObject itemSlotPrefab;
    public Transform contentPanel;
    private ItemManager itemManager;
    public GameObject backpackPanel;
    private bool isBackpackOpen = false;

    void Start()
    {
        itemManager = FindObjectOfType<ItemManager>();
        UpdateBackpackUI();
    }

    public void UpdateBackpackUI()
    {
        // 清理旧的物品槽
        List<GameObject> slotsToDestroy = new List<GameObject>();
        foreach (Transform child in contentPanel)
        {
            slotsToDestroy.Add(child.gameObject);
        }
        foreach (var slot in slotsToDestroy)
        {
            Destroy(slot);
        }

        Dictionary<Type, GameObject> itemSlotsByType = new Dictionary<Type, GameObject>();

        if (itemManager != null)
        {
            var inventoryReadOnly = itemManager.GetInventoryReadOnly();

            foreach (var itemPair in inventoryReadOnly)
            {
                GameObject newSlot;
                if (!itemSlotsByType.ContainsKey(itemPair.Value.type))
                {
                    newSlot = UnityEngine.Object.Instantiate(itemSlotPrefab, contentPanel);
                    itemSlotsByType[itemPair.Value.type] = newSlot;
                }
                else
                {
                    newSlot = itemSlotsByType[itemPair.Value.type];
                }

                Image itemIcon = newSlot.GetComponentInChildren<Image>();
                Text itemCountText = newSlot.GetComponentInChildren<Text>();
                Button useButton = newSlot.GetComponentInChildren<Button>();

                RectTransform buttonRectTransform = useButton.GetComponent<RectTransform>();
                RectTransform iconRectTransform = itemIcon.GetComponent<RectTransform>();
                buttonRectTransform.anchoredPosition = new Vector2(iconRectTransform.anchoredPosition.x + iconRectTransform.rect.width / 2 + buttonRectTransform.rect.width / 2 + 5, iconRectTransform.anchoredPosition.y);

                Sprite sprite = GetDefaultSpriteForType(itemPair.Value.type);
                if (sprite != null)
                {
                    itemIcon.sprite = sprite;
                }
                else
                {
                    // 如果没有图标资源，设置一个默认图标或者不设置图标
                    itemIcon.sprite = null;

                }

                itemCountText.text = itemPair.Value.quantity.ToString();

                // 根据物品类型设置不同的点击事件
                switch (item.name)
                {
                    case "HealthPotion":
                        useButton.onClick.AddListener(() => tool.UseHealthPotion(itemPair.Value.type));
                        break;
                    case "StoneAxe":
                        useButton.onClick.AddListener(() => tool.UseStoneAxe(itemPair.Value.type));
                        break;
                    case "StonePickaxe":
                        useButton.onClick.AddListener(() => tool.UseStonePickaxe(itemPair.Value.type));
                        break;
                }
            }
        }
    }

    private Sprite GetDefaultSpriteForType(string name)
    {
        string resourcePath;
        switch (name)
        {
            case "Wood":
                resourcePath = "wood_icon";
                break;
            case "Gold":
                resourcePath = "gold_coin_icon";
                break;
            case"Stone":
                resourcePath = "stone_icon";
                break;
            case "HealthPotion":
                resourcePath = "health_potion_icon";
                break;
            case "StoneAxe":
                resourcePath = "stone_axe_icon";
                break;
            case "StonePickaxe":
                resourcePath = "stone_pickaxe_icon";
                break;
            default:
                return null;
        }

        Sprite sprite = Resources.Load<Sprite>(resourcePath);
        return sprite;
    }

    public void ShowBackpack()
    {
        isBackpackOpen = !isBackpackOpen;
        backpackPanel.SetActive(isBackpackOpen);
        UpdateBackpackUI();
    }



}
    