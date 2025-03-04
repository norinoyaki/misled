using Godot;

public partial class Camera : Camera3D {
  [Export] public float LerpSpeed = 2.0f;
  [Export] public float MinZoom = 15.0f;
  [Export] public float MaxZoom = 50.0f;
  [Export] public float ZoomFactor = 2.0f;

  public override void _Process(double delta) {
    var players = GetTree().GetNodesInGroup("players");
    if (players.Count == 0)
      return;

    Vector3 center = Vector3.Zero;
    float maxDistance = 0f;

    foreach (Node playerNode in players) {
      if (playerNode is CharacterBody3D player) {
        center += player.GlobalPosition;
      }
    }

    center /= players.Count;

    foreach (Node playerNode in players) {
      if (playerNode is CharacterBody3D player) {
        float distance = center.DistanceTo(player.GlobalPosition);
        maxDistance = Mathf.Max(maxDistance, distance);
      }
    }


    float zoom = Mathf.Clamp(MinZoom + (maxDistance * ZoomFactor), MinZoom, MaxZoom);

    ZoomFactor = zoom;
    Vector3 targetPosition = new Vector3(center.X, Mathf.Clamp(center.Y, 2.15f, 50), zoom);

    Position = Position.Lerp(targetPosition, (float)delta * LerpSpeed);
  }
}
