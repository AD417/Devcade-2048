using System;
using Microsoft.Xna.Framework;

namespace Devcade2048.App.State;

public abstract class WaitingState : BaseState {
    public override bool IsAcceptingInput() {
        return true;
    }
}