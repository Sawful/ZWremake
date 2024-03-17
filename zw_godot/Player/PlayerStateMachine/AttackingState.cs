using Godot;
using System;
using System.Collections.Generic;

public partial class AttackingState : SimpleState
{
    SimpleStateMachine StateMachine;
    Enemy Target;
    Player Player;
    Dictionary<string, object> Message;
    private AbilityHandler Ability;
    private AbilityUI AbilityUI;
    PackedScene TargetCircle;
    Node3D TargetCircleObject;
    public override void _Ready()
	{
        StateMachine = (SimpleStateMachine)GetParent().GetParent();
        Player = (Player)StateMachine.GetParent();
        AbilityUI = GetTree().Root.GetNode("Main").GetNode("PlayerUI").GetNode("BottomBar").GetNode<AbilityUI>("AbilityUI");
        Ability = Player.GetNode<AbilityHandler>("Abilities");

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
        
        if (!IsInstanceValid(Target))
        {
            StateMachine.ChangeState("IdleState");
        }

        else
        {
            Vector3 targetPosition = Target.Position;
            
            // Call a point and click ability

            if (Message.ContainsKey("Ability"))
            {
                if ((string)Message["Ability"] == "Warrior1")
                {
                    float range = (float)Message["Range"];
                    if (Player.Position.DistanceTo(targetPosition) >= range)
                    {
                        Player.MoveTo(dt, targetPosition);
                    }

                    else
                    {
                        Player.RotateTo(targetPosition, Entity.RotationWeight);
                        //Play spell
                        Player.DealDamage(Target, (int) Mathf.Round(Player.Damage * (float)Message["DamageMultiplier"]));
                        AbilityUI.SetAbilityCooldown("Ability1", (float)Message["Cooldown"]); // Set Cooldown

                        //Reset attack
                        Dictionary<string, object> message = new()
                        {
                            {"Target",  Target},
                        };
                        Player.AttackReload = 0.25;
                        StateMachine.ChangeState("AttackingState", message);
                }}

                else if ((string)Message["Ability"] == "Leap")
                {
                    float leapRange = (float)Message["Leap Range"];

                    if (Player.Position.DistanceTo(targetPosition) >= leapRange)
                    {
                        Player.MoveTo(dt, targetPosition);
                    }

                    else
                    {
                        StateMachine.ChangeState("LeapState", Message);
                    }
                }
            }
            
            else
            {
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

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed && eventMouseButton.ButtonIndex == MouseButton.Right)
        {
            Player.RightClickRaycast(eventMouseButton);
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
