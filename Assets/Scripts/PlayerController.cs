using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float sprintSpeed;

    [SerializeField]
    private float jumpHeight;

    private bool isGround = true;
    private bool isWalking = true;
    private bool isPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isPaused = GameObject.Find("GameManager").GetComponent<GameManager>().getIsPaused();
        Move(isPaused);
        CheckAction(isPaused);
    }

    private void Move(bool paused)
    {
        if (Input.GetKey(KeyCode.W) && !paused)
        {
            
        }
    }

    private void CheckAction(bool paused)
    {
        isPaused = GameObject.Find("GameManager").GetComponent<GameManager>().getIsPaused();

        if (Input.GetKey(KeyCode.Q) && !paused)
        {
            // check inventory
        }

        if (Input.GetKey(KeyCode.F) && !paused)
        {
            // pick up tool
        }

        if (Input.GetKey(KeyCode.E) && !paused)
        {
            // interact with objects
        }

        if (Input.GetKey(KeyCode.Mouse1) && !paused)  // right click of mouse
        {
            // use tool
        }
    }
}
