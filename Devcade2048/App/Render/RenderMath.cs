using System;
using Microsoft.Xna.Framework;

namespace Devcade2048.App.Render;

public static class RenderMath {
        public static Color TextColorFade(Color c) {
        int r = 251, g = 194, b = 27;
        double opacity = Animation.Opacity();
        r = (int) (c.R * (1 - opacity) + r * opacity);
        g = (int) (c.G * (1 - opacity) + g * opacity);
        b = (int) (c.B * (1 - opacity) + b * opacity);
        return new Color(r, g, b);
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
            AnimationState.ResetFromWin
        )) {
            return 1.0;
        }
        if (Animation.State != AnimationState.ResetFromWin) {
            return Math.Pow(Animation.PercentComplete(), 3);
        }
        double percent = 1 - Animation.PercentComplete();
        return percent * percent * percent;
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