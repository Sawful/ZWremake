using Godot;
using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

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
    PlayerInfo PlayerInfo;

    public bool PlayerDead = false;

    public int TimeSeconds = 0;
    Label TimeDisplay;

    public PackedScene SoundEffectPlayer;
    AudioStreamPlayer audioStreamPlayer;

    ProgressBar ThreatBar;
    int ThreatRank;

    public AudioStreamWav SlowDownSFX;
    public AudioStreamWav SpeedUpSFX;

    PackedScene PauseMenu;
    PackedScene DeathMenu;

    public override void _Ready()
    {
        PlayerInfo = GetNode<PlayerInfo>("/root/PlayerInfo");

        Player = GetNode<Player>("Player");
        PlayerUI = GetNode<Control>("PlayerUI");
        SpawnLocation = GetNode<Marker3D>("SpawnLocation");
        HealthBar = PlayerUI.GetNode<ProgressBar>("HealthBar");
        TimeDisplay = PlayerUI.GetNode<CenterContainer>("TopBarDisplay").GetNode<Label>("TimeDisplay");
        TimeDisplay.Text = TimeSeconds.ToString();
        ThreatBar = PlayerUI.GetNode<ProgressBar>("ThreatBar");

        SpawnPosition = SpawnLocation.Position;

        SlowDownSFX = (AudioStreamWav)ResourceLoader.Load("res://Sound/SlowDown.wav");
        SpeedUpSFX = (AudioStreamWav)ResourceLoader.Load("res://Sound/SpeedUp.wav");

        SoundEffectPlayer = (PackedScene)ResourceLoader.Load("res://Sound/SoundEffect.tscn");
        audioStreamPlayer = (AudioStreamPlayer)SoundEffectPlayer.Instantiate();

        TopLeft = new(40, 0, -40);
        TopRight = new(40, 0, 40);
        BottomLeft = new(-40, 0, -40);
        BottomRight = new(-40, 0, 40);

        BaseEnemyScene = (PackedScene)ResourceLoader.Load("res://Enemies/Type/BaseEnemy/BaseEnemy.tscn");
        TankEnemyScene = (PackedScene)ResourceLoader.Load("res://Enemies/Type/TankEnemy/TankEnemy.tscn");
        RangeEnemyScene = (PackedScene)ResourceLoader.Load("res://Enemies/Type/RangeEnemy/RangeEnemy.tscn");

        PauseMenu = (PackedScene)ResourceLoader.Load("res://Main/PauseMenu.tscn");
        DeathMenu = (PackedScene)ResourceLoader.Load("res://Main/DeathMenu.tscn");

        Load();

        PlayerInfo.GameNumber += 1;

        Start();
    }
    public override void _Process(double delta)
    {
        PlayerDead = !IsInstanceValid(Player);
        if (PlayerDead)
        {
            DeathMenu deathMenu = DeathMenu.Instantiate<DeathMenu>();
            AddChild(deathMenu);
            GetTree().Paused = true;
        }
    }

    public override void _Input(InputEvent @event)  
    {
        if (@event.IsActionPressed("Pause"))
        {
            Node pauseMenu = PauseMenu.Instantiate();
            AddChild(pauseMenu);
            GetTree().Paused = true;
        }
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

        Save();

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

        Save();

        #endregion

        #region Round 3

        SpawnObjectCorners(BaseEnemyScene, 2);

        await ToSignal(GetTree().CreateTimer(25), "timeout");

        SpawnObjectCorners(TankEnemyScene, 1);
        SpawnObjectCorners(RangeEnemyScene, 1);

        Save();

        await ToSignal(GetTree().CreateTimer(5), "timeout");

        Save();

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


    public void IncreaseThreat(int increaseBy)
    {
        ThreatBar.Value += increaseBy;
        if(ThreatRank < ThreatBar.Value / 10000)
        {
            ThreatRank += 1;
            GD.Print("Threat Rank has increased to");
            GD.Print(ThreatRank);
        }
    }
    public void Save()
    {
        PlayerInfo.PlayerLevel = Player.GetLevel();
        PlayerInfo.PlayerExperience = Player.GetExperience();

        PlayerInfo.Save();
    }

    public override void _Notification(int what)
    {
    if (what == NotificationWMCloseRequest)
        {
        GD.Print("EXIT");
        Save();
        GetTree().Quit(); // default behavior
        }
    }

    public void StartSlowMo()
    {
        StartCoroutine(SlowDown((float)Engine.TimeScale, 0.5f, 0.1f));
        StartCoroutine(Zoom(Player.MainCamera.Fov, 97.5f, 0.1f));
        AudioStreamPlayer soundEffect = (AudioStreamPlayer)SoundEffectPlayer.Instantiate();
        soundEffect.Stream = SlowDownSFX;
        AddChild(soundEffect);
    }

    public void StopSlowMo()
    {
        StartCoroutine(DeZoom(Player.MainCamera.Fov, 107.5f, 0.1f));
        StartCoroutine(SpeedUp((float)Engine.TimeScale, 1f, 0.1f));
        AudioStreamPlayer soundEffect = (AudioStreamPlayer)SoundEffectPlayer.Instantiate();
        soundEffect.Stream = SpeedUpSFX;
        AddChild(soundEffect);
    }

    IEnumerable Zoom(float currentFOV, float newFOV, float changeSpeed)
    {
        for (float newfov = currentFOV; newfov > newFOV + 0.005; newfov = Mathf.Lerp(newfov, newFOV, changeSpeed))
        {
            GD.Print(newfov);
            Player.MainCamera.Fov = newfov;
            yield return null;
        }
        Player.MainCamera.Fov = newFOV;
    }

    IEnumerable SlowDown(float currentSpeed, float reducedSpeed, float changeSpeed)
    {
        for (float newSpeed = currentSpeed; newSpeed > reducedSpeed + 0.005; newSpeed = Mathf.Lerp(newSpeed, reducedSpeed, changeSpeed))
        {
            GD.Print(newSpeed);
            Engine.TimeScale = newSpeed;
            yield return null;
        }
        Engine.TimeScale = reducedSpeed;
    }

    IEnumerable SpeedUp(float currentSpeed, float reducedSpeed, float changeSpeed)
    {
        for (float newSpeed = currentSpeed; newSpeed < reducedSpeed - 0.005; newSpeed = Mathf.Lerp(newSpeed, reducedSpeed, changeSpeed))
        {
            GD.Print(newSpeed);
            Engine.TimeScale = newSpeed;
            yield return null;
        }
        Engine.TimeScale = reducedSpeed;
    }

    IEnumerable DeZoom(float currentFOV, float newFOV, float changeSpeed)
    {
        for (float newfov = currentFOV; newfov < newFOV - 0.005; newfov = Mathf.Lerp(newfov, newFOV, changeSpeed))
        {
            GD.Print(newfov);
            Player.MainCamera.Fov = newfov;
            yield return null;
        }
        Player.MainCamera.Fov = newFOV;
    }

    public void Load()
    {
        using var file = FileAccess.Open("user://save_game.dat", FileAccess.ModeFlags.Read);

        Variant level = file.GetVar();
        Variant experience = file.GetVar();
        Variant gameNumber = file.GetVar();

        int levelInt = (int)level.AsInt64();
        int experienceInt = (int)experience.AsInt64();
        int gameNumberInt = (int)gameNumber.AsInt64();

        Player.SetLevel(levelInt);
        Player.SetExperience(experienceInt);
        PlayerInfo.GameNumber = gameNumberInt;
    }

    public static async void StartCoroutine(IEnumerable objects)
       {
        var mainLoopTree = Engine.GetMainLoop();
        foreach (var _ in objects)
        {
            await mainLoopTree.ToSignal(mainLoopTree, SceneTree.SignalName.ProcessFrame);
        }
    }

}
