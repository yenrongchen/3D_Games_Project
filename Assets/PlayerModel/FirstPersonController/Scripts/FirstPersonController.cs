﻿using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using Fungus;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.5f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.4f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private bool isWalking = true;
        private bool canMove = true;

        // check game status
        private bool isPaused;

		// animation
		private Animator animator;

        // HP
        [Header("HP")]
		public int HP = 3;

		private int hp;

#if ENABLE_INPUT_SYSTEM
		private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;

		private bool IsCurrentDeviceMouse
		{
			get
			{
				#if ENABLE_INPUT_SYSTEM
				return _playerInput.currentControlScheme == "KeyboardMouse";
				#else
				return false;
				#endif
			}
		}

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
			_playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            
            animator = this.GetComponentInChildren<Animator>();

			hp = HP;
		}

		private void Update()
		{
            isPaused = GameObject.Find("GameManager").GetComponent<GameManager>().getIsPaused();
			if (!isPaused)
			{
                if (!canMove)
                {
                    return;  // 禁止移动
                }
                if (Input.GetKeyDown(KeyCode.LeftShift))
				{
					isWalking = !isWalking;
				}
				GroundedCheck();
                Move(isWalking);
				CheckAction();
			}

			if (hp <= 0)
			{
				// Game Over
			}
		}

        public void EnableMovement()
        {
            canMove = true;
        }

        public void DisableMovement()
        {
            canMove = false;
        }

        private void LateUpdate()
		{
			if (!isPaused)
			{
				CameraRotation();
			}
		}

		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void Move(bool isWalking)
		{
			// set target speed based on move or sprint
			float targetSpeed;

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
                animator.SetInteger("state", 0);
            }
            else if (isWalking)
			{
				targetSpeed = MoveSpeed;
				animator.SetInteger("state", 1);
			}
			else
			{
				targetSpeed = SprintSpeed;
                animator.SetInteger("state", 2);
            }

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }

        private void CheckAction()
		{
			isPaused = GameObject.Find("GameManager").GetComponent<GameManager>().getIsPaused();

            if (Input.GetKey(KeyCode.Q) && !isPaused)
			{
				// check inventory
			}

            if (Input.GetKey(KeyCode.F) && !isPaused)
            {
                // pick up tool
            }

            if (Input.GetKey(KeyCode.E) && !isPaused)
            {
                // interact with objects
            }

            if (Input.GetKey(KeyCode.Mouse1) && !isPaused)  // right click of mouse
            {
                // use tool
            }
        }

		public Vector3 getPosition()
		{
			return transform.position;
		}

		public void Hurt()
		{
			hp--;
			//print(hp);
		}

		public int getHP()
		{
			return hp;
		}
    }
}