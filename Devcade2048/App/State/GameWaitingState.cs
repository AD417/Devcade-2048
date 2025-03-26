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
}