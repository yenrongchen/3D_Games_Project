using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

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

    //[SerializeField]
    //private Transform cameraTransform;

    private bool isGround = true;
    private bool isWalking = true;
    private bool isPaused = false;
    private bool onGround;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private float gravityValue = -9.8f;

    private float rotationX = 0;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    //public Camera playerCamera;

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
        rotateView(isPaused);
        CheckAction(isPaused);
    }

    private void Move(bool paused, bool state)
    {
        if (!paused)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (isWalking)
                {
                    isWalking = false;
                }
                else
                {
                    isWalking = true;
                }
            }

            float speed;
            if (isWalking)
            {
                speed = walkSpeed;
            }
            else
            {
                speed = sprintSpeed;
            }

            onGround = controller.isGrounded;
            if (onGround && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            controller.Move(move * Time.deltaTime * speed);

            if (move != Vector3.zero)
            {
                gameObject.transform.forward = move;
            }

            // Makes the player jump
            if (Input.GetKey(KeyCode.Space) && onGround)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
            }

            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }

    private void rotateView(bool paused)
    {
        if (!paused)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            //playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
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
