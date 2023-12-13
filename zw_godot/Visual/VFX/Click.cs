using Godot;
using System;

public partial class Click : Node3D
{
	void OnAnimationFinished(StringName animName)
	{
		QueueFree();
	}
}