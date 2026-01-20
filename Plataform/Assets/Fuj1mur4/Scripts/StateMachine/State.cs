using System.Collections.Generic;
using System.Diagnostics;

public abstract class State 
{
    public readonly StateMachine machine;
    public readonly State parent;
    public State activeChild;

    public State(StateMachine stateMachine, State parent)
    {
        this.machine = stateMachine;
        this.parent = parent;
    }

    protected virtual State GetInitialState() => null; // Initial state when this state starts (null = this is the leaf)
    protected virtual State GetTransition() => null; // Target state to switch in this frame (null = continue in this state)


    // lifecycle hook
    protected virtual void OnEnter() { Debug.WriteLine($"Entrei no: {this.GetType()}"); }
    protected virtual void OnUpdate(float deltaTime) { }
    protected virtual void OnExit() { Debug.WriteLine($"Saí do: {this.GetType()}"); }

    internal void Enter()
    {
        if (parent != null)
        {
            parent.activeChild = this;
        }

        OnEnter();

        State initialState = GetInitialState();

        if (initialState != null)
        {
            initialState.Enter();
        }
    }

    internal void Update(float deltaTime)
    {
        State nextState = GetTransition();

        if (nextState != null)
        {
            machine.sequencer.RequestTransition(this, nextState);
            return;
        }

        if(activeChild != null)
        {
            activeChild.Update(deltaTime);
        }

        OnUpdate(deltaTime);
    }

    internal void Exit()
    {
        if (activeChild != null)
        {
            activeChild.Exit();
        }

        activeChild = null;
        OnExit();
    }

    // Return the deepest currently-active descent state (the leaf of the active path)
    public State GetLeaf()
    {
        State leaf = this;

        while (leaf.activeChild != null)
        {
            leaf = leaf.activeChild;
        }

        return leaf;
    }

    // Yields this state and then each ancestor to the root state (this -> parent -> ... -> root)
    public IEnumerable<State> GetPathToRoot()
    {
        for (State state = this; state != null; state = state.parent) yield return state;
    }
}
