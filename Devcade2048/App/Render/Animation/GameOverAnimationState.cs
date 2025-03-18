using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.Render.Animation;

public class GameOverAnimationState : WaitingAnimationState {

    public override AnimationState ProcessInput()
    {
        if (InputManager.isButtonPressed(Button.Red)) {
            return new GameResetAnimationState(true);
        }
        return this;
    }





    public override void Draw() {
        base.Draw();

        DrawAsset(Asset.Grid, new Vector2(10, 290));
        DrawAllTiles();
        DrawScore();
        DrawLossText();
    }


    private void DrawAllTiles() {

        void drawTile(Tile t) {
            if (t is null) return;

            Vector2 pos = new Vector2(
                ( 12 + t.Position.X * 100),
                (292 + t.Position.Y * 100)
            );
            Rectangle location = new Rectangle(pos.ToPoint(), new Point(96,96));

            Texture2D blob = determineBlobTexture(t);

            DrawAsset(blob, location, Color.White);
        }
        Game.Grid.EachCell((int _x, int _y, Tile t) => drawTile(t));
    }

    private Texture2D determineBlobTexture(Tile t) {
        if (t.TextureId > 5) return Asset.LoseTile[1];
        return Asset.LoseTile[0];
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

    public static void DrawLossText() {
        DrawAsset(Asset.BigFont, "GAME OVER!", new Vector2(20, 700), Color.Black);
        DrawAsset(Asset.Button, new Vector2(20, 720), Color.Red);
        DrawAsset(Asset.BigFont, "Try again", new Vector2(125, 750), Color.Red);
        DrawAsset(Asset.Button, new Vector2(20, 780), Color.Blue);
        DrawAsset(Asset.BigFont, "Exit to Menu", new Vector2(125, 810), Color.Blue);
    }
}