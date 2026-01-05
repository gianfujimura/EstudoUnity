using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.Tilemaps;

public class StateMachine
{
    public readonly TransitionSequencer sequencer;
    public readonly State rootState;

    private bool _started;

    public StateMachine(State playerRoot)
    {
        this.sequencer = new TransitionSequencer(this);
        this.rootState = playerRoot;
    }

    public void Start()
    {
        if(_started) return;

        _started = true;
        rootState.Enter();
    }

    public void Tick(float deltaTime)
    {
        if (!_started) Start();
        sequencer.Tick(deltaTime);
    }

    internal void InternalTick(float deltaTime) => rootState.Update(deltaTime);

    // Perform the actual switch from 'from' to 'to' by exiting up to the shared ancestor, then entering down to the target
    public void ChangeState(State from, State to)
    {
        if (from == to || from == null || to == null) return;

        State lca = TransitionSequencer.Lca(from, to);

        // Exit current branch up to (but not including) LCA
        for (State s = from; s != lca; s = s.parent) s.Exit();

        // Enter target branch from LCA down to target
        var stack = new Stack<State>();
        for (State s = to; s != lca; s = s.parent) stack.Push(s);
        while(stack.Count > 0) stack.Pop().Enter();
    }
}
