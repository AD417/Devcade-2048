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
        sprite.Draw(Asset.Title, new Vector2(60, 0), Color.White);
    }


    public static void AllTiles() {
        void drawTile(Tile t) {
            if (t is null) return;
            bool renderMerge = t.MergedFrom != null && Animation.UpdatingGrid();
            if (renderMerge) {
                drawTile(t.MergedFrom[0]);
                drawTile(t.MergedFrom[1]);
            }

            Rectangle location = Animation.PositionOfTile(t);

            Texture2D blob = DetermineBlob(t);

            sprite.Draw(blob, location, Color.White);
        }
        manager.Grid.EachCell((int _x, int _y, Tile t) => drawTile(t));
    }

    public static void Win() {
        if (manager.State != GameState.Won) return;
        if (Animation.UpdatingGrid()) return;
        void drawWin(Tile t) {
            if (t is null || t.Value != 2048) return;
            Rectangle location = Animation.PositionOfWinTile(t);

            sprite.Draw(Asset.Tile[t.TextureId], location, Color.White);
        }
        manager.Grid.EachCell((int _x, int _y, Tile t) => drawWin(t));
        if (Animation.State == AnimationState.WaitingForInput) {
            sprite.DrawString(Asset.Score, "YOU WIN!", new Vector2(20, 700), Color.Black);
        }
    }

    public static void Lose() {
        if (manager.State != GameState.Lost) return;
        if (Animation.State != AnimationState.WaitingForInput) return;
        sprite.DrawString(Asset.Score, "GAME OVER!", new Vector2(20, 700), Color.Black);
    }

    private static Texture2D DetermineBlob(Tile t) {
        if (manager.State != GameState.Lost) return Asset.Tile[t.TextureId];
        if (t.TextureId > Animation.BiggestLossTile()) return Asset.Tile[t.TextureId];
        int loseTileId = 0;
        if (t.TextureId > 5) loseTileId++;
        return Asset.LoseTile[loseTileId];
    }

    public static void addScore(ScoreDelta score) {
        ScoreContainer.Add(score);
    }

    public static void Scores() {
		string scoreStr = "Score: " + manager.Score.ToString().PadLeft(5);
		int scoreWidth = (int)Asset.Score.MeasureString(scoreStr).X;
		sprite.DrawString(
            Asset.Score, 
            scoreStr, 
            new Vector2(400 - scoreWidth, 200), 
            Color.Black
        );

		string highScoreStr = 
            "Best: " + HighScoreTracker.HighScore.ToString().PadLeft(5);
		int highScoreWidth = (int)Asset.Score.MeasureString(highScoreStr).X;
		sprite.DrawString(
            Asset.Score, 
            highScoreStr, 
            new Vector2(400 - highScoreWidth, 250), 
            Color.Black
        );

        foreach (ScoreDelta score in ScoreContainer.scores) {
            string deltaStr = "+" + score.ToString();
            int width = (int) Asset.Score.MeasureString(deltaStr).X;
            Vector2 position = new Vector2(400 - width, 200 - score.ShiftUp());
            sprite.DrawString(
                Asset.Score, 
                deltaStr, 
                position, 
                score.DrawColor()
            );
        }
    }


    public static void Menu() {
		sprite.Draw(Asset.Menu[0], new Vector2(9, 300), Color.White);
		sprite.Draw(Asset.Menu[1], new Vector2(219, 300), Color.White);
		sprite.Draw(Asset.Menu[2], new Vector2(114, 500), Color.White);
    }


}