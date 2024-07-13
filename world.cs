using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class world : Node2D
{
	private RECT screenSize;

	public override void _Ready()
	{
		if (OS.HasFeature("server")) return;

		screenSize = WindowsApi.GetScreenSize();

		var window = GetTree().Root.GetWindow();
		window.Size = new Vector2I(
			screenSize.Right - screenSize.Left,
			screenSize.Bottom - screenSize.Top);

		window.Position = new Vector2I(
			screenSize.Left,
			screenSize.Top);

		WindowsApi.EnableClickthrough();

		AddFloorCollision();
		SetupPlayerSpawn();

		// Get reference to camera
		var camera = GetNode<Camera2D>("Camera2D");
		camera.Position = new Vector2(0, -((screenSize.Bottom - screenSize.Top) / 2));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (OS.HasFeature("server")) return;


		Visible = !MouseInScreen();
	}

	private bool MouseInScreen()
	{
		var mousePos = DisplayServer.MouseGetPosition();
		return mousePos.X >= screenSize.Left && mousePos.X <= screenSize.Right &&
			mousePos.Y >= screenSize.Top && mousePos.Y <= screenSize.Bottom;
	}

	private void AddFloorCollision()
	{
		var viewport = GetViewportRect();
		GD.Print(GetViewportRect().Size);

		var floor = new StaticBody2D();
		floor.Name = "Floor";

		var collision = new CollisionShape2D();
		floor.AddChild(collision);

		var shape = new RectangleShape2D
		{
			Size = new Vector2(  viewport.Size.X, 10)
		};
		collision.Shape = shape;

		floor.Position = new Vector2(0,  0);

		AddChild(floor);
	}

	private void SetupPlayerSpawn()
	{
		var multiplayerContext = GetNode<MultiplayerContext>("/root/MultiplayerContext");		
		Multiplayer.PeerConnected += SpawnPlayer;
		Multiplayer.PeerDisconnected += RemovePlayer;

		SpawnPlayer(Multiplayer.GetUniqueId());

		foreach (var playerId in multiplayerContext.PlayerIds)
			SpawnPlayer(playerId);

	}

	private void SpawnPlayer(long id)
	{
		if (id == 1) return;

		GD.Print("Spawning player: " + id);
		var playerScene = GD.Load<PackedScene>("res://player.tscn");
		var player = playerScene.Instantiate<player>();
		player.SetPlayerId(id);
		player.Name = id.ToString();
		AddChild(player);
	}

	private void RemovePlayer(long id)
	{
		GD.Print("Removing player: " + id);
		var player = GetNodeOrNull<player>(id.ToString());
		if (player != null)
			player.QueueFree();
	}
}
