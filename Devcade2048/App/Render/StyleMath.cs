using System;
using Microsoft.Xna.Framework;

namespace Devcade2048.App.Render;

public static class RenderMath {
    // Color management
    public static Color Interpolate(Color c1, Color c2, double percent) {
        int r, g, b;
        r = (int) (c1.R * (1 - percent) + c2.R * percent);
        g = (int) (c1.G * (1 - percent) + c2.G * percent);
        b = (int) (c1.B * (1 - percent) + c2.B * percent);
        return new Color(r,g,b);
    }

    public static Color GetBackgroundColor() {
        if (Animation.State != AnimationState.InitFadeIn) return new Color(251, 194, 27);
        double opacity = Animation.FastEnd(2);
        return RenderMath.Interpolate(new Color(0,0,0), new Color(251, 194, 27), opacity);
    }

    public static Color GetInitialBrightness() {
        if (Animation.State != AnimationState.InitFadeIn) return Color.White;
        return Interpolate(Color.Black, Color.White, Animation.FastEnd(2));
    }

    // Tile management
    private static Vector2 ToScreenPosition(Vector2 pos) {
        return new Vector2(
            (int) (12 + pos.X * 100),
            (int) (292 + pos.Y * 100)
        );
    }

    private static int TileScale(Tile t) {
        if (Animation.StateIsAny(
            AnimationState.ResetFromLost, 
            AnimationState.ResetFromNormal
        )) {
            return ResetTileScale();
        }
        if (t.PreviousPosition != null) return 96;

        if (Animation.State == AnimationState.Moving) return 0;
        if (Animation.State != AnimationState.Spawning) return 96;

        double scaleFactor = Animation.PercentComplete();


        if (t.MergedFrom is null) return (int) (96 * scaleFactor);

        scaleFactor *= 2;
        if (scaleFactor > 1) scaleFactor = (2 - scaleFactor) * 0.25 + 1;
        return (int) (96 * scaleFactor);
    }

    private static int ResetTileScale() {
        double scaleFactor = 1 - Animation.PercentComplete();
        return (int) (96 * scaleFactor * scaleFactor);
    }

    private static Vector2 TilePosition(Tile t) {
        Vector2 currentPos = ToScreenPosition(t.Position);
        
        if (Animation.State != AnimationState.Moving) return currentPos;
        if (t.PreviousPosition is null) return currentPos;

        Vector2 oldPos = ToScreenPosition((Vector2)t.PreviousPosition);
        float percent = (float) Animation.PercentComplete();

        return oldPos * (1 - percent) + currentPos * percent;
    }

    public static Rectangle PositionOfTile(Tile t) {
        Vector2 currentPos = TilePosition(t);
        int scale = TileScale(t);
        currentPos += new Vector2(48 - scale/2, 48 - scale/2);

        return new Rectangle(
            (int) currentPos.X,
            (int) currentPos.Y,
            scale,
            scale
        );
    }


    // Win and Loss States
    private static double WinScale() {
        if (!Animation.StateIsAny(
            AnimationState.ToWon, 
            AnimationState.EasterEgg, 
            AnimationState.ResetFromWin,
            AnimationState.ContinueFromWin
        )) {
            return 1.0;
        }
        double percent = Animation.PercentComplete();
        if (Animation.State == AnimationState.ResetFromWin) {
            percent = 1 - percent;
            return Math.Pow(percent, 3);
        }
        if (Animation.State == AnimationState.ContinueFromWin) {
            return 1 - Math.Pow(percent, 3);
        }
        return Math.Pow(percent, 3);
    }

    public static Rectangle PositionOfWinTile(Tile t) {
        int scale = (int) (96 + 304 * WinScale());
        return new Rectangle(
            (int) (12 + t.Position.X * 100 * (1 - WinScale())),
            (int) (292 + t.Position.Y * 100 * (1 - WinScale())),
            scale,
            scale
        );
    }
    
    public static Rectangle PositionOfWinTile() {
        int scale = (int) (400 * WinScale());
        return new Rectangle(212 - scale / 2, 492 - scale / 2, scale, scale);
    }

    public static int BiggestLossTile() {
        if (Animation.StateIsAny(AnimationState.WaitingForInput, AnimationState.ResetFromLost)) return 12;
        if (Animation.State != AnimationState.ToLost) return -1;

        return (int) ((Animation.PercentComplete() - 0.5) * 20);
    }
}