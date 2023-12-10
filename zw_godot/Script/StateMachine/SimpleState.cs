using Godot;
using System;
using System.Collections.Generic;

public partial class SimpleState : Node
{
	private bool HasBeenInitialized = false;

	private bool OnUpdateHasFired = false;
    // State Events

    [Signal] public delegate void StateStartEventHandler();

    [Signal] public delegate void StateUpdatedEventHandler();

    [Signal] public delegate void StateExitedEventHandler();


    public virtual void OnStart(Dictionary<string, object> message)
	{
		EmitSignal(nameof(StateStart));
		HasBeenInitialized = true;
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
		OnUpdateHasFired = false;
    }
}
