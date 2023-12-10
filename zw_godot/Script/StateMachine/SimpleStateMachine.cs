using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class SimpleStateMachine : Node
{
	[Signal] public delegate void PreStartEventHandler();

	[Signal] public delegate void PostStartEventHandler();

	[Signal] public delegate void PreExitEventHandler();

	[Signal] public delegate void PostExitEventHandler();

	public List<SimpleState> States;

	public string CurrentState;

	public string LastState;

	protected SimpleState state = null;

	public override void _Ready()
	{
		States = GetNode<Node>("States").GetChildren().OfType<SimpleState>().ToList();
	}

	private void SetState(SimpleState _state, Dictionary<string, object> message)
	{
		if(_state == null)
		{
			return;
		}
		if (state != null) 
		{
			EmitSignal(nameof(PreExit));
			state.OnExit(_state.GetType().ToString());
			EmitSignal(nameof(PostExit));
		}

		LastState = CurrentState;
		CurrentState = _state.GetType().ToString();

		state = _state;
		EmitSignal(nameof(PreStart));
		state.OnStart(message);
		EmitSignal(nameof(PostStart));
		state.OnUpdate();
	}

	public void ChangeState(string stateName, Dictionary<string, object> message = null)
	{
		foreach(SimpleState _state in States)
		{
			if (stateName == _state.GetType().ToString())
			{
				SetState(_state, message);
				return;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if(state == null)
			return;
		state.UpdateState(delta);
	}
}
