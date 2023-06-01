using System;
using Microsoft.Xna.Framework;

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

    private double PercentComplete() {
        if (IsDone()) return 1;
        return existTime / cutoff;
    }

    public int ShiftUp() {
        return (int) (80 * PercentComplete());
    }

    private double Opacity() {
        if (IsDone()) return 0.0;
        double percent = 1.5 * (1 - PercentComplete());
        if (percent > 1) return 1.0;
        return percent;
    }

    public Color DrawColor() {
        double opacity = 1 - Opacity();
        int r = (int) (251 * opacity);
        int g = (int) (255 - 61 * opacity);
        int b = (int) (27 * opacity);
        return new Color(r, g, b);
    }

    public override string ToString() {
        return Score.ToString();
    }
}