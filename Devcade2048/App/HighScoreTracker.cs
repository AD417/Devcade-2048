using Devcade;

namespace Devcade2048.App;

public static class HighScoreTracker {

    public static int HighScore { get; private set; } = 0;

    public static async void Load() {
        Persistence.SetLocalPath("/tmp/");
        HighScore = (await Persistence.Load<int>("2blob48", "highscore", null)).GetObject<int>(null);
    }

    public static async void Save() {
        _ = await Persistence.Save<int>("2blob48", "highscore", HighScore, null);
    }

    public static void Update(int score) {
        if (score > HighScore) {
            HighScore = score;
        }
    }
}