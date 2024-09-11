using Godot;
namespace LD56;public partial class Root : Node2D {
  public static Root New() {
    return GD.Load<PackedScene>("res://root.tscn").Instantiate<Root>();
  }
  public Root() {
    foreach (var @interface in GetType().GetInterfaces()) {
      AddToGroup(@interface.Name);
    }
  }

  public partial class RootNodes {
    Root parent;
    public RootNodes(Root parent) {
      this.parent = parent;
    }
    private Sprite2D? _Background;
    public Sprite2D Background {
      get {
        return _Background ??= parent.GetNode<Sprite2D>("Background");
      }
    }

    private Sprite2D? _Background_MagicCircle;
    public Sprite2D Background_MagicCircle {
      get {
        return _Background_MagicCircle ??= parent.GetNode<Sprite2D>("Background/MagicCircle");
      }
    }

    private Sprite2D? _Background_Goat;
    public Sprite2D Background_Goat {
      get {
        return _Background_Goat ??= parent.GetNode<Sprite2D>("Background/Goat");
      }
    }

    private Sprite2D? _Background_Demon;
    public Sprite2D Background_Demon {
      get {
        return _Background_Demon ??= parent.GetNode<Sprite2D>("Background/Demon");
      }
    }

    private Sprite2D? _Background_MagicCircleLight;
    public Sprite2D Background_MagicCircleLight {
      get {
        return _Background_MagicCircleLight ??= parent.GetNode<Sprite2D>("Background/MagicCircleLight");
      }
    }

    private Sprite2D? _Background_SpotlightR;
    public Sprite2D Background_SpotlightR {
      get {
        return _Background_SpotlightR ??= parent.GetNode<Sprite2D>("Background/SpotlightR");
      }
    }

    private Sprite2D? _Background_SpotlightL;
    public Sprite2D Background_SpotlightL {
      get {
        return _Background_SpotlightL ??= parent.GetNode<Sprite2D>("Background/SpotlightL");
      }
    }

    private Node2D? _Background_CatContainer;
    public Node2D Background_CatContainer {
      get {
        return _Background_CatContainer ??= parent.GetNode<Node2D>("Background/CatContainer");
      }
    }

    private Sprite2D? _Background_CatContainer_Cat;
    public Sprite2D Background_CatContainer_Cat {
      get {
        return _Background_CatContainer_Cat ??= parent.GetNode<Sprite2D>("Background/CatContainer/Cat");
      }
    }

    private Node2D? _Background_CrowdContainer;
    public Node2D Background_CrowdContainer {
      get {
        return _Background_CrowdContainer ??= parent.GetNode<Node2D>("Background/CrowdContainer");
      }
    }

    private Sprite2D? _Background_CrowdContainer_CrowdBG;
    public Sprite2D Background_CrowdContainer_CrowdBG {
      get {
        return _Background_CrowdContainer_CrowdBG ??= parent.GetNode<Sprite2D>("Background/CrowdContainer/CrowdBG");
      }
    }

    private Sprite2D? _Background_CrowdContainer_CrowdFG;
    public Sprite2D Background_CrowdContainer_CrowdFG {
      get {
        return _Background_CrowdContainer_CrowdFG ??= parent.GetNode<Sprite2D>("Background/CrowdContainer/CrowdFG");
      }
    }

    private Control? _UI;
    public Control UI {
      get {
        return _UI ??= parent.GetNode<Control>("UI");
      }
    }

    private Label? _UI_KeyPressed;
    public Label UI_KeyPressed {
      get {
        return _UI_KeyPressed ??= parent.GetNode<Label>("UI/KeyPressed");
      }
    }

    private Label? _UI_Score;
    public Label UI_Score {
      get {
        return _UI_Score ??= parent.GetNode<Label>("UI/Score");
      }
    }

    private Panel? _UI_GameOver;
    public Panel UI_GameOver {
      get {
        return _UI_GameOver ??= parent.GetNode<Panel>("UI/GameOver");
      }
    }

    private MarginContainer? _UI_GameOver_MarginContainer;
    public MarginContainer UI_GameOver_MarginContainer {
      get {
        return _UI_GameOver_MarginContainer ??= parent.GetNode<MarginContainer>("UI/GameOver/MarginContainer");
      }
    }

    private VBoxContainer? _UI_GameOver_MarginContainer_VBoxContainer;
    public VBoxContainer UI_GameOver_MarginContainer_VBoxContainer {
      get {
        return _UI_GameOver_MarginContainer_VBoxContainer ??= parent.GetNode<VBoxContainer>("UI/GameOver/MarginContainer/VBoxContainer");
      }
    }

    private Label? _UI_GameOver_MarginContainer_VBoxContainer_Label;
    public Label UI_GameOver_MarginContainer_VBoxContainer_Label {
      get {
        return _UI_GameOver_MarginContainer_VBoxContainer_Label ??= parent.GetNode<Label>("UI/GameOver/MarginContainer/VBoxContainer/Label");
      }
    }

    private HSeparator? _UI_GameOver_MarginContainer_VBoxContainer_HSeparator;
    public HSeparator UI_GameOver_MarginContainer_VBoxContainer_HSeparator {
      get {
        return _UI_GameOver_MarginContainer_VBoxContainer_HSeparator ??= parent.GetNode<HSeparator>("UI/GameOver/MarginContainer/VBoxContainer/HSeparator");
      }
    }

    private Control? _UI_GameOver_MarginContainer_VBoxContainer_Control;
    public Control UI_GameOver_MarginContainer_VBoxContainer_Control {
      get {
        return _UI_GameOver_MarginContainer_VBoxContainer_Control ??= parent.GetNode<Control>("UI/GameOver/MarginContainer/VBoxContainer/Control");
      }
    }

    private Label? _UI_GameOver_MarginContainer_VBoxContainer_Label2;
    public Label UI_GameOver_MarginContainer_VBoxContainer_Label2 {
      get {
        return _UI_GameOver_MarginContainer_VBoxContainer_Label2 ??= parent.GetNode<Label>("UI/GameOver/MarginContainer/VBoxContainer/Label2");
      }
    }

    private Label? _UI_GameOver_MarginContainer_VBoxContainer_Label3;
    public Label UI_GameOver_MarginContainer_VBoxContainer_Label3 {
      get {
        return _UI_GameOver_MarginContainer_VBoxContainer_Label3 ??= parent.GetNode<Label>("UI/GameOver/MarginContainer/VBoxContainer/Label3");
      }
    }

    private Control? _UI_GameOver_MarginContainer_VBoxContainer_Control2;
    public Control UI_GameOver_MarginContainer_VBoxContainer_Control2 {
      get {
        return _UI_GameOver_MarginContainer_VBoxContainer_Control2 ??= parent.GetNode<Control>("UI/GameOver/MarginContainer/VBoxContainer/Control2");
      }
    }

    private Button? _UI_GameOver_MarginContainer_VBoxContainer_Label4;
    public Button UI_GameOver_MarginContainer_VBoxContainer_Label4 {
      get {
        return _UI_GameOver_MarginContainer_VBoxContainer_Label4 ??= parent.GetNode<Button>("UI/GameOver/MarginContainer/VBoxContainer/Label4");
      }
    }

    private Control? _UI_CountdownContainer;
    public Control UI_CountdownContainer {
      get {
        return _UI_CountdownContainer ??= parent.GetNode<Control>("UI/CountdownContainer");
      }
    }

    private Label? _UI_CountdownContainer_Countdown;
    public Label UI_CountdownContainer_Countdown {
      get {
        return _UI_CountdownContainer_Countdown ??= parent.GetNode<Label>("UI/CountdownContainer/Countdown");
      }
    }

    private Label? _UI_CountdownContainer_Combo;
    public Label UI_CountdownContainer_Combo {
      get {
        return _UI_CountdownContainer_Combo ??= parent.GetNode<Label>("UI/CountdownContainer/Combo");
      }
    }

    private AudioStreamPlayer2D? _AudioStreamPlayer2D;
    public AudioStreamPlayer2D AudioStreamPlayer2D {
      get {
        return _AudioStreamPlayer2D ??= parent.GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
      }
    }

    private Arrow? _LeftArrow;
    public Arrow LeftArrow {
      get {
        return _LeftArrow ??= parent.GetNode<Arrow>("LeftArrow");
      }
    }

    private Arrow? _DownArrow;
    public Arrow DownArrow {
      get {
        return _DownArrow ??= parent.GetNode<Arrow>("DownArrow");
      }
    }

    private Arrow? _UpArrow;
    public Arrow UpArrow {
      get {
        return _UpArrow ??= parent.GetNode<Arrow>("UpArrow");
      }
    }

    private Arrow? _RightArrow;
    public Arrow RightArrow {
      get {
        return _RightArrow ??= parent.GetNode<Arrow>("RightArrow");
      }
    }

    private BeatMapSync? _BeatMapSync;
    public BeatMapSync BeatMapSync {
      get {
        return _BeatMapSync ??= parent.GetNode<BeatMapSync>("BeatMapSync");
      }
    }

  }

  public RootNodes? _Nodes;
  public RootNodes Nodes {
    get {
      return _Nodes ??= new RootNodes(this);
    }
  }
}
