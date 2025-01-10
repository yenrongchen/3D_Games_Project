//using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBackpack : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public List<int> amount = new List<int>();
    public int maxSlots = 20;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddItem(Item newItem)
    {
        int index = items.IndexOf(newItem);

        if (index >= 0)
        {
            // 如果道具已存在，检查数量是否已达到上限
            if (amount[index] < maxSlots)
            {
                amount[index]++;
                Debug.Log($"{newItem.itemName} amount increased to {amount[index]}.");
            }
            else
            {
                Debug.Log($"{newItem.itemName} has reached the maximum amount.");
            }
        }
        else
        {
            // 如果道具不存在，检查是否还有空位
            if (items.Count < maxSlots)
            {
                items.Add(newItem);
                amount.Add(1);
                Debug.Log($"{newItem.itemName} added to inventory.");
            }
            else
            {
                Debug.Log("Inventory is full!");
            }
        }
    }

    public void RemoveItem(Item itemToRemove)
    {
        int index = items.IndexOf(itemToRemove);

        if (index >= 0)
        {
            if (amount[index] > 1)
            {
                amount[index]--;
                Debug.Log($"{itemToRemove.itemName} amount decreased to {amount[index]}.");
            }
            else
            {
                items.RemoveAt(index);
                amount.RemoveAt(index);
                Debug.Log($"{itemToRemove.itemName} removed from inventory.");
            }
        }
        else
        {
            Debug.Log("Item not found in inventory.");
        }
    }


}
