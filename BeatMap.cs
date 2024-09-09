using System.Collections.Generic;

public class BeatMap
{
    public List<BeatMapNote> Notes { get; set; }

    public static BeatMap GetBeatMap()
    {
        return new BeatMap
        {
            Notes = new List<BeatMapNote> {
        new(0, (0, 4), ArrowType.Right),
        new(1, (0, 4), ArrowType.Right),
        new(2, (0, 4), ArrowType.Right),
        new(3, (0, 4), ArrowType.Right),
        new(4, (0, 4), ArrowType.Right),
        new(5, (0, 4), ArrowType.Left),
        // new(6, (0, 4), ArrowType.Left),
        // new(7, (0, 4), ArrowType.Down),
        // new(8, (0, 4), ArrowType.Right),
        // new(9, (0, 4), ArrowType.Up),
        // new(10, (0, 4), ArrowType.Left),
        // new(11, (0, 4), ArrowType.Down),
        // new(12, (0, 4), ArrowType.Right),
        // new(13, (0, 4), ArrowType.Right),
        // new(14, (0, 4), ArrowType.Up),
        // new(15, (0, 4), ArrowType.Left),
        // new(16, (0, 4), ArrowType.Down),
        // new(17, (0, 4), ArrowType.Right),
        // new(18, (0, 4), ArrowType.Up),
        // new(19, (0, 4), ArrowType.Left),
        // new(20, (0, 4), ArrowType.Down),
        // new(21, (0, 4), ArrowType.Right),
        // new(22, (0, 4), ArrowType.Right),

      },
        };
    }
}

public record BeatMapNote(
  int StartBeat,
  (int numerator, int denominator) Division,
  ArrowType ArrowType
);
