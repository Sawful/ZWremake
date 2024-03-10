using Godot;
using System;

public partial class Menu : Control
{
	PackedScene MainScene;
	private PlayerInfo PlayerInfo;
	public override void _Ready()
	{
		MainScene = (PackedScene)ResourceLoader.Load("res://Main/Main.tscn");
		PlayerInfo = GetNode<PlayerInfo>("/root/PlayerInfo");
		PlayerInfo.PlayerClass = "Warrior";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_start_button_pressed()
	{
		GetTree().ChangeSceneToPacked(MainScene);
	}

	public void _on_settings_button_pressed()
	{
		// Instantiate settings node as child
	}
	public void _on_exit_button_pressed()
	{
		GetTree().Quit();
	}

	public void _on_warrior_check_box_toggled(bool toggled_on)
	{
		if(toggled_on){PlayerInfo.PlayerClass = "Warrior";}
	}

	public void _on_sorcerer_check_box_toggled(bool toggled_on)
	{
		if(toggled_on){PlayerInfo.PlayerClass = "Sorcerer";}
	}

	public void _on_archer_check_box_toggled(bool toggled_on)
	{
		if(toggled_on){PlayerInfo.PlayerClass = "Archer";}
	}
	
}
