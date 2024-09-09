using System.Collections.Generic;

public class BeatMap
{
    public List<BeatMapNote> Notes { get; set; }

    public static BeatMap GetBeatMap()
    {
        return new BeatMap
        {
            Notes = new List<BeatMapNote> {
        new(4, (0, 4), ArrowType.Right),
        new(5, (0, 4), ArrowType.Right),
        new(6, (0, 4), ArrowType.Right),
        new(7, (0, 4), ArrowType.Right),
        new(8, (0, 4), ArrowType.Right),
        new(9, (0, 4), ArrowType.Right),
        new(10, (0, 4), ArrowType.Right),
        new(11, (0, 4), ArrowType.Right),
        new(12, (0, 4), ArrowType.Right),
        new(13, (0, 4), ArrowType.Right),
        new(14, (0, 4), ArrowType.Right),
        new(15, (0, 4), ArrowType.Right),
        new(16, (0, 4), ArrowType.Right),
        new(17, (0, 4), ArrowType.Right),
        new(18, (0, 4), ArrowType.Right),
        new(19, (0, 4), ArrowType.Right),
        new(20, (0, 4), ArrowType.Right),
        new(21, (0, 4), ArrowType.Right),
        new(22, (0, 4), ArrowType.Right),

      },
        };
    }
}

public record BeatMapNote(
  int StartBeat,
  (int numerator, int denominator) Division,
  ArrowType ArrowType
);
