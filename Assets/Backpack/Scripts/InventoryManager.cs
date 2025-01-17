using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> items = new List<Item>();

    public Transform ItemContent;
    public GameObject InventoryItem;

    private void Awake()
    {
        Instance = this;
    }

    public void Add(Item item)
    {
        var existingItem = items.Find(i => i.itemName == item.itemName);
        if (existingItem != null)
        {
            existingItem.quantity += 1;
        }
        else
        {
            item.quantity = 1;
            items.Add(item);
        }

        ListItems();
    }

    public void Remove(Item item)
    {
        if (item.quantity > 1)
        {
            item.quantity -= 1;
        }
        else
        {
            items.Remove(item);
        }

        ListItems();
    }

    public void ListItems()
    {
        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }

        foreach (Item item in items)
        {
            GameObject obj = Instantiate(InventoryItem, ItemContent);
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            var itemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            var itemQuan = obj.transform.Find("ItemQuan").GetComponent<TextMeshProUGUI>();

            itemIcon.sprite = item.itemIcon;
            itemName.text = item.itemName;
            itemQuan.text = item.quantity.ToString();

            obj.GetComponent<ItemButtonController>().SetID(item.id);
            obj.GetComponent<ItemButtonController>().SetItem(item);
        }
    }
}
