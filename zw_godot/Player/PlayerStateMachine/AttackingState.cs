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
    public override void _Ready()
	{
        StateMachine = (SimpleStateMachine)GetParent().GetParent();
        Player = (Player)StateMachine.GetParent();
        AbilityUI = GetTree().Root.GetNode("Main").GetNode("PlayerUI").GetNode("BottomBar").GetNode<AbilityUI>("AbilityUI");
    }

    public override void OnStart(Dictionary<string, object> message)
    {
        base.OnStart(message);
        Message = message;
        Target = (Enemy)Message["Target"];
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
}
