﻿using Microsoft.Xna.Framework;
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
	private Texture2D[] _TileTextures = new Texture2D[11];

	private Manager _GameData { get; }

	private int graceFrames { get; set; }
	private bool holding { get; set; }
	
	/// <summary>
	/// Game constructor
	/// </summary>
	public Game1()
	{
		_graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = false;

		_GameData = new Manager(size: 4);
		graceFrames = 0;
		holding = false;

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
		if (direction != Manager.Direction.None && Animation.IsComplete()) {

			_GameData.Move(direction);
			DebugRender.Write(_GameData.Grid);
			Animation.Reset();
		} else {
			holding = false;
		}

		base.Update(gameTime);
	}

	/// <summary>
	/// Your main draw loop. This runs once every frame, over and over.
	/// </summary>
	/// <param name="gameTime">This is the gameTime object you can use to get the time since last frame.</param>
	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(new Color(242, 194, 0));
		
		// Batches all the draw calls for this frame, and then performs them all at once
		_spriteBatch.Begin();
		// TODO: Add your drawing code here

		_spriteBatch.Draw(_GridTexture, new Vector2(10,290), Color.White);
		void drawTile(int x, int y, Tile t) {
			if (t == null) return;

			Vector2 animPosition = t.Position;
			int scale = 96;

			if (t.PreviousPosition == null) scale = Animation.Scale();
			else animPosition = Animation.Interpolate((Vector2)t.PreviousPosition, t.Position);

			// Vector2 animPosition = t.Position;
			// Rectangle rect = new Rectangle(0, 0, 96, 96);
			// if (t.PreviousPosition != null) {
			// 	animPosition = Animation.Interpolate((Vector2)t.PreviousPosition, t.Position);
			// } else {
			// 	int scale = Animation.Scale();
			// 	rect = new Rectangle(0, 0, 96, 96);
			// }
			int drawX = (int) (12 + animPosition.X * 100);
			int drawY = (int) (292 + animPosition.Y * 100);
			// Texture, rectangle, color
			Rectangle location = new Rectangle(drawX, drawY, scale, scale);
			_spriteBatch.Draw(_TileTextures[t.TextureId], location, Color.White);
			//_spriteBatch.Draw(_TileTextures[t.TextureId], new Vector2(drawX, drawY), Color.White, rect);
		}
		_GameData.Grid.EachCell(drawTile);

		// _spriteBatch.Draw(_TileTextures[1], new Vector2(0,0), Color.White, );

		// for (int i = 0; i < 11; i++) {
		// 	int x = 12 + (i % 4) * 100;
		// 	int y = 292 + (i / 4) * 100;
		// 	_spriteBatch.Draw(_TileTextures[i], new Vector2(x,y), Color.White);
		// }
		
		_spriteBatch.End();

		base.Draw(gameTime);
	}
}