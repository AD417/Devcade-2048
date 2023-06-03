using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.Render;

public static class Asset {
    public static Texture2D Title;
    public static Texture2D[] Menu = new Texture2D[3];
    public static Texture2D Grid;
    public static Texture2D[] Tile = new Texture2D[11];
    public static Texture2D[] LoseTile = new Texture2D[2];
    public static SpriteFont ScoreFont;
    public static Texture2D Button;
}