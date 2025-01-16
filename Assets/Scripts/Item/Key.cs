using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Fungus;

public class Key : MonoBehaviour
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
        if (isInsideTrigger)
        {
            {
                TriggerEventAction();
            }
        }
    }



    private void TriggerEventAction()
    {
        //Flowchart.BroadcastFungusMessage("Key1");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            TriggerEventAction();
        }
    }
}
