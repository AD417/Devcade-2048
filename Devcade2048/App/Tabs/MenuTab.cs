using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Devcade;

using Devcade2048.App;
using Devcade2048.App.Render;

namespace Devcade2048.App.Tabs;

public class MenuTab : ITab {

    public SelectedTab _nextTab = SelectedTab.None;

    public SelectedTab Id() => SelectedTab.Menu;


    public void Begin() {
        _nextTab = SelectedTab.None;
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
    }

	private void StartGame() {
		TabHandler.SetNextTab(SelectedTab.Game);
	}

	private void GotoInfo() {
		Animation.ChangeStateTo(AnimationState.FromTab);
		//  _GameData.State = GameState.InInfo;
	}


    public void Draw(GameTime gameTime) {
		Display.MenuBlobs();
    }

    public SelectedTab NextTab() {
        return _nextTab;
    }
}