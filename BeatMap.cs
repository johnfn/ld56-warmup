using System.Collections.Generic;

public class BeatMap {
  public List<BeatMapNote> Notes { get; set; }

  public static BeatMap GetBeatMap() {
    return new BeatMap {
      Notes = new List<BeatMapNote> {
        new BeatMapNote(0, (0, 4), BeatType.Up),
        new BeatMapNote(0, (2, 4), BeatType.Up),
        new BeatMapNote(1, (0, 4), BeatType.Up),
        new BeatMapNote(1, (2, 4), BeatType.Up),
        new BeatMapNote(2, (0, 4), BeatType.Up),
        new BeatMapNote(2, (2, 4), BeatType.Up),
      },
    };
  }
}

public record BeatMapNote(
  int StartBeat,
  (int numerator, int denominator) Division,
  BeatType BeatType
);

public enum BeatType {
  Up,
  Down,
  Left,
  Right,
  UpLeft,
  UpRight,
  DownLeft,
  DownRight,
}
