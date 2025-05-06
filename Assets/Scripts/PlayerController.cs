using UnityEngine;
using UnityEngine.TextCore.Text;

[DefaultExecutionOrder(-1)]
public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Camera _playerCamera;
    private PlayerLocomotionInput _playerLocomotionInput;

    [Header("Basic Movement")]
    public float runAcceleration = 0.25f;
    public float runSpeed = 4f;
    public float drag;

    [Header("Camera Settings")]
    public float lookSenseH = 0.1f;
    public float lookSenseV = 0.1f;
    public float lookLimitV = 89f;

    public Vector2 _cameraRotation = Vector2.zero;
    public Vector2 _playerTargetRotation = Vector2.zero;




    private void Awake()
    {    //assign reference, get component
        _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
    }

    private void Update()
    {

        Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;

        Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;

        Vector3 movementDirection = cameraRightXZ * _playerLocomotionInput.MovementInput.x + cameraForwardXZ * _playerLocomotionInput.MovementInput.y;

        Vector3 movementDelta = movementDirection * runAcceleration * Time.deltaTime;
        Vector3 newVelocity = _characterController.velocity + movementDelta;

        //Add drag to player
        Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
        //Apply drag if character is moving, else, 0 drag
        newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        //Limit speed
        newVelocity = Vector3.ClampMagnitude(newVelocity, runSpeed);

        //Move character (unity suggests only calling this once per tick)
        _characterController.Move(newVelocity * Time.deltaTime);
    }

    private void LateUpdate()
    {
        _cameraRotation.x += lookSenseH * _playerLocomotionInput.LookInput.x;
        _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSenseV * _playerLocomotionInput.LookInput.y, -lookLimitV, lookLimitV);

    }
}
