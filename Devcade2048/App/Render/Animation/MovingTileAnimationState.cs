using System;

namespace Devcade2048.App.Render.Animation;

public class MovingTileAnimationState : TransientAnimationState {
    public MovingTileAnimationState() : base(TileMoveTime) {

    }

    public override void Draw() {
           
    }

  public override AnimationState NextState()
  {
    throw new NotImplementedException();
  }
}