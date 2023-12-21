using Godot;
using System;
public partial class GameManager : Node3D
{
    
    [Export] public Marker3D SpawnLocation;
    PackedScene EnemyScene;
    PackedScene TankEnemyScene;

    Vector3 SpawnPosition;
    Control PlayerUI;
    Entity Player;
    ProgressBar HealthBar;

    public bool IsPlayerDead = false;

    public int TimeSeconds = 0;
    Label TimeDisplay;

    public override void _Ready()
    {
        Player = GetNode<Player>("Player");
        PlayerUI = GetNode<Control>("PlayerUI");
        SpawnLocation = GetNode<Marker3D>("SpawnLocation");
        HealthBar = PlayerUI.GetNode<ProgressBar>("HealthBar");
        TimeDisplay = PlayerUI.GetNode<CenterContainer>("TopBarDisplay").GetNode<Label>("TimeDisplay");
        TimeDisplay.Text = TimeSeconds.ToString();

        SpawnPosition = SpawnLocation.Position;

        EnemyScene = (PackedScene)ResourceLoader.Load("res://Enemies/Prefabs/enemy.tscn");
        TankEnemyScene = (PackedScene)ResourceLoader.Load("res://Enemies/Prefabs/tankenemy.tscn");


        Round1();
    }
    public override void _Process(double delta)
    {
        IsPlayerDead = !IsInstanceValid(Player);
    }

    void SpawnObject(PackedScene obj, Vector3 pos, int number)
    {
        for (int i = 0; i < number; i++)
        {
            Random rand = new();

            Enemy new_enemy = (Enemy)obj.Instantiate();


            InitializeEnemy(new_enemy, 30, 5, 2, 2, 1);

            new_enemy.Position = pos + new Vector3((float)rand.NextDouble(), 0, (float)rand.NextDouble());



            AddChild(new_enemy);
        }
    }

    static void InitializeEnemy(Enemy new_enemy, int maxHealth, int damage, float Range, float speed, double attackSpeed) 
    {
        new_enemy.MaxHealth = maxHealth;
        new_enemy.Damage = damage;
        new_enemy.Range = Range;
        new_enemy.Speed = speed;
        new_enemy.AttackSpeed = attackSpeed;
    }

    void OnSecondsTimeout()
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
