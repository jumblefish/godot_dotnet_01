using Godot;
using System;
using System.Collections.Generic;

public partial class MainScene : Node3D
{
    [Export]
    public PackedScene LampScene { get; set; }

    private Button _loadLampButton;
    private Button _clearSceneButton;
    private MeshInstance3D _existingMeshInstance3D; // Reference to the existing MeshInstance3D
    private List<Node3D> _loadedLamps = new List<Node3D>(); // To keep track of instantiated lamps

    public override void _Ready()
    {
        _existingMeshInstance3D = GetNode<MeshInstance3D>("MeshInstance3D");
        _loadLampButton = GetNode<Button>("LoadLampButton");
        _clearSceneButton = GetNode<Button>("ClearSceneButton");

        if (_loadLampButton != null)
        {
            _loadLampButton.Pressed += OnLoadLampButtonPressed;
        }
        if (_clearSceneButton != null)
        {
            _clearSceneButton.Pressed += OnClearSceneButtonPressed;
        }
    }

    private void OnLoadLampButtonPressed()
    {
        if (LampScene != null)
        {
            Node3D lampInstance = LampScene.Instantiate<Node3D>();
            AddChild(lampInstance); // Add the lamp instance as a child of this Node3D
            _loadedLamps.Add(lampInstance); // Add to our list for clearing

            // Optional: Position the lamp instance relative to the existing MeshInstance3D
            // For example, place it slightly above the existing mesh
            if (_existingMeshInstance3D != null)
            {
                // Get the global position of the existing mesh and add an offset in Y-direction
                // We'll offset each new lamp slightly to avoid them stacking perfectly
                float offset = _loadedLamps.Count * 0.5f; // Adjust offset as needed
                lampInstance.GlobalPosition = _existingMeshInstance3D.GlobalPosition + new Vector3(0, 1.5f + offset, 0);
            }

            GD.Print("Lamp scene loaded successfully!");
        }
        else
        {
            GD.PrintErr("LampScene is not assigned in the inspector!");
        }
    }

    private void OnClearSceneButtonPressed()
    {
        foreach (Node3D lamp in _loadedLamps)
        {
            lamp.QueueFree(); // Safely remove the node from the scene tree
        }
        _loadedLamps.Clear(); // Clear the list
        GD.Print("All loaded lamps cleared!");
    }
}
