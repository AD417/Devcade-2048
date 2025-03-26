using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.Render;

public class ScoreDelta {
    private static readonly TimeSpan cutoff = TimeSpan.FromMilliseconds(500);
    public int Score { get; }
    public TimeSpan existTime { get; private set; }

    public ScoreDelta(int score) {
        Score = score;
        existTime = new TimeSpan();
    }

    public void Increment(GameTime gt) {
        existTime += gt.ElapsedGameTime;
    }

    public bool IsDone() {
        return existTime > cutoff;
    }

    private float PercentComplete() {
        if (IsDone()) return 1;
        return (float) (existTime / cutoff);
    }

    public int ShiftUp() {
        return (int) (80 * PercentComplete());
    }

    private float Opacity() {
        if (IsDone()) return 0.0F;
        float percent = 3/2 * (1 - PercentComplete());
        if (percent > 1) return 1.0F;
        return percent;
    }

    public Color DrawColor() {
        return new Color(0, 191, 0) * Opacity();
    }

    public override string ToString() {
        return Score.ToString();
    }
}