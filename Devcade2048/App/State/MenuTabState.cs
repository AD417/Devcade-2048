using System;
using Devcade2048.App.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.State;

public class MenuTabState : WaitingState {

    private readonly Selection selection;

    public MenuTabState(Selection selection) {
        this.selection = selection;
    }

  public override BaseState ProcessInput() {
    if (InputManager.AnyButtonPressed()) return new FromTabState(this);
    return this;
  }



    public override void Draw() {
        base.Draw();
        Texture2D blob = getBlob();
        Vector2 top = new Vector2(114, 130);
        DrawAsset(blob, top);

        string text = getTabText();
        Color color = getColor();
        Rectangle region = new Rectangle(40, 350, 360, 600);
        TextBox.DrawInArea(Pen, text, region, color);
    }

    private Texture2D getBlob() {
        switch (selection) {
            case Selection.Info:
                return Asset.Menu[2];
            case Selection.Credits:
                return Asset.Menu[3];
            default:
                throw new Exception("Tab type '" + selection + "' has no codepath!");
        }
    }

    private Color getColor() {
        switch (selection) {
            case Selection.Info:
                return Color.Blue;
            case Selection.Credits:
                return Color.DarkGreen;
            default:
                return Color.White;
        }
    }

    private string getTabText() {
        switch (selection) {
            case Selection.Info:
                return @"    2048, with blobs!
    Use the joystick to move tiles up, down, left, and right to slide them around the grid.
    When 2 identical tiles merge together, they combine into one! 
    Get to 2048 to win!
    
    From the menu, press RED to begin, BLUE to see this info menu, and GREEN to see the credits.
    In game, press RED to reset and BLUE to exit.
    
    (Press any button to return.)";
            case Selection.Credits:
                return @"   Original Game by Gabriele Cirulli.
    Ported to the Devcade with help from:
    - Noah Emke
    - Ella Soccolli
    - Jeremy Smart

    Special Thanks to the Computer Science House for the Devcade and Documentation, and you, for playing.
    
    (Press Any button to return.)";
            default:
                return "ERROR: Tab type '" + selection + "' has no string... how are we here?";
        }
    }
}