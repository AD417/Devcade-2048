using Microsoft.Xna.Framework;

using Devcade2048.App.Render;

namespace Devcade2048.App.Tabs;


public class CreditsTab : ITab {
    public SelectedTab _nextTab = SelectedTab.None;

    public SelectedTab Id() => SelectedTab.Credits;


    public void Begin() {
        _nextTab = SelectedTab.None;
    }

    public void End() {
        // None;
    }

    public void Update(GameTime gameTime) {
		if (!Animation.AcceptInput()) return;
        if (InputManager.AnyButtonPressed()) TabHandler.SetNextTab(SelectedTab.Menu);
    }


    public void Draw(GameTime gameTime) {
		Display.MenuBlobs();
        Display.Credits();
    }

    public SelectedTab NextTab() {
        return _nextTab;
    }
}