using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.Render;

public static class Display {
    private static SpriteBatch sprite;
    internal static Manager manager;

    public static void Initialize(SpriteBatch sb, Manager man) {
        sprite = sb;
        manager = man;
    }

    public static void Title() {
        Color brightness = Color.White;
        if (Animation.State == AnimationState.InitFadeIn) {
            brightness = StyleMath.GetInitialBrightness();
        }
        sprite.Draw(Asset.Title, new Vector2(60, 0), brightness);
    }

    public static void Grid() {
        Vector2 position = new Vector2(10, 290);
        // Time to go overkill on this.
        position += StyleMath.GridDisplacement();
        sprite.Draw(Asset.Grid, position, Color.White);
    }

    public static void AllTiles() {
        if (!Animation.RenderingTiles()) return;
        void drawTile(Tile t) {
            if (t is null) return;
            bool renderMerge = t.MergedFrom != null && Animation.UpdatingGrid();
            if (renderMerge) {
                drawTile(t.MergedFrom[0]);
                drawTile(t.MergedFrom[1]);
            }

            Rectangle location = StyleMath.PositionOfTile(t);

            Texture2D blob = DetermineBlob(t);

            sprite.Draw(blob, location, Color.White);
        }
        manager.Grid.EachCell((int _x, int _y, Tile t) => drawTile(t));
    }

    public static void Win() {
        if (manager.State != GameState.Won) return;
        if (Animation.UpdatingGrid() || Animation.State == AnimationState.ResetFromWin) return;
        void drawWin(Tile t) {
            if (t is null || t.Value != 2048) return;
            Rectangle location = StyleMath.PositionOfWinTile(t);

            sprite.Draw(Asset.Tile[t.TextureId], location, Color.White);
        }
        manager.Grid.EachCell((int _x, int _y, Tile t) => drawWin(t));
        if (Animation.AcceptInput()) {
            sprite.DrawString(Asset.ScoreFont, "YOU WIN!", new Vector2(20, 700), Color.Black);
            sprite.Draw(Asset.Button, new Vector2(20, 720), Color.Red);
            sprite.DrawString(Asset.ScoreFont, "Play again", new Vector2(125, 750), Color.Red);
            sprite.Draw(Asset.Button, new Vector2(20, 780), Color.Blue);
            sprite.DrawString(Asset.ScoreFont, "Exit to Menu", new Vector2(125, 810), Color.Blue);
            sprite.Draw(Asset.Button, new Vector2(20, 840), Color.White);
            sprite.DrawString(Asset.ScoreFont, "Continue", new Vector2(125, 870), Color.White);
        }
    }

    public static void WinReset() {
        if (Animation.State != AnimationState.ResetFromWin) return;
        Rectangle location = StyleMath.PositionOfWinTile();
        sprite.Draw(Asset.Tile[10], location, Color.White);
    }

    public static void Lose() {
        if (manager.State != GameState.Lost) return;
        if (Animation.State != AnimationState.WaitingForInput) return;
        sprite.DrawString(Asset.ScoreFont, "GAME OVER!", new Vector2(20, 700), Color.Black);
            sprite.Draw(Asset.Button, new Vector2(20, 720), Color.Red);
            sprite.DrawString(Asset.ScoreFont, "Try again", new Vector2(125, 750), Color.Red);
            sprite.Draw(Asset.Button, new Vector2(20, 780), Color.Blue);
            sprite.DrawString(Asset.ScoreFont, "Exit to Menu", new Vector2(125, 810), Color.Blue);
    }

    private static Texture2D DetermineBlob(Tile t) {
        if (manager.State != GameState.Lost) return Asset.Tile[t.TextureId];
        if (t.TextureId > StyleMath.BiggestLossTile()) return Asset.Tile[t.TextureId];
        int loseTileId = 0;
        if (t.TextureId > 5) loseTileId++;
        return Asset.LoseTile[loseTileId];
    }

    public static void addScore(ScoreDelta score) {
        ScoreContainer.Add(score);
    }

    public static void Scores() {
        Color scoreColor = StyleMath.GetScoreColor();

		string scoreStr = "Score: " + manager.Score.ToString().PadLeft(5);
		int scoreWidth = (int)Asset.ScoreFont.MeasureString(scoreStr).X;
		sprite.DrawString(
            Asset.ScoreFont, 
            scoreStr, 
            new Vector2(400 - scoreWidth, 190), 
            scoreColor
        );

		string highScoreStr = 
            "Best: " + HighScoreTracker.HighScore.ToString().PadLeft(5);
		int highScoreWidth = (int)Asset.ScoreFont.MeasureString(highScoreStr).X;
		sprite.DrawString(
            Asset.ScoreFont, 
            highScoreStr, 
            new Vector2(400 - highScoreWidth, 240), 
            scoreColor
        );

        foreach (ScoreDelta score in ScoreContainer.scores) {
            string deltaStr = "+" + score.ToString();
            int width = (int) Asset.ScoreFont.MeasureString(deltaStr).X;
            Vector2 position = new Vector2(400 - width, 190 - score.ShiftUp());
            sprite.DrawString(
                Asset.ScoreFont, 
                deltaStr, 
                position, 
                score.DrawColor()
            );
        }
    }


    public static void Menu() {
        Color brightness = StyleMath.GetInitialBrightness();
        Vector2 pos1 = new Vector2(9, 300);
        Vector2 pos2 = new Vector2(219, 300);
        Vector2 pos3 = new Vector2(114, 500);
        double percent = 0;
        if (Animation.State == AnimationState.FromMenu) {
            percent = Animation.FastEnd(2);
        }
        if (Animation.State == AnimationState.ToMenu) {
            percent = 1 - Animation.FastStart(2);
        }

        pos1 = StyleMath.Interpolate(pos1, new Vector2(-300, 300), percent);
        pos2 = StyleMath.Interpolate(pos2, new Vector2(530, 300), percent);
        pos3 = StyleMath.Interpolate(pos3, new Vector2(-300, 500), percent);

		sprite.Draw(Asset.Menu[0], pos1, brightness);
		sprite.Draw(Asset.Menu[1], pos2, brightness);
		sprite.Draw(Asset.Menu[2], pos3, brightness);
    }


}