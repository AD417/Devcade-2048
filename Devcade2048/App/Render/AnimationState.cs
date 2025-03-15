

namespace Devcade2048.App.Render;

public enum AnimationState1 {
    WaitingForInput,

    // Startup animations
    InitFadeIn,
    // InitToTile,


    ToTab,
    FromTab,

    // Game animations
    Spawning,
    Moving,

    // Endgame animations
    ToWon,
    EasterEgg,
    ToLost,
    ResetFromWin, 
    ContinueFromWin,
    ResetFromLost,
    ResetFromNormal,
}