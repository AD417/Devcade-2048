using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;
using System;

// MAKE SURE YOU RENAME ALL PROJECT FILES FROM DevcadeGame TO YOUR YOUR GAME NAME
namespace Devcade2048.App;

public class Game1 : Game
{
	private GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;
	
	/// <summary>
	/// Stores the window dimensions in a rectangle object for easy use
	/// </summary>
	private Rectangle windowSize;

	private Texture2D _GridTexture;
	private Texture2D[] _TileTextures = new Texture2D[13];
	private Texture2D _MergeTexture;
	private SpriteFont _ScoreFont;

	private Manager _GameData { get; }

	
	/// <summary>
	/// Game constructor
	/// </summary>
	public Game1()
	{
		_graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = false;

		_GameData = new Manager(size: 4);

		DebugRender.Write(_GameData.Grid);
	}

	/// <summary>
	/// Performs any setup that doesn't require loaded content before the first frame.
	/// </summary>
	protected override void Initialize()
	{
		// Sets up the input library
		Input.Initialize();

		// Set window size if running debug (in release it will be fullscreen)
		#region
#if DEBUG
		_graphics.PreferredBackBufferWidth = 420;
		_graphics.PreferredBackBufferHeight = 980;
		_graphics.ApplyChanges();
#else
		_graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
		_graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
		_graphics.ApplyChanges();
#endif
		#endregion
		
		// TODO: Add your initialization logic here
		Animation.Reset();

		windowSize = GraphicsDevice.Viewport.Bounds;
		
		base.Initialize();
	}

	/// <summary>
	/// Performs any setup that requires loaded content before the first frame.
	/// </summary>
	protected override void LoadContent()
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);

		// TODO: use this.Content to load your game content here
		// ex:
		// texture = Content.Load<Texture2D>("fileNameWithoutExtension");
		_GridTexture = Content.Load<Texture2D>("2048Grid");
		for (int i = 0; i < 11; i++) {
			int powerOfTwo = (int) Math.Pow(2, i + 1);
			_TileTextures[i] = Content.Load<Texture2D>($"{powerOfTwo.ToString("0000")} Tile");
		}
		_TileTextures[11] = Content.Load<Texture2D>("DED1 Blob");
		_TileTextures[12] = Content.Load<Texture2D>("DED2 Blob");
		_MergeTexture = Content.Load<Texture2D>("Merge Mask");
		_ScoreFont = Content.Load<SpriteFont>("ComfortaaLight");
	}

	/// <summary>
	/// Your main update loop. This runs once every frame, over and over.
	/// </summary>
	/// <param name="gameTime">This is the gameTime object you can use to get the time since last frame.</param>
	protected override void Update(GameTime gameTime)
	{
		Input.Update(); // Updates the state of the input library
		Animation.Increment(gameTime);

		// Exit when both menu buttons are pressed (or escape for keyboard debugging)
		// You can change this but it is suggested to keep the keybind of both menu
		// buttons at once for a graceful exit.
		if (Keyboard.GetState().IsKeyDown(Keys.Escape) ||
			(Input.GetButton(1, Input.ArcadeButtons.Menu) &&
			Input.GetButton(2, Input.ArcadeButtons.Menu)))
		{
			Exit();
		}

		// TODO: Add your update logic here
		Manager.Direction direction = InputManager.GetStickDirection();
		if (
			direction != Manager.Direction.None 
		 && Animation.IsComplete() 
		 && _GameData.State == GameState.Playing
		) {

			_GameData.Move(direction);
			DebugRender.Write(_GameData.Grid);
			Animation.Reset();
		}

		base.Update(gameTime);
	}

	/// <summary>
	/// Your main draw loop. This runs once every frame, over and over.
	/// </summary>
	/// <param name="gameTime">This is the gameTime object you can use to get the time since last frame.</param>
	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(new Color(251, 194, 27));
		
		// Batches all the draw calls for this frame, and then performs them all at once
		_spriteBatch.Begin();

		_spriteBatch.Draw(_GridTexture, new Vector2(10,290), Color.White);
		DrawNormalTiles();

		if (_GameData.State == GameState.Won) {
			DrawWinAnimation();
		}

		if (_GameData.State == GameState.Lost) {
			DrawLossAnimation();
		}
		
		DrawScore();
		
		_spriteBatch.End();

		base.Draw(gameTime);
	}

	private void DrawNormalTiles() {
		void drawTile(int x, int y, Tile t) {
			if (t == null) return;
			bool isMerged = (t.MergedFrom != null && !Animation.IsComplete());
			if (isMerged) {
				drawTile(x, y, t.MergedFrom[0]);
				drawTile(x, y, t.MergedFrom[1]);
			}

			Vector2 animPosition = t.Position;
			int scale = 96;

			if (t.PreviousPosition == null) scale = Animation.NewTileScale();
			else animPosition = Animation.InterpolatePosition((Vector2)t.PreviousPosition, t.Position);

			int drawX = (int) (12 + animPosition.X * 100) + (48 - scale / 2);
			int drawY = (int) (292 + animPosition.Y * 100) + (48 - scale / 2);
			// Texture, rectangle, color
			Rectangle location = new Rectangle(drawX, drawY, scale, scale);
			if (isMerged) _spriteBatch.Draw(_MergeTexture, location, Color.White);
			_spriteBatch.Draw(_TileTextures[t.TextureId], location, Color.White);
		}
		_GameData.Grid.EachCell(drawTile);
	}

	private void DrawWinAnimation() {
		void resize2048Tile(int _x, int _y, Tile t) {
			if (t is null || t.Value != 2048) return;
			int scale = Animation.WinScale();
			Vector2 pos = Animation.WinPosition(t.Position);
			Rectangle location = new Rectangle((int)pos.X, (int)pos.Y, scale, scale);
			_spriteBatch.Draw(_TileTextures[t.TextureId], location, Color.White);
		}
		_GameData.Grid.EachCell(resize2048Tile);

		if (Animation.IsWinComplete()) {
			_spriteBatch.DrawString(
				_ScoreFont, 
				"YOU WIN!", 
				new Vector2(20, 700), 
				Color.Black
			);
		}
	}

	private void DrawLossAnimation() {
		int maximumBlob = Animation.LossMaximumBlob();
		void killBlobs(int _x, int _y, Tile t) {
			// This should be impossible, but CYA. 
			if (t is null) return;
			if (t.TextureId > maximumBlob) return;
			int deadBlobId = 11;
			if (t.TextureId > 3) deadBlobId = 12;
			Rectangle location = new Rectangle(
				(int) (12 + t.Position.X * 100),
				(int) (292 + t.Position.Y * 100),
				96,
				96
			);
			_spriteBatch.Draw(_TileTextures[deadBlobId], location, Color.White);
		}
		_GameData.Grid.EachCell(killBlobs);

		if (Animation.IsLossComplete()) {
			_spriteBatch.DrawString(
				_ScoreFont, 
				"GAME OVER!",
				new Vector2(20, 700),
				Color.Black
			);
		}
	}

	private void DrawScore() {
		String scoreStr = "Score: " + _GameData.Score.ToString().PadLeft(5);
		int scoreWidth = (int)_ScoreFont.MeasureString(scoreStr).X;
		_spriteBatch.DrawString(_ScoreFont, scoreStr, new Vector2(400 - scoreWidth, 250), Color.Black);
		
		if (_GameData.ScoreDelta != 0 && Animation.IsScoreVisible()) {
			String deltaStr = $"+{_GameData.ScoreDelta}";
			int deltaWidth = (int)_ScoreFont.MeasureString(deltaStr).X;
			_spriteBatch.DrawString(
				_ScoreFont, 
				deltaStr, 
				new Vector2(400 - deltaWidth, 250 - Animation.ScoreDisplacement()), 
				Animation.InterpolateColor(Color.Green, new Color(251, 194, 27))
			);
		}
	}

}