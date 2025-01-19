using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.ProBuilder;
using UnityEngine.UI;
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
		public float MoveSpeed = 3.5f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 5f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Header("Gravity")]
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

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
		public float TopClamp = 75.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -75.0f;
		[Tooltip("Animation curve for controlling camera shaking effect")]
		public AnimationCurve curve;

        [Header("HP")]
        public int HP = 3;

        [Header("Shoes")]
        public float shoesSpeedBuff = 2f;
        public float shoesLastTime = 5f;
		public GameObject ShoesCanvas;

        // cinemachine
        private float _cinemachineTargetPitch;
		private GameObject cameraRoot;

        // player
        private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private int hp;
		private float orgY;

        // timeout deltatime
        private float _fallTimeoutDelta;

        // player movement
        private bool canMove = true;
        private bool isWalking = true;
        private bool isWearingShoes = false;
        private bool freezing = false;

        // animation
        private Animator animator;


#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		private StarterAssetsInputs _input;

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

		private void Start()
		{
            _controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
			_playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            hp = HP;
			orgY = this.transform.position.y;
            animator = this.GetComponentInChildren<Animator>();
			cameraRoot = GameObject.Find("PlayerCameraRoot");
        }

        private void Update()
		{
			if (!canMove || freezing)
			{
				return;
			}

            if (Input.GetKeyDown(KeyCode.LeftShift))
			{
				isWalking = !isWalking;
			}

			GravityCheck();
            GroundedCheck();
            Move();
          
			if (hp <= 0 || orgY - transform.position.y > 5f)
			{
				GameObject.Find("GameManager").GetComponent<GameManager>().GameOver();
            }
        }

        private void LateUpdate()
		{
			if (Time.timeScale == 0 || freezing)
			{
				return;
			}
			CameraRotation();
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

        private void GravityCheck()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
            }
            else
            {
				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}
			}

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void Move()
		{
			float targetSpeed;

            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
                animator.SetInteger("state", 0);
            }
            else if (isWalking)
			{
                if (isWearingShoes)
                {
                    targetSpeed = MoveSpeed + shoesSpeedBuff;
                }
				else
				{
					targetSpeed = MoveSpeed;
				}
				animator.SetInteger("state", 1);
			}
			else
			{
                if (isWearingShoes)
                {
                    targetSpeed = SprintSpeed + shoesSpeedBuff;
                }
                else
                {
                    targetSpeed = SprintSpeed;
                }
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

		public void Hurt()
		{
			hp--;
			//StartCoroutine(Shaking(0.3f));
		}

		//private IEnumerator Shaking(float duration)
		//{
		//	Vector3 orgPosition = cameraRoot.transform.position;
		//	float timeElapsed = 0f;
		//	float strength;

  //          while (timeElapsed < duration)
		//	{
		//		timeElapsed += Time.deltaTime;

  //              strength = curve.Evaluate(timeElapsed / duration);
  //              cameraRoot.transform.position = orgPosition + Random.insideUnitSphere * strength;
		//		yield return null;
  //          }
            
  //          cameraRoot.transform.position = orgPosition;
  //      }

		public int getMaxHP()
		{
			return HP;
		}

		public int getCurrentHP()
		{
			return hp;
		}

		public void Heal(int type)
		{
			if (type == 1)
			{
				if(hp < HP)
				{
                    hp++;
                }
			}
			if (type == 2)
			{
				hp = HP;
			}
        }

		public bool getIsWalking()
		{
			return isWalking;
		}

        public void EnableMovement()
        {
            canMove = true;
        }

        public void DisableMovement()
        {
            canMove = false;
        }

        public void WearShoes()
		{
            if (GameObject.FindGameObjectWithTag("CountDownBar") != null)
            {
                return;
            }
            StartCoroutine(getShoesBuff());
		}

		private IEnumerator getShoesBuff()
		{
            isWearingShoes = true;

            GameObject shoesPanel = Instantiate(ShoesCanvas);

            shoesPanel.GetComponentInChildren<ProgressBar>().StartCountdown(shoesLastTime);
            yield return new WaitForSeconds(shoesLastTime);

            Destroy(GameObject.FindGameObjectWithTag("CountDownBar"));

            isWearingShoes = false;
        }

        public Vector3 getPosition()
        {
            return transform.position;
        }

        public void Teleport(Vector3 targetPosition)
        {
			animator.SetInteger("state", 0);
            _controller.enabled = false;
            _controller.transform.position = targetPosition;
            _controller.enabled = true;
        }

        public void Freeze()
        {
            freezing = true;
        }

        public void Unfreeze()
        {
            freezing = false;
        }

		public bool getFreezing()
		{
			return freezing;
		}
    }
}