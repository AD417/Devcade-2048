using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.Render.Animation;

public class GameResetAnimationState : TransientAnimationState {
    private bool gameLost;

    public GameResetAnimationState(bool gameLost) : base(TransitionTime) {
        this.gameLost = gameLost;
    }

    public override AnimationState NextState() {
        Game.Setup();
        return new SpawningAnimationState();
    }



    public override void Draw() {
        base.Draw();

        DrawAsset(Asset.Grid, new Vector2(10, 290));
        DrawAllTiles();
        DrawScore();
        if (gameLost) DrawLossText();
    }



    private void DrawAllTiles() {

        void drawTile(Tile t) {
            if (t is null) return;

            Vector2 pos = new Vector2(
                ( 12 + t.Position.X * 100),
                (292 + t.Position.Y * 100)
            );

            int scale = (int) (96 * (1 - FastEnd(2)));
            pos += new Vector2(48 - scale/2, 48 - scale/2);
            Rectangle location = new Rectangle(pos.ToPoint(), new Point(scale, scale));

            Texture2D blob = determineBlobTexture(t);

            DrawAsset(blob, location, Color.White);
        }
        Game.Grid.EachCell((int _x, int _y, Tile t) => drawTile(t));
    }

    private Texture2D determineBlobTexture(Tile t) {
        if (!gameLost) return Asset.Tile[t.TextureId];
        // There are 2 sizes of loss tiles. 
        if (t.TextureId > 5) return Asset.LoseTile[1];
        return Asset.LoseTile[0];
    }


    private void DrawScore() {
        float opacity = 4 * MathF.Abs((float) PercentComplete() - 0.75f);
        if (opacity > 1) opacity = 1;
        Color scoreColor = Color.Black * opacity;

        string scoreStr = "Score: " + Game.Score.ToString().PadLeft(5);
        if (PercentComplete() > 0.75) scoreStr = "Score: " + "0".PadLeft(5);
        int scoreWidth = (int)Asset.BigFont.MeasureString(scoreStr).X;
        DrawAsset(
            Asset.BigFont, 
            scoreStr, 
            new Vector2(400 - scoreWidth, 190), 
            scoreColor
        );

        scoreColor = Color.Black;

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
        float opacity = (float) (1 - FastEnd());
        DrawAsset(Asset.BigFont, "GAME OVER!", new Vector2(20, 700), Color.Black * opacity);
        DrawAsset(Asset.Button, new Vector2(20, 720), Color.Red * opacity);
        DrawAsset(Asset.BigFont, "Try again", new Vector2(125, 750), Color.Red * opacity);
        DrawAsset(Asset.Button, new Vector2(20, 780), Color.Blue * opacity);
        DrawAsset(Asset.BigFont, "Exit to Menu", new Vector2(125, 810), Color.Blue * opacity);
    }

}