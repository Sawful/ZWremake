using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;

public partial class SimpleState : Node
{
	private bool HasBeenInitialized = false;
    
    public bool Enabled = false;

	private bool OnUpdateHasFired = false;
    // State Events

    [Signal] public delegate void StateStartEventHandler();

    [Signal] public delegate void StateUpdatedEventHandler();

    [Signal] public delegate void StateExitedEventHandler();

    public virtual void OnStart(Dictionary<string, object> message)
	{
		EmitSignal(nameof(StateStart));
		HasBeenInitialized = true;
        Enabled = true;
	}

	public virtual void OnUpdate()
	{
		if (!HasBeenInitialized)
			return;
		EmitSignal(nameof (StateUpdated));
		OnUpdateHasFired = true;
	}

    public virtual void UpdateState(double dt)
    {
        if (!OnUpdateHasFired)
			return;
    }

    public virtual void OnExit(string NextState)
    {
        if (!HasBeenInitialized)
            return;
        EmitSignal(nameof(StateExited));
        HasBeenInitialized = false;
        Enabled = false;
		OnUpdateHasFired = false;
    }
}
