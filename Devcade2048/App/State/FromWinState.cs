using Devcade2048.App.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.State;

public class FromWinState : TransientState {

    public FromWinState() : base(GameEndingTime) {
    }

    public override BaseState NextState() {
        Game.Continue();
        return new GameWaitingState();
    }

    public override void Draw() {
        base.Draw();

        DrawAsset(Asset.Grid, new Vector2(10, 290));
        DrawAllTiles();
        DrawWinningTile();
        DrawScore();
        DrawWinText();
    }

    private void DrawWinningTile() {

        void drawWin(Tile t) {
            if (t is null || t.Value != 2048) return;
            Rectangle location = winTilePos(t);

            DrawAsset(Asset.Tile[t.TextureId], location, Color.White);
        }

        Game.Grid.EachCell((int _x, int _y, Tile t) => drawWin(t));
    }

    private Rectangle winTilePos(Tile t) {
        float factor = 1 - FastEnd();
        float scale = 96 + 304 * factor;
        Vector2 pos = new Vector2(
             12 + t.Position.X * 100 * (1 - factor),
            292 + t.Position.Y * 100 * (1 - factor)
        );
        return new Rectangle(pos.ToPoint(), new Point((int) scale, (int) scale));
    }

    private float cubicPercentComplete() {
        // Scales cubically from 0 to 1 as the percent goes from 0.5 to 1. 
        float percent = PercentComplete();
        percent -= 0.5F;
        percent *= 2;
        return 1 - percent * percent * percent;
    }

    public void DrawWinText() {
        float opacity = cubicPercentComplete();
        DrawAsset(Asset.BigFont, "YOU WIN!", new Vector2(20, 700), Color.Black * opacity);
        DrawAsset(Asset.Button, new Vector2(20, 720), Color.Red * opacity);
        DrawAsset(Asset.BigFont, "Play again", new Vector2(125, 750), Color.Red * opacity);
        DrawAsset(Asset.Button, new Vector2(20, 780), Color.Blue * opacity);
        DrawAsset(Asset.BigFont, "Exit to Menu", new Vector2(125, 810), Color.Blue * opacity);
        DrawAsset(Asset.Button, new Vector2(20, 840), Color.White * opacity);
        DrawAsset(Asset.BigFont, "Continue", new Vector2(125, 870), Color.White * opacity);
    }
}