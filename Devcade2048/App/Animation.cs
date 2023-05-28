using Microsoft.Xna.Framework;
using System;

namespace Devcade2048.App; 

public static class Animation {
    public static TimeSpan Timer { get; set; } = new TimeSpan();
    private static TimeSpan TranslationCutoff = new TimeSpan(1_500_000);

    public static void Increment(GameTime gameTime) {
        Timer += gameTime.ElapsedGameTime;
    }

    public static void Reset() {
        Timer = new TimeSpan();
    }

    public static bool IsComplete() {
        return Timer > new TimeSpan(2_500_000);
    }

    public static double TranslationFactor() {
        if (Timer > TranslationCutoff) return 1;
        return Timer / TranslationCutoff;
    }

    public static Vector2 Interpolate(Vector2 a, Vector2 b) {
        double percent = TranslationFactor();
        return new Vector2(
            (float) (a.X * (1 - percent) + b.X * percent),
            (float) (a.Y * (1 - percent) + b.Y * percent)
        );
    }

    public static int Scale() {
        if (Timer < TranslationCutoff) return 0;
        if (IsComplete()) return 96;
        TimeSpan scaleTime = Timer - TranslationCutoff;
        TimeSpan fiftyMs = new TimeSpan(500_000);

        double scale = 1.0;

        if (scaleTime > fiftyMs) {
            scale = 1 + 0.2 *  (1 - scaleTime / fiftyMs);
        } else {
            scale = 1.2 * (scaleTime / fiftyMs);
        }
        return (int) (scale * 96);
    }
}