using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-2)] //Change execution order(run this earlier and get inputs first)
public class PlayerLocomotionInput : MonoBehaviour, PlayerControls.IPlayerLocomotionActions
{
  //init PlayerControls 
  public PlayerControls PlayerControls { get; private set; } //prop, can access, can't set from outside

  //Movement inputs
  public Vector2 MovementInput { get; private set; } //WASD
  public Vector2 LookInput { get; private set; } //Mouse input

  private void OnEnable()
  {
    //Enable PlayerControls
    PlayerControls = new PlayerControls();
    PlayerControls.Enable();

    //Enable PlayerControls.PlayerLocomotionMap
    PlayerControls.PlayerLocomotion.Enable();
    PlayerControls.PlayerLocomotion.SetCallbacks(this);
  }


  private void OnDisable()
  {
    //Disable PlayerControls.PlayerLocomotionMap
    PlayerControls.PlayerLocomotion.Disable();
    PlayerControls.PlayerLocomotion.RemoveCallbacks(this);
  }


  public void OnMovement(InputAction.CallbackContext context)
  {
    MovementInput = context.ReadValue<Vector2>();
  }

  public void OnLook(InputAction.CallbackContext context)
  {
    LookInput = context.ReadValue<Vector2>();
  }
}
