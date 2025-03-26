using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.State;

public class FadeInState : TransientState {
    private readonly MainMenuState substate = new MainMenuState();

    public FadeInState() : base(TransitionTime) {}

    public override void Draw() {
        // Draw the menu first. 
        substate.Draw();

        // Draw a black rectangle of varying opacity over the entire UI. 
        GraphicsDevice gd = Pen.GraphicsDevice;
        Texture2D uiCover = new Texture2D(gd, 1, 1);
        uiCover.SetData(new Color[] { Color.Black });
        
        // This black pixel is stretched to cover everything. 
        Rectangle screen = new Rectangle(0, 0, gd.DisplayMode.Width, gd.DisplayMode.Height);

        DrawAsset(uiCover, screen, Color.Black * (1 - PercentComplete()));
    }

    public override BaseState NextState() {
        return substate;
    }
}