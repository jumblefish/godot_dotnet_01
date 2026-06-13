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
    private MultiplayerSpawner _multiplayerSpawner; // Reference to the MultiplayerSpawner

    public override void _Ready()
    {
        _existingMeshInstance3D = GetNode<MeshInstance3D>("MeshInstance3D");
        _loadLampButton = GetNode<Button>("LoadLampButton");
        _clearSceneButton = GetNode<Button>("ClearSceneButton");
        _multiplayerSpawner = GetNode<MultiplayerSpawner>("LampSpawner"); // Assuming a MultiplayerSpawner named "LampSpawner"

        if (_loadLampButton != null)
        {
            _loadLampButton.Pressed += OnLoadLampButtonPressed;
        }
        if (_clearSceneButton != null)
        {
            _clearSceneButton.Pressed += OnClearSceneButtonPressed;
        }
    }

    // Public methods for lamps to register/unregister themselves
    public void RegisterLamp(Node3D lamp)
    {
        if (!_loadedLamps.Contains(lamp))
        {
            _loadedLamps.Add(lamp);
            GD.Print($"Lamp registered. Total lamps: {_loadedLamps.Count}");
        }
    }

    public void UnregisterLamp(Node3D lamp)
    {
        if (_loadedLamps.Contains(lamp))
        {
            _loadedLamps.Remove(lamp);
            GD.Print($"Lamp unregistered. Total lamps: {_loadedLamps.Count}");
        }
    }

    private void OnLoadLampButtonPressed()
    {
        // Request the server to spawn a lamp
        // MultiplayerApi.RpcMode.Server ensures this RPC is only sent to the server
        Rpc(nameof(RequestSpawnLamp));
    }

    [Rpc(MultiplayerApi.RpcMode.Server)] // This RPC method will only run on the server
    private void RequestSpawnLamp()
    {
        if (LampScene == null)
        {
            GD.PrintErr("LampScene is not assigned in the inspector!");
            return;
        }

        // The server uses the MultiplayerSpawner to spawn the lamp.
        // The MultiplayerSpawner will automatically replicate this to all connected clients.
        Node3D lampInstance = (Node3D)_multiplayerSpawner.Spawn(LampScene);
        
        // Position the lamp instance
        if (_existingMeshInstance3D != null)
        {
            // Calculate offset based on the current number of loaded lamps (before this one is registered)
            float offset = _loadedLamps.Count * 0.5f; 
            lampInstance.GlobalPosition = _existingMeshInstance3D.GlobalPosition + new Vector3(0, 1.5f + offset, 0);
        }

        // The lamp's _Ready() method (in Lamp.cs) will call RegisterLamp on all peers.
        GD.Print($"Server requested spawn of lamp. Spawner path: {_multiplayerSpawner.GetPath()}");
    }

    private void OnClearSceneButtonPressed()
    {
        // Request all peers (including the server) to clear lamps
        // MultiplayerApi.RpcMode.AnyPeer means all connected peers will receive and execute this RPC
        // CallLocal = true ensures the caller also executes the RPC locally
        Rpc(nameof(RequestClearAllLamps));
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)] // This RPC method will run on all peers
    private void RequestClearAllLamps()
    {
        // Iterate through a copy of the list to avoid issues when modifying it during iteration
        foreach (Node3D lamp in new List<Node3D>(_loadedLamps))
        {
            if (Multiplayer.IsServer())
            {
                _multiplayerSpawner.Despawn(lamp);
            }
            // Clients will have their lamps removed by the spawner, and UnregisterLamp will be called from the lamp's _ExitTree.
        }
        // The _loadedLamps list will be cleared as lamps are despawned and UnregisterLamp is called.
        GD.Print("All loaded lamps requested to be cleared!");
    }
}
