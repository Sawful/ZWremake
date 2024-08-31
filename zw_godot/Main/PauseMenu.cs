using Godot;
using System;

public partial class PauseMenu : Control
{
	Menu MainMenu;
	PackedScene SettingsScene;
	GameManager CurrentMain;
	public override void _Ready()
	{
		MainMenu = (Menu)ResourceLoader.Load<PackedScene>("res://Main/Menu.tscn").Instantiate();
		CurrentMain = GetTree().Root.GetNode<GameManager>("Main");
		SettingsScene = (PackedScene)ResourceLoader.Load("res://Main/Settings/Settings.tscn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void _on_resume_button_pressed()
	{
		GetTree().Paused = false;
		QueueFree();
	}

	public void _on_settings_button_pressed()
	{
		// Instantiate settings node as child
		Node settingsScene = SettingsScene.Instantiate();
		AddChild(settingsScene);
	}
	public void _on_exit_button_pressed()
	{
		GetTree().Paused = false;
		GetTree().Root.AddChild(MainMenu);
		CurrentMain.QueueFree();
	}
	
}
