using Godot;
using System;
using System.Collections.Generic;

public partial class RangeAttackingState : SimpleState
{
    SimpleStateMachine StateMachine;
    Enemy Target;
    Player Player;
    Dictionary<string, object> Message;
    PackedScene Projectile;
    float Speed;
    PackedScene TargetCircle;
    Node3D TargetCircleObject;

    public override void _Ready()
    {
        StateMachine = (SimpleStateMachine)GetParent().GetParent();
        Player = (Player)StateMachine.GetParent();
        Projectile = (PackedScene)ResourceLoader.Load("res://Player/Projectiles/Projectile.tscn");
        TargetCircle = (PackedScene)ResourceLoader.Load("res://Enemies/TargetCircle.tscn");
    }

    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        Message = message;
        Target = (Enemy)Message["Target"];
        Speed = (float)Message["Projectile Speed"];

        TargetCircleObject = (Node3D)TargetCircle.Instantiate();
        Target.AddChild(TargetCircleObject);
    }

    public override void UpdateState(double dt)
    {
        base.UpdateState(dt);

        if (!IsInstanceValid(Target))
        {
            StateMachine.ChangeState("IdleState");
        }

        else
        {
            Vector3 targetPosition = Target.Position;

            if (Player.Position.DistanceTo(targetPosition) >= Player.Range)
            {
                Player.MoveTo(dt, targetPosition);
            }

            else
            {
                Player.RotateTo(targetPosition, Entity.RotationWeight);

                if (Player.AttackReload <= 0)
                {
                    InstanciateProjectile();
                    Player.AttackReload = 1 / Player.AttackSpeed;

                    AudioStreamPlayer soundEffect = (AudioStreamPlayer)Player.SoundEffectPlayer.Instantiate();
                    soundEffect.Stream = Player.AttackSound;
                    Player.Main.AddChild(soundEffect);
                }
            }
        }
    }

    public void InstanciateProjectile()
    {
        Projectile newProjectile = (Projectile)Projectile.Instantiate();
        newProjectile.Target = Target;
        newProjectile.Speed = Speed;
        newProjectile.ProjectileOwner = Player;
        newProjectile.Damage = Player.Damage;
        newProjectile.Position = Player.Position;
        AddChild(newProjectile);
    }

    public override void OnExit(string NextState)
    {
        if (IsInstanceValid(Target))
        {
            TargetCircleObject.QueueFree();
        }

        base.OnExit(NextState);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Right && Enabled)
        {
            Player.RightClickRaycast(eventMouseButton);
        }
    }
}
