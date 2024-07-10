using Godot;
using System;

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

		floor.Position = new Vector2(0,  viewport.End.Y/ 2);

		AddChild(floor);
	}
}
