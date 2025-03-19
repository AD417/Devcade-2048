using System;
using Microsoft.Xna.Framework;

namespace Devcade2048.App.State;

public abstract class TransientState : BaseState {


    protected static readonly TimeSpan TransitionTime = TimeSpan.FromMilliseconds(1000);
    protected static readonly TimeSpan TileMoveTime = TimeSpan.FromMilliseconds(150);
    protected static readonly TimeSpan TileSpawnTime = TimeSpan.FromMilliseconds(100);
    protected static readonly TimeSpan GameEndingTime = TimeSpan.FromMilliseconds(2000);

    internal TimeSpan Timer = new();
    internal TimeSpan MaxTime;

    public TransientState(TimeSpan maxTime) : base() {
        MaxTime = maxTime;
    }

    public override void Tick(GameTime gt)
    {
        base.Tick(gt);
        Timer += gt.ElapsedGameTime;
    }

    public override float PercentComplete()
    {
        return MathF.Min((float) (Timer / MaxTime), 1.0F);
    }

    public override bool IsComplete()
    {
        return PercentComplete() == 1.0;
    }

  public override BaseState ProcessInput()
  {
    return this;
  }



    public abstract override BaseState NextState();

    public float FastEnd(float factor = 3) {
        float normalPercent = PercentComplete();
        return MathF.Pow(normalPercent, factor);
    }

    public float FastStart(float factor = 3) {
        float normalPercent = PercentComplete();
        return 1 - MathF.Pow(1 - normalPercent, factor);
    }

    protected static Vector2 Interpolate(Vector2 v1, Vector2 v2, float percent) {
        float x, y;
        x = v1.X * (1 - percent) + v2.X * percent;
        y = v1.Y * (1 - percent) + v2.Y * percent;
        return new Vector2(x, y);
    }

    protected static Color Interpolate(Color c1, Color c2, float percent) {
        return Color.Lerp(c1, c2, percent);
    }
}