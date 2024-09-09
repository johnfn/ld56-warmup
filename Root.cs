using Godot;
using System;

public enum GamePhase
{
    Countdown,
    Playing,
    GameOver
}

public partial class Root : Node2D
{
    private const int BPM = 180;
    private const float MARGIN_OF_ERROR = 0.1f;
    private const float BEAT_DURATION = 60f / BPM;

    private BeatMap _beatMap = BeatMap.GetBeatMap();
    private float _songElapsed = 0;
    private GamePhase _currentPhase = GamePhase.Countdown;
    private float _lastBeatTime = 0;

    private Label _countdown;
    private ColorRect _beatIndicator;

    private AudioStreamPlayer2D _audioStreamPlayer2D;

    private Sprite2D _catSprite;

    // Different exported sprites for the cat
    [Export] public Texture2D CatDown;
    [Export] public Texture2D CatLeft;
    [Export] public Texture2D CatUp;
    [Export] public Texture2D CatRight;

    public override void _Ready()
    {
        _countdown = GetNode<Label>("UI/Countdown");
        _beatIndicator = GetNode<ColorRect>("UI/BeatIndicator");
        _audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        _catSprite = GetNode<Sprite2D>("Background/CatContainer/Cat");

        StartCountdown();
    }

    private void StartCountdown()
    {
        _countdown.Visible = true;
        _countdown.Text = "3";

        var timer = GetTree().CreateTimer(1);
        timer.Timeout += OnCountdownTimeout;
    }

    private void OnCountdownTimeout()
    {
        var currentNumber = int.Parse(_countdown.Text);
        currentNumber--;

        if (currentNumber == 0)
        {
            _countdown.Text = "GO!";
            var timer = GetTree().CreateTimer(BEAT_DURATION);
            timer.Timeout += StartGame;

            var arrows = BeatMapSync.Instantiate();
            GetTree().Root.AddChild(arrows);
        }
        else
        {
            _countdown.Text = currentNumber.ToString();
            var timer = GetTree().CreateTimer(BEAT_DURATION);
            timer.Timeout += OnCountdownTimeout;
        }
    }

    private void StartGame()
    {
        _countdown.Visible = false;
        _currentPhase = GamePhase.Playing;
        _audioStreamPlayer2D.Play();
        _lastBeatTime = 0;
    }

    public override void _Process(double delta)
    {
        if (_currentPhase == GamePhase.Playing)
        {
            _songElapsed += (float)delta;
            UpdateBeatIndicator();

            if (_catSprite.Scale.Length() < 1)
                _catSprite.Scale -= new Vector2(1, 1) * (float)delta;
        }
    }

    private void UpdateBeatIndicator()
    {
        float timeSinceLastBeat = _songElapsed - _lastBeatTime;
        float alpha = 1 - (timeSinceLastBeat / BEAT_DURATION);
        _beatIndicator.Modulate = new Color(1, 1, 1, Mathf.Clamp(alpha, 0, 1));

        if (timeSinceLastBeat >= BEAT_DURATION)
        {
            _lastBeatTime = _songElapsed;
            _beatIndicator.Modulate = new Color(1, 1, 1, 1);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            GetNode<Label>("UI/KeyPressed").Text = keyEvent.Keycode.ToString();
            ShowBeatResult();

            // Switch the cat sprite based on the key pressed
            switch (keyEvent.Keycode)
            {
                case Key.W:
                    _catSprite.Texture = CatUp;
                    break;
                case Key.A:
                    _catSprite.Texture = CatLeft;
                    break;
                case Key.S:
                    _catSprite.Texture = CatDown;
                    break;
                case Key.D:
                    _catSprite.Texture = CatRight;
                    break;
            }

            // Set cat scale to 1.2
            _catSprite.Scale = new Vector2(1.2f, 1.2f);
        }
    }

    private void ShowBeatResult()
    {
        if (BeatMapSync.Instance == null)
            return;

        var node = new Label();
        var arrows = BeatMapSync.Instance.Arrows;

        BeatMapArrow? closestArrow = null;
        float closestTimeDifference = float.MaxValue;

        foreach (var arrow in arrows)
        {
            var expectedArrowTime = arrow.note.StartBeat * BEAT_DURATION + (
                (float)arrow.note.Division.numerator / (float)arrow.note.Division.denominator
            ) * BEAT_DURATION;

            var timeDifference = Mathf.Abs(expectedArrowTime - _songElapsed);

            if (timeDifference < closestTimeDifference)
            {
                closestTimeDifference = timeDifference;
                closestArrow = arrow;
            }
        }

        if (closestTimeDifference >= 0.2)
        {
            // Too far away to even flag the note as an error, just die!
            node.Text = "Miss";
        }
        else
        {
            if (closestTimeDifference < MARGIN_OF_ERROR)
            {
                node.Text = "Perfect";
            }
            else if (closestTimeDifference < MARGIN_OF_ERROR * 2)
            {
                node.Text = "Good";
            }
            else if (closestTimeDifference < MARGIN_OF_ERROR * 3)
            {
                node.Text = "Almost";
            }
            else
            {
                node.Text = "Miss";
            }

            if (closestArrow == null)
                return;

            closestArrow.arrow.Visible = false;
        }

        node.SetAnchorsPreset(Control.LayoutPreset.Center);
        GetNode<Control>("UI").AddChild(node);

        var tween = CreateTween();
        tween.TweenProperty(node, "modulate:a", 0, MARGIN_OF_ERROR * 2);
        tween.TweenCallback(Callable.From(() => node.QueueFree()));
    }
}
