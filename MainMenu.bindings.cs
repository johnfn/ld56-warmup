using Godot;
namespace LD56;public partial class MainMenu : ColorRect {
  public static MainMenu New() {
    return GD.Load<PackedScene>("res://main_menu.tscn").Instantiate<MainMenu>();
  }
  public MainMenu() {
    foreach (var @interface in GetType().GetInterfaces()) {
      AddToGroup(@interface.Name);
    }
  }

  public partial class MainMenuNodes {
    MainMenu parent;
    public MainMenuNodes(MainMenu parent) {
      this.parent = parent;
    }
    private TextureRect? _Spotlight;
    public TextureRect Spotlight {
      get {
        return _Spotlight ??= parent.GetNode<TextureRect>("Spotlight");
      }
    }

    private Label? _Title;
    public Label Title {
      get {
        return _Title ??= parent.GetNode<Label>("Title");
      }
    }

    private Button? _PlayButton;
    public Button PlayButton {
      get {
        return _PlayButton ??= parent.GetNode<Button>("PlayButton");
      }
    }

    private TextureRect? _CatSprite;
    public TextureRect CatSprite {
      get {
        return _CatSprite ??= parent.GetNode<TextureRect>("CatSprite");
      }
    }

    private AudioStreamPlayer2D? _AudioStreamPlayer2D;
    public AudioStreamPlayer2D AudioStreamPlayer2D {
      get {
        return _AudioStreamPlayer2D ??= parent.GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
      }
    }

  }

  public MainMenuNodes? _Nodes;
  public MainMenuNodes Nodes {
    get {
      return _Nodes ??= new MainMenuNodes(this);
    }
  }
}
