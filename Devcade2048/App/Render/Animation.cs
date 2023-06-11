using System;
using Microsoft.Xna.Framework;

using Devcade2048.App.Tabs;

namespace Devcade2048.App.Render;

public static class Animation {
    internal static TimeSpan Timer;
    public static AnimationState State { get; private set; } = AnimationState.InitFadeIn;
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

    public static void BeginReset(Manager man) {
        if (!AcceptInput()) return;
        switch (man.State) {
            case GameState.Won:
                ChangeStateTo(AnimationState.ResetFromWin);
                break;
            case GameState.Lost:
                ChangeStateTo(AnimationState.ResetFromLost);
                break;
            case GameState.Playing:
            case GameState.Continuing:
                ChangeStateTo(AnimationState.ResetFromNormal);
                break;
            default:
            // Short circuit the reset animation that means nothing here.
            // TODO: change it to "Transition to the game". 
                throw new NotSupportedException("AYO WTF, WE CAN'T RESET FROM " + man.State);
            //     man.Setup();
            //     ChangeStateTo(AnimationState.Spawning);
            //     break;
        }
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

            case AnimationState.InitFadeIn:
            case AnimationState.ToTab:
            case AnimationState.FromTab:
            case AnimationState.ResetFromWin:
            case AnimationState.ContinueFromWin:
            case AnimationState.ResetFromLost:
            case AnimationState.ResetFromNormal:
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
            case AnimationState.InitFadeIn:
            case AnimationState.ContinueFromWin:
                State = AnimationState.WaitingForInput;
                break;
            case AnimationState.ResetFromWin:
            case AnimationState.ResetFromLost:
            case AnimationState.ResetFromNormal:
                State = AnimationState.Spawning;
                break;
            case AnimationState.FromTab:
                State = AnimationState.ToTab;
                break;
            case AnimationState.ToTab:
                if (TabHandler.CurrentTab.Id() == SelectedTab.Game) {
                    State = AnimationState.Spawning;
                } else {
                    State = AnimationState.WaitingForInput;
                }
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
            case AnimationState.InitFadeIn: 
            case AnimationState.ToTab:
            case AnimationState.FromTab:
            case AnimationState.ResetFromWin:
            case AnimationState.ContinueFromWin:
            case AnimationState.ResetFromLost:
            case AnimationState.ResetFromNormal:
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
            AnimationState.ResetFromWin,
            AnimationState.ToTab
        );
    }

    public static bool ResetGrid() {
        return (
            State == AnimationState.Spawning 
         && JustChanged 
         && StateWasAny(AnimationState.ResetFromWin, AnimationState.ResetFromLost, AnimationState.ResetFromNormal)
        );
    }

    public static bool SwitchTabs() {
        return (
            State == AnimationState.ToTab
         && JustChanged
         && LastState == AnimationState.FromTab
        );
    }

    public static bool ContinueGame() {
        return (
            State == AnimationState.WaitingForInput
         && JustChanged
         && LastState == AnimationState.ContinueFromWin
        );
    }

    public static double FastEnd(double factor = 3) {
        double normalPercent = PercentComplete();
        return Math.Pow(normalPercent, factor);
    }

    public static double FastStart(double factor = 3) {
        double normalPercent = PercentComplete();
        return 1 - Math.Pow(1 - normalPercent, factor);
    }
}