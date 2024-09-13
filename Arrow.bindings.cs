using Godot;
namespace LD56;public partial class Arrow : Node2D {
  public static Arrow New() {
    return GD.Load<PackedScene>("res://arrow.tscn").Instantiate<Arrow>();
  }
  public Arrow() {
    foreach (var @interface in GetType().GetInterfaces()) {
      AddToGroup(@interface.Name);
    }
  }

  public partial class ArrowNodes {
    Arrow parent;
    public ArrowNodes(Arrow parent) {
      this.parent = parent;
    }
    private Sprite2D? _ArrowUp;
    public Sprite2D ArrowUp {
      get {
        return _ArrowUp ??= parent.GetNode<Sprite2D>("ArrowUp");
      }
    }

    private Sprite2D? _ArrowDown;
    public Sprite2D ArrowDown {
      get {
        return _ArrowDown ??= parent.GetNode<Sprite2D>("ArrowDown");
      }
    }

    private Sprite2D? _ArrowLeft;
    public Sprite2D ArrowLeft {
      get {
        return _ArrowLeft ??= parent.GetNode<Sprite2D>("ArrowLeft");
      }
    }

    private Sprite2D? _ArrowRight;
    public Sprite2D ArrowRight {
      get {
        return _ArrowRight ??= parent.GetNode<Sprite2D>("ArrowRight");
      }
    }

  }

  public ArrowNodes? _Nodes;
  public ArrowNodes Nodes {
    get {
      return _Nodes ??= new ArrowNodes(this);
    }
  }
}
