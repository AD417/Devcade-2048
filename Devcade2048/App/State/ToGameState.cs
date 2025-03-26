using Devcade2048.App.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.State;

public class ToGameState : TransientState {
    private bool ContinueSavedGame;
    public ToGameState(bool continueSavedGame) : base(TransitionTime) {
        ContinueSavedGame = continueSavedGame;
        if (!continueSavedGame) {
            Game.NewGame();
        } else {
            Game.LoadGame();
        }
    }

    public override BaseState NextState() {
        if (ContinueSavedGame) {
            // If a user 
            return new GameWaitingState();
        }
        return new SpawningState();
    }



    public override void Draw() {
        base.Draw();

        Vector2 gridPos = getGridPos();
        DrawAsset(Asset.Grid, gridPos);
        DrawScore();
        if (ContinueSavedGame) DrawAllTiles();
    }

    private Vector2 getGridPos() {
        // The top left corner of the grid, for rendering.
        float x = 10F;
        float y = 290 + 710 * (1 - FastStart());
        return new Vector2(x,y);
    }

    protected override void DrawTile(Tile t) {
        if (t is null) return;

        Vector2 pos = new Vector2(
            2 + t.Position.X * 100,
            2 + t.Position.Y * 100
        );
        pos += getGridPos();
        Rectangle location = new Rectangle(pos.ToPoint(), new Point(96,96));

        Texture2D blob = Asset.Tile[t.TextureId];

        DrawAsset(blob, location, Color.White);
    }

    protected override float ScoreTextOpacity() {
        return FastEnd(2);
    }
}