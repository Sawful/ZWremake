using Godot;
using System;

public partial class Settings : Control
{
	public void _on_exit_settings_button_pressed()
	{
		QueueFree();
	}
}
