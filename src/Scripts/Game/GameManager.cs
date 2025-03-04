using Godot;

public partial class GameManager : Node {
  [Export] public PackedScene? PlayerScene;

  private ENetMultiplayerPeer? peer;

  public override void _Ready() {
    JoinServer();
  }

  public void JoinServer() {
    GD.Print("Joining server...");
    peer = new ENetMultiplayerPeer();
    Error error = peer.CreateClient("127.0.0.1", 31415);
    if (error != Error.Ok) {
      GD.PrintErr("Failed to initialize client: " + error);
      return;
    }

    Multiplayer.MultiplayerPeer = peer;

    // Set up connection timeout timer
    Timer connectionTimer = new Timer();
    connectionTimer.WaitTime = 2.0;
    connectionTimer.OneShot = true;
    connectionTimer.Timeout += OnConnectionTimeout;
    AddChild(connectionTimer);
    connectionTimer.Start();

    GD.Print("");

    Multiplayer.PeerConnected += (long id) => {
      GD.Print($"Successfully connected to server with ID {id}.");
      connectionTimer.Stop();
      connectionTimer.QueueFree();
    };
  }


  private void OnConnectionTimeout() {
    GD.PrintErr("Connection timed out after 2 seconds.");
    peer!.Close();
    Multiplayer.MultiplayerPeer = null;
  }

  [Rpc]
  public void SpawnPlayer(long id) {
    GD.Print($"Spawning player for ID {id}.");

    if (PlayerScene == null) {
      GD.PrintErr("Player scene not set!");
      return;
    }

    var player = (CharacterBody3D)PlayerScene.Instantiate();
    player.Name = id.ToString();

    if (id == Multiplayer.GetUniqueId()) {
      player.SetMultiplayerAuthority((int)id);
    }

    var pos = new RandomNumberGenerator();

    AddChild(player);
    player.Position = new Vector3(pos.RandfRange(-1, 3), 2.772f, 5.942f);
  }

  [Rpc]
  public void DespawnPlayer(long id) {
    var player = GetNodeOrNull<Node3D>(id.ToString());
    if (player != null) {
      player.QueueFree();
      GD.Print($"Player {id} despawned.");
    }
  }
}
