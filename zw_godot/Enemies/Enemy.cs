using Godot;
using System;
public partial class Enemy : Entity
{
	public Player Player;
    private Vector3 PlayerPos;
    HealthBar3D HealthBar;

    PackedScene SoundEffect;
    AudioStreamWav HitSound;
    AudioStreamWav DeathSound;

    PackedScene DeathParticles;

    Node Main;

    // States
    private bool MoveState;
    private bool AttackState;

    int RessourceOnDeath = 1;
    int ExperienceOnDeath = 2;

    public override void _Ready()
	{
        // Stats
        base._Ready();
        Main = GetTree().Root.GetNode("Main");
        Player = Main.GetNode<Player>("Player");
        HealthBar = GetNode<HealthBar3D>("HealthBar3D");
        HealthBar.Update(Health, MaxHealth);

        SoundEffect = (PackedScene)ResourceLoader.Load("res://Sound/SoundEffect.tscn");

        HitSound = (AudioStreamWav)ResourceLoader.Load("res://Sound/hitsound.wav");
        DeathSound = (AudioStreamWav)ResourceLoader.Load("res://Sound/DeathSound.wav");

        DeathParticles = (PackedScene)ResourceLoader.Load("res://Visual/VFX/DeathParticles.tscn");


        // Initialise states
        MoveState = true;
        AttackState = false;
    }

	public override void _PhysicsProcess(double delta)
	{
        // Get player position
        PlayerPos = Player.Position;
        AttackReload -= delta;
        #region States
        SetStates();

        // Move state
        if (MoveState)
        {
            MoveTo(delta, PlayerPos);
        }

        // Attack state
        else if (AttackState)
        {
            RotateTo(PlayerPos, RotationWeight);
            if (AttackReload <= 0)
            {
                DealDamage(Player, Damage);
                AttackReload = 1 / AttackSpeed;
            }
        }
        #endregion
    }


    private void SetStates()
    {
        if (Range > Position.DistanceTo(PlayerPos))
        {
            AttackState = true;
            MoveState = false;
        }
        else
        {
            MoveState = true;
            AttackState = false;
        }
    }

    public override void TakeDamage(Entity attacker, int damageAmount)
    {
        base.TakeDamage(attacker, damageAmount);
        HealthBar.Update(Health, MaxHealth);

        AudioStreamPlayer soundEffect = (AudioStreamPlayer)SoundEffect.Instantiate();
        soundEffect.Stream = HitSound;
        Main.AddChild(soundEffect);
    }

    public override void Die()
    {
        GiveReward();

        AudioStreamPlayer soundEffect = (AudioStreamPlayer)SoundEffect.Instantiate();
        soundEffect.Stream = DeathSound;
        Main.AddChild(soundEffect);

        GpuParticles3D particle = (GpuParticles3D)DeathParticles.Instantiate();
        particle.Position = Position;
        particle.Emitting = true;
        Main.AddChild(particle);

        base.Die();
    }

    private void GiveReward()
    {
        Player.GetRewards(RessourceOnDeath, ExperienceOnDeath);
    }
}
