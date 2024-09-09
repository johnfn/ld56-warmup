using Godot;
public partial class Root : Node2D {
  public static Root New() {
    return GD.Load<PackedScene>("res://root.tscn").Instantiate<Root>();
  }
  public Root() {
    foreach (var @interface in GetType().GetInterfaces()) {
      AddToGroup(@interface.Name);
    }
  }

  public partial class RootNodes {
    Root parent;
    public RootNodes(Root parent) {
      this.parent = parent;
    }
    private Arrow? _Arrow;
    public Arrow Arrow {
      get {
        if (_Arrow == null) {
          _Arrow = parent.GetNode<Arrow>("Arrow");
        }
        return _Arrow;
      }
    }

  }

  public RootNodes? _Nodes;
  public RootNodes Nodes {
    get {
      if (_Nodes == null) {
        _Nodes = new RootNodes(this);
      }
      return _Nodes;
    }
  }
}
