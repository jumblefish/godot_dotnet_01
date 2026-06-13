using Godot;
using System;

public partial class Lamp : Node3D
{
    public override void _Ready()
    {
        // Ensure this runs only after the lamp is part of the main scene tree
        if (GetParent() is MainScene mainScene)
        {
            mainScene.RegisterLamp(this);
        }
        else
        {
            GD.PrintErr("Lamp not parented to MainScene!");
        }
    }

    public override void _ExitTree()
    {
        // Ensure this runs only when the lamp is removed from the main scene tree
        if (GetParent() is MainScene mainScene)
        {
            mainScene.UnregisterLamp(this);
        }
    }
}
