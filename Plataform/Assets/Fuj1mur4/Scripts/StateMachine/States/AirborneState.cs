using UnityEngine;

public class AirborneState : State
{
    [Header("Player Context")]
    readonly PlayerContext ctx;

    public AirborneState(StateMachine stateMachine, State parent, PlayerContext ctx) : base(stateMachine, parent)
    {
        this.ctx = ctx;
    }

    protected override State GetInitialState() => null;
    protected override State GetTransition()
    {
        if (ctx.isGrounded)
        {
            return ((RootState)parent).GroundedState;
        }

        return null;
    }

    protected override void OnExit()
    {
        ctx.ResetJump();
    }

    protected override void OnUpdate(float deltaTime)
    {
        if (ctx.isJumpRequested)
        {
            ctx.Jump();
        }

        var target = ctx.move.x * ctx.airSpeed;
        ctx.velocity.x = Mathf.MoveTowards(ctx.velocity.x, target, ctx.airAcceleration * deltaTime);

        ctx.CalculateAirVelocity(
            ctx.airSpeed,
            ctx.airAcceleration,
            ctx.airDeceleration,
            deltaTime);
    }
}
