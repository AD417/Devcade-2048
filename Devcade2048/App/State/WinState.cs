using Devcade2048.App.Render;
using Microsoft.Xna.Framework;

namespace Devcade2048.App.State;

public class WinState : WaitingState {

    public override BaseState ProcessInput() {
        if (InputManager.IsButtonPressed(Button.Red)) {
            return new ResetWinState();
        }
        if (InputManager.IsButtonPressed(Button.White)) {
            return new FromWinState();
        }
        if (InputManager.IsButtonPressed(Button.Blue)) {
            return new FromGameState();
        }
        return this;
    }

    public override void Draw() {
        base.Draw();

        DrawWinningTile();
        DrawScore();
        DrawWinText();
    }

    private void DrawWinningTile() {
        DrawAsset(Asset.Tile[10], new Rectangle(12, 292, 400, 400));
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