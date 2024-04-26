using Godot;
using System;
public partial class Enemy : Entity
{
	public Player Player;
    public Vector3 PlayerPos;

    HealthBar3D HealthBar;

    PackedScene SoundEffectPlayer;
    AudioStreamWav DeathSound;

    PackedScene DeathParticles;

    Node Main;

    // States
    public SimpleStateMachine EnemyStateMachine;

    public int RessourceOnDeath;
    public int ExperienceOnDeath;

    public override void _Ready()
	{
        // Stats
        base._Ready();
        Main = GetTree().Root.GetNode("Main");

        if(IsInstanceValid(Main.GetNode<Player>("Player")))
        {
            Player = Main.GetNode<Player>("Player");
        }
        else
        {
            EnemyStateMachine.ChangeState("EnemyIdleState");
        }
        HealthBar = GetNode<HealthBar3D>("HealthBar3D");
        HealthBar.Update(Health, MaxHealth);

        SoundEffectPlayer = (PackedScene)ResourceLoader.Load("res://Sound/SoundEffect.tscn");
        DeathSound = (AudioStreamWav)ResourceLoader.Load("res://Sound/DeathSound.wav");
        DeathParticles = (PackedScene)ResourceLoader.Load("res://Visual/VFX/DeathParticles.tscn");
    }

	public override void _PhysicsProcess(double delta)
	{
        AttackReload -= delta;
    }

    public override void TakeDamage(Entity attacker, int damageAmount)
    {
        base.TakeDamage(attacker, damageAmount);
        HealthBar.Update(Health, MaxHealth);
    }

    public override void Die()
    {
        GiveReward();

        AudioStreamPlayer soundEffect = (AudioStreamPlayer)SoundEffectPlayer.Instantiate();
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
