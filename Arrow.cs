using Godot;

namespace LD56;

public enum ArrowType {
  Up,
  Down,
  Left,
  Right,
}

[Tool]
public partial class Arrow : Node2D {
  public bool isInMotion = true;

  private ArrowType _type;
  [Export(PropertyHint.Enum)]
  public ArrowType Type {
    get => _type;
    set {
      _type = value;
      UpdateArrowVisibility();
    }
  }

  [Export]
  public Color Tint {
    get => Modulate;
    set => Modulate = value;
  }

  public static Arrow Instantiate(ArrowType type) {
    var arrow = GD.Load<PackedScene>("res://arrow.tscn").Instantiate<Arrow>();
    arrow.Type = type;
    return arrow;
  }

  public override void _Ready() {
    Nodes.ArrowDown.Visible = false;
    Nodes.ArrowLeft.Visible = false;
    Nodes.ArrowRight.Visible = false;
    Nodes.ArrowUp.Visible = false;

    UpdateArrowVisibility();
  }

  public override void _Process(double delta) {
  }

  private void UpdateArrowVisibility() {
    Nodes.ArrowDown.Visible = _type == ArrowType.Down;
    Nodes.ArrowLeft.Visible = _type == ArrowType.Left;
    Nodes.ArrowRight.Visible = _type == ArrowType.Right;
    Nodes.ArrowUp.Visible = _type == ArrowType.Up;
  }
}
