using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Devcade2048.App.Tabs;

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
            sprite.DrawString(Asset.BigFont, "YOU WIN!", new Vector2(20, 700), Color.Black);
            sprite.Draw(Asset.Button, new Vector2(20, 720), Color.Red);
            sprite.DrawString(Asset.BigFont, "Play again", new Vector2(125, 750), Color.Red);
            sprite.Draw(Asset.Button, new Vector2(20, 780), Color.Blue);
            sprite.DrawString(Asset.BigFont, "Exit to Menu", new Vector2(125, 810), Color.Blue);
            sprite.Draw(Asset.Button, new Vector2(20, 840), Color.White);
            sprite.DrawString(Asset.BigFont, "Continue", new Vector2(125, 870), Color.White);
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
        sprite.DrawString(Asset.BigFont, "GAME OVER!", new Vector2(20, 700), Color.Black);
            sprite.Draw(Asset.Button, new Vector2(20, 720), Color.Red);
            sprite.DrawString(Asset.BigFont, "Try again", new Vector2(125, 750), Color.Red);
            sprite.Draw(Asset.Button, new Vector2(20, 780), Color.Blue);
            sprite.DrawString(Asset.BigFont, "Exit to Menu", new Vector2(125, 810), Color.Blue);
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
		int scoreWidth = (int)Asset.BigFont.MeasureString(scoreStr).X;
		sprite.DrawString(
            Asset.BigFont, 
            scoreStr, 
            new Vector2(400 - scoreWidth, 190), 
            scoreColor
        );

		string highScoreStr = 
            "Best: " + HighScoreTracker.HighScore.ToString().PadLeft(5);
		int highScoreWidth = (int)Asset.BigFont.MeasureString(highScoreStr).X;
		sprite.DrawString(
            Asset.BigFont, 
            highScoreStr, 
            new Vector2(400 - highScoreWidth, 240), 
            scoreColor
        );

        foreach (ScoreDelta score in ScoreContainer.scores) {
            string deltaStr = "+" + score.ToString();
            int width = (int) Asset.BigFont.MeasureString(deltaStr).X;
            Vector2 position = new Vector2(400 - width, 190 - score.ShiftUp());
            sprite.DrawString(
                Asset.BigFont, 
                deltaStr, 
                position, 
                score.DrawColor()
            );
        }
    }


    public static void MenuBlobs() {
        Display.GameBlob();
        Display.InfoBlob();
        Display.CreditsBlob();
    }

    public static void GameBlob() {
        if (TabHandler.CurrentTab.Id() != SelectedTab.Menu) return;
        Color brightness = StyleMath.GetInitialBrightness();
        Vector2 pos = new Vector2(9, 300);
        double percent = 0;
        Vector2 endPos = new Vector2(-300, 300);
        if (Animation.State == AnimationState.FromTab) {
            percent = Animation.FastEnd(2);
        }
        if (Animation.State == AnimationState.ToTab) {
            percent = 1 - Animation.FastStart(2);
        }

        pos = StyleMath.Interpolate(pos, endPos, percent);

        sprite.Draw(Asset.Menu[0], pos, brightness);
    }

    public static void InfoBlob() {
        if (
            TabHandler.CurrentTab.Id() != SelectedTab.Info 
         && TabHandler.CurrentTab.Id() != SelectedTab.Menu
        ) return; 

        Color brightness = StyleMath.GetInitialBrightness();
        Vector2 pos = StyleMath.MenuBlobPosition(
            new Vector2(219, 300),
            new Vector2(530, 300),
            SelectedTab.Info
        );

        sprite.Draw(Asset.Menu[1], pos, brightness);
    }

    public static void CreditsBlob() {
        if (
            TabHandler.CurrentTab.Id() != SelectedTab.Credits 
         && TabHandler.CurrentTab.Id() != SelectedTab.Menu
        ) return; 

        Color brightness = StyleMath.GetInitialBrightness();
        Vector2 pos = StyleMath.MenuBlobPosition(
            new Vector2(114, 500),
            new Vector2(-300, 500),
            SelectedTab.Credits
        );

        sprite.Draw(Asset.Menu[2], pos, brightness);
    }

    public static void MenuHighScore() {
        Color color = StyleMath.GetScoreColor();
        string highScore = "HIGH SCORE: " + HighScoreTracker.HighScore.ToString();
        TextBox.WriteCenteredText(sprite, highScore, new Vector2(210, 200), color);
    }

    public static void Info() {
        double percent = 0;

        if (Animation.State == AnimationState.ToTab) {
            percent = 1 - Animation.FastStart(2);
        } else if (Animation.State == AnimationState.FromTab) {
            percent = Animation.FastEnd(2);
        }

        Color color = StyleMath.Interpolate(new Color(0, 127, 255), new Color(251, 194, 27), percent);

        Rectangle region = new Rectangle(40, 350, 360, 600);
        region.Y += (int) (percent * 300);

        TextBox.DrawInArea(
            sprite, 
    @"    2048, with blobs!
    Use the joystick to move tiles up, down, left, and right to slide them around the grid. (Diagonals do nothing.)
    When 2 identical tiles merge together, they combine into one! 
    Get to 2048 to win!
    
    From the menu, press RED to begin, BLUE to see this info menu, and GREEN to see the credits.
    In game, press RED to reset and BLUE to exit.
    
    (Press any button to return.)",
            region,
            color
        );
    }

    public static void Credits() {
        double percent = 0;

        if (Animation.State == AnimationState.ToTab) {
            percent = 1 - Animation.FastStart(2);
        } else if (Animation.State == AnimationState.FromTab) {
            percent = Animation.FastEnd(2);
        }

        Color color = StyleMath.Interpolate(new Color(0, 127, 0), new Color(251, 194, 27), percent);

        Rectangle region = new Rectangle(40, 350, 360, 600);
        region.Y += (int) (percent * 300);

        TextBox.DrawInArea(
            sprite, 
    @"   Original Game by Gabriele Cirulli.
    Ported to the Devcade by Alexander Day, with help from:
    - Noah Emke
    - Jeremy Start
    - Wilson McDade
    
    Special Thanks to the Computer Science House for the Devcade and Documentation, and you, for playing.
    
    (Press Any button to return.)",
            region,
            color
        );
    }
}