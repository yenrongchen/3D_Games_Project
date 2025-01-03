using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public PlayerController p;
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
            if (p.getKey == true)
            {
                TriggerEventAction0();
            }
            else
            {
                TriggerEventAction1(); // 呼叫觸發事件
            }
            
        }
    }

    private void TriggerEventAction0()
    {
        Flowchart.BroadcastFungusMessage("getKey1");

    }

    private void TriggerEventAction1()
    {
        Flowchart.BroadcastFungusMessage("Door");

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isInsideTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 確認玩家是否離開光圈
        if (other.CompareTag("Player"))
        {
            isInsideTrigger = false;
            Debug.Log("Player left the trigger zone!");
        }
    }
}
