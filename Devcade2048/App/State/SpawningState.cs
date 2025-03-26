using Devcade2048.App.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.State;

public class SpawningState : TransientState {
    public SpawningState() : base(TileSpawnTime) {}

    public override void Tick(GameTime gt) {
        base.Tick(gt);
        // If the user is pressing a button, make things go faster.
        if (InputManager.AnyButtonPressed()) base.Tick(gt);
    }

    public override BaseState NextState() {
        if (Game.State == GameState.Won) {
            return new ToWinState();
        }
        if (!Game.MovesAvailable()) {
            return new ToGameOverState();
        }
        return new GameWaitingState();
    }

    public override void Draw() {
        base.Draw();

        DrawAsset(Asset.Grid, new Vector2(10, 290));
        DrawAllTiles();
        DrawNewTiles();
        DrawScore();
    }

    protected override void DrawTile(Tile t) {
        if (t is null) return;
        // This ensures that new tiles appear above old ones.
        if (t.MergedFrom != null) {
            DrawTile(t.MergedFrom[0]);
            DrawTile(t.MergedFrom[1]);
        }
        // New tiles aren't drawn in this step. 
        // They need to appear on top.
        if (t.PreviousPosition == null) return;

        Vector2 pos = toScreenPosition(t.Position); 
        pos += new Vector2(10, 290);

        Rectangle location = new Rectangle(pos.ToPoint(), new Point(96, 96));

        Texture2D blob = Asset.Tile[t.TextureId];

        DrawAsset(blob, location, Color.White);
    }

    private void DrawNewTiles() {

        void DrawTile(Tile t) {
            if (t is null) return;
            if (t.PreviousPosition != null) return;

            Vector2 pos = toScreenPosition(t.Position); 
            pos += new Vector2(10, 290);

            int scale = TileScale(t);
            pos += new Vector2(48 - scale/2, 48 - scale/2);

            Rectangle location = new Rectangle(pos.ToPoint(), new Point(scale, scale));

            Texture2D blob = Asset.Tile[t.TextureId];

            DrawAsset(blob, location, Color.White);
        }
        Game.Grid.EachCell((int _x, int _y, Tile t) => DrawTile(t));

    }

    private Vector2 toScreenPosition(Vector2 position) {
        return new Vector2(
            (2 + position.X * 100),
            (2 + position.Y * 100)
        );
    }

    private int TileScale(Tile t) {
        // This is a formula that meets the following properties:
        // passes through (0,0)
        // passes through (1,1)
        // is tangent to y=1.2 in the range (0,1)
        float x = PercentComplete();
        float y = -2.38F * x * x + 3.38F * x;
        return (int) (y * 96);
    }
}