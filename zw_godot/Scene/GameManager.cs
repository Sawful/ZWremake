using Godot;
using System;

public partial class GameManager : Node3D
{
    [Export] public Node3D SpawnLocation;
    PackedScene enemyScene;
    Vector3 SpawnPosition;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SpawnPosition = SpawnLocation.Position;
        enemyScene = (PackedScene)ResourceLoader.Load("res://Scene/Prefabs/enemy.tscn");

        Round1();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    void SpawnObject(PackedScene obj, Vector3 pos, int number)
    {
        for (int i = 0; i < number; i++)
        {
            Enemy new_enemy = (Enemy)obj.Instantiate();
            new_enemy.Position = pos;
            AddChild(new_enemy);
        }
    }

    void Round1()
    {
        SpawnObject(enemyScene, SpawnPosition, 5);
    }
}
