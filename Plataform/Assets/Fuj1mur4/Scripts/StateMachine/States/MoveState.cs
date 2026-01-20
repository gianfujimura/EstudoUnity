using UnityEditor.Tilemaps;
using UnityEngine;

public class MoveState : State
{
    [Header("Player Context")]
    readonly PlayerContext ctx;

    [Header("Child States")]
    public readonly State walkState;
    public readonly State runState;

    public MoveState(StateMachine stateMachine, State parent, PlayerContext ctx) : base(stateMachine, parent)
    {
        this.ctx = ctx;
        walkState = new WalkState(stateMachine, this, ctx);
        runState = new RunState(stateMachine, this, ctx);
    }

    protected override State GetInitialState() => walkState;
    protected override State GetTransition()
    {
        if (!ctx.isMoveRequested)
        {
            return ((GroundedState)parent).idleState;
        }

        if (ctx.isCrouchRequested)
        {
            return ((GroundedState)parent).crouchState;
        }

        return null;
    }
}
