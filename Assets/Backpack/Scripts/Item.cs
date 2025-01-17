using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public int id;
    public string itemName;
    public Sprite itemIcon;
    public int quantity;

    public override bool Equals(object obj)
    {
        if (obj is Item otherItem)
        {
            return itemName == otherItem.itemName;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return itemName.GetHashCode();
    }
}
