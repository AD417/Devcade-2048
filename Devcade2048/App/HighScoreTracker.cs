

namespace Devcade2048.App;

public static class HighScoreTracker {

    public static int HighScore { get; private set; } = 0;

    public static void Load() {
        // TODO
    }

    public static void Save() {
        // TODO
    }

    public static void Update(int score) {
        if (score > HighScore) {
            HighScore = score;
        }
    }
}