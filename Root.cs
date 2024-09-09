using Godot;

public partial class Root : Node2D {
  private BeatMap _beatMap;

  public override void _Ready() {
    _beatMap = BeatMap.GetBeatMap();
    GD.Print(_beatMap.Notes);
  }

  public override void _Process(double delta) {
    GD.Print(_beatMap.Notes);
  }
}
