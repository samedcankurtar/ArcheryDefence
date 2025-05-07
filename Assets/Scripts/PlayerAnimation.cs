using Unity.Cinemachine;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float locomotionBlendSpeed; // Time between animations
    private PlayerLocomotionInput _playerLocomotionInput;
    private PlayerState _playerState;

    //Create single reference and get id number from anim parameters
    private static int inputXHash = Animator.StringToHash("InputX");
    private static int inputYHash = Animator.StringToHash("InputY");
    private static int inputMagnitudeHash = Animator.StringToHash("InputMagnitude");

    public Vector3 _currentBlendInput = Vector3.zero;

    private void Awake()
    {
        _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        _playerState = GetComponent<PlayerState>();
    }

    private void Update()
    {
        UpdateAnimationState();
    }


    private void UpdateAnimationState()
    {

        bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
        //Get 2d vector from user movement input
        Vector2 inputTarget = isSprinting ? _playerLocomotionInput.MovementInput * 1.5f : _playerLocomotionInput.MovementInput;
        //Lerp 
        _currentBlendInput = Vector3.Lerp(_currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);


        //Set float animation parameters
        _animator.SetFloat(inputXHash, _currentBlendInput.x);
        _animator.SetFloat(inputYHash, _currentBlendInput.y);
        _animator.SetFloat(inputMagnitudeHash, _currentBlendInput.magnitude);
    }





}
