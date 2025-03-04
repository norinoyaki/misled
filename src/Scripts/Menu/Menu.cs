using Godot;
using System;

public partial class Menu : Node3D
{
    [Export] private Sprite2D? title;

    public override void _Process(double delta)
    {
        if (GetViewport() is Viewport viewport)
        {
            Vector2 screenSize = viewport.GetVisibleRect().Size;

            // Base size of the sprite (adjust as needed)
            Vector2 baseScreenSize = new Vector2(1920, 1080); // Reference resolution
            float scaleFactor = Mathf.Min(screenSize.X / baseScreenSize.X, screenSize.Y / baseScreenSize.Y);

            // Apply scale
            title!.Scale = new Vector2(scaleFactor, scaleFactor);

            // Update position
            title.Position = new Vector2(
                screenSize.X - 64,
                64
            );
        }
    }
}
