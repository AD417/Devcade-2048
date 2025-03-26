using Devcade2048.App.Render;
using Microsoft.Xna.Framework;

namespace Devcade2048.App.State;

public class ToMenuState : TransientState {
    private static Vector2[][] blobPositions = new Vector2[][] {
        new Vector2[] { new(9, 300), new(-300, 300) },
        new Vector2[] { new(219, 300), new(530, 300) },
        new Vector2[] { new(9, 500), new(-300, 500) },
        new Vector2[] { new(219, 500), new(530, 500) },
    };

    public ToMenuState() : base(TransitionTime) {
    }

  public override void Draw() {
    base.Draw();
    DrawHighScore();

    for (int i = 0; i < 4; i++) {
        Vector2 drawPos = Interpolate(blobPositions[i][1], blobPositions[i][0], FastStart());
        DrawAsset(Asset.Menu[i], drawPos);
    }
  }

    private void DrawHighScore() {
        string highScore = "HIGH SCORE: " + HighScoreTracker.HighScore.ToString();
        float opacity = PercentComplete();
        TextBox.WriteCenteredText(Pen, highScore, new Vector2(210, 200), Color.Black * opacity);
    }

  public override BaseState NextState() {
    return new MainMenuState();
  }
}