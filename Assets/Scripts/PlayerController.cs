using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed = 4f;

    [SerializeField]
    private float sprintSpeed = 6f;

    [SerializeField]
    private float jumpHeight = 1.2f;

    [SerializeField]
    private float rotationSpeed = 1.0f;

    [SerializeField]
    private Transform cameraTransform;

    private bool isGround = true;
    private bool isWalking = true;
    private bool isPaused = false;
    private CharacterController controller;
    //private PlayerInput playerInput;
    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isPaused = GameObject.Find("GameManager").GetComponent<GameManager>().getIsPaused();
        Move(isPaused, isWalking);
        //rotateView();
        CheckAction(isPaused);
    }

    private void Move(bool paused, bool state)
    {
        float speed;
        if (isWalking)
        {
            speed = walkSpeed;
        }
        else
        {
            speed = sprintSpeed;
        }
        
        if (!paused)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            transform.position = new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime;

            Vector3 moveDirection = cameraTransform.forward * vertical + cameraTransform.right * horizontal;
            moveDirection.y = 0f;
            moveDirection.Normalize();

            controller.Move(moveDirection * speed * Time.deltaTime);
        }
    }

    private void rotateView()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime; 

        Vector3 cameraEuler = cameraTransform.localEulerAngles;
        cameraEuler.x -= mouseY;
        cameraEuler.x = Mathf.Clamp(cameraEuler.x, -80f, 80f);
        cameraTransform.localEulerAngles = cameraEuler;

        transform.Rotate(0f, mouseX, 0f);
    }

    private void CheckAction(bool paused)
    {
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
