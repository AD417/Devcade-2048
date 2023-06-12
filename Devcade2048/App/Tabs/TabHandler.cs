using Devcade2048.App.Render;

namespace Devcade2048.App.Tabs;

public static class TabHandler {
    private static ITab[] Tabs = new ITab[5];
    public static ITab LastTab { get; private set; }
    public static ITab CurrentTab { get; private set; }
    public static ITab NextTab { get; private set; } 

    public static void Initialize(Manager manager) {
        Tabs[0] = new MenuTab();
        Tabs[1] = new GameTab(manager);
        Tabs[2] = new InfoTab();
        Tabs[3] = new CreditsTab();
        // TODO: more tabs. (Not important for right now.)

        CurrentTab = Tabs[0];
        NextTab = Tabs[0];
        CurrentTab.Begin();
    }

    public static void SetNextTab(SelectedTab selected) {
        switch (selected) {
            case SelectedTab.None:
                return;
            case SelectedTab.Menu:
                NextTab = Tabs[0];
                break;
            case SelectedTab.Game:
                NextTab = Tabs[1];
                break;
            case SelectedTab.Info:
                NextTab = Tabs[2];
                break;
            case SelectedTab.Credits:
                NextTab = Tabs[3];
                break;
        }
        Animation.ChangeStateTo(AnimationState.FromTab);
    }

    public static void CheckTabSwitching() {
        if (Animation.SwitchTabs()) {
            LastTab = CurrentTab;
            CurrentTab = NextTab;
            CurrentTab.Begin();
        }
    }
}