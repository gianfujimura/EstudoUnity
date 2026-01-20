using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class IdleState : State
{
    [Header("Player Context")]
    readonly PlayerContext ctx;

    public IdleState(StateMachine stateMachine, State parent, PlayerContext ctx) : base(stateMachine, parent)
    {
        this.ctx = ctx;
    }

    protected override State GetInitialState() => null;
    protected override State GetTransition()
    {
        if(ctx.isMoveRequested)
        {
            return ((GroundedState)parent).moveState;
        }

        if (ctx.isCrouchRequested)
        {
            return ((GroundedState)parent).crouchState;
        }

        return null;
    }

    protected override void OnUpdate(float deltaTime)
    {
        ctx.CalculateGroundedVelocity(
            ctx.velocity.x,
            0f,
            ctx.walkDeceleration,
            deltaTime);
    }
}
