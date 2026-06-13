using Godot;
using System;

public partial class NetworkManager : Control
{
    private const int Port = 8910;
    private const int MaxPlayers = 4;

    private LineEdit _addressLineEdit;
    private Button _hostButton;
    private Button _joinButton;
    private Label _statusLabel;

    public override void _Ready()
    {
        _addressLineEdit = GetNode<LineEdit>("VBoxContainer/AddressLineEdit");
        _hostButton = GetNode<Button>("VBoxContainer/HostButton");
        _joinButton = GetNode<Button>("VBoxContainer/JoinButton");
        _statusLabel = GetNode<Label>("VBoxContainer/StatusLabel");

        _hostButton.Pressed += OnHostButtonPressed;
        _joinButton.Pressed += OnJoinButtonPressed;

        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;
        Multiplayer.ConnectionFailed += OnConnectionFailed;
        Multiplayer.ConnectionSucceeded += OnConnectionSucceeded;

        _addressLineEdit.Text = "127.0.0.1"; // Default to localhost for testing
    }

    private void OnHostButtonPressed()
    {
        var peer = new ENetMultiplayerPeer();
        var error = peer.CreateServer(Port, MaxPlayers);
        if (error != Error.Ok)
        {
            _statusLabel.Text = $"Error creating server: {error}";
            return;
        }

        Multiplayer.MultiplayerPeer = peer;
        _statusLabel.Text = "Hosting game...";
        GD.Print("Server created, listening on port " + Port);
        LoadGameScene();
    }

    private void OnJoinButtonPressed()
    {
        var peer = new ENetMultiplayerPeer();
        var error = peer.CreateClient(_addressLineEdit.Text, Port);
        if (error != Error.Ok)
        {
            _statusLabel.Text = $"Error creating client: {error}";
            return;
        }

        Multiplayer.MultiplayerPeer = peer;
        _statusLabel.Text = "Attempting to join...";
        GD.Print($"Client created, attempting to connect to {_addressLineEdit.Text}:{Port}");
    }

    private void OnPeerConnected(long id)
    {
        GD.Print($"Peer connected: {id}");
        _statusLabel.Text = $"Peer {id} connected.";
    }

    private void OnPeerDisconnected(long id)
    {
        GD.Print($"Peer disconnected: {id}");
        _statusLabel.Text = $"Peer {id} disconnected.";
    }

    private void OnConnectionFailed()
    {
        GD.PrintErr("Connection failed!");
        _statusLabel.Text = "Connection failed!";
        Multiplayer.MultiplayerPeer = null; // Clear the peer
    }

    private void OnConnectionSucceeded()
    {
        GD.Print("Connection succeeded!");
        _statusLabel.Text = "Connection succeeded!";
        LoadGameScene();
    }

    private void LoadGameScene()
    {
        // Load the main game scene after connection is established
        GetTree().ChangeSceneToFile("res://main.tscn");
    }
}
