using System;
using Microsoft.Xna.Framework;

namespace Devcade2048.App.State;

public abstract class TransientState : BaseState {
	/// <summary>
	/// How long a transition takes. 1 second.
	/// </summary>
    protected static readonly TimeSpan TransitionTime = TimeSpan.FromMilliseconds(1000);
	/// <summary>
	/// How long it takes for the tiles to move across the screen. 0.15 seconds.
	/// </summary>
    protected static readonly TimeSpan TileMoveTime = TimeSpan.FromMilliseconds(150);
	/// <summary>
	/// How long it takes for the tiles to move across the screen. 0.15 seconds.
	/// </summary>
    protected static readonly TimeSpan TileSpawnTime = TimeSpan.FromMilliseconds(150);
	/// <summary>
	/// How long game ending states take. 2 seconds.
	/// </summary>
    protected static readonly TimeSpan GameEndingTime = TimeSpan.FromMilliseconds(2000);

	/// <summary>
	/// Keeps track of how long this state has been running for.
	/// </summary>
    internal TimeSpan Timer = new();
	/// <summary>
	/// How long the state should run for until it changes. 
	/// </summary>
    internal TimeSpan MaxTime;

    public TransientState(TimeSpan maxTime) : base() {
        MaxTime = maxTime;
    }

    public override void Tick(GameTime gt) {
        base.Tick(gt);
        Timer += gt.ElapsedGameTime;
    }

    public override float PercentComplete() {
        return MathF.Min((float) (Timer / MaxTime), 1.0F);
    }

    public override bool IsComplete() {
        return PercentComplete() == 1.0;
    }

    public override BaseState ProcessInput() {
        return this;
    }



    public abstract override BaseState NextState();

	/// <summary>
	/// An animation parameter that starts slowly and accelerates near the end.
	/// </summary>
	/// <param name="factor">The acceleration exponent factor.</param>
    public float FastEnd(float factor = 3) {
        float normalPercent = PercentComplete();
        return MathF.Pow(normalPercent, factor);
    }

	/// <summary>
	/// An animation parameter that starts rapidly and slows down near the end.
	/// </summary>
	/// <param name="factor">The deceleration exponent factor.</param>
    public float FastStart(float factor = 3) {
        float normalPercent = PercentComplete();
        return 1 - MathF.Pow(1 - normalPercent, factor);
    }

	/// <summary>
	/// Interpolate linearly between two points. 
	/// </summary>
	/// <param name="v1">The first vector. Returned if fraction=0</param>
	/// <param name="v2">The second vector. Returned if fraction=1</param>
	/// <param name="fraction">The amount each vector contributes.</param>
    protected static Vector2 Interpolate(Vector2 v1, Vector2 v2, float fraction) {
        float x, y;
        x = v1.X * (1 - fraction) + v2.X * fraction;
        y = v1.Y * (1 - fraction) + v2.Y * fraction;
        return new Vector2(x, y);
    }
}