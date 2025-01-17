using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;

    public void Pickup()
    {
        // pick up props
        InventoryManager.Instance.Add(item);

        // destroy object
        Destroy(gameObject);
    }
}
