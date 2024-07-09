using Godot;
using System;

public partial class world : Node2D
{
	private RECT screenSize;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		screenSize = WindowsApi.GetScreenSize();

		var window = GetTree().Root.GetWindow();
		window.Size = new Vector2I(
			screenSize.Right - screenSize.Left,
			screenSize.Bottom - screenSize.Top);

		window.Position = new Vector2I(
			screenSize.Left,
			screenSize.Top);

		// window.Borderless = true;
		// window.AlwaysOnTop = true;
		// window.TransparentBg = true;

		WindowsApi.EnableClickthrough();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Visible = !MouseInScreen();
	}

	private bool MouseInScreen()
	{
		var mousePos = DisplayServer.MouseGetPosition();
		return mousePos.X >= screenSize.Left && mousePos.X <= screenSize.Right &&
			mousePos.Y >= screenSize.Top && mousePos.Y <= screenSize.Bottom;
	}
}
