using Microsoft.Xna.Framework;
using Devcade;

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
		GameData.Export();
		//Persistence.Flush();
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
			// DebugRender.Write(GameData.Grid);
		}

		if (InputManager.isButtonPressed(Button.Red)) {
			Reset();
		}

		if (InputManager.isButtonPressed(Button.Blue)) {
			EndGame();
		}

		if (
			GameData.State == GameState.Won
		 && InputManager.isButtonPressed(Button.White)
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