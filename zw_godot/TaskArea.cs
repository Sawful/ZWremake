using Godot;
using System;
using System.Numerics;

public partial class TaskArea : Area3D
{
	const float ConfirmationTime = 1.5f;
	GameManager Main;
	Player Player;
	Timer ConfirmationTimer;
	MeshInstance3D AreaIndicator;
	Godot.Vector3 AreaSize = new();
	bool Task;

	public override void _Ready()
	{
		Main = GetTree().Root.GetNode<GameManager>("Main");
		Player = Main.GetNode<Player>("Player");
		ConfirmationTimer = GetNode<Timer>("ConfirmationTimer");
		AreaIndicator = GetNode<MeshInstance3D>("AreaIndicator");
		SetTask();
	}

	public override void _Process(double delta)
	{
		AreaSize = Godot.Vector3.Left * (float)ConfirmationTimer.TimeLeft + Godot.Vector3.Up + Godot.Vector3.Forward * (float)ConfirmationTimer.TimeLeft;
		AreaIndicator.Scale = AreaSize;
	}

	public void OnBodyEntered(Node3D body)
	{
		if (body == Player && Task)
		{
			ConfirmationTimer.WaitTime = ConfirmationTime;
			ConfirmationTimer.Start();
		}
	}

	public void OnBodyExited(Node3D body)
	{
		if (body == Player)
		{
			ConfirmationTimer.WaitTime = ConfirmationTime;
			ConfirmationTimer.Stop();
		}
	}

	public void OnTimeout()
	{
		Main.IncreaseThreat(1000);
		GD.Print("Task completed");
		Task = false;
	}

	public void SetTask()
	{
		ConfirmationTimer.WaitTime = 1.5f;
		Task = true;
	}
}
