using Godot;
using System;

public partial class FullscreenCheckBox : CheckBox
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void _on_toggled(bool toggled_on)
	{
		if(toggled_on){DisplayServer.WindowSetMode((DisplayServer.WindowMode)3);}
		else{DisplayServer.WindowSetMode(0);}
	}

}