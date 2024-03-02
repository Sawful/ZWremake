using Godot;
using System;
using System.Collections.Generic;
public partial class GameManager : Node3D
{
    
    [Export] public Marker3D SpawnLocation;
    PackedScene BaseEnemyScene;
    PackedScene TankEnemyScene;
    PackedScene RangeEnemyScene;

    Vector3 SpawnPosition;
    Vector3 TopLeft;
    Vector3 TopRight;
    Vector3 BottomLeft;
    Vector3 BottomRight;

    Control PlayerUI;
    Player Player;
    ProgressBar HealthBar;

    public bool IsPlayerDead = false;

    public int TimeSeconds = 0;
    Label TimeDisplay;

    Variant Content;
    Variant Content2;


    public override void _Ready()
    {
        Player = GetNode<Player>("Player");
        PlayerUI = GetNode<Control>("PlayerUI");
        SpawnLocation = GetNode<Marker3D>("SpawnLocation");
        HealthBar = PlayerUI.GetNode<ProgressBar>("HealthBar");
        TimeDisplay = PlayerUI.GetNode<CenterContainer>("TopBarDisplay").GetNode<Label>("TimeDisplay");
        TimeDisplay.Text = TimeSeconds.ToString();

        SpawnPosition = SpawnLocation.Position;

        TopLeft = new(40, 0, -40);
        TopRight = new(40, 0, 40);
        BottomLeft = new(-40, 0, -40);
        BottomRight = new(-40, 0, 40);

        

        BaseEnemyScene = (PackedScene)ResourceLoader.Load("res://Enemies/Type/BaseEnemy/BaseEnemy.tscn");
        TankEnemyScene = (PackedScene)ResourceLoader.Load("res://Enemies/Type/TankEnemy/TankEnemy.tscn");
        RangeEnemyScene = (PackedScene)ResourceLoader.Load("res://Enemies/Type/RangeEnemy/RangeEnemy.tscn");

        Content = "GaMiNg";
        Content2 = "don't feel like gaming rn";
        Load();
        Start();
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

            InitializeEnemy(new_enemy, 1);

            new_enemy.Position = pos + new Vector3((float)rand.NextDouble(), 0, (float)rand.NextDouble());

            AddChild(new_enemy);
        }
    }

    void SpawnObjectCorners(PackedScene obj,  int number)
    {
        SpawnObject(obj, TopLeft, number);
        SpawnObject(obj, TopRight, number);
        SpawnObject(obj, BottomLeft, number);
        SpawnObject(obj, BottomRight, number);
    }

    static void InitializeEnemy(Enemy new_enemy, float multiplier) 
    {
        new_enemy.MaxHealth = (int)Math.Floor(new_enemy.MaxHealth * multiplier);
        new_enemy.Damage = (int)Math.Floor(new_enemy.Damage * multiplier);
        new_enemy.Speed = (new_enemy.Speed * 0.9f) + (new_enemy.Speed * 0.1f * multiplier);
    }

    void OnSecondsTimeout()
    {
        TimeSeconds++;
        TimeDisplay.Text = TimeSeconds.ToString();
    }

    async void Start()
    {
        #region Round 1

        
        await ToSignal(GetTree().CreateTimer(5), "timeout");

        SpawnObjectCorners(BaseEnemyScene, 4);

        await ToSignal(GetTree().CreateTimer(25), "timeout");

        #endregion

        #region Round 2

        SpawnObjectCorners(BaseEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(5), "timeout");

        SpawnObjectCorners(BaseEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(5), "timeout");

        SpawnObjectCorners(RangeEnemyScene, 1);
        SpawnObjectCorners(BaseEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(10), "timeout");

        SpawnObjectCorners(RangeEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(10), "timeout");

        #endregion

        #region Round 3

        SpawnObjectCorners(BaseEnemyScene, 2);

        await ToSignal(GetTree().CreateTimer(25), "timeout");

        SpawnObjectCorners(TankEnemyScene, 1);
        SpawnObjectCorners(RangeEnemyScene, 1);

        Save();
        GD.Print(Load());

        await ToSignal(GetTree().CreateTimer(5), "timeout");
        #endregion

        #region Round 4
        SpawnObjectCorners(BaseEnemyScene, 2);

        await ToSignal(GetTree().CreateTimer(15), "timeout");

        SpawnObjectCorners(BaseEnemyScene, 2);

        await ToSignal(GetTree().CreateTimer(15), "timeout");
        #endregion

        #region Round 5

        SpawnObjectCorners(TankEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(15), "timeout");

        SpawnObjectCorners(BaseEnemyScene, 2);
        SpawnObjectCorners(TankEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(15), "timeout");
        #endregion

        #region Round 6


        await ToSignal(GetTree().CreateTimer(5), "timeout");

        SpawnObjectCorners(BaseEnemyScene, 4);

        await ToSignal(GetTree().CreateTimer(25), "timeout");

        #endregion

        #region Round 7

        SpawnObjectCorners(BaseEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(5), "timeout");

        SpawnObjectCorners(BaseEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(5), "timeout");

        SpawnObjectCorners(RangeEnemyScene, 1);
        SpawnObjectCorners(BaseEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(10), "timeout");

        SpawnObjectCorners(RangeEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(10), "timeout");

        #endregion

        #region Round 8

        SpawnObjectCorners(BaseEnemyScene, 2);

        await ToSignal(GetTree().CreateTimer(25), "timeout");

        SpawnObjectCorners(TankEnemyScene, 1);
        SpawnObjectCorners(RangeEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(5), "timeout");
        #endregion

        #region Round 9
        SpawnObjectCorners(BaseEnemyScene, 2);

        await ToSignal(GetTree().CreateTimer(15), "timeout");

        SpawnObjectCorners(BaseEnemyScene, 2);

        await ToSignal(GetTree().CreateTimer(15), "timeout");
        #endregion

        #region Round 10

        SpawnObjectCorners(TankEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(15), "timeout");

        SpawnObjectCorners(BaseEnemyScene, 2);
        SpawnObjectCorners(TankEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(15), "timeout");
        #endregion

        #region Round 11


        await ToSignal(GetTree().CreateTimer(5), "timeout");

        SpawnObjectCorners(BaseEnemyScene, 4);

        await ToSignal(GetTree().CreateTimer(25), "timeout");

        #endregion

        #region Round 12

        SpawnObjectCorners(BaseEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(5), "timeout");

        SpawnObjectCorners(BaseEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(5), "timeout");

        SpawnObjectCorners(RangeEnemyScene, 1);
        SpawnObjectCorners(BaseEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(10), "timeout");

        SpawnObjectCorners(RangeEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(10), "timeout");

        #endregion

        #region Round 13

        SpawnObjectCorners(BaseEnemyScene, 2);

        await ToSignal(GetTree().CreateTimer(25), "timeout");

        SpawnObjectCorners(TankEnemyScene, 1);
        SpawnObjectCorners(RangeEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(5), "timeout");
        #endregion

        #region Round 14
        SpawnObjectCorners(BaseEnemyScene, 2);

        await ToSignal(GetTree().CreateTimer(15), "timeout");

        SpawnObjectCorners(BaseEnemyScene, 2);

        await ToSignal(GetTree().CreateTimer(15), "timeout");
        #endregion

        #region Round 15

        SpawnObjectCorners(TankEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(15), "timeout");

        SpawnObjectCorners(BaseEnemyScene, 2);
        SpawnObjectCorners(TankEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(15), "timeout");
        #endregion

        #region Round 16


        await ToSignal(GetTree().CreateTimer(5), "timeout");

        SpawnObjectCorners(BaseEnemyScene, 4);

        await ToSignal(GetTree().CreateTimer(25), "timeout");

        #endregion

        #region Round 17

        SpawnObjectCorners(BaseEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(5), "timeout");

        SpawnObjectCorners(BaseEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(5), "timeout");

        SpawnObjectCorners(RangeEnemyScene, 1);
        SpawnObjectCorners(BaseEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(10), "timeout");

        SpawnObjectCorners(RangeEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(10), "timeout");

        #endregion

        #region Round 18

        SpawnObjectCorners(BaseEnemyScene, 2);

        await ToSignal(GetTree().CreateTimer(25), "timeout");

        SpawnObjectCorners(TankEnemyScene, 1);
        SpawnObjectCorners(RangeEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(5), "timeout");
        #endregion

        #region Round 19
        SpawnObjectCorners(BaseEnemyScene, 2);

        await ToSignal(GetTree().CreateTimer(15), "timeout");

        SpawnObjectCorners(BaseEnemyScene, 2);

        await ToSignal(GetTree().CreateTimer(15), "timeout");
        #endregion

        #region Round 20

        SpawnObjectCorners(TankEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(15), "timeout");

        SpawnObjectCorners(BaseEnemyScene, 2);
        SpawnObjectCorners(TankEnemyScene, 1);

        await ToSignal(GetTree().CreateTimer(15), "timeout");
        #endregion
    }

    public void Save()
    {
        using var file = FileAccess.Open("user://save_game.dat", FileAccess.ModeFlags.Write);
        file.StoreVar(Player.GetLevel());
        file.StoreVar(Player.GetExperience());

    }

    public Variant Load()
    {
        using var file = FileAccess.Open("user://save_game.dat", FileAccess.ModeFlags.Read);
        Variant level = file.GetVar();
        Variant experience = file.GetVar();

        int levelInt = (int)level.AsInt64();
        int experienceInt = (int)experience.AsInt64();


        Player.SetLevel(levelInt);
        Player.SetExperience(experienceInt);

        return level.ToString() + experience.ToString();
    }
}
