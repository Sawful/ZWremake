using Godot;
using System;

public partial class DeathMenu : Control
{
	PackedScene MainMenuScene;
	GameManager Main;
	GameManager CurrentMain;
	public override void _Ready()
	{
		MainMenuScene = (PackedScene)ResourceLoader.Load("res://Main/Menu.tscn");
		Main = (GameManager)ResourceLoader.Load<PackedScene>("res://Main/Main.tscn").Instantiate();
		CurrentMain = GetTree().Root.GetNode<GameManager>("Main");
	}
	public void _on_retry_button_pressed()
	{
		GetTree().Paused = false;
		GetTree().Root.AddChild(Main);
		CurrentMain.QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void _on_main_menu_button_pressed()
	{
		GetTree().Paused = false;
		GetTree().Root.AddChild(MainMenuScene.Instantiate());
		CurrentMain.QueueFree();
	}
}
