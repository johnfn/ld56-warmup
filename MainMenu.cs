using Godot;
using System;
namespace LD56;

public partial class MainMenu : ColorRect {

  private const int BPM = 180;

  private const float BEAT_DURATION = 60f / BPM;

  [Export] private Texture2D[] _catSprites;

  private TextureRect _catSprite;

  private Button _startButton;
  private Color[] _colors = [
    new Color(4, 2, 162), // Navy
    new Color(235, 36, 108), // Pink
    new Color(255, 227, 89), // Yellow
    new Color(255, 255, 255) // White
  ];


  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {
    _catSprite = GetNode<TextureRect>("CatSprite");
    _startButton = GetNode<Button>("PlayButton");
    _startButton.Pressed += OnStartButtonPressed;

    Timer t = new() {
      WaitTime = BEAT_DURATION,
      OneShot = false
    };
    t.Timeout += () => {
      // Switch the sprite to a random other sprite every second
      var nextSprite = GD.RandRange(0, _catSprites.Length);
      if (_catSprite.Texture == _catSprites[nextSprite]) {
        nextSprite = (nextSprite + 1) % _catSprites.Length;
      }
      _catSprite.Texture = _catSprites[nextSprite];
    };
    AddChild(t);
    t.Start();

  }

  public void OnStartButtonPressed() {
    GetTree().ChangeSceneToFile("res://root.tscn");
  }
}
