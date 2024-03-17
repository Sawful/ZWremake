using Godot;
using System;

public partial class ResolutionMenuButton : MenuButton
{
	PopupMenu Popup;
	public override void _Ready()
	{
		Callable callable = new Callable(this, MethodName.ChangeResolution);
		Popup = GetPopup();
		Popup.Connect("id_pressed", callable);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ChangeResolution(int id)
	{
		Vector2I resolution = new();
		if(id == 0){resolution = new (1920, 1080); }
		if(id == 1){resolution = new (1600, 900); }
		if(id == 2){resolution = new (1440, 900); }
		if(id == 3){resolution = new (1366, 768); }
		if(id == 4){resolution = new (1280, 1024); }

		DisplayServer.WindowSetSize(resolution);
		Text = resolution.ToString();
		GD.PrintS(id);
	}
}
