using Unity.Cinemachine;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float locomotionBlendSpeed; // Time between animations
    private PlayerLocomotionInput _playerLocomotionInput;
    private PlayerState _playerState;
    private PlayerController _playerController;

    //Create single reference and get id number from anim parameters
    private static int inputXHash = Animator.StringToHash("InputX");
    private static int inputYHash = Animator.StringToHash("InputY");
    private static int inputMagnitudeHash = Animator.StringToHash("InputMagnitude");
    private static int isIdlingHash = Animator.StringToHash("IsIdling");
    private static int isRotatingToTargethash = Animator.StringToHash("IsRotatingToTarget");
    public static int isGroundedHash = Animator.StringToHash("IsGrounded");
    public static int isJumpingHash = Animator.StringToHash("IsJumping");
    public static int isFallingHash = Animator.StringToHash("IsFalling");
    private static int rotationMismatchHash = Animator.StringToHash("RotationMismatch");

    public Vector3 _currentBlendInput = Vector3.zero;

    private void Awake()
    {
        _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        _playerState = GetComponent<PlayerState>();
        _playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        UpdateAnimationState();
    }


    private void UpdateAnimationState()
    {
        //redundant but for readibility
        bool isIdling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
        bool isRunning = _playerState.CurrentPlayerMovementState == PlayerMovementState.Running;
        bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
        bool isJumping = _playerState.CurrentPlayerMovementState == PlayerMovementState.Jumping;
        bool isFalling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Falling;
        bool isGrounded = _playerState.InGroundedState();


        //Get 2d vector from user movement input
        Vector2 inputTarget = isSprinting ? _playerLocomotionInput.MovementInput * 1.5f : isRunning ? _playerLocomotionInput.MovementInput * 1f : _playerLocomotionInput.MovementInput * 0.5f;
        //Lerp 
        _currentBlendInput = Vector3.Lerp(_currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);


        //Set float animation parameters
        _animator.SetBool(isGroundedHash, isGrounded);
        _animator.SetBool(isFallingHash, isFalling);
        _animator.SetBool(isJumpingHash, isJumping);
        _animator.SetBool(isIdlingHash, isIdling);
        _animator.SetBool(isRotatingToTargethash, _playerController.IsRotatingToTarget);

        _animator.SetFloat(inputXHash, _currentBlendInput.x);
        _animator.SetFloat(inputYHash, _currentBlendInput.y);
        _animator.SetFloat(inputMagnitudeHash, _currentBlendInput.magnitude);
        _animator.SetFloat(rotationMismatchHash, _playerController.RotationMismatch);
    }





}
