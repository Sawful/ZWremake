using Godot;
using System;

public partial class PauseMenu : Control
{
	PackedScene MainMenuScene;
	PackedScene SettingsScene;
	public override void _Ready()
	{
		MainMenuScene = (PackedScene)ResourceLoader.Load("res://Main/Menu.tscn");
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
		GetTree().ChangeSceneToPacked(MainMenuScene);
	}
	
}
