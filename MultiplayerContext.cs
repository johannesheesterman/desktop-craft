using Godot;
using System;
using System.Collections.Generic;

public partial class MultiplayerContext : Node
{
	private string _serverAddress = "127.0.0.1";
	private int _serverPort = 8190;
	private ENetMultiplayerPeer _peer;
	public List<long> PlayerIds = new List<long>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Multiplayer.PeerConnected += OnPlayerConnected;
		Multiplayer.PeerDisconnected += OnPlayerDisconnected;

		if (OS.HasFeature("server"))
			SetupServer();
		else
			SetupClient();
	}

	private void SetupServer()
	{
		_peer = new ENetMultiplayerPeer();
		var error = _peer.CreateServer(_serverPort);
		if (error != Error.Ok)
		{
			GD.PrintErr("Failed to create server: " + error);
			return;
		}

		Multiplayer.MultiplayerPeer = _peer;	
		GD.Print("Server started on port " + _serverPort);
	}

	private void SetupClient()
	{
		Multiplayer.ConnectedToServer += OnConnection;
		Multiplayer.ConnectionFailed += OnConnectionFail;

		_peer = new ENetMultiplayerPeer();
		var error = _peer.CreateClient(_serverAddress, _serverPort);
		if (error != Error.Ok)
		{
			GD.PrintErr("Failed to connect to server: " + error);
			return;
		}

		Multiplayer.MultiplayerPeer = _peer;
		GD.Print("Connected to server at " + _serverAddress + ":" + _serverPort);
	}

	private void OnPlayerConnected(long id)
	{
		GD.Print("Player connected: " + id);
		PlayerIds.Add(id);
	}

	private void OnPlayerDisconnected(long id)
	{
		GD.Print("Player disconnected: " + id);
		PlayerIds.Remove(id);
	}

	private void OnConnection()
	{
		GD.Print("Connected to server");
	}

	private void OnConnectionFail()
	{
		GD.PrintErr("Failed to connect to server");
	}
}
