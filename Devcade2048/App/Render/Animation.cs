using System;
using Microsoft.Xna.Framework;

namespace Devcade2048.App.Render;

// Yes, yes, there already exists this, but the other one is getting nuked soon. 
public static class Animation {
    internal static TimeSpan Timer;
    public static AnimationState State { get; private set; } = AnimationState.WaitingForInput;

    private static readonly TimeSpan FadeTime = TimeSpan.FromMilliseconds(500);
    private static readonly TimeSpan MoveTime = TimeSpan.FromMilliseconds(150);
    private static readonly TimeSpan SpawnTime = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan EndTime = TimeSpan.FromMilliseconds(3000);

    // Internal updates to Timer.
    public static void ChangeStateTo(AnimationState newState) {
        State = newState;
        Timer = new TimeSpan();
    }

    public static void Increment(GameTime gt) {
        Timer += gt.ElapsedGameTime;
        CheckCompletion();
    }

    private static void CheckCompletion() {
        switch (State) {
            case AnimationState.WaitingForInput: return;

            case AnimationState.ToWon:
            case AnimationState.ToLost:
                CheckEndCompletion();
                break;

            case AnimationState.ToMenu:
            case AnimationState.ToGame:
            case AnimationState.ToInfo:
            case AnimationState.FromMenu:
            case AnimationState.FromGame:
            case AnimationState.FromInfo:
            case AnimationState.ResetFromWin:
            case AnimationState.ResetFromLost:
                CheckFadeCompletion();
                break;
            
            case AnimationState.Moving: 
                CheckMoveCompletion();
                break;
            case AnimationState.Spawning:
                CheckSpawnCompletion();
                break;
        }
    }

    private static void CheckEndCompletion() {
        if (Timer < EndTime) return;
        Timer = new TimeSpan();
        switch (State) {
            case AnimationState.ToWon:
            case AnimationState.ToLost:
            default: 
                State = AnimationState.WaitingForInput;
                break;
        }
    }

    private static void CheckFadeCompletion() {
        if (Timer < FadeTime) return;
        Timer = new TimeSpan();
        switch (State) {
            case AnimationState.ToMenu:
            case AnimationState.ToInfo:
                State = AnimationState.WaitingForInput;
                break;
            case AnimationState.ToGame:
                State = AnimationState.Spawning;
                break;
            case AnimationState.FromMenu:
                // TODO: handle going to Info.
                State = AnimationState.ToGame;
                break;
            case AnimationState.FromGame:
            case AnimationState.FromInfo:
                State = AnimationState.ToMenu;
                break;
        }
    }

    private static void CheckMoveCompletion() {
        if (Timer < MoveTime) return;
        Timer = new TimeSpan();
        State = AnimationState.Spawning;
    }

    private static void CheckSpawnCompletion() {
        if (Timer < SpawnTime) return;
        Timer = new TimeSpan(0);
        State = AnimationState.WaitingForInput;
        if (Display.manager.State == GameState.Lost) {
            State = AnimationState.ToLost;
        }
        if (Display.manager.State == GameState.Won) {
            State = AnimationState.ToWon;
        }
    }

    // Basic stats for 
    public static double PercentComplete() {
        switch (State) {
            case AnimationState.ToMenu:
            case AnimationState.ToGame:
            case AnimationState.ToInfo:
            case AnimationState.FromMenu:
            case AnimationState.FromGame:
            case AnimationState.FromInfo:
            case AnimationState.ResetFromWin:
            case AnimationState.ResetFromLost:
                return Timer / FadeTime;

            case AnimationState.Moving:
                return Timer / MoveTime;

            case AnimationState.Spawning:
                return Timer / SpawnTime;

            case AnimationState.ToWon:
            case AnimationState.ToLost:
                return Timer / EndTime;

            default:
                return 1.0;
        }
    }

    public static bool AcceptInput() {
        return State == AnimationState.WaitingForInput;
    }
    public static bool UpdatingGrid() {
        return (
            State == AnimationState.Moving 
         || State == AnimationState.Spawning
        );
    }

    // Fading animation parameters:
    public static double Opacity() {
        double percent = PercentComplete();
        switch (State) {
            case AnimationState.ToMenu:
            case AnimationState.ToGame:
            case AnimationState.ToInfo:
                return percent;
            case AnimationState.FromGame:
            case AnimationState.FromMenu:
            case AnimationState.FromInfo:
                return 1.0 - percent;
            default:
                return 1.0;
        }
    }

    public static Color TextColorFade(Color c) {
        int r = 251, g = 194, b = 27;
        double opacity = Opacity();
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
        if (t.PreviousPosition != null) return 96;
        if (State == AnimationState.Moving) return 0;
        if (State != AnimationState.Spawning) return 96;

        double scaleFactor = PercentComplete();
        if (t.MergedFrom is null) return (int) (96 * scaleFactor);

        scaleFactor *= 2;
        if (scaleFactor > 1) scaleFactor = (2 - scaleFactor) * 0.25 + 1;
        return (int) (96 * scaleFactor);
    }

    private static Vector2 TilePosition(Tile t) {
        Vector2 currentPos = ToScreenPosition(t.Position);
        
        if (State != AnimationState.Moving) return currentPos;
        if (t.PreviousPosition is null) return currentPos;

        Vector2 oldPos = ToScreenPosition((Vector2)t.PreviousPosition);
        float percent = (float) PercentComplete();

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
        if (State != AnimationState.ToWon) return 1.0;
        return PercentComplete() * PercentComplete() * PercentComplete();
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

    public static int BiggestLossTile() {
        if (State == AnimationState.WaitingForInput) return 12;
        if (State != AnimationState.ToLost) return -1;

        return (int) ((PercentComplete() - 0.5) * 20);
    }

}