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
            if (existingItem.id < 10)
            {
                existingItem.quantity += 1;
            }
        }
        else
        {
            item.quantity = 1;
            items.Add(item);
        }

        if (item.id == 10)
        {
            PlayerPrefs.SetInt("Lv1Gem", 1);
            PlayerPrefs.Save();
        }

        if (item.id == 11)
        {
            PlayerPrefs.SetInt("Lv2Gem", 1);
            PlayerPrefs.Save();
        }

        if (item.id == 12)
        {
            PlayerPrefs.SetInt("Lv3Gem", 1);
            PlayerPrefs.Save();
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

            obj.GetComponent<ItemButtonController>().SetItem(item);
        }
    }
}
