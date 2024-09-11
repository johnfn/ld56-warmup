using Godot;
namespace LD56;public partial class BeatMapSync : Node2D {
  public static BeatMapSync New() {
    return GD.Load<PackedScene>("res://beat_map_sync.tscn").Instantiate<BeatMapSync>();
  }
  public BeatMapSync() {
    foreach (var @interface in GetType().GetInterfaces()) {
      AddToGroup(@interface.Name);
    }
  }

  public partial class BeatMapSyncNodes {
    BeatMapSync parent;
    public BeatMapSyncNodes(BeatMapSync parent) {
      this.parent = parent;
    }
  }

  public BeatMapSyncNodes? _Nodes;
  public BeatMapSyncNodes Nodes {
    get {
      return _Nodes ??= new BeatMapSyncNodes(this);
    }
  }
}
