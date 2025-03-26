using System;
using Devcade2048.App.Render;
using Microsoft.Xna.Framework;

namespace Devcade2048.App.State;

public class ResetWinState : TransientState {

    public ResetWinState() : base(TransitionTime) {
    }

    public override BaseState NextState() {
        Game.Setup();
        return new SpawningState();
    }

    public override void Draw() {
        base.Draw();

        DrawAsset(Asset.Grid, new Vector2(10, 290));
        DrawWinningTile();
        DrawScore();
        DrawWinText();
    }

    private void DrawWinningTile() {
        int scale = (int) (400 * FastEnd());
        DrawAsset(Asset.Tile[10], new Rectangle(12 + scale/2, 292 + scale/2, 400 - scale, 400 - scale));
    }

    protected override void DrawScore() {
        float opacity = 4 * MathF.Abs(PercentComplete() - 0.75f);
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

    public void DrawWinText() {
        float opacity = 1 - FastStart();
        DrawAsset(Asset.BigFont, "YOU WIN!", new Vector2(20, 700), Color.Black * opacity);
        DrawAsset(Asset.Button, new Vector2(20, 720), Color.Red * opacity);
        DrawAsset(Asset.BigFont, "Play again", new Vector2(125, 750), Color.Red * opacity);
        DrawAsset(Asset.Button, new Vector2(20, 780), Color.Blue * opacity);
        DrawAsset(Asset.BigFont, "Exit to Menu", new Vector2(125, 810), Color.Blue * opacity);
        DrawAsset(Asset.Button, new Vector2(20, 840), Color.White * opacity);
        DrawAsset(Asset.BigFont, "Continue", new Vector2(125, 870), Color.White * opacity);
    }
}