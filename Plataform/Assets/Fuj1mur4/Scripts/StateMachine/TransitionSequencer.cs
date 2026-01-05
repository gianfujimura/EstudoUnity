using NUnit.Framework;
using System.Collections.Generic;

public class TransitionSequencer
{
    public readonly StateMachine stateMachine;

    public TransitionSequencer(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public void RequestTransition(State from, State to)
    {
        if (from == to || from == null || to == null) return;
        
        stateMachine.ChangeState(from, to);
    }

    public void Tick(float deltaTime)
    {
        stateMachine.InternalTick(deltaTime);
    }

    public static State Lca(State a, State b)
    {
        // Create a set of all parents of 'a'
        var ap = new HashSet<State>();
        for (State s = a; s != null; s = s.parent) ap.Add(s);

        // Find the common ancestor between 'a' and 'b'
        for (State s = b; s != null; s = s.parent)
        {
            if (ap.Contains(s))
            {
                return s;
            }
        }

        // If don't exist a common ancestor, return null
        return null;
    }
}
