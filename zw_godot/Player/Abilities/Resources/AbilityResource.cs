using Godot;
using System;

public partial class AbilityResource : Resource
{
	[Export] public string AbilityName;
	[Export] public int Slot;
	[Export] public float Cooldown;
	[Export] public float CurrentCooldown;
	[Export] public bool OnCooldown;
	public Ability AbilityNode;
	[Export] public string AbilityNodePath;
	[Export] public CompressedTexture2D Icon;

	public void SetAbility(AbilityHandler abilityHandler)
	{
		PackedScene ability = (PackedScene)ResourceLoader.Load(AbilityNodePath);
		AbilityNode = (Ability)ability.Instantiate();
		AbilityNode.Cooldown = Cooldown;
		abilityHandler.AddChild(AbilityNode);
	}
}
