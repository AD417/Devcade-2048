using System;
using Microsoft.Xna.Framework;

namespace Devcade2048.App.Render.Animation;

public abstract class WaitingAnimationState : AnimationState {
    public override bool IsAcceptingInput()
    {
        return true;
    }

    public override bool IsGameContinuing()
    {
        // TODO: Evaluate necessity of this.
        return true;
    }
}