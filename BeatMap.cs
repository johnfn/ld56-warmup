using System.Collections.Generic;

public class BeatMap
{
    public List<BeatMapNote> Notes { get; set; }

    public static BeatMap GetBeatMap()
    {
        return new BeatMap
        {
            Notes = new List<BeatMapNote> {
        new(0, (0, 4), ArrowType.Up),
        new(0, (2, 4), ArrowType.Down),
        new(1, (0, 4), ArrowType.Up),
        new(1, (2, 4), ArrowType.Down),
        new(2, (0, 4), ArrowType.Left),
        new(2, (2, 4), ArrowType.Right),
      },
        };
    }
}

public record BeatMapNote(
  int StartBeat,
  (int numerator, int denominator) Division,
  ArrowType ArrowType
);
