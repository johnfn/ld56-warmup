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
        var beatHeight = 300;

        for (int i = 0; i < _beatMap.Notes.Count; i++)
        {
            var note = _beatMap.Notes[i];
            var arrow = Arrow.Instantiate(note.ArrowType);
            AddChild(arrow);

            var height = note.StartBeat * beatHeight;
            var div = ((float)note.Division.numerator / (float)note.Division.denominator);
            GD.Print($"Numerator and denominator: {note.Division.numerator} / {note.Division.denominator} {note.Division.numerator / note.Division.denominator}");
            height += (int)(beatHeight * div);

            switch (_beatMap.Notes[i].ArrowType)
            {
                case ArrowType.Left:
                    arrow.Position = new Vector2(0, height);
                    break;

                case ArrowType.Down:
                    arrow.Position = new Vector2(100, height);
                    break;

                case ArrowType.Right:
                    arrow.Position = new Vector2(200, height);
                    break;

                case ArrowType.Up:
                    arrow.Position = new Vector2(300, height);
                    break;
            }
        }
    }

    public override void _Process(double delta)
    {
    }
}
