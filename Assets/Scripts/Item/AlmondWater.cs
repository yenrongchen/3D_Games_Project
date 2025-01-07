using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlmondWater : MonoBehaviour
{

    public PlayerBackpack backpack;
    private bool isInsideTrigger = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isInsideTrigger && Input.GetMouseButtonDown(0))
        {
            TriggerEventAction(); // �I�sĲ�o�ƥ�
        }
    }

    void TriggerEventAction()
    {
        if (backpack.almondFirstTime == 0)
        {
            Flowchart.BroadcastFungusMessage("GetAlmondWaterF");
        }
        else
        {
            Flowchart.BroadcastFungusMessage("Pickornot");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isInsideTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // �T�{���a�O�_���}����
        if (other.CompareTag("Player"))
        {
            isInsideTrigger = false;
            Debug.Log("Player left the trigger zone!");
        }
    }
}
