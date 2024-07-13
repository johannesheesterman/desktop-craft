using Godot;
using System;

public partial class player : CharacterBody2D
{
	public long PlayerId { get; private set; } = -1;

	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	private Node2D sprites;
	private MultiplayerSynchronizer multiplayerSynchronizer;

	public override void _Ready()
	{
		sprites = GetNode<Node2D>("Sprites");
		multiplayerSynchronizer = GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer");
		multiplayerSynchronizer.SetMultiplayerAuthority(Convert.ToInt32(PlayerId), true);

		GD.Print("PlayerId: " + PlayerId);
		GD.Print("Multiplayer.GetUniqueId(): " + Multiplayer.GetUniqueId());
	}

	public override void _PhysicsProcess(double delta)
	{
		if (multiplayerSynchronizer.GetMultiplayerAuthority() != Multiplayer.GetUniqueId())
			return;
			

		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		// If velocity.X > 0, rotate sprite to the right.
		if (velocity.X != 0 )
		{
			sprites.Scale = new Vector2(velocity.X > 0 ? -1 : 1, 1);
		}
		

		

		Velocity = velocity;
		MoveAndSlide();
	}

	public void SetPlayerId(long id)
	{
		PlayerId = id;
	}
}
