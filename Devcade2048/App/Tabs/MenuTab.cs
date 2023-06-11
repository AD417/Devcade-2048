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
		if (
			Keyboard.GetState().IsKeyDown(Keys.R) 
		 || Input.GetButton(1, Input.ArcadeButtons.A1)
		) {
			StartGame();
		}

		if (
			Keyboard.GetState().IsKeyDown(Keys.E)
		 || Input.GetButton(1, Input.ArcadeButtons.A2)
		) {
			GotoInfo();
		}

		if (
			Keyboard.GetState().IsKeyDown(Keys.C)
		 || Input.GetButton(1, Input.ArcadeButtons.A3)
		) {
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