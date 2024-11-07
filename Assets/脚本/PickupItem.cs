using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    private Player player;
    public Type itemType; // ��Ʒ����
    private List<Type> pickableItem; // �ܱ�ʰȡ����Ʒ�б�

    private void Start() // ʹ�� Start �������г�ʼ��
    {
        player = FindObjectOfType<Player>(); // ���� Player ��һ�������򳡾��еĶ���

        if (player != null) // ��� player �Ƿ�ɹ���ʼ��
        {
            player.canBePickedUp = true; // ͨ�� player ʵ������ canBePickedUp
        }
        Type[] itemTypes = (Type[])System.Enum.GetValues(typeof(Type)); // ��ȡ ItemType ö�ٵ�����ֵ
        pickableItem = new List<Type>(itemTypes); // �� ItemType ö�ٵ�����ֵ��ӵ� pickableItem �б���
    }
}