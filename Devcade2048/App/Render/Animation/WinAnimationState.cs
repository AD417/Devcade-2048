using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.Render.Animation;

public class WinAnimationState : WaitingAnimationState {

    public override AnimationState ProcessInput()
    {
        if (InputManager.IsButtonPressed(Button.Red)) {
            return new ResetWinAnimationState();
        }
        if (InputManager.IsButtonPressed(Button.White)) {
            return new FromWinAnimationState();
        }
        if (InputManager.IsButtonPressed(Button.Blue)) {
            return new FromGameAnimationState();
        }
        return this;
    }

    public override void Draw()
    {
        base.Draw();

        DrawWinningTile();
        DrawScore();
        DrawWinText();
    }

    private void DrawWinningTile() {
        DrawAsset(Asset.Tile[10], new Rectangle(12, 292, 400, 400));
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

    private void DrawWinText() {
        DrawAsset(Asset.BigFont, "YOU WIN!", new Vector2(20, 700), Color.Black);
        DrawAsset(Asset.Button, new Vector2(20, 720), Color.Red);
        DrawAsset(Asset.BigFont, "Play again", new Vector2(125, 750), Color.Red);
        DrawAsset(Asset.Button, new Vector2(20, 780), Color.Blue);
        DrawAsset(Asset.BigFont, "Exit to Menu", new Vector2(125, 810), Color.Blue);
        DrawAsset(Asset.Button, new Vector2(20, 840), Color.White);
        DrawAsset(Asset.BigFont, "Continue", new Vector2(125, 870), Color.White);
    }
}