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
    public static BeatMapSync Instance { get; private set; }
    public bool started = false;

    public List<BeatMapArrow> Arrows => _arrows;

    public static BeatMapSync Instantiate()
    {
        var beatMapSync = GD.Load<PackedScene>("res://beat_map_sync.tscn").Instantiate<BeatMapSync>();
        return beatMapSync;
    }

    public override void _Ready()
    {
        Instance = this;
        CreateArrows(180);
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

            height += 100;

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
        if (!started)
            return;

        var beatHeight = 450;
        var beatTime = (double)60 / (double)_bpm; // (1 / 3)

        for (int i = 0; i < _arrows.Count; i++)
        {
            var (arrow, note) = _arrows[i];

            if (!arrow.isInMotion)
                continue;

            var speed = beatHeight / beatTime;
            var movement = (float)(speed * delta);
            arrow.Position -= new Vector2(0, movement);
        }
    }
}
