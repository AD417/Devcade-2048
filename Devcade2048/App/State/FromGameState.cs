using Devcade2048.App;
using Devcade2048.App.Render;
using Devcade2048.App.State;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class FromGameState : TransientState {
    
    public FromGameState() : base(TransitionTime) {
        if (Game.State == GameState.Continuing || Game.State == GameState.Playing) {
            Game.Export();
        }
        HighScoreTracker.Save();
    }

    public override BaseState NextState() { 
        return new ToMenuState();
    }



    public override void Draw() {
        base.Draw();

        Vector2 gridPos = GetGridPos();
        DrawAsset(Asset.Grid, gridPos);
        DrawScore();
        if (Game.State == GameState.Lost) {
            DrawLossText();
        }
        if (Game.State == GameState.Won) {
            DrawWinText();
            DrawWinningTile();
        } else {
            DrawAllTiles();
        }
    }

    private Vector2 GetGridPos() {
        // The top left corner of the grid, for rendering.
        float x = 10F;
        float y = 290 + 710 * FastEnd();
        return new Vector2(x,y);
    }

    protected override void DrawTile(Tile t) {
        if (t is null) return;

        Vector2 pos = new Vector2(
            2 + t.Position.X * 100,
            2 + t.Position.Y * 100
        );
        pos += GetGridPos();
        Rectangle location = new Rectangle(pos.ToPoint(), new Point(96,96));

        Texture2D blob = determineBlobTexture(t);

        DrawAsset(blob, location, Color.White);
    }

    private Texture2D determineBlobTexture(Tile t) {
        if (Game.State != GameState.Lost) return Asset.Tile[t.TextureId];
        if (t.TextureId > 5) return Asset.LoseTile[1];
        return Asset.LoseTile[0];
    }

    private void DrawWinningTile() {
        Vector2 pos = GetGridPos() + new Vector2(2,2);
        DrawAsset(Asset.Tile[10], new Rectangle(pos.ToPoint(), new(400,400)));
    }

    protected override float ScoreTextOpacity() {
        return 1 - FastStart();
    }

    private void DrawLossText() {
        // This ensures that the grid has covered them. 
        // Thus, the text "seamlessly" dissappears under the grid.
        if (PercentComplete() > 0.7) return;
        DrawAsset(Asset.BigFont, "GAME OVER!", new Vector2(20, 700), Color.Black);
        DrawAsset(Asset.Button, new Vector2(20, 720), Color.Red);
        DrawAsset(Asset.BigFont, "Try again", new Vector2(125, 750), Color.Red);
        DrawAsset(Asset.Button, new Vector2(20, 780), Color.Blue);
        DrawAsset(Asset.BigFont, "Exit to Menu", new Vector2(125, 810), Color.Blue);
    }

    private void DrawWinText() {
        if (PercentComplete() > 0.7) return;
        DrawAsset(Asset.BigFont, "YOU WIN!", new Vector2(20, 700), Color.Black);
        DrawAsset(Asset.Button, new Vector2(20, 720), Color.Red);
        DrawAsset(Asset.BigFont, "Play again", new Vector2(125, 750), Color.Red);
        DrawAsset(Asset.Button, new Vector2(20, 780), Color.Blue);
        DrawAsset(Asset.BigFont, "Exit to Menu", new Vector2(125, 810), Color.Blue);
        DrawAsset(Asset.Button, new Vector2(20, 840), Color.White);
        DrawAsset(Asset.BigFont, "Continue", new Vector2(125, 870), Color.White);
    }
}