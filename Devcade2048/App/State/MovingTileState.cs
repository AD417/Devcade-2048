using Devcade2048.App.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.State;

public class MovingTileState : TransientState {
    public MovingTileState() : base(TileMoveTime) {
    }

    public override void Tick(GameTime gt) {
        base.Tick(gt);
        // If the user is pressing a button, make things go faster.
        if (InputManager.AnyButtonPressed()) base.Tick(gt);
    }

    public override BaseState NextState() {
        return new SpawningState();
    }

    public override void Draw() {
        base.Draw();
        DrawAsset(Asset.Grid, new Vector2(10, 290));
        DrawAllTiles();
        DrawScore();
    }
    
    protected override void DrawTile(Tile t) {
        if (t is null) return;
        if (t.MergedFrom != null) {
            DrawTile(t.MergedFrom[0]);
            DrawTile(t.MergedFrom[1]);
        }
        // New tiles aren't drawn in this state. They haven't spawned yet. 
        if (t.PreviousPosition == null) return;

        Vector2 pos = Interpolate(
            toScreenPosition((Vector2) t.PreviousPosition), 
            toScreenPosition(t.Position), 
            PercentComplete()
        );
        Rectangle location = new Rectangle(pos.ToPoint(), new Point(96,96));

        Texture2D blob = Asset.Tile[t.TextureId];

        DrawAsset(blob, location, Color.White);
    }

    private Vector2 toScreenPosition(Vector2 position) {
        return new Vector2(
             12 + position.X * 100,
            292 + position.Y * 100
        );
    }
}