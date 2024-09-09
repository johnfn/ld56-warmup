using Godot;

public enum ArrowType
{
    Up,
    Down,
    Left,
    Right,
}

[Tool]
public partial class Arrow : Node2D
{
    private Sprite2D _arrowUp
    {
        get
        {
            return GetNode<Sprite2D>("ArrowUp");
        }
    }
    private Sprite2D _arrowDown
    {
        get
        {
            return GetNode<Sprite2D>("ArrowDown");
        }
    }
    private Sprite2D _arrowLeft
    {
        get
        {
            return GetNode<Sprite2D>("ArrowLeft");
        }
    }
    private Sprite2D _arrowRight
    {
        get
        {
            return GetNode<Sprite2D>("ArrowRight");
        }
    }

    private ArrowType _type;
    [Export(PropertyHint.Enum)]
    public ArrowType Type
    {
        get => _type;
        set
        {
            _type = value;
            UpdateArrowVisibility();
        }
    }

    public override void _Ready()
    {
        _arrowDown.Visible = false;
        _arrowLeft.Visible = false;
        _arrowRight.Visible = false;
        _arrowUp.Visible = false;

        UpdateArrowVisibility();
    }

    public override void _Process(double delta)
    {
    }


    private void UpdateArrowVisibility()
    {
        GD.Print($"Arrow type: {_type}");
        _arrowDown.Visible = _type == ArrowType.Down;
        _arrowLeft.Visible = _type == ArrowType.Left;
        _arrowRight.Visible = _type == ArrowType.Right;
        _arrowUp.Visible = _type == ArrowType.Up;
    }

}
