using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace Devcade2048.App;

[Serializable]
public class Tile {
    private Vector2 position;
    private readonly int value;
    private readonly int textureId;
    [NonSerialized]
    private Vector2? previousPosition = null;
    [NonSerialized]
    private Tile[] mergedFrom = null;

    public Vector2 Position => position;
    public int Value => value;
    public int TextureId => textureId;
    public Vector2? PreviousPosition => previousPosition;
    public Tile[] MergedFrom { get => mergedFrom; set => mergedFrom = value; }

    public Tile(Vector2 position, int id) {
        this.position = position;
        this.textureId = id;
        this.value = 2;
    }

    public Tile(Vector2 position, int id, int value) : this(position, id) {
        this.value = value;
    }

    public void SavePosition() {
        this.previousPosition = new Vector2(position.X, position.Y);
    }

    public void UpdatePosition(Vector2 position) {
        this.position = new Vector2(position.X, position.Y);
    }
}