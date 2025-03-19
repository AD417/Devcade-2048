using Devcade2048.App.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.State;

public class GameWaitingState : WaitingState {

    public override BaseState ProcessInput() {
        if (InputManager.IsButtonPressed(Button.Blue)) {
            return new FromGameState();
        }
        if (InputManager.GetStickDirection() != Manager.Direction.None) {
            if (Game.Move(InputManager.GetStickDirection())) return new MovingTileState();
        }
        if (InputManager.IsButtonPressed(Button.Red)) {
            return new GameResetState(false);
        }
        return this;
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