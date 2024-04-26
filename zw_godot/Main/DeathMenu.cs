using Godot;
using System;

public partial class DeathMenu : Control
{
	PackedScene MainMenuScene;
	PackedScene MainScene;
	public override void _Ready()
	{
		MainMenuScene = (PackedScene)ResourceLoader.Load("res://Main/Menu.tscn");
		MainScene = (PackedScene)ResourceLoader.Load("res://Main/Main.tscn");
	}
	public void _on_retry_button_pressed()
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToPacked(MainScene);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void _on_main_menu_button_pressed()
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToPacked(MainMenuScene);
	}
}
