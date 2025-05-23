using UnityEngine;
using UnityEngine.TextCore.Text;

[DefaultExecutionOrder(-1)]
public class PlayerController : MonoBehaviour
{
    #region  Class Variables
    [Header("Components")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Camera _playerCamera;
    public float RotationMismatch { get; private set; }
    public bool IsRotatingToTarget { get; private set; } = false;
    private PlayerLocomotionInput _playerLocomotionInput;
    private PlayerState _playerState;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public bool isGroundedRaycast;




    [Header("Basic Movement")]
    public float walkAcceleration = 0.15f;
    public float walkSpeed = 2.0f;
    public float runAcceleration = 0.25f;
    public float runSpeed = 6f;
    public float drag = 20f;
    public float inAirDrag = 5f;
    public float sprintAcceleration = 0.5f;
    public float sprintSpeed = 9f;
    public float gravity = 25f;
    public float jumpSpeed = 1.0f;
    public float movingThreshold = 0.01f; //To check movement with so small value
    public float _verticalVelocity = 0f; // to apply gravity
    public float terminalVelocity = 50f;
    public float _antiBump; // to prevent sudden falls from heights
    public float inAirAcceleration = 0.15f;


    [Header("Animation")]
    public float playerModelRotationSpeed = 10.0f; //rotation speed
    public float rotateToTargetTime = 0.25f; //rotation time

    [Header("Camera Settings")]
    public float lookSenseH = 0.1f;
    public float lookSenseV = 0.1f;
    public float lookLimitV = 89f;
    private float _rotatingToTargetTimer = 0f; // measure how long it'll take to rotate


    private bool _isRotatingClockwise = false;


    public Vector2 _cameraRotation = Vector2.zero;
    public Vector2 _playerTargetRotation = Vector2.zero;

    public float spaceCooldown = 0.5f; // Cooldown time
    private float lastSpaceTime = -Mathf.Infinity;

    private float _stepOffset;
    private bool _jumpedLastFrame = false;

    private PlayerMovementState _lastMovementState = PlayerMovementState.Falling;

    #endregion

    // #region Startup
    // private void Awake()
    // {
    //     _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
    //     _playerState = GetComponent<PlayerState>();

    //     _antiBump = sprintSpeed; // it should be minimum as sprint speed
    //     _stepOffset = _characterController.stepOffset;
    // }
    // #endregion

    // #region Update Logic
    // private void Update()
    // {
    //     UpdateMovementState();
    //     HandleVerticalMovement();
    //     HandleLateralMovement();
    // }

    // private void UpdateMovementState()
    // {
    //     _lastMovementState = _playerState.CurrentPlayerMovementState;
    //     Debug.Log(_lastMovementState);

    //     bool canRun = CanRun();
    //     bool isMovementInput = _playerLocomotionInput.MovementInput != Vector2.zero;
    //     bool isMovingLaterally = IsMovingLaterally();
    //     bool isSprinting = _playerLocomotionInput.SprintToggledOn && isMovingLaterally;
    //     bool isWalking = isMovingLaterally && (!canRun || _playerLocomotionInput.WalkToggledOn);
    //     bool isGrounded = IsGrounded();

    //     PlayerMovementState lateralState = isWalking ? PlayerMovementState.Walking :
    //                                        isSprinting ? PlayerMovementState.Sprinting :
    //                                        isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;

    //     _playerState.SetPlayerMovementState(lateralState);

    //     // Control Airborn State
    //     //jumping
    //     if ((!isGrounded) && _characterController.velocity.y > 0f)
    //     {
    //         _playerState.SetPlayerMovementState(PlayerMovementState.Jumping);
    //         _characterController.stepOffset = 0f;
    //     }
    //     //falling
    //     else if ((!isGrounded) && _characterController.velocity.y <= 0f)
    //     {
    //         _playerState.SetPlayerMovementState(PlayerMovementState.Falling);
    //         _characterController.stepOffset = 0f;
    //     }
    //     else
    //     {
    //         _characterController.stepOffset = _stepOffset;
    //     }
    // }

    // private void HandleVerticalMovement()
    // {
    //     bool isGrounded = _playerState.InGroundedState();


    //     _verticalVelocity -= gravity * Time.deltaTime;

    //     if (isGrounded && _verticalVelocity < 0)
    //     {
    //         _verticalVelocity = -_antiBump;
    //     }



    //     if (_playerState.IsStateGroundedState(_lastMovementState) && !isGrounded)
    //     {

    //         _verticalVelocity = _antiBump + gravity;
    //     }

    //     if (_playerLocomotionInput.JumpPressed && isGrounded)
    //     {
    //         if (Time.time - lastSpaceTime >= spaceCooldown)
    //         {
    //             Debug.Log("Jump Triggered!");
    //             lastSpaceTime = Time.time;
    //             //Jump
    //             _verticalVelocity += Mathf.Sqrt(jumpSpeed * 3 * gravity); //jump velocity

    //         }
    //     }

    // }

    // private void HandleLateralMovement()
    // {
    //     // Create quick references for current state
    //     bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
    //     bool isGrounded = _playerState.InGroundedState();
    //     bool isWalking = _playerState.CurrentPlayerMovementState == PlayerMovementState.Walking;

    //     // State dependent acceleration and speed
    //     float lateralAcceleration = !isGrounded ? inAirAcceleration : isWalking ? walkAcceleration : isSprinting ? sprintAcceleration : runAcceleration;
    //     float clampLateralMagnitude = !isGrounded ? sprintSpeed : isWalking ? walkSpeed : isSprinting ? sprintSpeed : runSpeed;

    //     Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
    //     Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
    //     Vector3 movementDirection = cameraRightXZ * _playerLocomotionInput.MovementInput.x + cameraForwardXZ * _playerLocomotionInput.MovementInput.y;

    //     Vector3 movementDelta = movementDirection * lateralAcceleration * Time.deltaTime;
    //     Vector3 newVelocity = _characterController.velocity + movementDelta;

    //     // Add drag to player
    //     Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
    //     newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
    //     newVelocity = Vector3.ClampMagnitude(new Vector3(newVelocity.x, 0f, newVelocity.z), clampLateralMagnitude); //newVelocity 1. para ???
    //     newVelocity.y += _verticalVelocity;
    //     newVelocity = !isGrounded ? HandleSteepWalls(newVelocity) : newVelocity;

    //     // Move character (Unity suggests only calling this once per tick)
    //     _characterController.Move(newVelocity * Time.deltaTime);
    // }

    // private Vector3 HandleSteepWalls(Vector3 velocity)
    // {
    //     Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(_characterController, groundMask);
    //     float angle = Vector3.Angle(normal, Vector3.up);
    //     bool validAngle = angle <= _characterController.slopeLimit;

    //     if (!validAngle && _verticalVelocity < 0f)
    //         velocity = Vector3.ProjectOnPlane(velocity, normal);

    //     return velocity;
    // }



    // #endregion

    // #region Late Update Logic
    // private void LateUpdate()
    // {
    //     UpdateCameraRotation();
    // }

    // private void UpdateCameraRotation()
    // {
    //     _cameraRotation.x += lookSenseH * _playerLocomotionInput.LookInput.x;
    //     _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSenseV * _playerLocomotionInput.LookInput.y, -lookLimitV, lookLimitV);

    //     _playerTargetRotation.x += transform.eulerAngles.x + lookSenseH * _playerLocomotionInput.LookInput.x;

    //     float rotationTolerance = 90f;
    //     bool isIdling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
    //     IsRotatingToTarget = _rotatingToTargetTimer > 0;

    //     // ROTATE if we're not idling
    //     if (!isIdling)
    //     {
    //         RotatePlayerToTarget();
    //     }
    //     // If rotation mismatch not within tolerance, or rotate to target is active, ROTATE
    //     else if (Mathf.Abs(RotationMismatch) > rotationTolerance || IsRotatingToTarget)
    //     {
    //         UpdateIdleRotation(rotationTolerance);
    //     }

    //     _playerCamera.transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);

    //     // Get angle between camera and player
    //     Vector3 camForwardProjectedXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
    //     Vector3 crossProduct = Vector3.Cross(transform.forward, camForwardProjectedXZ);
    //     float sign = Mathf.Sign(Vector3.Dot(crossProduct, transform.up));
    //     RotationMismatch = sign * Vector3.Angle(transform.forward, camForwardProjectedXZ);
    // }

    // private void UpdateIdleRotation(float rotationTolerance)
    // {
    //     // Initiate new rotation direction
    //     if (Mathf.Abs(RotationMismatch) > rotationTolerance)
    //     {
    //         _rotatingToTargetTimer = rotateToTargetTime;
    //         _isRotatingClockwise = RotationMismatch > rotationTolerance;
    //     }
    //     _rotatingToTargetTimer -= Time.deltaTime;

    //     // Rotate player
    //     if (_isRotatingClockwise && RotationMismatch > 0f ||
    //         !_isRotatingClockwise && RotationMismatch < 0f)
    //     {
    //         RotatePlayerToTarget();
    //     }
    // }

    // private void RotatePlayerToTarget()
    // {
    //     Quaternion targetRotationX = Quaternion.Euler(0f, _playerTargetRotation.x, 0f);
    //     transform.rotation = Quaternion.Lerp(transform.rotation, targetRotationX, playerModelRotationSpeed * Time.deltaTime);
    // }
    // #endregion

    // #region State Checks
    // private bool IsMovingLaterally()
    // {
    //     Vector3 lateralVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.y);

    //     return lateralVelocity.magnitude > movingThreshold;
    // }

    // private bool IsGrounded()
    // {
    //     bool grounded = _playerState.InGroundedState() ? IsGroundedWhileGrounded() : IsGroundedWhileAirborne();

    //     return grounded;
    // }

    // private bool IsGroundedWhileGrounded()
    // {
    //     Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _characterController.radius, transform.position.z);

    //     bool grounded = Physics.CheckSphere(spherePosition, _characterController.radius, groundMask, QueryTriggerInteraction.Ignore);

    //     return grounded;
    // }

    // private bool IsGroundedWhileAirborne()
    // {
    //     Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(_characterController, groundMask);
    //     float angle = Vector3.Angle(normal, Vector3.up);
    //     print(angle);
    //     bool validAngle = angle <= _characterController.slopeLimit;

    //     return _characterController.isGrounded && validAngle;
    // }



    // private bool CanRun()
    // {
    //     // This means player is moving diagonally at 45 degrees or forward, if so, we can run
    //     return _playerLocomotionInput.MovementInput.y >= Mathf.Abs(_playerLocomotionInput.MovementInput.x);
    // }
    // #endregion

    #region Startup
    private void Awake()
    {
        _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        _playerState = GetComponent<PlayerState>();

        _antiBump = sprintSpeed;
        _stepOffset = _characterController.stepOffset;
    }
    #endregion

    #region Update Logic
    private void Update()
    {
        UpdateMovementState();
        print(_characterController.velocity);

        HandleVerticalMovement();
        HandleLateralMovement();
    }

    private void UpdateMovementState()
    {
        _lastMovementState = _playerState.CurrentPlayerMovementState;

        bool canRun = CanRun();
        bool isMovementInput = _playerLocomotionInput.MovementInput != Vector2.zero;             //order
        bool isMovingLaterally = IsMovingLaterally();                                            //matters
        bool isSprinting = _playerLocomotionInput.SprintToggledOn && isMovingLaterally;          //order
        bool isWalking = isMovingLaterally && (!canRun || _playerLocomotionInput.WalkToggledOn); //matters
        bool isGrounded = IsGrounded();

        PlayerMovementState lateralState = isWalking ? PlayerMovementState.Walking :
                                           isSprinting ? PlayerMovementState.Sprinting :
                                           isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;

        _playerState.SetPlayerMovementState(lateralState);

        // Control Airborn State
        if ((!isGrounded || _jumpedLastFrame) && _characterController.velocity.y > 0f)
        {
            _playerState.SetPlayerMovementState(PlayerMovementState.Jumping);
            _jumpedLastFrame = false;
            _characterController.stepOffset = 0f;
        }
        else if ((!isGrounded || _jumpedLastFrame) && _characterController.velocity.y <= 0f)
        {
            _playerState.SetPlayerMovementState(PlayerMovementState.Falling);
            _jumpedLastFrame = false;
            _characterController.stepOffset = 0f;
        }
        else
        {
            _characterController.stepOffset = _stepOffset;
        }
    }

    private void HandleVerticalMovement()
    {
        bool isGrounded = _playerState.InGroundedState();

        _verticalVelocity -= gravity * Time.deltaTime;

        if (isGrounded && _verticalVelocity < 0)
            _verticalVelocity = -_antiBump;

        if (_playerLocomotionInput.JumpPressed && isGrounded)
        {
            _verticalVelocity += Mathf.Sqrt(jumpSpeed * 3 * gravity);
            _jumpedLastFrame = true;
        }

        if (_playerState.IsStateGroundedState(_lastMovementState) && !isGrounded)
        {
            _verticalVelocity += _antiBump;
        }

        // Clamp at terminal velocity
        if (Mathf.Abs(_verticalVelocity) > Mathf.Abs(terminalVelocity))
        {
            _verticalVelocity = -1f * Mathf.Abs(terminalVelocity);
        }
    }

    private void HandleLateralMovement()
    {
        // Create quick references for current state
        bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
        bool isGrounded = _playerState.InGroundedState();
        bool isWalking = _playerState.CurrentPlayerMovementState == PlayerMovementState.Walking;

        // State dependent acceleration and speed
        float lateralAcceleration = !isGrounded ? inAirAcceleration :
                                    isWalking ? walkAcceleration :
                                    isSprinting ? sprintAcceleration : runAcceleration;

        float clampLateralMagnitude = !isGrounded ? sprintSpeed :
                                      isWalking ? walkSpeed :
                                      isSprinting ? sprintSpeed : runSpeed;

        Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
        Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
        Vector3 movementDirection = cameraRightXZ * _playerLocomotionInput.MovementInput.x + cameraForwardXZ * _playerLocomotionInput.MovementInput.y;

        Vector3 movementDelta = movementDirection * lateralAcceleration * Time.deltaTime;
        Vector3 newVelocity = _characterController.velocity + movementDelta;

        // Add drag to player
        float dragMagnitude = isGrounded ? drag : inAirDrag;
        Vector3 currentDrag = newVelocity.normalized * dragMagnitude * Time.deltaTime;
        newVelocity = (newVelocity.magnitude > dragMagnitude * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(new Vector3(newVelocity.x, 0f, newVelocity.z), clampLateralMagnitude);
        newVelocity.y += _verticalVelocity;
        newVelocity = !isGrounded ? HandleSteepWalls(newVelocity) : newVelocity;

        // Move character (Unity suggests only calling this once per tick)
        _characterController.Move(newVelocity * Time.deltaTime);
    }

    private Vector3 HandleSteepWalls(Vector3 velocity)
    {
        Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(_characterController, groundMask);
        float angle = Vector3.Angle(normal, Vector3.up);
        bool validAngle = angle <= _characterController.slopeLimit;

        if (!validAngle && _verticalVelocity < 0f)
            velocity = Vector3.ProjectOnPlane(velocity, normal);

        return velocity;
    }
    #endregion

    #region Late Update Logic
    private void LateUpdate()
    {
        UpdateCameraRotation();
    }

    private void UpdateCameraRotation()
    {
        _cameraRotation.x += lookSenseH * _playerLocomotionInput.LookInput.x;
        _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSenseV * _playerLocomotionInput.LookInput.y, -lookLimitV, lookLimitV);

        _playerTargetRotation.x += transform.eulerAngles.x + lookSenseH * _playerLocomotionInput.LookInput.x;

        float rotationTolerance = 90f;
        bool isIdling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
        IsRotatingToTarget = _rotatingToTargetTimer > 0;

        // ROTATE if we're not idling
        if (!isIdling)
        {
            RotatePlayerToTarget();
        }
        // If rotation mismatch not within tolerance, or rotate to target is active, ROTATE
        else if (Mathf.Abs(RotationMismatch) > rotationTolerance || IsRotatingToTarget)
        {
            UpdateIdleRotation(rotationTolerance);
        }

        _playerCamera.transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);

        // Get angle between camera and player
        Vector3 camForwardProjectedXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
        Vector3 crossProduct = Vector3.Cross(transform.forward, camForwardProjectedXZ);
        float sign = Mathf.Sign(Vector3.Dot(crossProduct, transform.up));
        RotationMismatch = sign * Vector3.Angle(transform.forward, camForwardProjectedXZ);
    }

    private void UpdateIdleRotation(float rotationTolerance)
    {
        // Initiate new rotation direction
        if (Mathf.Abs(RotationMismatch) > rotationTolerance)
        {
            _rotatingToTargetTimer = rotateToTargetTime;
            _isRotatingClockwise = RotationMismatch > rotationTolerance;
        }
        _rotatingToTargetTimer -= Time.deltaTime;

        // Rotate player
        if (_isRotatingClockwise && RotationMismatch > 0f ||
            !_isRotatingClockwise && RotationMismatch < 0f)
        {
            RotatePlayerToTarget();
        }
    }

    private void RotatePlayerToTarget()
    {
        Quaternion targetRotationX = Quaternion.Euler(0f, _playerTargetRotation.x, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotationX, playerModelRotationSpeed * Time.deltaTime);
    }
    #endregion

    #region State Checks
    private bool IsMovingLaterally()
    {
        Vector3 lateralVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z);

        return lateralVelocity.magnitude > movingThreshold;
    }

    private bool IsGrounded()
    {
        bool grounded = _playerState.InGroundedState() ? IsGroundedWhileGrounded() : IsGroundedWhileAirborne();

        return grounded;
    }

    private bool IsGroundedWhileGrounded()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _characterController.radius, transform.position.z);

        bool grounded = Physics.CheckSphere(spherePosition, _characterController.radius, groundMask, QueryTriggerInteraction.Ignore);

        return grounded;
    }

    private bool IsGroundedWhileAirborne()
    {
        Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(_characterController, groundMask);
        float angle = Vector3.Angle(normal, Vector3.up);
        bool validAngle = angle <= _characterController.slopeLimit;

        return _characterController.isGrounded && validAngle;
    }

    private bool CanRun()
    {
        // This means player is moving diagonally at 45 degrees or forward, if so, we can run
        return _playerLocomotionInput.MovementInput.y >= Mathf.Abs(_playerLocomotionInput.MovementInput.x);
    }
    #endregion

}
