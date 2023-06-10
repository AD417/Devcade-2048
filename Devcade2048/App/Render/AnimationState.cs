

namespace Devcade2048.App.Render;

public enum AnimationState {
    WaitingForInput,

    // Startup animations
    InitFadeIn,
    // InitToTile,


    ToTab,
    FromTab,
    /*
    // Major animations between Game and Menu
    ToMenu,
    ToGame,
    FromMenu, // TODO: Check whether this jumps to Game or to Info. 
    FromGame,

    // Menu animations
    ToInfo,
    FromInfo,
    */

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