using Fungus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeDialog : MonoBehaviour
{
    private bool finished = false;

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject.CompareTag("Door"))
        {
            if (finished)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Flowchart.BroadcastFungusMessage("DoorEnd");
            }
            else
            {
                Flowchart.BroadcastFungusMessage("Door");
            }
        }
    }

    public void Finish()
    {
        finished = true;
    }
}
