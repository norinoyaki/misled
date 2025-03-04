using Godot;

namespace Player {
  public partial class Player : CharacterBody3D {
    public const float Speed = 10.0f;
    public const float JumpVelocity = 16.0f;
    public const float Gravity = -50.0f;
    public const float AirControl = 0.7f;
    public const float Acceleration = 70.0f;
    public const float Deceleration = 70.0f;
    public const float PlungeVelocity = -20.0f;

    private int jumpCount;
    private const int MaxJumps = 2;
    private bool isDashing;
    private float dashTimer;
    private const float DashSpeed = 20.0f;
    private const float DashTime = 0.2f;

    public override void _PhysicsProcess(double delta) {
      if (!IsMultiplayerAuthority())
        return;

      Vector3 velocity = Velocity;

      if (!IsOnFloor() && !isDashing) {
        velocity.Y += Gravity * (float)delta;
      }

      if (IsOnFloor()) {
        jumpCount = 0;
      }

      // Input for double jumping
      // This will check if the user not plunging to avoid accidental click
      if (Input.IsActionJustPressed("Jump") && jumpCount < MaxJumps && !Input.IsActionPressed("Plunge")) {
        velocity.Y = JumpVelocity;
        jumpCount++;
      }

      // Input for normal plunging
      if (Input.IsActionPressed("Plunge") && !IsOnFloor()) {
        velocity.Y = PlungeVelocity;
      }

      // Input for dash plunging
      if (Input.IsActionPressed("Plunge") && Input.IsActionPressed("Dash") && !IsOnFloor()) {
        velocity.Y = PlungeVelocity * 10;
      }

      // Input for side dashing, for some reason it dash to up too?
      if (Input.IsActionJustPressed("Dash") && !isDashing) {
        isDashing = true;
        dashTimer = DashTime;
      }

      // Input for movement and modify the speed based on dash status
      if (isDashing) {
        float dashDirection = Input.GetAxis("Left", "Right");
        velocity.X = (dashDirection != 0 ? dashDirection : Mathf.Sign(Velocity.X)) * DashSpeed;

        dashTimer -= (float)delta;
        if (dashTimer <= 0) {
          isDashing = false;
        }
      }
      else {
        float inputX = Input.GetAxis("Left", "Right");
        float currentSpeed = IsOnFloor() ? Speed : Speed * AirControl;

        if (inputX != 0) {
          velocity.X = Mathf.MoveToward(Velocity.X, inputX * currentSpeed, Acceleration * (float)delta);
        }
        else {
          velocity.X = Mathf.MoveToward(Velocity.X, 0, Deceleration * (float)delta);
        }
      }

      velocity.Z = 0;

      Velocity = velocity;
      MoveAndSlide();


      foreach (var peerId in Multiplayer.GetPeers()) {
        if (peerId != 1) {
          RpcId(peerId, "UpdatePosition", Position, Rotation);
        }
      }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = false)]
    public void UpdatePosition(Vector3 newPosition, Vector3 newRotation) {
      if (!IsMultiplayerAuthority()) {
        Position = newPosition;
        Rotation = newRotation;
      }
    }
  }
}
