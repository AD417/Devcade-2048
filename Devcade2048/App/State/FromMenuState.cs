using Devcade2048.App.Render;
using Microsoft.Xna.Framework;

namespace Devcade2048.App.State;

public class FromMenuState : TransientState {
    private static Vector2[][] blobPositions = new Vector2[][] {
        new Vector2[] { new(9, 300), new(-300, 300) },
        new Vector2[] { new(219, 300), new(530, 300) },
        new Vector2[] { new(9, 500), new(-300, 500) },
        new Vector2[] { new(219, 500), new(530, 500) },
    };
    private readonly Selection selection;

    public FromMenuState(Selection selection) : base(TransitionTime) {
        this.selection = selection;
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
        float opacity = 1 - PercentComplete();
        TextBox.WriteCenteredText(Pen, highScore, new Vector2(210, 200), Color.Black * opacity);
    }


    public override BaseState NextState()
    {
        if (selection == Selection.Game) {
            return new ToGameState(false);
        }
        if (selection == Selection.Continue) {
            return new ToGameState(true);
        }
        return new ToTabState(selection);
    }
}