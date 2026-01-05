using UnityEngine;

public class RootState : State
{
    [Header("Player Context")]
    public PlayerContext PlayerContext;

    [Header("Child States")]
    public GroundedState GroundedState;
    public AirborneState AirborneState;

    public RootState(StateMachine stateMachine, PlayerContext playerContext) : base(stateMachine, null)
    {
        this.PlayerContext = playerContext;
        this.GroundedState = new GroundedState(stateMachine, this, playerContext);
        this.AirborneState = new AirborneState(stateMachine, this, playerContext);
    }

    protected override State GetInitialState() => GroundedState;
    protected override State GetTransition() => PlayerContext.IsGrounded ? null : AirborneState;
}
