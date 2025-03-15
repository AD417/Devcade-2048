using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.Render.Animation;

public class FadeInAnimationState : TransientAnimationState {
    private readonly MainMenuAnimationState substate = new MainMenuAnimationState();

    public FadeInAnimationState() : base(TransitionTime) {}

    public override void Draw() {
      // Draw the menu first. 
        substate.Draw();

        // Draw a black rectangle of varying opacity over the entire UI. 
        GraphicsDevice gd = Pen.GraphicsDevice;
        Texture2D uiCover = new Texture2D(gd, 1, 1);
        uiCover.SetData(new Color[] { Color.Black });
        
        // This black pixel is stretched to cover everything. 
        Rectangle screen = new Rectangle(0, 0, gd.DisplayMode.Width, gd.DisplayMode.Height);

        DrawAsset(uiCover, screen, Color.Black * (float) (1 - PercentComplete()));
    }

  public override AnimationState NextState()
  {
    return substate;
  }
}