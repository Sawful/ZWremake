using Godot;
using System;

public partial class SkillNode : Button
{
	[Export] public string SkillName;
	[Export] SkillNode ParentNode1;
	[Export] SkillNode ParentNode2;
	[Export] public CompressedTexture2D SkillIcon;
	[Export] public AbilityResource Ability;
	private PlayerInfo PlayerInfo;


	public override void _Ready()
	{
        PlayerInfo = GetNode<PlayerInfo>("/root/PlayerInfo");
		Toggled += ButtonPress;
		
		//If It has at least one Parent Node
		if (IsInstanceValid(ParentNode1))
		{
			//Two Parent Nodes
			if(IsInstanceValid(ParentNode2))
			{
				//If one of them is pressed
				if(ParentNode1.ButtonPressed | ParentNode2.ButtonPressed)
				{
					Disabled = false;
				}

				else
				{
					Disabled = true;
					ButtonPressed = false;
				}
				ParentNode1.Toggled += ParentButtonPress;
				ParentNode2.Toggled += ParentButtonPress;

			}
			//One Parent Node
			else
			{
				//If it is pressed
				if(ParentNode1.ButtonPressed)
				{
					Disabled = false;
				}

				else
				{
					Disabled = true;
					ButtonPressed = false;
				}
				ParentNode1.Toggled += ParentButtonPress;
			}
		}
		
		Icon = SkillIcon;
	}



	private void ButtonPress(bool toggledOn)
	{
		if(toggledOn)
		{
			SetAbility();
		}
		else
		{
			RemoveAbility();
		}
	}

	private void ParentButtonPress(bool toggledOn)
	{

		if (IsInstanceValid(ParentNode1))
		{
			//Two Parent Nodes
			if(IsInstanceValid(ParentNode2))
			{
				//If one of them is pressed
				if(ParentNode1.ButtonPressed | ParentNode2.ButtonPressed)
				{
					Disabled = false;
				}

				else
				{
					Disabled = true;
					ButtonPressed = false;
				}
			}
			//One Parent Node
			else
			{
				//If it is pressed
				if(ParentNode1.ButtonPressed)
				{
					Disabled = false;
				}

				else
				{
					Disabled = true;
					ButtonPressed = false;
				}
			}
		}
	}

	private void SetAbility()
	{
		if (IsInstanceValid(Ability))
		{
			PlayerInfo.AbilityResource[Ability.Slot - 1] = Ability;
		}
	}

	private void RemoveAbility()
	{
		if (IsInstanceValid(Ability))
		{
			PlayerInfo.AbilityResource[Ability.Slot - 1] = null;
		}
	}	

	
}
