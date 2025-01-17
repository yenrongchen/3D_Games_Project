using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButtonController : MonoBehaviour
{
    private int ID;
    private Item self;
    private PlayerActionController player;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(Hold);

        player = GameObject.Find("Player").GetComponent<PlayerActionController>();
    }

    private void Hold()
    {
        if (ID == 1)
        {
            player.HoldJammer();
            InventoryManager.Instance.Remove(self);
            player.CloseBackpack();
        }

        if (ID == 2)
        {
            player.HoldPortal();
            InventoryManager.Instance.Remove(self);
            player.CloseBackpack();
        }

        if (ID == 3)
        {
            player.HoldBoard();
            InventoryManager.Instance.Remove(self);
            player.CloseBackpack();
        }

        if (ID == 4)
        {
            player.HoldAlmond();
            InventoryManager.Instance.Remove(self);
            player.CloseBackpack();
        }

        if (ID == 5)
        {
            player.HoldRations();
            InventoryManager.Instance.Remove(self);
            player.CloseBackpack();
        }
    }

    public void SetID(int id)
    {
        ID = id;
    }

    public void SetItem(Item item)
    {
        self = item;
    }
}
