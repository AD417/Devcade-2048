using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;

using Devcade2048.App;
using Devcade2048.App.Render;

namespace Devcade2048.App.Tabs;

public class GameTab : ITab {
    private Manager GameData;

    public GameTab(Manager gameData) {
        // TODO: determine if the GameData / Manager can be created here, without needing the Game. 
        GameData = gameData;
    }

    public SelectedTab Id() => SelectedTab.Game;

    public void Begin() {
        GameData.Setup();
        GameData.State = GameState.Playing;
    }

    public void End() {
        HighScoreTracker.Save();
        GameData.State = GameState.Suspended;
    }

    public void Update(GameTime gameTime) {
		if (Animation.ResetGrid()) GameData.Setup();
		if (Animation.ContinueGame()) GameData.Continue();

		if (!Animation.AcceptInput()) return;

		Manager.Direction direction = InputManager.GetStickDirection();
		if (
			direction != Manager.Direction.None 
		 && (GameData.State == GameState.Playing || GameData.State == GameState.Continuing)
		) {

			GameData.Move(direction);
			DebugRender.Write(GameData.Grid);
		}

		if (
			Keyboard.GetState().IsKeyDown(Keys.R) 
		 || Input.GetButton(1, Input.ArcadeButtons.A1)
		) {
			Reset();
		}

		if (
			Keyboard.GetState().IsKeyDown(Keys.E)
		 || Input.GetButton(1, Input.ArcadeButtons.A2)
		) {
			EndGame();
		}

		if (
			GameData.State == GameState.Won
		 && (Keyboard.GetState().IsKeyDown(Keys.C) || Input.GetButton(1, Input.ArcadeButtons.A4))
		) {
			Continue();
		}
    }

	private void Reset() {
		HighScoreTracker.Save();
		Animation.BeginReset(GameData);
	}

	private void EndGame() {
		HighScoreTracker.Save();
        TabHandler.SetNextTab(SelectedTab.Menu);
	}

	private void Continue() {
		Animation.ChangeStateTo(AnimationState.ContinueFromWin);
	}

    public void Draw(GameTime gameTime) {
		Display.Grid();
		Display.AllTiles();
		Display.Scores();
		if (GameData.State == GameState.Won) Display.Win();
		if (Animation.State == AnimationState.ResetFromWin) Display.WinReset();
		if (GameData.State == GameState.Lost) Display.Lose();
    }
}