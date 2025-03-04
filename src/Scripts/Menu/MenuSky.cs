using Godot;
using System;

public partial class MenuSky : MeshInstance3D
{
    [Export] public float speed = 1.0f;

    public override void _Process(double delta)
    {
        RotateY(speed * (float)delta);
    }
}
