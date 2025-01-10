using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class PlayerInteractionField : MonoBehaviour
{
    private bool isInsideTrigger = false;
    private string tag;
        // Start is called before the first frame update
    void Start()
    {
        tag = null;
    }
    // Update is called once per frame
    
    void Update()
    {

        if (isInsideTrigger && Input.GetMouseButtonDown(0))
        {
            TriggerEventAction(tag);
        }
    }

    private void TriggerEventAction(string tagName)
    {
        Flowchart.BroadcastFungusMessage(tagName);
    }

    private void OnTriggerEnter(Collider other)
    {
        bool notUntag = other.gameObject.tag.Equals("Untagged");
        bool notPlayer = other.gameObject.tag.Equals("Player");
        if (!notUntag && !notPlayer)
        {
            isInsideTrigger = true;
            tag = other.gameObject.tag;       
            Debug.Log("Player enter" + other.gameObject.tag + "trigger zone!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 確認玩家是否離開光圈
        isInsideTrigger = false;
        Debug.Log("Player left the" + other.gameObject.tag + "trigger zone!");
    }
}
