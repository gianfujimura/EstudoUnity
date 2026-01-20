using UnityEngine;

public class RunState : State
{
    [Header("Player Context")]
    readonly PlayerContext ctx;

    public RunState(StateMachine stateMachine, State parent, PlayerContext ctx) : base(stateMachine, parent)
    {
        this.ctx = ctx;
    }

    protected override State GetInitialState() => null;

    protected override State GetTransition()
    {
        if (!ctx.isRunRequested)
        {
            return ((MoveState)parent).walkState;
        }

        return null;
    }

    protected override void OnUpdate(float deltaTime)
    {
        ctx.CalculateGroundedVelocity(
            ctx.runSpeed, 
            ctx.runAcceleration, 
            ctx.runDeceleration, 
            deltaTime);
    }
}
