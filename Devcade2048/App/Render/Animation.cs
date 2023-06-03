using System;
using Microsoft.Xna.Framework;

namespace Devcade2048.App.Render;

// Yes, yes, there already exists this, but the other one is getting nuked soon. 
public static class Animation {
    internal static TimeSpan Timer;
    public static AnimationState State { get; private set; } = AnimationState.WaitingForInput;
    public static AnimationState LastState { get; private set; } = AnimationState.WaitingForInput;
    public static bool JustChanged { get; private set; } = false;

    private static readonly TimeSpan TransitionTime = TimeSpan.FromMilliseconds(1000);
    private static readonly TimeSpan MoveTime = TimeSpan.FromMilliseconds(150);
    private static readonly TimeSpan SpawnTime = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan EndTime = TimeSpan.FromMilliseconds(2000);

    // Internal updates to Timer.
    public static void ChangeStateTo(AnimationState newState) {
        LastState = State;
        State = newState;
        JustChanged = true;
        Timer = new TimeSpan();
    }

    public static void Increment(GameTime gt) {
        Timer += gt.ElapsedGameTime;
        JustChanged = false;
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
                CheckTransitionCompletion();
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
        LastState = State;
        JustChanged = true;
        if (State != AnimationState.ToWon || new Random().NextDouble() <= 0.95) {
            Timer = new TimeSpan();
            State = AnimationState.WaitingForInput;
        } else {
            State = AnimationState.EasterEgg;
        }
    }

    private static void CheckTransitionCompletion() {
        if (Timer < TransitionTime) return;
        Timer = new TimeSpan();
        LastState = State;
        JustChanged = true;
        switch (State) {
            case AnimationState.ToMenu:
            case AnimationState.ToInfo:
                State = AnimationState.WaitingForInput;
                break;
            case AnimationState.ToGame:
            case AnimationState.ResetFromWin:
            case AnimationState.ResetFromLost:
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
        LastState = State;
        JustChanged = true;
        Timer = new TimeSpan();
        State = AnimationState.Spawning;
    }

    private static void CheckSpawnCompletion() {
        if (Timer < SpawnTime) return;
        LastState = State;
        JustChanged = true;
        Timer = new TimeSpan(0);
        State = AnimationState.WaitingForInput;
        if (Display.manager.State == GameState.Lost) {
            State = AnimationState.ToLost;
        }
        if (Display.manager.State == GameState.Won) {
            State = AnimationState.ToWon;
        }
    }

    // General stats.
    public static bool StateIsAny(params AnimationState[] states) {
        foreach (AnimationState state in states) {
            if (state == State) return true;
        }
        return false;
    }
    
    public static bool StateWasAny(params AnimationState[] states) {
        foreach (AnimationState state in states) {
            if (state == LastState) return true;
        }
        return false;

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
                return Timer / TransitionTime;

            case AnimationState.Moving:
                return Timer / MoveTime;

            case AnimationState.Spawning:
                return Timer / SpawnTime;

            case AnimationState.ToWon:
            case AnimationState.ToLost:
            case AnimationState.EasterEgg:
                return Timer / EndTime;

            default:
                return 1.0;
        }
    }

    public static bool AcceptInput() {
        return StateIsAny(
            AnimationState.WaitingForInput,
            AnimationState.EasterEgg
        );
    }
    public static bool UpdatingGrid() {
        return StateIsAny(
            AnimationState.Moving,
            AnimationState.Spawning
        );
    }

    public static bool RenderingTiles() {
        return !StateIsAny(
            AnimationState.ResetFromLost,
            AnimationState.ResetFromWin
        );
    }

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
}