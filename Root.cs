using Godot;

public partial class Root : Node2D {
  public override void _Ready() {
    // print hello
    GD.Print("Hello");
  }

  public override void _Process(double delta) {
  }
}
