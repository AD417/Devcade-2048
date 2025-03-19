using System;
using Microsoft.Xna.Framework;

using Devcade2048.App.Tabs;

namespace Devcade2048.App.Render;

public static class StyleMath {
    public static Color Background = new Color(251, 194, 27);

    // Color management
    public static Color Interpolate(Color c1, Color c2, float percent) {
        int r, g, b;
        r = (int) (c1.R * (1 - percent) + c2.R * percent);
        g = (int) (c1.G * (1 - percent) + c2.G * percent);
        b = (int) (c1.B * (1 - percent) + c2.B * percent);
        return new Color(r,g,b);
    }

    public static Color GetBackgroundColor() {
        if (Animation1.State != AnimationState1.InitFadeIn) return Background;
        float opacity = Animation1.FastEnd(2);
        return Interpolate(new Color(0,0,0), Background, opacity);
    }

    public static Color GetInitialBrightness() {
        if (Animation1.State != AnimationState1.InitFadeIn) return Color.White;
        return Interpolate(Color.Black, Color.White, Animation1.FastEnd(2));
    }

    public static Color GetScoreColor() {
        if (Animation1.State == AnimationState1.ToTab) {
            return Interpolate(Background, Color.Black, Animation1.FastEnd(2));
        }
        if (Animation1.State == AnimationState1.FromTab) {
            return Interpolate(Color.Black, Background, Animation1.FastStart(2));
        }
        return Color.Black;
    }

    // Tile management
    public static Vector2 GridDisplacement() {
        if (Animation1.State == AnimationState1.ToTab) {
            return new Vector2(0, 710 * (float)(1 - Animation1.FastStart()));
        }
        if (Animation1.State == AnimationState1.FromTab) {
            return new Vector2(0, (float)(710 * Animation1.FastEnd()));
        }
        return new Vector2();
    }

    private static Vector2 ToScreenPosition(Vector2 pos) {
        return new Vector2(
            (int) (12 + pos.X * 100),
            (int) (292 + pos.Y * 100)
        ) + GridDisplacement();
    }


    public static Vector2 Interpolate(Vector2 v1, Vector2 v2, float percent) {
        float x, y;
        x = v1.X * (1 - percent) + v2.X * percent;
        y = v1.Y * (1 - percent) + v2.Y * percent;
        return new Vector2(x, y);
    }

    private static int TileScale(Tile t) {
        if (Animation1.StateIsAny(
            AnimationState1.ResetFromLost, 
            AnimationState1.ResetFromNormal
        )) {
            return ResetTileScale();
        }
        if (t.PreviousPosition != null) return 96;

        if (Animation1.State == AnimationState1.Moving) return 0;
        if (Animation1.State != AnimationState1.Spawning) return 96;

        float scaleFactor = Animation1.PercentComplete();


        if (t.MergedFrom is null) return (int) (96 * scaleFactor);

        scaleFactor *= 2;
        if (scaleFactor > 1) scaleFactor = (2 - scaleFactor) * 1/4 + 1;
        return (int) (96 * scaleFactor);
    }

    private static int ResetTileScale() {
        float scaleFactor = 1 - Animation1.PercentComplete();
        return (int) (96 * scaleFactor * scaleFactor);
    }

    private static Vector2 TilePosition(Tile t) {
        Vector2 currentPos = ToScreenPosition(t.Position);
        
        if (Animation1.State != AnimationState1.Moving) return currentPos;
        if (t.PreviousPosition is null) return currentPos;

        Vector2 oldPos = ToScreenPosition((Vector2)t.PreviousPosition);
        float percent = Animation1.PercentComplete();

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
    private static float WinScale() {
        if (!Animation1.StateIsAny(
            AnimationState1.ToWon, 
            AnimationState1.EasterEgg, 
            AnimationState1.ResetFromWin,
            AnimationState1.ContinueFromWin
        )) {
            return 1.0F;
        }
        float percent = Animation1.PercentComplete();
        if (Animation1.State == AnimationState1.ResetFromWin) {
            percent = 1 - percent;
            return MathF.Pow(percent, 3);
        }
        if (Animation1.State == AnimationState1.ContinueFromWin) {
            return 1 - MathF.Pow(percent, 3);
        }
        return MathF.Pow(percent, 3);
    }

    public static Rectangle PositionOfWinTile(Tile t) {
        int scale = (int) (96 + 304 * WinScale());
        Vector2 pos = t.Position;
        pos = pos * 100 * (float)(1 - WinScale());
        pos += new Vector2(12, 292) + GridDisplacement();

        return new Rectangle(
            (int) pos.X,
            (int) pos.Y,
            scale,
            scale
        );
    }
    
    public static Rectangle PositionOfWinTile() {
        int scale = (int) (400 * WinScale());
        return new Rectangle(212 - scale / 2, 492 - scale / 2, scale, scale);
    }

    public static int BiggestLossTile() {
        if (Animation1.StateIsAny(
            AnimationState1.WaitingForInput, 
            AnimationState1.ResetFromLost, 
            AnimationState1.FromTab
        )) return 16;
        if (Animation1.State != AnimationState1.ToLost) return -1;

        return (int) ((Animation1.PercentComplete() - 0.5) * 20);
    }


    // Menu Blobs
    public static Vector2 MenuBlobPosition(Vector2 menuPos, Vector2 offScreen, SelectedTab tab) {
        if (Animation1.State == AnimationState1.InitFadeIn) return menuPos;


        Vector2 top = new Vector2(114, 130);
        if (Animation1.State == AnimationState1.WaitingForInput) {
            if (TabHandler.CurrentTab.Id() == tab) return top;
            if (TabHandler.CurrentTab.Id() == SelectedTab.Menu) return menuPos;
            return offScreen;
        }
        if (Animation1.State == AnimationState1.ToTab) {
            if (TabHandler.CurrentTab.Id() == tab && TabHandler.LastTab.Id() == SelectedTab.Menu) {
                return top;
            }
            if (TabHandler.CurrentTab.Id() == SelectedTab.Menu && TabHandler.LastTab.Id() == tab) {
                return Interpolate(top, menuPos, Animation1.FastStart(2));
            }
            if (TabHandler.CurrentTab.Id() == SelectedTab.Menu) {
                return Interpolate(offScreen, menuPos, Animation1.FastStart(2));
            }
            return offScreen;
        }
        if (Animation1.State == AnimationState1.FromTab) {
            if (TabHandler.CurrentTab.Id() == tab && TabHandler.NextTab.Id() == SelectedTab.Menu) {
                return top;
            }
            if (TabHandler.CurrentTab.Id() == SelectedTab.Menu && TabHandler.NextTab.Id() == tab) {
                return Interpolate(menuPos, top, Animation1.FastEnd(2));
            }
            if (TabHandler.CurrentTab.Id() == SelectedTab.Menu) {
                return Interpolate(menuPos, offScreen, Animation1.FastEnd(2));
            }
            return offScreen;
        }

        return offScreen;
    }

}