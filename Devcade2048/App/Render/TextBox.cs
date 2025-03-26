using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.Render;
public static class TextBox {
    // XXX: Copy of code located at Display.DrawAsset. This is probably bad...
    public static void DrawString(SpriteBatch sb, SpriteFont font, string data, Vector2 pos, Color color) {
        #region
#if DEBUG
        sb.DrawString(font, data, pos, color);
#else
        float scale = 18f / 7f;
        Vector2 actualPos = new(
            (int) (pos.X * scale),
            (int) (pos.Y * scale)
        );
        sb.DrawString(
            spriteFont: font, 
            text: data, 
            position: actualPos, 
            color: color, 
            rotation: 0.0f, 
            origin: new(), 
            scale: scale, 
            effects: SpriteEffects.None, 
            layerDepth: 0.0f
        );
#endif
        #endregion
    }

    public static void DrawInArea(SpriteBatch sb, string text, Rectangle area, Color color) {
        string[] paragraphs = text.Split('\n');
        foreach (string paragraph in paragraphs) {
            int dy = WriteParagraph(sb, paragraph, area, color);
            area.Y += dy;
            area.Height -= dy;
        }
    }

    public static int WriteParagraph(SpriteBatch sb, string text, Rectangle area, Color color) {
        string[] words = text.Split(' ');
        string line = "";
        Vector2 pos = new Vector2(area.Left, area.Top);
        Vector2 size = Vector2.Zero;
        foreach (string word in words) {
            string newLine = line + " " + word;
            size = Asset.SmallFont.MeasureString(newLine);
            if (size.X > area.Width) {
                DrawString(sb, Asset.SmallFont, line, pos, color);
                line = word;
                pos.Y += size.Y + 1;
            } else {
                line = newLine;
            }
        }
        DrawString(sb, Asset.SmallFont, line, pos, color);
        return (int) (pos.Y - area.Top + 2 * size.Y);
    }

    public static void WriteCenteredText(SpriteBatch sb, string text, Vector2 pos, Color color) {
        Vector2 offset = Asset.BigFont.MeasureString(text) / 2; 
        DrawString(sb, Asset.BigFont, text, pos - offset, color);
    }
}