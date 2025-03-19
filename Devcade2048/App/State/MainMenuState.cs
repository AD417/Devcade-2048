using Devcade2048.App.Render;
using Microsoft.Xna.Framework;

namespace Devcade2048.App.State;

public class MainMenuState : WaitingState {

    public override void Draw() {
        base.Draw();
        DrawHighScore();
        DrawGameBlob();
        DrawContinueBlub();
        DrawInfoBlob();
        DrawCreditsBlob();

    }

    private void DrawHighScore() {
        string highScore = "HIGH SCORE: " + HighScoreTracker.HighScore.ToString();
        TextBox.WriteCenteredText(Pen, highScore, new Vector2(210, 200), Color.Black);
    }

    private void DrawGameBlob() {
        DrawAsset(Asset.Menu[0], new Vector2(9, 300));
    }
    private void DrawContinueBlub() {
        DrawAsset(Asset.Menu[1], new Vector2(219, 300));
    }

    private void DrawInfoBlob() {
        DrawAsset(Asset.Menu[2], new Vector2(9, 500));
    }

    private void DrawCreditsBlob() {
        DrawAsset(Asset.Menu[3], new Vector2(219, 500));
    }

  public override BaseState ProcessInput() {
    if (InputManager.IsButtonPressed(Button.Blue)) {
        return new FromMenuState(Selection.Info);
    }
    if (InputManager.IsButtonPressed(Button.Green)) {
        return new FromMenuState(Selection.Credits);
    }
    if (InputManager.IsButtonPressed(Button.Red)) {
        return new FromMenuState(Selection.Game);
    }
    if (InputManager.IsButtonPressed(Button.White)) {
        return new FromMenuState(Selection.Continue);
    }
    return this;
  }
}