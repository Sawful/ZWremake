using Godot;
using System;

public partial class TalentTreeBranch : Control
{
    Button Node1;
    Button Node2;
    Button Node3_1;
    Button Node3_2;
    Button Node4_1;
    Button Node4_2;
    Button Node5;

    public override void _Ready()
    {
        Node1 = GetNode<Button>("Node1");
        Node2 = GetNode<Button>("Node2");
        Node3_1 = GetNode<Button>("Node3_1");
        Node3_2 = GetNode<Button>("Node3_2");
        Node4_1 = GetNode<Button>("Node4_1");
        Node4_2 = GetNode<Button>("Node4_2");
        Node5 = GetNode<Button>("Node5");

        UpdateNodes();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public void _on_toggled(bool button_pressed)
    {
        UpdateNodes();
    }

    public void UpdateNodes()
    {
        if (!Node1.ButtonPressed)
        { Node2.Disabled = true; }
        else { Node2.Disabled = false; }

        if (!Node2.ButtonPressed ^ Node3_2.ButtonPressed)
        {
            Node3_1.Disabled = true;
        }
        else
        {
            Node3_1.Disabled = false;
        }

        if (!Node2.ButtonPressed ^ Node3_1.ButtonPressed)
        {
            Node3_2.Disabled = true;
        }
        else
        {
            Node3_2.Disabled = false;
        }

        if (!Node3_1.ButtonPressed)
        {
            Node4_1.Disabled = true;
        }
        else
        {
            Node4_1.Disabled = false;
        }

        if (!Node3_2.ButtonPressed)
        {
            Node4_2.Disabled = true;
        }
        else
        {
            Node4_2.Disabled = false;
        }

        if (!Node4_1.ButtonPressed & !Node4_2.ButtonPressed)
        {
            Node5.Disabled = true;
        }
        else
        {
            Node5.Disabled = false;
        }
    }
}
