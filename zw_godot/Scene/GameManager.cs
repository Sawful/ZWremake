using Godot;
using System;

public partial class GameManager : Node3D
{
    [Export] public Marker3D SpawnLocation;
    PackedScene EnemyScene;
    PackedScene TankEnemyScene;
    Vector3 SpawnPosition;

    public int TimeSeconds = 0;
    Label TimeDisplay;


    //double time_start = 0;
    //double time_now = 0;
    public override void _Ready()
    {
        TimeDisplay = GetNode<CenterContainer>("TopBarDisplay").GetNode<Label>("TimeDisplay");
        TimeDisplay.Text = TimeSeconds.ToString();

        SpawnPosition = SpawnLocation.Position;

        //time_start = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        EnemyScene = (PackedScene)ResourceLoader.Load("res://Scene/Prefabs/enemy.tscn");
        TankEnemyScene = (PackedScene)ResourceLoader.Load("res://Scene/Prefabs/tankenemy.tscn");

        Round1();
    }
    public override void _Process(double delta)
    {


        // POUR LE JOUR OU JE VEUX FAIRE UN "OPENED SINCE: ...HOURS / ...MINUTES / ...SECONDS"
        //time_now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        //double time_elapsed = time_now - time_start;
    }

    void SpawnObject(PackedScene obj, Vector3 pos, int number)
    {
        for (int i = 0; i < number; i++)
        {
            Random rand = new Random();

            Enemy new_enemy = (Enemy)obj.Instantiate();
            new_enemy.Position = pos + new Vector3((float)rand.NextDouble(), 0, (float)rand.NextDouble());
            AddChild(new_enemy);
        }
    }

    void _on_seconds_timeout()
    {
        TimeSeconds++;
        TimeDisplay.Text = TimeSeconds.ToString();
    }

    async void Round1()
    {
        SpawnObject(EnemyScene, SpawnPosition, 5);

        await ToSignal(GetTree().CreateTimer(5), "timeout");

        SpawnObject(EnemyScene, SpawnPosition, 5);

        await ToSignal(GetTree().CreateTimer(5), "timeout");

        SpawnObject(TankEnemyScene, SpawnPosition, 1);
    }
}
