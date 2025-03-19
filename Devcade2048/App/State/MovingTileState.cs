using Devcade2048.App.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.State;

public class MovingTileState : TransientState {
    public MovingTileState() : base(TileMoveTime) {
    }

    public override void Tick(GameTime gt) {
        base.Tick(gt);
        // If the user is pressing a button, make things go faster.
        if (InputManager.AnyButtonPressed()) base.Tick(gt);
    }

    public override BaseState NextState()
    {
        return new SpawningState();
    }

    public override void Draw() {
        base.Draw();
        DrawAsset(Asset.Grid, new Vector2(10, 290));
        DrawAllTiles();
        DrawScore();
    }
    
    private void DrawAllTiles() {

        void drawTile(Tile t) {
            if (t is null) return;
            if (t.MergedFrom != null) {
                drawTile(t.MergedFrom[0]);
                drawTile(t.MergedFrom[1]);
            }
            // New tiles aren't drawn in this state. They haven't spawned yet. 
            if (t.PreviousPosition == null) return;

            Vector2 pos = Interpolate(
                toScreenPosition((Vector2) t.PreviousPosition), 
                toScreenPosition(t.Position), 
                PercentComplete()
            );
            pos += new Vector2(10, 290);
            Rectangle location = new Rectangle(pos.ToPoint(), new Point(96,96));

            Texture2D blob = Asset.Tile[t.TextureId];

            DrawAsset(blob, location, Color.White);
        }
        Game.Grid.EachCell((int _x, int _y, Tile t) => drawTile(t));
    }

    private Vector2 toScreenPosition(Vector2 position) {
        return new Vector2(
            (2 + position.X * 100),
            (2 + position.Y * 100)
        );
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
}