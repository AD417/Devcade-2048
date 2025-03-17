using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.Render.Animation;

public class MenuTabAnimationState : WaitingAnimationState {

    private readonly Selection selection;

    public MenuTabAnimationState(Selection selection) {
        this.selection = selection;
    }

  public override AnimationState ProcessInput()
  {
    if (InputManager.AnyButtonPressed()) return new FromTabAnimationState(this);
    return this;
  }



    public override void Draw()
    {
        base.Draw();
        Texture2D blob = getBlob();
        Vector2 top = new Vector2(114, 130);
        DrawAsset(blob, top);

        string text = getTabText();
        Rectangle region = new Rectangle(40, 350, 360, 600);
        TextBox.DrawInArea(Pen, text, region, Color.White);
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

    private string getTabText() {
        switch (selection) {
            case Selection.Info:
                return "This is the CREDITS tab!";
            case Selection.Credits:
                return "This is the INFO tab!";
            default:
                return "ERROR: Tab type '" + selection + "' has no string... how are we here?";
        }
    }
}