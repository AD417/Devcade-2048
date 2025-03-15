using System;
using Microsoft.Xna.Framework;

namespace Devcade2048.App.Render.Animation;

public abstract class TransientAnimationState : AnimationState {


    protected static readonly TimeSpan TransitionTime = TimeSpan.FromMilliseconds(1000);
    protected static readonly TimeSpan TileMoveTime = TimeSpan.FromMilliseconds(150);
    protected static readonly TimeSpan TileSpawnTime = TimeSpan.FromMilliseconds(100);
    protected static readonly TimeSpan WinAnimationTime = TimeSpan.FromMilliseconds(2000);

    internal TimeSpan Timer = new();
    internal TimeSpan MaxTime;

    public TransientAnimationState(TimeSpan maxTime, AnimationState previous = null) : base(previous) {
        MaxTime = maxTime;
    }

    public override void Tick(GameTime gt)
    {
        base.Tick(gt);
        Timer += gt.ElapsedGameTime;
    }

    public override double PercentComplete()
    {
        return Math.Min(Timer / MaxTime, 1.0);
    }

    public override bool IsComplete()
    {
        return PercentComplete() == 1.0;
    }

  public override AnimationState ProcessInput()
  {
    return this;
  }



    public abstract override AnimationState NextState();

    public double FastEnd(double factor = 3) {
        double normalPercent = PercentComplete();
        return Math.Pow(normalPercent, factor);
    }

    public double FastStart(double factor = 3) {
        double normalPercent = PercentComplete();
        return 1 - Math.Pow(1 - normalPercent, factor);
    }

    protected static Vector2 Interpolate(Vector2 v1, Vector2 v2, double percent) {
        float x, y;
        x = (float) (v1.X * (1 - percent) + v2.X * percent);
        y = (float) (v1.Y * (1 - percent) + v2.Y * percent);
        return new Vector2(x, y);
    }
}