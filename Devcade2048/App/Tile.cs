using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;


namespace Devcade2048.App;

public class Tile
{
    public Vector2 Position { get; private set; }
    public int Value { get; }

    public Vector2? PreviousPosition { get; private set; } = null;
    #nullable enable
    public Tile[]? MergedFrom { get; set; } = null;
    #nullable disable

    public Tile(Vector2 position) {
        Position = new Vector2(position.X, Position.Y);
        Value = 2;
    }

    public Tile(Vector2 position, int value) : this(position) {
        Value = value;
    }

    public void SavePosition() {
        PreviousPosition = new Vector2(Position.X, Position.Y);
    }

    public void UpdatePosition(Vector2 position) {
        Position = new Vector2(position.X, position.Y);
    }
}