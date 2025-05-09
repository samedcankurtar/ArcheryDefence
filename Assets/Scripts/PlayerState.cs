using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [field: SerializeField] public PlayerMovementState CurrentPlayerMovementState { get; private set; } = PlayerMovementState.Idling;

    //Set player movement state
    public void SetPlayerMovementState(PlayerMovementState playerMovementState)
    {
        CurrentPlayerMovementState = playerMovementState;
    }
    //Get grounded state with helper method
    public bool InGroundedState()
    {
        return IsStateGroundedState(CurrentPlayerMovementState);
    }

    public bool IsStateGroundedState(PlayerMovementState movementState)
    {
        return movementState == PlayerMovementState.Idling ||
               movementState == PlayerMovementState.Walking ||
               movementState == PlayerMovementState.Running ||
               movementState == PlayerMovementState.Sprinting;
    }


}

public enum PlayerMovementState
{
    Idling = 0,
    Walking,
    Running,
    Sprinting,
    Jumping,
    Falling,
    Strafing,
}
