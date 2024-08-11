using Godot;
using System;

public partial class TaskArea : Area3D
{
	const float ConfirmationTime = 1.5f;
	GameManager Main;
	Player Player;
	Timer ConfirmationTimer;

	public override void _Ready()
	{
		Main = GetTree().Root.GetNode<GameManager>("Main");
		Player = Main.GetNode<Player>("Player");
		ConfirmationTimer = GetNode<Timer>("ConfirmationTimer");
	}

	public void OnBodyEntered(Node3D body)
	{
		if (body == Player)
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
	}
}
