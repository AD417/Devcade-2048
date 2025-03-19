using System;
using Microsoft.Xna.Framework;

using Devcade2048.App.Tabs;

namespace Devcade2048.App.Render;

public static class Animation1 {
    internal static TimeSpan Timer;
    public static AnimationState1 State { get; private set; } = AnimationState1.InitFadeIn;
    public static AnimationState1 LastState { get; private set; } = AnimationState1.WaitingForInput;
    public static bool JustChanged { get; private set; } = false;

    private static readonly TimeSpan TransitionTime = TimeSpan.FromMilliseconds(1000);
    private static readonly TimeSpan MoveTime = TimeSpan.FromMilliseconds(150);
    private static readonly TimeSpan SpawnTime = TimeSpan.FromMilliseconds(100);
    private static readonly TimeSpan EndTime = TimeSpan.FromMilliseconds(2000);

    // Internal updates to Timer.
    public static void ChangeStateTo(AnimationState1 newState) {
        LastState = State;
        State = newState;
        JustChanged = true;
        Timer = new TimeSpan();
    }

    public static void BeginReset(Manager man) {
        if (!AcceptInput()) return;
        switch (man.State) {
            case GameState.Won:
                ChangeStateTo(AnimationState1.ResetFromWin);
                break;
            case GameState.Lost:
                ChangeStateTo(AnimationState1.ResetFromLost);
                break;
            case GameState.Playing:
            case GameState.Continuing:
                ChangeStateTo(AnimationState1.ResetFromNormal);
                break;
            default:
            // Short circuit the reset animation that means nothing here.
            // TODO: change it to "Transition to the game". 
                throw new NotSupportedException("AYO WTF, WE CAN'T RESET FROM " + man.State);
            //     man.Setup();
            //     ChangeStateTo(AnimationState1.Spawning);
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
            case AnimationState1.WaitingForInput: return;

            case AnimationState1.ToWon:
            case AnimationState1.ToLost:
                CheckEndCompletion();
                break;

            case AnimationState1.InitFadeIn:
            case AnimationState1.ToTab:
            case AnimationState1.FromTab:
            case AnimationState1.ResetFromWin:
            case AnimationState1.ContinueFromWin:
            case AnimationState1.ResetFromLost:
            case AnimationState1.ResetFromNormal:
                CheckTransitionCompletion();
                break;
            
            case AnimationState1.Moving: 
                CheckMoveCompletion();
                break;
            case AnimationState1.Spawning:
                CheckSpawnCompletion();
                break;
        }
    }

    private static void CheckEndCompletion() {
        if (Timer < EndTime) return;
        LastState = State;
        JustChanged = true;
        if (State != AnimationState1.ToWon || new Random().NextSingle() <= 0.9) {
            Timer = new TimeSpan();
            State = AnimationState1.WaitingForInput;
        } else {
            State = AnimationState1.EasterEgg;
        }
    }

    private static void CheckTransitionCompletion() {
        if (Timer < TransitionTime) return;
        Timer = new TimeSpan();
        LastState = State;
        JustChanged = true;
        switch (State) {
            case AnimationState1.InitFadeIn:
            case AnimationState1.ContinueFromWin:
                State = AnimationState1.WaitingForInput;
                break;
            case AnimationState1.ResetFromWin:
            case AnimationState1.ResetFromLost:
            case AnimationState1.ResetFromNormal:
                State = AnimationState1.Spawning;
                break;
            case AnimationState1.FromTab:
                State = AnimationState1.ToTab;
                break;
            case AnimationState1.ToTab:
                if (TabHandler.CurrentTab.Id() == SelectedTab.Game) {
                    State = AnimationState1.Spawning;
                } else {
                    State = AnimationState1.WaitingForInput;
                }
                break;
        }
    }

    private static void CheckMoveCompletion() {
        if (Timer < MoveTime) return;
        LastState = State;
        JustChanged = true;
        Timer = new TimeSpan();
        State = AnimationState1.Spawning;
    }

    private static void CheckSpawnCompletion() {
        if (Timer < SpawnTime) return;
        LastState = State;
        JustChanged = true;
        Timer = new TimeSpan(0);
        State = AnimationState1.WaitingForInput;
        if (Display.manager.State == GameState.Lost) {
            State = AnimationState1.ToLost;
        }
        if (Display.manager.State == GameState.Won) {
            State = AnimationState1.ToWon;
        }
    }

    // General stats.
    public static bool StateIsAny(params AnimationState1[] states) {
        foreach (AnimationState1 state in states) {
            if (state == State) return true;
        }
        return false;
    }
    
    public static bool StateWasAny(params AnimationState1[] states) {
        foreach (AnimationState1 state in states) {
            if (state == LastState) return true;
        }
        return false;

    }

    public static float PercentComplete() {
        switch (State) {
            case AnimationState1.InitFadeIn: 
            case AnimationState1.ToTab:
            case AnimationState1.FromTab:
            case AnimationState1.ResetFromWin:
            case AnimationState1.ContinueFromWin:
            case AnimationState1.ResetFromLost:
            case AnimationState1.ResetFromNormal:
                return (float) (Timer / TransitionTime);

            case AnimationState1.Moving:
                return (float) (Timer / MoveTime);

            case AnimationState1.Spawning:
                return (float) (Timer / SpawnTime);

            case AnimationState1.ToWon:
            case AnimationState1.ToLost:
            case AnimationState1.EasterEgg:
                return (float) (Timer / EndTime);

            default:
                return 1.0F;
        }
    }

    public static bool AcceptInput() {
        return StateIsAny(
            AnimationState1.WaitingForInput,
            AnimationState1.EasterEgg
        );
    }
    public static bool UpdatingGrid() {
        return StateIsAny(
            AnimationState1.Moving,
            AnimationState1.Spawning
        );
    }

    public static bool RenderingTiles() {
        return !StateIsAny(
            AnimationState1.ResetFromWin,
            AnimationState1.ToTab
        );
    }

    public static bool ResetGrid() {
        return 
            State == AnimationState1.Spawning 
         && JustChanged 
         && StateWasAny(AnimationState1.ResetFromWin, AnimationState1.ResetFromLost, AnimationState1.ResetFromNormal)
        ;
    }

    public static bool SwitchTabs() {
        return 
            State == AnimationState1.ToTab
         && JustChanged
         && LastState == AnimationState1.FromTab
        ;
    }

    public static bool ContinueGame() {
        return 
            State == AnimationState1.WaitingForInput
         && JustChanged
         && LastState == AnimationState1.ContinueFromWin
        ;
    }

    public static float FastEnd(float factor = 3) {
        float normalPercent = PercentComplete();
        return MathF.Pow(normalPercent, factor);
    }

    public static float FastStart(float factor = 3) {
        float normalPercent = PercentComplete();
        return 1 - MathF.Pow(1 - normalPercent, factor);
    }
}