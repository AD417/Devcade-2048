using Devcade2048.App.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.State;

public abstract class BaseState {
    
	/// <summary>
	/// The object responsible for "Drawing" all content on the screen.
	/// </summary>
    protected static SpriteBatch Pen {get; private set; }

	/// <summary>
	/// The object that stores all of the data about the game. 
	/// </summary>
    protected static Manager Game {get; private set; }

	/// <summary>
	/// The state that the program should begin in. 
	/// </summary>
    public static readonly BaseState InitialState = new FadeInState();
    
	/// <summary>
	/// The color of the background. 
	/// </summary>
    public static readonly Color Background = new Color(251, 194, 27);

	/// <summary>
	/// Set (or reset) the tools used for rendering and game management. 
	/// </summary>
	/// <param name="pen">The spritebatch that draws to the screen.</param>
	/// <param name="game">The game manager object with the game state.</param>
    public static void SetRendering(SpriteBatch pen, Manager game) {
        Pen = pen;
        Game = game;
    }

    public BaseState() {
    }

	/// <summary>
	/// Simulate the logic of this state for the given amount of time.
	/// </summary>
	/// <param name="gt">
    /// A GameTime object, holding the amount of time that has passed since 
    /// the last frame.
    /// </param>
    public virtual void Tick(GameTime gt) {
        // Nothing by default
    }

	/// <summary>
	/// Determine how complete this state is, if at all. 
	/// </summary>
    /// <returns>A value from 0 to 1, inclusive.</returns>
    public virtual float PercentComplete() {
        return 0.0F;
    }

	/// <summary>
	/// Determine if this state is complete.
	/// </summary>
    /// <returns>True iff this state is finished and should be changed.</returns>
    public virtual bool IsComplete() {
        return false;
    }

	/// <summary>
	/// Determine what state comes after this one.
	/// </summary>
    /// <returns>A State. Can be this state.</returns>
    public virtual BaseState NextState() {
        return this;
    }

	/// <summary>
	/// Determine if this state allows the user to enter inputs.
	/// </summary>
    public virtual bool IsAcceptingInput() {
        return false;
    }

	/// <summary>
	/// Process the input the user provided. This may change the current state.
	/// </summary>
    /// <returns>The state that the input puts us in. Can be this state.</returns>
    public virtual BaseState ProcessInput() {
        return this;
    }



	/// <summary>
	/// Draw the current state as an animation to the screen.
	/// </summary>
    public virtual void Draw() {
        DrawTitle();
    }

	/// <summary>
	/// Draw the game title. This appears at all times.
	/// </summary>
    private static void DrawTitle() {
        DrawAsset(Asset.Title, new Vector2(60, 0), Color.White);
    }

	/// <summary>
	/// Render all of the tiles on the grid, using <see cref="DrawTile" />.
    /// Override DrawTile to get custom tile behaviour. 
	/// </summary>
    protected void DrawAllTiles() {
        Game.Grid.EachCell((int _x, int _y, Tile t) => DrawTile(t));
    }

	/// <summary>
	/// Draw a given tile. 
	/// </summary>
    protected virtual void DrawTile(Tile t) {
        if (t is null) return;

        Vector2 pos = new Vector2(
            12 + t.Position.X * 100,
            292 + t.Position.Y * 100
        );
        Rectangle location = new Rectangle(pos.ToPoint(), new Point(96,96));

        Texture2D blob = Asset.Tile[t.TextureId];

        DrawAsset(blob, location, Color.White);
    }

	/// <summary>
	/// Draw the game score and high score. Opacity can be controlled 
    /// via <see cref="ScoreTextOpacity" />
	/// </summary>
    protected virtual void DrawScore() {
        float opacity = ScoreTextOpacity();
        Color scoreColor = Color.Black * opacity;

        string scoreStr = "Score: " + Game.Score.ToString().PadLeft(5);
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

        foreach(ScoreDelta score in ScoreContainer.Scores) {
            string deltaStr = "+" + score.ToString();
            int width = (int) Asset.BigFont.MeasureString(deltaStr).X;
            Vector2 position = new Vector2(400 - width, 190 - score.ShiftUp());
            DrawAsset(
                Asset.BigFont, 
                deltaStr, 
                position, 
                score.DrawColor() * opacity
            );
        }
    }

	/// <summary>
	/// Determine the opacity of the text used in <see cref="DrawScore" />
	/// </summary>
    /// <returns> A float between 0.0 and 1.0 inclusive. </returns>
    protected virtual float ScoreTextOpacity() {
        return 1.0F;
    }

	/// <summary>
	/// Draw a given asset. This ensures proper scaling between development
    /// and release environments.
	/// </summary>
	/// <param name="image">An asset to draw</param>
	/// <param name="pos">
    /// The location of where the top left corner of the image should be drawn.
    /// </param>
    protected static void DrawAsset(Texture2D image, Vector2 pos) {
        // Color does not create compile-time constants.
        DrawAsset(image, pos, Color.White);
    }

	/// <summary>
	/// Draw a given asset. This ensures proper scaling between development
    /// and release environments.
	/// </summary>
	/// <param name="image">An asset to draw</param>
	/// <param name="pos">
    /// The bounds of where the image should be drawn.
    /// The (x,y) position is the top left corner of the image's loation.
    /// The (width, height) values are the dimensions of the on-screen drawing 
    /// in the X and Y direction respectively.
    /// </param>
    protected static void DrawAsset(Texture2D image, Rectangle pos) {
        // Color does not create compile-time constants.
        DrawAsset(image, pos, Color.White);
    }

	/// <summary>
	/// Draw a given text with the given font. This ensures proper scaling
    /// between development and release environments.
	/// </summary>
	/// <param name="font">An asset to draw</param>
	/// <param name="pos">
    /// The location of where the top left corner of the image should be drawn.
    /// </param>
    protected static void DrawAsset(SpriteFont font, string data, Vector2 pos) {
        DrawAsset(font, data, pos, Color.White);
    }

	/// <summary>
	/// Draw a given asset. This ensures proper scaling between development
    /// and release environments.
	/// </summary>
	/// <param name="image">An asset to draw</param>
	/// <param name="pos">
    /// The location of where the top left corner of the image should be drawn.
    /// </param>
	/// <param name="color">
    /// A color filter. White draws it as normal. Red only draws the red channel. 
    /// Other colors only draw a proportional amount of their RGB channels.
    /// Black draws a silouhette. 
    /// </param>
    protected static void DrawAsset(Texture2D image, Vector2 pos, Color color) {
        #region 
#if DEBUG
        Pen.Draw(image, pos, color);
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
        Pen.Draw(image, devcadePos, color);
#endif
        #endregion
    }

	/// <summary>
	/// Draw a given asset. This ensures proper scaling between development
    /// and release environments.
	/// </summary>
	/// <param name="image">An asset to draw</param>
	/// <param name="pos">
    /// The bounds of where the image should be drawn.
    /// The (x,y) position is the top left corner of the image's loation.
    /// The (width, height) values are the dimensions of the on-screen drawing 
    /// in the X and Y direction respectively.
    /// </param>
	/// <param name="color">
    /// A color filter. White draws it as normal. Red only draws the red channel. 
    /// Other colors only draw a proportional amount of their RGB channels.
    /// Black draws a silouhette. 
    /// </param>
    protected static void DrawAsset(Texture2D image, Rectangle pos, Color color) {
        #region 
#if DEBUG
        Pen.Draw(image, pos, color);
#else
        Rectangle devcadePos = new(
            pos.X * 18 / 7,
            pos.Y * 18 / 7,
            pos.Width * 18 / 7,
            pos.Height * 18 / 7
        );
        Pen.Draw(image, devcadePos, color);
#endif
        #endregion
    }

	/// <summary>
	/// Draw a given text with the given font. This ensures proper scaling
    /// between development and release environments.
	/// </summary>
	/// <param name="font">An asset to draw</param>
	/// <param name="pos">
    /// The location of where the top left corner of the image should be drawn.
    /// </param>
	/// <param name="color">The text's color.</param>
    // XXX: Copy located At TextBox.DrawString
    protected static void DrawAsset(SpriteFont font, string data, Vector2 pos, Color color) {
        #region
#if DEBUG
        Pen.DrawString(font, data, pos, color);
#else
        Vector2 actualPos = new(
            (int) (pos.X * 18.0f / 7.0f),
            (int) (pos.Y * 18.0f / 7.0f)
        );
        Pen.DrawString(
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
}