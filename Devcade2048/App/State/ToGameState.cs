using Devcade2048.App.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.State;

public class ToGameState : TransientState {
    private bool ContinueSavedGame;
    public ToGameState(bool continueSavedGame) : base(TransitionTime) {
        ContinueSavedGame = continueSavedGame;
        if (!continueSavedGame) {
            Game.NewGame();
        } else {
            Game.LoadGame();
        }
    }

    public override BaseState NextState() {
        if (ContinueSavedGame) return new GameWaitingState();
        return new SpawningState();
    }



    public override void Draw() {
        base.Draw();

        Vector2 gridPos = getGridPos();
        DrawAsset(Asset.Grid, gridPos);
        DrawScore();
        if (ContinueSavedGame) DrawAllTiles();
    }

    private Vector2 getGridPos() {
        // The top left corner of the grid, for rendering.
        float x = 10F;
        float y = 290 + 710 * (1 - FastStart());
        return new Vector2(x,y);
    }

    private void DrawAllTiles() {

        void drawTile(Tile t) {
            if (t is null) return;

            Vector2 pos = new Vector2(
                (2 + t.Position.X * 100),
                (2 + t.Position.Y * 100)
            );
            pos += getGridPos();
            Rectangle location = new Rectangle(pos.ToPoint(), new Point(96,96));

            Texture2D blob = Asset.Tile[t.TextureId];

            DrawAsset(blob, location, Color.White);
        }
        Game.Grid.EachCell((int _x, int _y, Tile t) => drawTile(t));
    }

    private Color getTextColor() {
        return Interpolate(Background, Color.Black, FastEnd(2));
    }

    private void DrawScore() {
        Color scoreColor = getTextColor();

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
}