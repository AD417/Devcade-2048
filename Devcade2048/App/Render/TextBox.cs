using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.Render;

public static class TextBox {
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
        Vector2 size = new Vector2()    ;
        foreach (string word in words) {
            string newLine = line + " " + word;
            size = Asset.SmallFont.MeasureString(newLine);
            if (size.X > area.Width) {
                sb.DrawString(Asset.SmallFont, line, pos, color);
                line = word;
                pos.Y += size.Y + 1;
            } else {
                line = newLine;
            }
        }
        sb.DrawString(Asset.SmallFont, line, pos, color);
        return (int) (pos.Y - area.Top + 2 * size.Y);
    }

    public static void WriteCenteredText(SpriteBatch sb, string text, Vector2 pos, Color color) {
        Vector2 offset = Asset.BigFont.MeasureString(text) / 2; 
        sb.DrawString(Asset.BigFont, text, pos - offset, color);
    }
}