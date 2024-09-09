using Godot;

public partial class Root : Node2D {
  private BeatMap _beatMap = BeatMap.GetBeatMap();

  public override void _Ready() {
    // GD.Print(_beatMap.Notes);
  }

  public override void _Process(double delta) {
    // GD.Print(_beatMap.Notes);
  }
}
