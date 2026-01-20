using UnityEngine;

public class CrouchState : State
{
    [Header("Player Context")]
    readonly PlayerContext ctx;

    public CrouchState(StateMachine stateMachine, State parent, PlayerContext ctx) : base(stateMachine, parent)
    {
        this.ctx = ctx;
    }

    protected override State GetInitialState() => null;

    protected override State GetTransition()
    {
        if (!ctx.isCrouchRequested)
        {
            return ((GroundedState)parent).idleState;
        }

        return null;
    }

    protected override void OnUpdate(float deltaTime)
    {
        ctx.CalculateGroundedVelocity(
            ctx.crouchSpeed,
            ctx.crouchAcceleration,
            ctx.crouchDeceleration,
            deltaTime);
    }

    protected override void OnEnter()
    {
        ctx.Crouch();
    }
    protected override void OnExit()
    {
        ctx.StandUp();
    }

}
