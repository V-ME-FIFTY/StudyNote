using System.Xml;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public enum Type
{
    None,
    Weapon,
    Armor,
    Jewelry,
    Consumable,
    Material,
    QuestItem,
    Currency,
    KeyItem,
    Other
}

[System.Serializable]
public class ItemData
{
    public string name;
    public Type type;
    public string id;
    public int quality;
    public string price;
    public int stack;
    public string description;
    public List<string> attributes;
    public int quantity;
}



public class ItemReader : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();
    public void LoadItemsFromXML()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(Application.dataPath + "/StreamingAssets/Items.xml");
        XmlElement root = doc.LastChild as XmlElement;
        foreach (XmlElement item in root)
        {
            ItemData data = new ItemData();
            data.name = item.GetAttribute("name");
            data.type = (Type)System.Enum.Parse(typeof(Type), item.GetAttribute("type"));
            data.id = item.GetAttribute("id");
          
            data.quality = int.Parse(item.GetAttribute("quality"));
            data.price = item.GetAttribute("price");
            data.stack = int.Parse(item.GetAttribute("stack"));
            data.description = item.GetAttribute("description");
       
            data.attributes = new List<string>();
            foreach (XmlElement attribute in item.GetElementsByTagName("attribute"))
            {
                data.attributes.Add(attribute.Name + ": " + attribute.InnerText);
            }
            items.Add(data);
        }
    }
}
