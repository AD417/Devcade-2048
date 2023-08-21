using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Devcade;

using Devcade2048.App.Render;

namespace Devcade2048.App.Tabs;

public class MenuTab : ITab {
    public SelectedTab Id() => SelectedTab.Menu;


    public void Begin() {
		// None;
    }

    public void End() {
        // None;
    }

    public void Update(GameTime gameTime) {
		if (!Animation.AcceptInput()) return;
		if (InputManager.isButtonPressed(Button.Red)) {
			StartGame();
		}

		if (InputManager.isButtonPressed(Button.Blue)) {
			GotoInfo();
		}

		if (InputManager.isButtonPressed(Button.Green)) {
			GotoCredits();
		}
    }

	private void StartGame() {
		TabHandler.SetNextTab(SelectedTab.Game);
	}

	private void GotoInfo() {
		TabHandler.SetNextTab(SelectedTab.Info);
	}

	private void GotoCredits() {
		TabHandler.SetNextTab(SelectedTab.Credits);
	}

    public void Draw(GameTime gameTime) {
		Display.MenuHighScore();
		Display.MenuBlobs();
    }
}