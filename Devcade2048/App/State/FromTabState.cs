using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.State;

public class FromTabState : TransientState {
    MenuTabState substate;

    public FromTabState(MenuTabState state) : base(TransitionTime) {
        substate = state;
    }

    public override BaseState NextState() {
        return new ToMenuState();
    }



    public override void Draw() {
        substate.Draw();

        GraphicsDevice gd = Pen.GraphicsDevice;
        Texture2D uiCover = new Texture2D(gd, 1, 1);
        uiCover.SetData(new Color[] { Background });
        
        // This backgroundColor pixel is stretched to cover everything. 
        Rectangle screen = new Rectangle(0, 0, gd.DisplayMode.Width, gd.DisplayMode.Height);

        DrawAsset(uiCover, screen, Color.White * FastStart());
        base.Draw();
    }
}