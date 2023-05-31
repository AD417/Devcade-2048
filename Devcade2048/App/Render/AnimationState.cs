

namespace Devcade2048.App.Render;

public enum AnimationState {
    WaitingForInput,

    // Major animations between Game and Menu
    ToMenu,
    ToGame,
    FromMenu, // TODO: Check whether this jumps to Game or to Info. 
    FromGame,

    // Menu animations
    ToInfo,
    FromInfo,

    // Game animations
    Spawning,
    Moving,

    // Endgame animations
    ToWon,
    ToLost,
    ResetFromWin, 
    ResetFromLost,
}