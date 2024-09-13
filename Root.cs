using Godot;

namespace LD56;

using static Utils;

public enum GamePhase {
  Countdown,
  Playing,
  GameOver
}

public partial class Root : Node2D {
  private const int BPM = 180;
  private const float MARGIN_OF_ERROR = 0.1f;
  private const float BEAT_DURATION = 60f / BPM;

  private int combo;

  private float _songElapsed = 0;
  private GamePhase _currentPhase = GamePhase.Countdown;
  private float _lastBeatTime = 0;
  private float _timeSinceLastKeyPress = 0;

  // Different exported sprites for the cat
  [Export] public Texture2D CatDown;
  [Export] public Texture2D CatLeft;
  [Export] public Texture2D CatUp;
  [Export] public Texture2D CatRight;
  [Export] public Texture2D CatNeutral;

  // Background textures
  [Export] public Texture2D BackgroundDark;
  [Export] public Texture2D BackgroundLight;

  // Magic circle textures
  [Export] public Texture2D MagicCircleDark;
  [Export] public Texture2D MagicCircleLight;

  // Goat textures
  [Export] public Texture2D GoatHappy;
  [Export] public Texture2D GoatSad;
  [Export] public Texture2D GoatNeutral;

  public override void _Ready() {
    Nodes.AudioStreamPlayer2D.Play();
    Nodes.Background.Texture = BackgroundLight;
    Nodes.Background_MagicCircle.Texture = MagicCircleLight;
    Nodes.Background_SpotlightL.Visible = false;
    Nodes.Background_SpotlightR.Visible = false;

    StartCountdown();
  }

  private void StartCountdown() {
    var countdown = Nodes.UI_CountdownContainer_Countdown;

    countdown.Visible = true;
    countdown.Text = "3";

    countdown.Scale = new Vector2(3, 3);
    var tween = CreateTween();
    tween.TweenProperty(countdown, "scale", new Vector2(1, 1), BEAT_DURATION);

    var timer = GetTree().CreateTimer(BEAT_DURATION);
    timer.Timeout += OnCountdownTimeout;
  }

  private void OnCountdownTimeout() {
    var countdown = Nodes.UI_CountdownContainer_Countdown;
    var currentNumber = int.Parse(countdown.Text);
    currentNumber--;

    countdown.Scale = new(3, 3);
    var tween = CreateTween();
    tween.TweenProperty(countdown, "scale", new Vector2(1, 1), BEAT_DURATION);

    if (currentNumber == 0) {
      countdown.Text = "GO!";
      var timer = GetTree().CreateTimer(BEAT_DURATION);
      timer.Timeout += StartGame;
    }
    else {
      countdown.Text = currentNumber.ToString();
      var timer = GetTree().CreateTimer(BEAT_DURATION);
      timer.Timeout += OnCountdownTimeout;
    }
  }

  private void StartGame() {
    var countdown = Nodes.UI_CountdownContainer_Countdown;

    countdown.Visible = false;
    _currentPhase = GamePhase.Playing;
    Nodes.AudioStreamPlayer2D.Play();
    _lastBeatTime = 0;

    // Darken the room and turn on the spotlights
    Nodes.Background.Texture = BackgroundDark;
    Nodes.Background_MagicCircle.Texture = MagicCircleDark;
    Nodes.Background_SpotlightL.Visible = true;

    Timer timer = new() {
      WaitTime = BEAT_DURATION,
      OneShot = false
    };
    timer.Timeout += OnBeatTimerTimeout;
    AddChild(timer);
    timer.Start();

    Nodes.BeatMapSync.started = true;
  }

  private void EndGame() {
    _currentPhase = GamePhase.GameOver;
    Nodes.AudioStreamPlayer2D.Stop();

    Nodes.UI_GameOver.Visible = true;
  }

  private void OnBeatTimerTimeout() {
    if (_currentPhase == GamePhase.Playing) {
      Nodes.Background_SpotlightL.Visible = !Nodes.Background_SpotlightL.Visible;
      Nodes.Background_SpotlightR.Visible = !Nodes.Background_SpotlightR.Visible;

      Nodes.Background_CrowdContainer_CrowdBG.Scale = new Vector2(1.1f, 1.1f);
      Nodes.Background_CrowdContainer_CrowdFG.Scale = new Vector2(1.1f, 1.1f);
    }
  }

  public override void _Process(double delta) {
    if (_currentPhase == GamePhase.Playing) {
      _songElapsed += (float)delta;
      _timeSinceLastKeyPress += (float)delta;
      if (_timeSinceLastKeyPress > BEAT_DURATION) {
        Nodes.Background_CatContainer_Cat.Texture = CatNeutral;
      }

      Vector2 newScale = new(Mathf.Max(1, Nodes.Background_CatContainer_Cat.Scale.X - (float)delta * 2), Mathf.Max(1, Nodes.Background_CatContainer_Cat.Scale.Y - (float)delta * 2));
      Nodes.Background_CatContainer_Cat.Scale = newScale;

      // Lerp crowd scale and rotation back to 1
      Nodes.Background_CrowdContainer_CrowdBG.Scale = Nodes.Background_CrowdContainer_CrowdBG.Scale.Lerp(new Vector2(1, 1), (float)delta * 2);
      Nodes.Background_CrowdContainer_CrowdFG.Scale = Nodes.Background_CrowdContainer_CrowdFG.Scale.Lerp(new Vector2(1, 1), (float)delta * 2);

      Nodes.Background_CrowdContainer_CrowdBG.Rotation = Mathf.Lerp(Nodes.Background_CrowdContainer_CrowdBG.Rotation, 0, (float)delta * 2);
      Nodes.Background_CrowdContainer_CrowdFG.Rotation = Mathf.Lerp(Nodes.Background_CrowdContainer_CrowdFG.Rotation, 0, (float)delta * 2);
    }
  }

  public override void _Input(InputEvent @event) {
    if (@event is InputEventKey keyEvent && keyEvent.Pressed) {
      _timeSinceLastKeyPress = 0;
      Nodes.UI_KeyPressed.Text = keyEvent.Keycode.ToString();

      var wasMotion = false;

      // Switch the cat sprite based on the key pressed
      switch (keyEvent.Keycode) {
        case Key.W:
        case Key.Up:
          Nodes.Background_CatContainer_Cat.Texture = CatUp;
          wasMotion = true;
          break;
        case Key.A:
        case Key.Left:
          Nodes.Background_CatContainer_Cat.Texture = CatLeft;
          wasMotion = true;
          break;
        case Key.S:
        case Key.Down:
          Nodes.Background_CatContainer_Cat.Texture = CatDown;
          wasMotion = true;
          break;
        case Key.D:
        case Key.Right:
          Nodes.Background_CatContainer_Cat.Texture = CatRight;
          wasMotion = true;
          break;
      }

      if (wasMotion) {
        ShowBeatResult(keyEvent.Keycode.ToString());
        Nodes.Background_CatContainer_Cat.Scale = new Vector2(1.2f, 1.2f);
      }
    }
  }

  private void AddToScore(int score) {
    Nodes.UI_Score.Text = (int.Parse(
        Nodes.UI_Score.Text.Replace(",", "")
    ) + score).ToString("N0");
  }

  private void ShowBeatResult(string key) {
    if (Nodes.BeatMapSync == null)
      return;

    var node = new Label();
    var arrows = Nodes.BeatMapSync.Arrows;

    BeatMapArrow? closestArrow = null;
    float closestTimeDifference = float.MaxValue;

    foreach (var arrow in arrows) {
      var expectedArrowTime = arrow.note.StartBeat * BEAT_DURATION + (
          (float)arrow.note.Division.numerator / (float)arrow.note.Division.denominator
      ) * BEAT_DURATION;

      var timeDifference = Mathf.Abs(expectedArrowTime - _songElapsed);

      if (timeDifference < closestTimeDifference) {
        closestTimeDifference = timeDifference;
        closestArrow = arrow;
      }
    }

    if (closestTimeDifference >= 0.2) {
      // Too far away to even flag the note as an error, just die!
      node.Text = "Miss";
      Nodes.Background_Goat.Texture = GoatSad;
      combo = 0;
    }
    else if (
        closestArrow != null &&
        (closestArrow.arrow.Type == ArrowType.Up && key != "W" && key != "Up" ||
        closestArrow.arrow.Type == ArrowType.Down && key != "S" && key != "Down" ||
        closestArrow.arrow.Type == ArrowType.Left && key != "A" && key != "Left" ||
        closestArrow.arrow.Type == ArrowType.Right && key != "D" && key != "Right")
    ) {
      node.Text = "Miss";
      Nodes.Background_Goat.Texture = GoatSad;
      combo = 0;
    }
    else {
      if (closestTimeDifference < MARGIN_OF_ERROR) {
        node.Text = "Perfect";
        combo++;
        AddToScore(100);
        Nodes.Background_Goat.Texture = GoatHappy;
      }
      else if (closestTimeDifference < MARGIN_OF_ERROR * 2) {
        node.Text = "Good";
        combo++;
        AddToScore(15);
        Nodes.Background_Goat.Texture = GoatNeutral;
      }
      else if (closestTimeDifference < MARGIN_OF_ERROR * 3) {
        AddToScore(5);
        node.Text = "Almost";
        combo = 0;
        Nodes.Background_Goat.Texture = GoatSad;
      }
      else {
        node.Text = "Miss";
        combo = 0;
        Nodes.Background_Goat.Texture = GoatSad;
      }

      if (closestArrow == null)
        return;

      closestArrow.arrow.isInMotion = false;
      closestArrow.arrow.Visible = false;
    }

    var comboLabel = Nodes.UI_CountdownContainer_Combo;
    if (combo > 5) {
      comboLabel.Text = "Combo: " + combo;
    }
    else {
      comboLabel.Text = "";
    }

    node.SetAnchorsPreset(Control.LayoutPreset.FullRect);
    node.HorizontalAlignment = HorizontalAlignment.Center;
    node.VerticalAlignment = VerticalAlignment.Center;

    Nodes.UI.AddChild(node);

    var tween = CreateTween();
    tween.TweenProperty(node, "modulate:a", 0, MARGIN_OF_ERROR * 2);
    tween.TweenCallback(Callable.From(() => node.QueueFree()));

    Node2D? hitArrow = null;

    switch (key) {
      case "W":
      case "Up":
        hitArrow = Nodes.UpArrow;
        break;

      case "S":
      case "Down":
        hitArrow = Nodes.DownArrow;
        break;

      case "A":
      case "Left":
        hitArrow = Nodes.LeftArrow;
        break;

      case "D":
      case "Right":
        hitArrow = Nodes.RightArrow;
        break;
    }

    if (hitArrow != null) {
      hitArrow.Scale = new Vector2(2, 2);

      var arrowTween = CreateTween();
      arrowTween.TweenProperty(hitArrow, "scale", new Vector2(1, 1), 0.1f);
    }
  }
}
