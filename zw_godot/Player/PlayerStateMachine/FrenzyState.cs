using Godot;
using System;
using System.Collections.Generic;


public partial class FrenzyState : ImmobileState
{
    Enemy Target;
	PackedScene TimerPacked;
	Timer Timer;
    Dictionary<string, object> Message;
    PackedScene TargetCircle;
    Node3D TargetCircleObject;

    public override void _Ready()
    {
        base._Ready();
        TimerPacked = (PackedScene)ResourceLoader.Load("res://Player/PlayerStateMachine/StateDurationTimer.tscn");
        TargetCircle = (PackedScene)ResourceLoader.Load("res://Enemies/TargetCircle.tscn");
    }

    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        Message = message;

        Timer = TimerPacked.Instantiate<Timer>();
        Timer.WaitTime = (double)Message["Duration"];
        Timer.Timeout += OnTimeout;
        AddChild(Timer);
    }

    public override void UpdateState(double dt)
    {
        base.UpdateState(dt);

        if (Player.GetClosestEnemy() != null | IsInstanceValid(Target))
        {
            if (!IsInstanceValid(Target) | Target == null)
            {
                Target = Player.GetClosestEnemy();
                TargetCircleObject = (Node3D)TargetCircle.Instantiate();
                Target.AddChild(TargetCircleObject);
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
                    Player.AutoAttack(Target);
                }
            }
        }
    }

    private void OnTimeout()
    {
        if (IsInstanceValid(Target))
        {
            Dictionary<string, object> message = new()
            {
                { "Target", Target}
            };

            Player.PlayerStateMachine.ChangeState("AttackingState", message);
        }
        else
        {
            Player.PlayerStateMachine.ChangeState("IdleState");
            
        }
    }

}
