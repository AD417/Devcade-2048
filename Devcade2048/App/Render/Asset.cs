using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.Render;

public static class Asset {
    public static Texture2D Title;
    public static Texture2D[] Menu = new Texture2D[4];
    public static Texture2D Grid;
    public static Texture2D[] Tile = new Texture2D[16];
    public static Texture2D[] LoseTile = new Texture2D[2];
    public static SpriteFont BigFont;
    public static SpriteFont SmallFont;
    public static Texture2D Button;
}