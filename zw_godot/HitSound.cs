using Godot;
using System;

public partial class HitSound : AudioStreamPlayer
{
    void OnFinished() { QueueFree(); }
}
