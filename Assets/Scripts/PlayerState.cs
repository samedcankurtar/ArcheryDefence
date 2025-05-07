using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [field: SerializeField] public PlayerMovementState CurrentPlayerMovementState { get; private set; } = PlayerMovementState.Idling;

    //Set player movement state
    public void SetPlayerMovementState(PlayerMovementState playerMovementState)
    {
        CurrentPlayerMovementState = playerMovementState;
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
