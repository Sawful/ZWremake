using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerInfo : Node
{
	public string PlayerClass;
	public int GameNumber;
	public int PlayerLevel;
	public int PlayerExperience;
	public List<AbilityResource> AbilityResource = new(){null, null, null, null};


	public void Save()
    {
        using var file = FileAccess.Open("user://save_game.dat", FileAccess.ModeFlags.Write);
        file.StoreVar(PlayerLevel);
        file.StoreVar(PlayerExperience);
        file.StoreVar(GameNumber);
    }
}
