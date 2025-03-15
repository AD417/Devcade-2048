using System;
using Microsoft.Xna.Framework;

namespace Devcade2048.App.Render.Animation;

public class FromMenuAnimationState : TransientAnimationState {
    private static Vector2[][] blobPositions = new Vector2[][] {
        new Vector2[] { new(9, 300), new(-300, 300) },
        new Vector2[] { new(219, 300), new(530, 300) },
        new Vector2[] { new(9, 500), new(-300, 500) },
        new Vector2[] { new(219, 500), new(530, 500) },
    };
    private readonly MenuTabAnimationState.Tab tab;

    public FromMenuAnimationState(MenuTabAnimationState.Tab tab) : base(TransitionTime) {
        this.tab = tab;
    }

    public override void Draw() {
        base.Draw();
        DrawHighScore();

        for (int i = 0; i < 4; i++) {
            Vector2 drawPos = Interpolate(blobPositions[i][0], blobPositions[i][1], FastEnd());
            DrawAsset(Asset.Menu[i], drawPos);
        }
    }

    private void DrawHighScore() {
        string highScore = "HIGH SCORE: " + HighScoreTracker.HighScore.ToString();
        float opacity = (float) (1 - PercentComplete());
        TextBox.WriteCenteredText(Pen, highScore, new Vector2(210, 200), Color.Black * opacity);
    }


  public override AnimationState NextState()
  {
    return new ToTabAnimationState(tab);
  }
}