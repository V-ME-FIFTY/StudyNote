using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    private Player player;
    public Type itemType; // 物品类型
    private List<Type> pickableItem; // 能被拾取的物品列表

    private void Start() // 使用 Start 方法进行初始化
    {
        player = FindObjectOfType<Player>(); // 假设 Player 是一个单例或场景中的对象

        if (player != null) // 检查 player 是否成功初始化
        {
            player.canBePickedUp = true; // 通过 player 实例设置 canBePickedUp
        }
        Type[] itemTypes = (Type[])System.Enum.GetValues(typeof(Type)); // 获取 ItemType 枚举的所有值
        pickableItem = new List<Type>(itemTypes); // 将 ItemType 枚举的所有值添加到 pickableItem 列表中
    }
}