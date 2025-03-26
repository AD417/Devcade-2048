using Devcade2048.App.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.State;

public class GameOverState : WaitingState {

    public override BaseState ProcessInput() {
        if (InputManager.IsButtonPressed(Button.Red)) {
            return new GameResetState(true);
        }
        if (InputManager.IsButtonPressed(Button.Blue)) {
            return new FromGameState();
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


    protected override void DrawTile(Tile t) {
        if (t is null) return;

        Vector2 pos = new Vector2(
             12 + t.Position.X * 100,
            292 + t.Position.Y * 100
        );
        Rectangle location = new Rectangle(pos.ToPoint(), new Point(96,96));

        Texture2D blob = determineBlobTexture(t);

        DrawAsset(blob, location, Color.White);
    }

    private Texture2D determineBlobTexture(Tile t) {
        if (t.TextureId > 5) return Asset.LoseTile[1];
        return Asset.LoseTile[0];
    }

    public static void DrawLossText() {
        DrawAsset(Asset.BigFont, "GAME OVER!", new Vector2(20, 700), Color.Black);
        DrawAsset(Asset.Button, new Vector2(20, 720), Color.Red);
        DrawAsset(Asset.BigFont, "Try again", new Vector2(125, 750), Color.Red);
        DrawAsset(Asset.Button, new Vector2(20, 780), Color.Blue);
        DrawAsset(Asset.BigFont, "Exit to Menu", new Vector2(125, 810), Color.Blue);
    }
}