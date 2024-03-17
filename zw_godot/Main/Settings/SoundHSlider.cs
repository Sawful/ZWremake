using Godot;
using System;

public partial class SoundHSlider : HSlider
{
	int BusIndex;
	string BusName;

    public override void _Ready()
    {
        BusIndex = AudioServer.GetBusIndex("Master");
    }

    public void _on_value_changed(float value)
	{
		AudioServer.SetBusVolumeDb(BusIndex, Mathf.LinearToDb(value));
	}
}
