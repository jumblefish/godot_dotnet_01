using Godot;
using System;

public partial class ObjectLoader : Node
{
    [Export] public PackedScene ObjectToLoad { get; set; }
    [Export] public Node3D ParentNode { get; set; }

    public override void _Ready()
    {
        if (ParentNode == null)
        {
            GD.PrintErr("ParentNode is not assigned for ObjectLoader.");
        }
    }

    public void OnLoadObjectButtonPressed()
    {
        if (ObjectToLoad != null && ParentNode != null)
        {
            Node3D newObject = ObjectToLoad.Instantiate<Node3D>();
            ParentNode.AddChild(newObject);
            GD.Print("Object loaded successfully!");
        }
        else
        {
            GD.PrintErr("ObjectToLoad or ParentNode is not assigned.");
        }
    }
}
