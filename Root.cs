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

    // Nodes
    private Label _countdown;
    private AudioStreamPlayer2D _audioStreamPlayer2D;

    private Sprite2D _catSprite;

    private Sprite2D _goatSprite;

    private Sprite2D _background;

    private Sprite2D _magicCircle;

    private Sprite2D _spotlightR;
    private Sprite2D _spotlightL;

    private Sprite2D _crowdBG;
    private Sprite2D _crowdFG;

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


    public override void _Ready()
    {
        _countdown = GetNode<Label>("UI/Countdown");
        _audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        _catSprite = GetNode<Sprite2D>("Background/CatContainer/Cat");
        _background = GetNode<Sprite2D>("Background");
        _background.Texture = BackgroundLight;

        _magicCircle = GetNode<Sprite2D>("Background/MagicCircle");
        _magicCircle.Texture = MagicCircleLight;

        _spotlightR = GetNode<Sprite2D>("Background/SpotlightR");
        _spotlightL = GetNode<Sprite2D>("Background/SpotlightL");
        _spotlightL.Visible = false;
        _spotlightR.Visible = false;

        _goatSprite = GetNode<Sprite2D>("Background/Goat");

        _crowdBG = GetNode<Sprite2D>("Background/CrowdContainer/CrowdBG");
        _crowdFG = GetNode<Sprite2D>("Background/CrowdContainer/CrowdFG");


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

        // Darken the room and turn on the spotlights
        _background.Texture = BackgroundDark;
        _magicCircle.Texture = MagicCircleDark;
        _spotlightL.Visible = true;

        Timer timer = new Timer();
        timer.WaitTime = BEAT_DURATION;
        timer.OneShot = false;
        timer.Timeout += OnBeatTimerTimeout;
        AddChild(timer);
        timer.Start();
    }

    private void OnBeatTimerTimeout()
    {
        if (_currentPhase == GamePhase.Playing)
        {
            _spotlightL.Visible = !_spotlightL.Visible;
            _spotlightR.Visible = !_spotlightR.Visible;

            _crowdBG.Scale = new Vector2(1.1f, 1.1f);
            _crowdFG.Scale = new Vector2(1.1f, 1.1f);
        }
    }

    public override void _Process(double delta)
    {
        if (_currentPhase == GamePhase.Playing)
        {
            _songElapsed += (float)delta;
            _timeSinceLastKeyPress += (float)delta;
            if (_timeSinceLastKeyPress > BEAT_DURATION)
            {
                _catSprite.Texture = CatNeutral;
            }  

            Vector2 newScale = new Vector2(Mathf.Max(1, _catSprite.Scale.X - (float) delta * 2), Mathf.Max(1, _catSprite.Scale.Y - (float) delta * 2));
            _catSprite.Scale = newScale;

            // Lerp crowd scale and rotation back to 1
            _crowdBG.Scale = _crowdBG.Scale.Lerp(new Vector2(1, 1), (float)delta * 2);
            _crowdFG.Scale = _crowdFG.Scale.Lerp(new Vector2(1, 1), (float)delta * 2);

            _crowdBG.Rotation = Mathf.Lerp(_crowdBG.Rotation, 0, (float)delta * 2);
            _crowdFG.Rotation = Mathf.Lerp(_crowdFG.Rotation, 0, (float)delta * 2);
        }

        
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            _timeSinceLastKeyPress = 0;
            GetNode<Label>("UI/KeyPressed").Text = keyEvent.Keycode.ToString();
            ShowBeatResult(keyEvent.Keycode.ToString());

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

    private void ShowBeatResult(string key)
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
                _goatSprite.Texture = GoatHappy;
            }
            else if (closestTimeDifference < MARGIN_OF_ERROR * 2)
            {
                node.Text = "Good";
                _goatSprite.Texture = GoatNeutral;
            }
            else if (closestTimeDifference < MARGIN_OF_ERROR * 3)
            {
                node.Text = "Almost";
                _goatSprite.Texture = GoatSad;
            }
            else
            {
                node.Text = "Miss";
                _goatSprite.Texture = GoatSad;
            }

            if (closestArrow == null)
                return;

            closestArrow.arrow.Visible = false;
        }

        node.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        node.HorizontalAlignment = HorizontalAlignment.Center;
        node.VerticalAlignment = VerticalAlignment.Center;

        GetNode<Control>("UI").AddChild(node);

        var tween = CreateTween();
        tween.TweenProperty(node, "modulate:a", 0, MARGIN_OF_ERROR * 2);
        tween.TweenCallback(Callable.From(() => node.QueueFree()));

        var lArrow = GetTree().Root.GetNode<Node2D>("Root/LeftArrow");
        var dArrow = GetTree().Root.GetNode<Node2D>("Root/DownArrow");
        var uArrow = GetTree().Root.GetNode<Node2D>("Root/UpArrow");
        var rArrow = GetTree().Root.GetNode<Node2D>("Root/RightArrow");

        Node2D hitArrow = null;

        GD.Print(key);

        switch (key)
        {
            case "W":
            case "Up":
                hitArrow = lArrow;
                break;

            case "S":
            case "Down":
                hitArrow = dArrow;
                break;

            case "A":
            case "Left":
                hitArrow = uArrow;
                break;

            case "D":
            case "Right":
                hitArrow = rArrow;
                break;
        }

        if (hitArrow != null)
        {
            hitArrow.Scale = new Vector2(2, 2);

            var arrowTween = CreateTween();
            arrowTween.TweenProperty(hitArrow, "scale", new Vector2(1, 1), 0.1f);
        }
    }
}
