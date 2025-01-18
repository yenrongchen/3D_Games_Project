using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using StarterAssets;

public class RoomDialog : MonoBehaviour
{
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        int times = PlayerPrefs.GetInt("times", 0);
        if (times == 0)
        {
            Flowchart.BroadcastFungusMessage("Initial");
            PlayerPrefs.SetInt("times", 1);
        }
        else
        {
            Flowchart.BroadcastFungusMessage("Normal");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rayOrigin = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(rayOrigin, mainCamera.transform.forward, out RaycastHit hit, 80))
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                int status = PlayerPrefs.GetInt("status", 0);
                if (status == 0)
                {
                    Flowchart.BroadcastFungusMessage("CannotSleep");
                }
                else
                {
                    Flowchart.BroadcastFungusMessage("CanSleep");
                    // heal all hp
                    PlayerPrefs.SetInt("status", 0);
                }
            }
        }
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject.CompareTag("TableandChair"))
        {
            Flowchart.BroadcastFungusMessage("TableandChair");
        }

        if (collision.gameObject.CompareTag("CrateBox"))
        {
            Flowchart.BroadcastFungusMessage("CrateBox");
        }

        if (collision.gameObject.CompareTag("BeginDoor"))
        {
            Flowchart.BroadcastFungusMessage("BeginDoor");
        }
    }
}
