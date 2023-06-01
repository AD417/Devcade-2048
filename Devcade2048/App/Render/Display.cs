using System.Collections.Generic;
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
            bool renderMerge = t.MergedFrom != null && (
                Animation.State == AnimationState.Moving 
             || Animation.State == AnimationState.Spawning
            );
            if (renderMerge) {
                drawTile(t.MergedFrom[0]);
                drawTile(t.MergedFrom[1]);
            }

            Rectangle location = Animation.PositionOfTile(t);

            sprite.Draw(Asset.Tile[t.TextureId], location, Color.White);
        }
        manager.Grid.EachCell((int _x, int _y, Tile t) => drawTile(t));
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
}