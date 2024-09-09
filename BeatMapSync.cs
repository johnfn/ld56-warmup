using Godot;
using System;

public partial class BeatMapSync : Node2D
{
    private BeatMap _beatMap = BeatMap.GetBeatMap();

    public override void _Ready()
    {
        // Left is (0, 0)
        // Down is (100, 0)
        // Right is (200, 0)
        // Up is (0, 100)
        for (int i = 0; i < _beatMap.Notes.Count; i++)
        {
            var arrow = Arrow.Instantiate(_beatMap.Notes[i].ArrowType);
            AddChild(arrow);

            switch (_beatMap.Notes[i].ArrowType)
            {
                case ArrowType.Left:
                    arrow.Position = new Vector2(0, i * 100);
                    break;
                case ArrowType.Down:
                    arrow.Position = new Vector2(100, i * 100);
                    break;
                case ArrowType.Right:
                    arrow.Position = new Vector2(200, i * 100);
                    break;
                case ArrowType.Up:
                    arrow.Position = new Vector2(0, i * 100 + 100);
                    break;
            }
        }
    }

    public override void _Process(double delta)
    {
    }
}
