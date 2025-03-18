using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.Render.Animation;

public class SpawningAnimationState : TransientAnimationState {
    public SpawningAnimationState() : base(TileSpawnTime) {
        
    }

    public override AnimationState NextState() {
        return new GameWaitingAnimationState();
    }

    public override void Draw() {
        base.Draw();

        DrawAsset(Asset.Grid, new Vector2(10, 290));
        DrawAllTiles();
        DrawNewTiles();
        DrawScore();
    }

    private void DrawAllTiles() {

        void drawTile(Tile t) {
            if (t is null) return;
            // This ensures that new tiles appear above old ones.
            if (t.MergedFrom != null) {
                drawTile(t.MergedFrom[0]);
                drawTile(t.MergedFrom[1]);
            }
            // New tiles aren't drawn in this step. 
            // They need to appear on top.
            if (t.PreviousPosition == null) return;

            Vector2 pos = toScreenPosition(t.Position); 
            pos += new Vector2(10, 290);

            Rectangle location = new Rectangle(pos.ToPoint(), new Point(96, 96));

            Texture2D blob = Asset.Tile[t.TextureId];

            DrawAsset(blob, location, Color.White);
        }
        Game.Grid.EachCell((int _x, int _y, Tile t) => drawTile(t));
    }

    private void DrawNewTiles() {

        void drawTile(Tile t) {
            if (t is null) return;
            if (t.PreviousPosition != null) return;

            Vector2 pos = toScreenPosition(t.Position); 
            pos += new Vector2(10, 290);

            int scale = TileScale(t);
            pos += new Vector2(48 - scale/2, 48 - scale/2);

            Rectangle location = new Rectangle(pos.ToPoint(), new Point(scale, scale));

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

    private int TileScale(Tile t) {
        // This is a formula that meets the following properties:
        // passes through (0,0)
        // passes through (1,1)
        // is tangent to y=1.2 in the range (0,1)
        double x = PercentComplete();
        double y = -2.38 * x * x + 3.38 * x;
        return (int) (y * 96);
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