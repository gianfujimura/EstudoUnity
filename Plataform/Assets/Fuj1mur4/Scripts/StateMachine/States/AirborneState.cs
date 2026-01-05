using UnityEngine;

public class AirborneState : State
{
    [Header("Player Context")]
    public PlayerContext PlayerContext;

    public AirborneState(StateMachine stateMachine, State parent, PlayerContext playerContext) : base(stateMachine, parent)
    {
        this.PlayerContext = playerContext;
    }

    protected override State GetInitialState() => null;
    protected override State GetTransition() => PlayerContext.IsGrounded ? ((RootState)parent).GroundedState : null;
}
