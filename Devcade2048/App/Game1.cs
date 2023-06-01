using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;
using System;
using Devcade2048.App.Render;

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
		
		// Add initialization logic here
		Animation.Reset();
		HighScoreTracker.Load();

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

		Asset.Title = Content.Load<Texture2D>("TitleBar");

		Asset.Menu[0] = Content.Load<Texture2D>("BlobStart");
		Asset.Menu[1] = Content.Load<Texture2D>("BlobInfo");
		Asset.Menu[2] = Content.Load<Texture2D>("BlobCredits");

		Asset.Grid = Content.Load<Texture2D>("2048Grid");
		for (int i = 0; i < 11; i++) {
			int powerOfTwo = (int) Math.Pow(2, i + 1);
			Asset.Tile[i] = Content.Load<Texture2D>($"{powerOfTwo.ToString("0000")} Tile");
		}
		Asset.LoseTile[0] = Content.Load<Texture2D>("DED1 Blob");
		Asset.LoseTile[1] = Content.Load<Texture2D>("DED2 Blob");
		Asset.Score = Content.Load<SpriteFont>("MonospaceTypewriter");

		Display.Initialize(_spriteBatch, _GameData);
	}

	/// <summary>
	/// Your main update loop. This runs once every frame, over and over.
	/// </summary>
	/// <param name="gameTime">This is the gameTime object you can use to get the time since last frame.</param>
	protected override void Update(GameTime gameTime)
	{
		Input.Update(); // Updates the state of the input library

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
		 && Render.Animation.AcceptInput()
		 && _GameData.State == GameState.Playing
		) {

			_GameData.Move(direction);
			DebugRender.Write(_GameData.Grid);
		}

		if (
			Render.Animation.AcceptInput()
		 && (Keyboard.GetState().IsKeyDown(Keys.R) || Input.GetButton(1, Input.ArcadeButtons.A1))
		) {
			HighScoreTracker.Save();
			_GameData.Setup();
			Render.Animation.ChangeStateTo(AnimationState.Spawning);
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
		Render.Animation.Increment(gameTime);
		ScoreContainer.Increment(gameTime);
		
		// Batches all the draw calls for this frame, and then performs them all at once
		_spriteBatch.Begin();

		Display.Title();

		bool inGame = (
			_GameData.State == GameState.Playing
		 || _GameData.State == GameState.Continuing
		 || _GameData.State == GameState.Won
		 || _GameData.State == GameState.Lost
		);

		if (inGame) DrawGame();
		else DrawMenu();
		
		_spriteBatch.End();

		base.Draw(gameTime);
	}

	private void DrawMenu() {
		// Midpoint of screen is (210, 490);
		// 210 - 192 = 18 

		_spriteBatch.Draw(Asset.Menu[0], new Vector2(9, 300), Color.White);
		_spriteBatch.Draw(Asset.Menu[1], new Vector2(219, 300), Color.White);
		_spriteBatch.Draw(Asset.Menu[2], new Vector2(114, 500), Color.White);
	}

	private void DrawGame() {

		_spriteBatch.Draw(Asset.Grid, new Vector2(10,290), Color.White);
		// DrawNormalTiles();
		Display.AllTiles();
		Display.Scores();

		if (_GameData.State == GameState.Won) {
			Display.Win();
			DrawWinAnimation();
		}

		if (_GameData.State == GameState.Lost) {
			DrawLossAnimation();
		}
		
		// DrawScore();
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
			// if (isMerged) _spriteBatch.Draw(Asset.MergeBG, location, Color.White);
			_spriteBatch.Draw(Asset.Tile[t.TextureId], location, Color.White);
		}
		_GameData.Grid.EachCell(drawTile);
	}

	private void DrawWinAnimation() {
		void resize2048Tile(int _x, int _y, Tile t) {
			if (t is null || t.Value != 2048) return;
			int scale = Animation.WinScale();
			Vector2 pos = Animation.WinPosition(t.Position);
			Rectangle location = new Rectangle((int)pos.X, (int)pos.Y, scale, scale);
			_spriteBatch.Draw(Asset.Tile[t.TextureId], location, Color.White);
		}
		_GameData.Grid.EachCell(resize2048Tile);

		if (Animation.IsWinComplete()) {
			_spriteBatch.DrawString(
				Asset.Score, 
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
			int deadBlobId = 0;
			if (t.TextureId > 3) deadBlobId = 1;
			Rectangle location = new Rectangle(
				(int) (12 + t.Position.X * 100),
				(int) (292 + t.Position.Y * 100),
				96,
				96
			);
			_spriteBatch.Draw(Asset.LoseTile[deadBlobId], location, Color.White);
		}
		_GameData.Grid.EachCell(killBlobs);

		if (Animation.IsLossComplete()) {
			_spriteBatch.DrawString(
				Asset.Score, 
				"GAME OVER!",
				new Vector2(20, 700),
				Color.Black
			);
		}
	}

	private void DrawScore() {
		String scoreStr = "Score: " + _GameData.Score.ToString().PadLeft(4);
		int scoreWidth = (int)Asset.Score.MeasureString(scoreStr).X;
		_spriteBatch.DrawString(Asset.Score, scoreStr, new Vector2(400 - scoreWidth, 200), Color.Black);

		String highScoreStr = "Best: " + HighScoreTracker.HighScore.ToString().PadLeft(4);
		int highScoreWidth = (int)Asset.Score.MeasureString(highScoreStr).X;
		_spriteBatch.DrawString(Asset.Score, highScoreStr, new Vector2(400 - highScoreWidth, 250), Color.Black);
		
		/*if (_GameData.ScoreDelta != 0 && Animation.IsScoreVisible()) {
			String deltaStr = $"+{_GameData.ScoreDelta}";
			int deltaWidth = (int)Asset.Score.MeasureString(deltaStr).X;
			_spriteBatch.DrawString(
				Asset.Score, 
				deltaStr, 
				new Vector2(400 - deltaWidth, 200 - Animation.ScoreDisplacement()), 
				Animation.InterpolateColor(Color.Green, new Color(251, 194, 27))
			);
		} */
		// Display.Scores();
	}

}