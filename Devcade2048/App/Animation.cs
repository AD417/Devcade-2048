using Microsoft.Xna.Framework;
using System;

namespace Devcade2048.App; 

public static class Animation {
    public static TimeSpan Timer { get; set; } = new TimeSpan();
    private static readonly TimeSpan TranslationCutoff = TimeSpan.FromMilliseconds(150);
    private static readonly TimeSpan ScalingCutoff = TimeSpan.FromMilliseconds(250);
    private static readonly TimeSpan ScoreFadingCutoff = TimeSpan.FromMilliseconds(500);
    private static readonly TimeSpan WinAnimationCutoff = TimeSpan.FromSeconds(2);

    public static void Increment(GameTime gameTime) {
        Timer += gameTime.ElapsedGameTime;
    }

    public static void Reset() {
        Timer = new TimeSpan();
    }

    public static bool IsComplete() {
        return Timer > ScalingCutoff;
    }

    public static double TranslationFactor() {
        if (Timer > TranslationCutoff) return 1;
        return Timer / TranslationCutoff;
    }

    public static Vector2 InterpolatePosition(Vector2 a, Vector2 b) {
        double percent = TranslationFactor();
        return new Vector2(
            (float) (a.X * (1 - percent) + b.X * percent),
            (float) (a.Y * (1 - percent) + b.Y * percent)
        );
    }

    public static int NewTileScale() {
        if (Timer < TranslationCutoff) return 0;
        if (IsComplete()) return 96;
        TimeSpan scaleTime = Timer - TranslationCutoff;
        TimeSpan fiftyMs = TimeSpan.FromMilliseconds(50);

        double scale = 1.0;

        if (scaleTime > fiftyMs) {
            scale = 1 + 0.2 *  (1 - scaleTime / fiftyMs);
        } else {
            scale = 1.2 * (scaleTime / fiftyMs);
        }
        return (int) (scale * 96);
    }

    public static bool IsScoreVisible() {
        return Timer <= ScoreFadingCutoff;
    }

    public static int ScoreDisplacement() {
        if (!IsScoreVisible()) return 80;
        return (int) (80 * Timer / ScoreFadingCutoff);
    }

    public static Color InterpolateColor(Color c1, Color c2) {
        if (!IsComplete()) return c1;
        if (!IsScoreVisible()) return c2;

        TimeSpan fadeTime = Timer - ScalingCutoff;
        float percent = (float)(fadeTime / TimeSpan.FromMilliseconds(250));

        int r, g, b;
        r = (int) (c1.R * (1 - percent) + c2.R * percent);
        g = (int) (c1.G * (1 - percent) + c2.G * percent);
        b = (int) (c1.B * (1 - percent) + c2.B * percent);
        return new Color(r,g,b);
    }

    public static bool IsWinComplete() {
        return Timer > WinAnimationCutoff;
    }

    public static float InterpolateWin() {
        if (!IsComplete()) return 0.0f;
        if (IsWinComplete()) return 1.0f;
        return (float) ((Timer - ScalingCutoff) / TimeSpan.FromSeconds(1.75));
    }

    public static int WinScale() {
        if (!IsComplete()) return 0;
        return 96 + (int)(304 * InterpolateWin());
    }

    public static Vector2 WinPosition(Vector2 pos) {
        return new Vector2(
            12 + pos.X * 100 * (1 - InterpolateWin()),
            292 + pos.Y * 100 * (1 - InterpolateWin())
        );
    }
}