using Devcade2048.App;
using Devcade2048.App.Render;
using Devcade2048.App.Render.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class FromGameAnimationState : TransientAnimationState {
    
    public FromGameAnimationState() : base(TransitionTime) {
        if (Game.State == GameState.Continuing || Game.State == GameState.Playing) {
            Game.Export();
        }
        HighScoreTracker.Save();
    }

    public override AnimationState NextState()
    {
        return new ToMenuAnimationState();
    }



    public override void Draw() {
        base.Draw();

        Vector2 gridPos = getGridPos();
        DrawAsset(Asset.Grid, gridPos);
        DrawScore();
        if (Game.State == GameState.Lost) {
            DrawLossText();
        }
        if (Game.State == GameState.Won) {
            DrawWinText();
            DrawWinningTile();
        } else {
            DrawAllTiles();
        }
    }

    private Vector2 getGridPos() {
        // The top left corner of the grid, for rendering.
        float x = 10F;
        float y = 290 + 710 * (float) (FastEnd());
        return new Vector2(x,y);
    }

    private void DrawAllTiles() {

        void drawTile(Tile t) {
            if (t is null) return;

            Vector2 pos = new Vector2(
                2 + t.Position.X * 100,
                2 + t.Position.Y * 100
            );
            pos += getGridPos();
            Rectangle location = new Rectangle(pos.ToPoint(), new Point(96,96));

            Texture2D blob = determineBlobTexture(t);

            DrawAsset(blob, location, Color.White);
        }
        Game.Grid.EachCell((int _x, int _y, Tile t) => drawTile(t));
    }

    private Texture2D determineBlobTexture(Tile t) {
        if (Game.State != GameState.Lost) return Asset.Tile[t.TextureId];
        if (t.TextureId > 5) return Asset.LoseTile[1];
        return Asset.LoseTile[0];
    }

    private void DrawWinningTile() {
        Vector2 pos = getGridPos() + new Vector2(2,2);
        DrawAsset(Asset.Tile[10], new Rectangle(pos.ToPoint(), new(400,400)));
    }

    private Color getTextColor() {
        return Interpolate(Color.Black, Background, FastStart(2));
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

    public void DrawLossText() {
        if (PercentComplete() > 0.7) return;
        DrawAsset(Asset.BigFont, "GAME OVER!", new Vector2(20, 700), Color.Black);
        DrawAsset(Asset.Button, new Vector2(20, 720), Color.Red);
        DrawAsset(Asset.BigFont, "Try again", new Vector2(125, 750), Color.Red);
        DrawAsset(Asset.Button, new Vector2(20, 780), Color.Blue);
        DrawAsset(Asset.BigFont, "Exit to Menu", new Vector2(125, 810), Color.Blue);
    }

    private void DrawWinText() {
        if (PercentComplete() > 0.7) return;
        DrawAsset(Asset.BigFont, "YOU WIN!", new Vector2(20, 700), Color.Black);
        DrawAsset(Asset.Button, new Vector2(20, 720), Color.Red);
        DrawAsset(Asset.BigFont, "Play again", new Vector2(125, 750), Color.Red);
        DrawAsset(Asset.Button, new Vector2(20, 780), Color.Blue);
        DrawAsset(Asset.BigFont, "Exit to Menu", new Vector2(125, 810), Color.Blue);
        DrawAsset(Asset.Button, new Vector2(20, 840), Color.White);
        DrawAsset(Asset.BigFont, "Continue", new Vector2(125, 870), Color.White);
    }
}