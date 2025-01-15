using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // 关联的道具信息
    public bool isIn;

    private void Start()
    {
        isIn = false;
    }

    private void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Enter {item.itemName}");
        if (other.CompareTag("Player"))
        {
            PlayerBackpack backpack = other.GetComponent<PlayerBackpack>();
            if (backpack != null)
            {
                backpack.AddItem(item);
                Debug.Log($"{item.itemName} picked up!");
                Destroy(gameObject); // 销毁场景中的道具
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isIn = false;
    }
}
