using Godot;
using System;
using System.Collections.Generic;

public partial class AttackingState : SimpleState
{
    SimpleStateMachine StateMachine;
    Enemy Target;
    Player Player;
    Dictionary<string, object> Message;
    private AbilityUI AbilityUI;
    PackedScene TargetCircle;
    Node3D TargetCircleObject;
    public override void _Ready()
	{
        StateMachine = (SimpleStateMachine)GetParent().GetParent();
        Player = (Player)StateMachine.GetParent();
        AbilityUI = GetTree().Root.GetNode("Main").GetNode("PlayerUI").GetNode("BottomBar").GetNode<AbilityUI>("AbilityUI");

        TargetCircle = (PackedScene)ResourceLoader.Load("res://Enemies/TargetCircle.tscn");
    }

    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        Message = message;
        Target = (Enemy)Message["Target"];
        TargetCircleObject = (Node3D)TargetCircle.Instantiate();
        Target.AddChild(TargetCircleObject);
    }

    
    public override void UpdateState(double dt)
    {
        base.UpdateState(dt);

        GD.Print(TargetCircleObject);
        
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

                if (Message.ContainsKey("Ability"))
                {
                    GD.Print(Message["Ability"] + "GAMING");
                    //Play spell
                    Player.DealDamage(Target, Player.Damage * 10);
                    AbilityUI.SetAbilityCooldown("Ability1"); // Set Cooldown

                    //Reset attack
                    Dictionary<string, object> message = new()
                    {
                        {"Target",  Target},
                    };
                    StateMachine.ChangeState("AttackingState", message);
                }

                else if (Player.AttackReload <= 0)
                {
                    Player.DealDamage(Target, Player.Damage);
                    Player.AttackReload = 1 / Player.AttackSpeed;
                }
            }
        }
    }

    public override void OnExit(string NextState)
    {
        if (IsInstanceValid(Target))
        {
            TargetCircleObject.QueueFree();
        }

        base.OnExit(NextState);
    }
}
