using System;
using Microsoft.Xna.Framework;

namespace Devcade2048.App.Render;

// Yes, yes, there already exists this, but the other one is getting nuked soon. 
public static class Animation {
    internal static TimeSpan Timer;
    public static AnimationState State { get; private set; } = AnimationState.WaitingForInput;

    private static readonly TimeSpan FadeTime = TimeSpan.FromMilliseconds(500);
    private static readonly TimeSpan MoveTime = TimeSpan.FromMilliseconds(150);
    private static readonly TimeSpan SpawnTime = TimeSpan.FromMilliseconds(200);
    private static readonly TimeSpan EndTime = TimeSpan.FromMilliseconds(3000);

    public static void ChangeStateTo(AnimationState newState) {
        State = newState;
        Timer = new TimeSpan();
    }

    public static void Increment(GameTime gt) {
        Timer += gt.ElapsedGameTime;
        checkCompletion();
    }

    private static void checkCompletion() {
        switch (State) {
            case AnimationState.WaitingForInput: return;

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
            case AnimationState.ResetFromWin:
            case AnimationState.ResetFromLost:
            default: 
                State = AnimationState.WaitingForInput;
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
        // TODO: Handle losing or winning.
        State = AnimationState.WaitingForInput;
    }

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

            default:
                return 1.0;
        }
    }

    public static bool AcceptInput() {
        return State == AnimationState.WaitingForInput;
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
    public static Rectangle PositionOfTile(Tile t) {
        int scale = 96;
        Vector2 currentPos = ToScreenPosition(t.Position);
        Vector2 oldPos;
        if (t.PreviousPosition is null) {
            oldPos = currentPos;
        } else {
            oldPos = ToScreenPosition((Vector2)t.PreviousPosition);
        }

        if (State != AnimationState.Moving && State != AnimationState.Spawning) {
            return new Rectangle(
                (int)currentPos.X, (int)currentPos.Y, scale, scale
            );
        }

        double percent = PercentComplete();
        if (State == AnimationState.Moving) {
            currentPos = oldPos * (float)(1 - percent) + currentPos * (float)percent;
        }
        if (t.PreviousPosition == null) {
            if (State == AnimationState.Moving) scale = 0;
            else scale = (int) (scale * percent);
            currentPos += new Vector2(48 - scale/2, 48 - scale/2);
        }
        
        return new Rectangle(
            (int) currentPos.X,
            (int) currentPos.Y,
            scale,
            scale
        );
    }

}