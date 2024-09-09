using Godot;
using System;
using System.Collections.Generic;

public record BeatMapArrow(
  Arrow arrow,
  BeatMapNote note
);

public partial class BeatMapSync : Node2D
{
    private List<BeatMapArrow> _arrows = new List<BeatMapArrow>();
    private int _bpm;

    public static BeatMapSync Instantiate()
    {
        var beatMapSync = GD.Load<PackedScene>("res://beat_map_sync.tscn").Instantiate<BeatMapSync>();
        return beatMapSync;
    }

    public override void _Ready()
    {
        CreateArrows(180);

        GD.Print("BeatMapSync instantiated");
    }

    private void CreateArrows(int bpm)
    {
        var _beatMap = BeatMap.GetBeatMap();
        var _beatHeight = 450; // should probably calculate this based on bpm
        _bpm = bpm;

        for (int i = 0; i < _beatMap.Notes.Count; i++)
        {
            var note = _beatMap.Notes[i];
            var arrow = Arrow.Instantiate(note.ArrowType);
            AddChild(arrow);

            var height = note.StartBeat * _beatHeight;
            var div = ((float)note.Division.numerator / (float)note.Division.denominator);
            height += (int)(_beatHeight * div);

            switch (_beatMap.Notes[i].ArrowType)
            {
                case ArrowType.Left:
                    arrow.Position = new Vector2(700, height);
                    break;

                case ArrowType.Down:
                    arrow.Position = new Vector2(900, height);
                    break;

                case ArrowType.Up:
                    arrow.Position = new Vector2(1100, height);
                    break;

                case ArrowType.Right:
                    arrow.Position = new Vector2(1300, height);
                    break;
            }

            _arrows.Add(new(arrow, note));
        }
    }

    public override void _Process(double delta)
    {
        var beatHeight = 450;
        var beatTime = (double)60 / (double)_bpm;

        for (int i = 0; i < _arrows.Count; i++)
        {
            var (arrow, note) = _arrows[i];

            var speed = beatHeight / beatTime;
            var movement = (float)(speed * delta);
            arrow.Position -= new Vector2(0, movement);
        }
    }
}
