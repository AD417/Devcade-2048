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

    private static void DrawAsset(Texture2D image, Vector2 pos, Color color) {
        #region 
#if DEBUG
        sprite.Draw(image, pos, color);
#else
        Vector2 dim = new(
            image.Width,
            image.Height
        );
        Rectangle devcadePos = new(
            (int) (pos.X * 18 / 7),
            (int) (pos.Y * 18 / 7),
            (int) (dim.X * 18 / 7),
            (int) (dim.Y * 18 / 7)
        );
        sprite.Draw(image, devcadePos, color);
#endif
        #endregion
    }
    
    private static void DrawAsset(Texture2D image, Rectangle pos, Color color) {
        #region 
#if DEBUG
        sprite.Draw(image, pos, color);
#else
        Rectangle devcadePos = new(
            pos.X * 18 / 7,
            pos.Y * 18 / 7,
            pos.Width * 18 / 7,
            pos.Height * 18 / 7
        );
        sprite.Draw(image, devcadePos, color);
#endif
        #endregion
    }

    // XXX: Copy located At TextBox.DrawString
    private static void DrawAsset(SpriteFont font, string data, Vector2 pos, Color color) {
        #region
#if DEBUG
        sprite.DrawString(font, data, pos, color);
#else
        Vector2 actualPos = new(
            (int) (pos.X * 18.0f / 7.0f),
            (int) (pos.Y * 18.0f / 7.0f)
        );
        sprite.DrawString(
            spriteFont: font, 
            text: data, 
            position: actualPos, 
            color: color, 
            rotation: 0.0f, 
            origin: new(), 
            scale: 18.0f / 7.0f, 
            effects: SpriteEffects.None, 
            layerDepth: 0.0f
        );
#endif
        #endregion
    }


    public static void Title() {
        Color brightness = Color.White;
        if (Animation1.State == AnimationState1.InitFadeIn) {
            brightness = StyleMath.GetInitialBrightness();
        }
        DrawAsset(Asset.Title, new Vector2(60, 0), brightness);
    }

    public static void Grid() {
        Vector2 position = new Vector2(10, 290);
        // Time to go overkill on this.
        position += StyleMath.GridDisplacement();
        DrawAsset(Asset.Grid, position, Color.White);
    }

    public static void AllTiles() {
        if (!Animation1.RenderingTiles()) return;
        void drawTile(Tile t) {
            if (t is null) return;
            bool renderMerge = t.MergedFrom != null && Animation1.UpdatingGrid();
            if (renderMerge) {
                drawTile(t.MergedFrom[0]);
                drawTile(t.MergedFrom[1]);
            }

            Rectangle location = StyleMath.PositionOfTile(t);

            Texture2D blob = DetermineBlob(t);

            DrawAsset(blob, location, Color.White);
        }
        manager.Grid.EachCell((int _x, int _y, Tile t) => drawTile(t));
    }

    public static void Win() {
        if (manager.State != GameState.Won) return;
        if (Animation1.UpdatingGrid() || Animation1.State == AnimationState1.ResetFromWin) return;

        static void drawWin(Tile t) {
            if (t is null || t.Value != 2048) return;
            Rectangle location = StyleMath.PositionOfWinTile(t);

            DrawAsset(Asset.Tile[t.TextureId], location, Color.White);
        }

        manager.Grid.EachCell((int _x, int _y, Tile t) => drawWin(t));
        if (Animation1.AcceptInput()) {
            DrawAsset(Asset.BigFont, "YOU WIN!", new Vector2(20, 700), Color.Black);
            DrawAsset(Asset.Button, new Vector2(20, 720), Color.Red);
            DrawAsset(Asset.BigFont, "Play again", new Vector2(125, 750), Color.Red);
            DrawAsset(Asset.Button, new Vector2(20, 780), Color.Blue);
            DrawAsset(Asset.BigFont, "Exit to Menu", new Vector2(125, 810), Color.Blue);
            DrawAsset(Asset.Button, new Vector2(20, 840), Color.White);
            DrawAsset(Asset.BigFont, "Continue", new Vector2(125, 870), Color.White);
        }
    }

    public static void WinReset() {
        if (Animation1.State != AnimationState1.ResetFromWin) return;
        Rectangle location = StyleMath.PositionOfWinTile();
        DrawAsset(Asset.Tile[10], location, Color.White);
    }

    public static void Lose() {
        if (manager.State != GameState.Lost) return;
        if (Animation1.State != AnimationState1.WaitingForInput) return;
        DrawAsset(Asset.BigFont, "GAME OVER!", new Vector2(20, 700), Color.Black);
            DrawAsset(Asset.Button, new Vector2(20, 720), Color.Red);
            DrawAsset(Asset.BigFont, "Try again", new Vector2(125, 750), Color.Red);
            DrawAsset(Asset.Button, new Vector2(20, 780), Color.Blue);
            DrawAsset(Asset.BigFont, "Exit to Menu", new Vector2(125, 810), Color.Blue);
    }

    private static Texture2D DetermineBlob(Tile t) {
        if (manager.State != GameState.Lost) return Asset.Tile[t.TextureId];
        if (t.TextureId > StyleMath.BiggestLossTile()) return Asset.Tile[t.TextureId];
        int loseTileId = 0;
        if (t.TextureId > 5) loseTileId++;
        return Asset.LoseTile[loseTileId];
    }

    public static void AddScore(ScoreDelta score) {
        ScoreContainer.Add(score);
    }

    public static void Scores() {
        Color scoreColor = StyleMath.GetScoreColor();

        string scoreStr = "Score: " + manager.Score.ToString().PadLeft(5);
        int scoreWidth = (int)Asset.BigFont.MeasureString(scoreStr).X;
        DrawAsset(
            Asset.BigFont, 
            scoreStr, 
            new Vector2(400 - scoreWidth, 190), 
            scoreColor
        );

        string highScoreStr = 
            "Best: " + HighScoreTracker.HighScore.ToString().PadLeft(5);
        int highScoreWidth = (int)Asset.BigFont.MeasureString(highScoreStr).X;
        DrawAsset(
            Asset.BigFont, 
            highScoreStr, 
            new Vector2(400 - highScoreWidth, 240), 
            scoreColor
        );

        foreach (ScoreDelta score in ScoreContainer.scores) {
            string deltaStr = "+" + score.ToString();
            int width = (int) Asset.BigFont.MeasureString(deltaStr).X;
            Vector2 position = new Vector2(400 - width, 190 - score.ShiftUp());
            DrawAsset(
                Asset.BigFont, 
                deltaStr, 
                position, 
                score.DrawColor()
            );
        }
    }


    public static void MenuBlobs() {
        GameBlob();
        InfoBlob();
        CreditsBlob();
    }

    public static void GameBlob() {
        if (TabHandler.CurrentTab.Id() != SelectedTab.Menu) return;
        Color brightness = StyleMath.GetInitialBrightness();

        double percent = 0;
        if (Animation1.State == AnimationState1.FromTab) {
            percent = Animation1.FastEnd(2);
        }
        if (Animation1.State == AnimationState1.ToTab) {
            percent = 1 - Animation1.FastStart(2);
        }

        Vector2 newPos = StyleMath.Interpolate(
            new Vector2(9, 300), 
            new Vector2(-300, 300), 
            percent
        );
        Vector2 contPos = StyleMath.Interpolate(
            new Vector2(219, 300), 
            new Vector2(530, 300), 
            percent
        );

        DrawAsset(Asset.Menu[0], newPos, brightness);
        DrawAsset(Asset.Menu[1], contPos, brightness);
    }

    public static void InfoBlob() {
        if (
            TabHandler.CurrentTab.Id() != SelectedTab.Info 
         && TabHandler.CurrentTab.Id() != SelectedTab.Menu
        ) return; 

        Color brightness = StyleMath.GetInitialBrightness();
        Vector2 pos = StyleMath.MenuBlobPosition(
            new Vector2(9, 500),
            new Vector2(-300, 500),
            SelectedTab.Info
        );

        DrawAsset(Asset.Menu[2], pos, brightness);
    }

    public static void CreditsBlob() {
        if (
            TabHandler.CurrentTab.Id() != SelectedTab.Credits 
         && TabHandler.CurrentTab.Id() != SelectedTab.Menu
        ) return; 

        Color brightness = StyleMath.GetInitialBrightness();
        Vector2 pos = StyleMath.MenuBlobPosition(
            new Vector2(219, 500),
            new Vector2(530, 500),
            SelectedTab.Credits
        );

        DrawAsset(Asset.Menu[3], pos, brightness);
    }

    public static void MenuHighScore() {
        Color color = StyleMath.GetScoreColor();
        string highScore = "HIGH SCORE: " + HighScoreTracker.HighScore.ToString();
        TextBox.WriteCenteredText(sprite, highScore, new Vector2(210, 200), color);
    }

    public static void Info() {
        double percent = 0;

        if (Animation1.State == AnimationState1.ToTab) {
            percent = 1 - Animation1.FastStart(2);
        } else if (Animation1.State == AnimationState1.FromTab) {
            percent = Animation1.FastEnd(2);
        }

        Color color = StyleMath.Interpolate(new Color(0, 127, 255), new Color(251, 194, 27), percent);

        Rectangle region = new Rectangle(40, 350, 360, 600);
        region.Y += (int) (percent * 300);

        TextBox.DrawInArea(
            sprite, 
    @"    2048, with blobs!
    Use the joystick to move tiles up, down, left, and right to slide them around the grid.
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

        if (Animation1.State == AnimationState1.ToTab) {
            percent = 1 - Animation1.FastStart(2);
        } else if (Animation1.State == AnimationState1.FromTab) {
            percent = Animation1.FastEnd(2);
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