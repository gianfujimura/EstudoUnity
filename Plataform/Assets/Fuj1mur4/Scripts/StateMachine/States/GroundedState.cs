using System.Data.Common;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GroundedState : State
{
    [Header("Player Context")]
    readonly PlayerContext ctx;

    [Header("Child States")]
    public readonly State idleState;
    public readonly State moveState;
    public readonly State crouchState;

    public GroundedState(StateMachine stateMachine, State parent, PlayerContext ctx) : base(stateMachine, parent)
    {
        this.ctx = ctx;
        this.idleState = new IdleState(stateMachine, this, this.ctx);
        this.moveState = new MoveState(stateMachine, this, this.ctx);
        this.crouchState = new CrouchState(stateMachine, this, this.ctx);
    }

    protected override State GetInitialState() => idleState;
    protected override State GetTransition() 
    {
        if (!ctx.isGrounded)
        {
            return ((RootState)parent).AirborneState;
        }

        return null;
    }

    protected override void OnUpdate(float deltaTime)
    {
        if (ctx.isJumpRequested)
        {
            ctx.Jump();
        }
    }
}
