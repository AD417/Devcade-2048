using Devcade2048.App.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.State;

public class FromWinState : TransientState {

    public FromWinState() : base(GameEndingTime) {
    }

    public override BaseState NextState() {
        Game.Continue();
        return new GameWaitingState();
    }

    public override void Draw() {
        base.Draw();

        DrawAsset(Asset.Grid, new Vector2(10, 290));
        DrawAllTiles();
        DrawWinningTile();
        DrawScore();
        DrawWinText();
    }

    private void DrawAllTiles() {

        void drawTile(Tile t) {
            if (t is null) return;

            Vector2 pos = new Vector2(
                ( 12 + t.Position.X * 100),
                (292 + t.Position.Y * 100)
            );
            Rectangle location = new Rectangle(pos.ToPoint(), new Point(96,96));

            Texture2D blob = Asset.Tile[t.TextureId];

            DrawAsset(blob, location, Color.White);
        }
        Game.Grid.EachCell((int _x, int _y, Tile t) => drawTile(t));
    }

    private void DrawWinningTile() {

        void drawWin(Tile t) {
            if (t is null || t.Value != 2048) return;
            Rectangle location = winTilePos(t);

            DrawAsset(Asset.Tile[t.TextureId], location, Color.White);
        }

        Game.Grid.EachCell((int _x, int _y, Tile t) => drawWin(t));
    }

    private Rectangle winTilePos(Tile t) {
        float factor = 1 - FastEnd();
        float scale = 96 + 304 * factor;
        Vector2 pos = new Vector2(
             12 + t.Position.X * 100 * (1 - factor),
            292 + t.Position.Y * 100 * (1 - factor)
        );
        return new Rectangle(pos.ToPoint(), new Point((int) scale, (int) scale));
    }

    private float cubicPercentComplete() {
        // Scales cubically from 0 to 1 as the percent goes from 0.5 to 1. 
        float percent = PercentComplete();
        percent -= 1/2;
        percent *= 2;
        return 1 - percent * percent * percent;
    }


    private void DrawScore() {
        Color scoreColor = Color.Black;

        string scoreStr = "Score: " + Game.Score.ToString().PadLeft(5);
        int scoreWidth = (int)Asset.BigFont.MeasureString(scoreStr).X;
        DrawAsset(
            Asset.BigFont, 
            scoreStr, 
            new Vector2(400 - scoreWidth, 190), 
            scoreColor
        );

        string highScoreStr = 
            "Best: " + HighScoreTracker.HighScore.ToString().PadLeft(5);
        int highScoreWidth = (int)Asset.BigFont.MeasureString(highScoreStr).X;
        DrawAsset(
            Asset.BigFont, 
            highScoreStr, 
            new Vector2(400 - highScoreWidth, 240), 
            scoreColor
        );
    }

    public void DrawWinText() {
        float opacity = cubicPercentComplete();
        DrawAsset(Asset.BigFont, "YOU WIN!", new Vector2(20, 700), Color.Black * opacity);
        DrawAsset(Asset.Button, new Vector2(20, 720), Color.Red * opacity);
        DrawAsset(Asset.BigFont, "Play again", new Vector2(125, 750), Color.Red * opacity);
        DrawAsset(Asset.Button, new Vector2(20, 780), Color.Blue * opacity);
        DrawAsset(Asset.BigFont, "Exit to Menu", new Vector2(125, 810), Color.Blue * opacity);
        DrawAsset(Asset.Button, new Vector2(20, 840), Color.White * opacity);
        DrawAsset(Asset.BigFont, "Continue", new Vector2(125, 870), Color.White * opacity);
    }
}