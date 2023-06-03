﻿using Microsoft.Xna.Framework;
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

		windowSize = GraphicsDevice.Viewport.Bounds;
		
		base.Initialize();
	}

	/// <summary>
	/// Performs any setup that requires loaded content before the first frame.
	/// </summary>
	protected override void LoadContent()
	{
		_spriteBatch = new SpriteBatch(GraphicsDevice);

		HighScoreTracker.Load();
		
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
		Asset.ScoreFont = Content.Load<SpriteFont>("MonospaceTypewriter");

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
		 && Animation.AcceptInput()
		 && _GameData.State == GameState.Playing
		) {

			_GameData.Move(direction);
			DebugRender.Write(_GameData.Grid);
		}

		if (
		 	Animation.AcceptInput()
		 && (Keyboard.GetState().IsKeyDown(Keys.R) || Input.GetButton(1, Input.ArcadeButtons.A1))
		) {
			Reset();
		}
		if (Animation.ResetGrid()) _GameData.Setup();

		base.Update(gameTime);
	}

	private void Reset() {
		HighScoreTracker.Save();
		Animation.BeginReset(_GameData);
		/*switch (_GameData.State) {
			case GameState.Continuing:
			case GameState.Playing:
			case GameState.InMenu:
				Animation.ChangeStateTo(AnimationState.Spawning);
				break;
			case GameState.Won:
				Animation.ChangeStateTo(AnimationState.ResetFromWin);
				break;
			case GameState.Lost:
				Animation.ChangeStateTo(AnimationState.ResetFromLost);
				break;
		}
		_GameData.Setup(); */

	}

	/// <summary>
	/// Your main draw loop. This runs once every frame, over and over.
	/// </summary>
	/// <param name="gameTime">This is the gameTime object you can use to get the time since last frame.</param>
	protected override void Draw(GameTime gameTime)
	{
		// System.Console.WriteLine(Animation.State);
		GraphicsDevice.Clear(new Color(251, 194, 27));
		
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
		else Display.Menu();
		
		_spriteBatch.End();

		ScoreContainer.Increment(gameTime);
	 	Animation.Increment(gameTime);

		base.Draw(gameTime);
	}

	private void DrawGame() {
		_spriteBatch.Draw(Asset.Grid, new Vector2(10,290), Color.White);

		Display.AllTiles();
		Display.Scores();
		if (_GameData.State == GameState.Won) Display.Win();
		if (Animation.State == AnimationState.ResetFromWin) Display.WinReset();
		if (_GameData.State == GameState.Lost) Display.Lose();
	}
}