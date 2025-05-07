using UnityEngine;
using UnityEngine.TextCore.Text;

[DefaultExecutionOrder(-1)]
public class PlayerController : MonoBehaviour
{
    #region  Class Components
    [Header("Components")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Camera _playerCamera;
    private PlayerLocomotionInput _playerLocomotionInput;
    private PlayerState _playerState;


    [Header("Basic Movement")]
    public float runAcceleration = 0.25f;
    public float runSpeed = 4f;
    public float drag;
    public float sprintAcceleration = 0.5f;
    public float sprintSpeed = 7f;
    public float movingThreshold = 0.01f; //To check movement with so small value

    [Header("Camera Settings")]
    public float lookSenseH = 0.1f;
    public float lookSenseV = 0.1f;
    public float lookLimitV = 89f;



    public Vector2 _cameraRotation = Vector2.zero;
    public Vector2 _playerTargetRotation = Vector2.zero;

    #endregion

    #region Startup
    private void Awake()
    {    //assign reference, get component
        _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        _playerState = GetComponent<PlayerState>();
    }
    #endregion


    #region UpdateLogic
    private void Update()
    {
        UpdateMovementState();
        HandleLateralMovement();
    }

    private void UpdateMovementState()
    {
        //Check if there is any movement input value
        bool isMovementInput = _playerLocomotionInput.MovementInput != Vector2.zero;
        bool isMovingLaterally = IsMovingLaterally();
        bool isSprinting = _playerLocomotionInput.SprintToggledOn && isMovingLaterally;
        //Set state running if there's movement input and movement speed, else idle
        PlayerMovementState lateralState = isSprinting ? PlayerMovementState.Sprinting : isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;
        //Set movement state
        _playerState.SetPlayerMovementState(lateralState);
    }



    private void HandleLateralMovement()
    {
        //Create quick reference(to make it more readable, define extra bool)
        bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;

        float lateralAcceleration = isSprinting ? sprintAcceleration : runAcceleration;
        float clampLateralMagnitude = isSprinting ? sprintSpeed : runSpeed;

        //Get camera forward on XZ 
        Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
        //Get camera right on XZ 
        Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;

        Vector3 movementDirection = cameraRightXZ * _playerLocomotionInput.MovementInput.x + cameraForwardXZ * _playerLocomotionInput.MovementInput.y;

        Vector3 movementDelta = movementDirection * lateralAcceleration;
        Vector3 newVelocity = _characterController.velocity + movementDelta;

        //Add drag to player
        Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
        //Apply drag if character is moving, else, 0 drag
        newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        //Limit speed
        newVelocity = Vector3.ClampMagnitude(newVelocity, clampLateralMagnitude);

        //Move character (unity suggests only calling this once per tick)
        _characterController.Move(newVelocity * Time.deltaTime);
    }
    #endregion

    #region LateUpdate Logic
    private void LateUpdate()
    {
        //Camera rotation with mouse
        _cameraRotation.x += lookSenseH * _playerLocomotionInput.LookInput.x;

        _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSenseV * _playerLocomotionInput.LookInput.y, -lookLimitV, lookLimitV);

        _playerTargetRotation.x += transform.eulerAngles.x + lookSenseH * _playerLocomotionInput.LookInput.x;
        transform.rotation = Quaternion.Euler(0f, _playerTargetRotation.x, 0f);

        _playerCamera.transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);

    }
    #endregion


    #region State Checks
    private bool IsMovingLaterally()
    {
        Vector3 lateralVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z);

        return lateralVelocity.magnitude > movingThreshold;
    }
    #endregion
}
