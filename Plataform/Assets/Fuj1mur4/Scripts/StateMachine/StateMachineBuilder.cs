using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class StateMachineBuilder
{
    readonly State root;

    public StateMachineBuilder(State root)
    {
        this.root = root;
    }

    public StateMachine Build()
    {
        var m = new StateMachine(root);
        Wire(root, m, new HashSet<State>());
        return m;
    }

    public void Wire(State s, StateMachine m, HashSet<State> visited)
    {
        if (s == null) return;
        if (!visited.Add(s)) return; // state is already wired

        var flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
        var machineField = typeof(State).GetField("machine", flags);
        if (machineField != null) machineField.SetValue(s, m);

        foreach(var fld in s.GetType().GetFields(flags))
        {
            if (!typeof(State).IsAssignableFrom(fld.FieldType)) continue;
            if (fld.Name == "parent") continue;

            var child = (State)fld.GetValue(s);
            if (child == null) continue;
            if (!ReferenceEquals(child.parent, s)) continue; 

            Wire(child, m, visited);
        }
    }
}
