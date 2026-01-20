using UnityEngine;

public class WalkState : State
{
    [Header("Player Context")]
    readonly PlayerContext ctx;

    public WalkState(StateMachine stateMachine, State parent, PlayerContext ctx) : base(stateMachine, parent)
    {
        this.ctx = ctx;
    }

    protected override State GetInitialState() => null;
    protected override State GetTransition()
    {
        if (ctx.isRunRequested)
        {
            return ((MoveState)parent).runState;
        }

        return null; 
    }

    protected override void OnUpdate(float deltaTime)
    {
        ctx.CalculateGroundedVelocity(
            ctx.walkSpeed, 
            ctx.walkAcceleration, 
            ctx.walkDeceleration, 
            deltaTime);
    }
}
