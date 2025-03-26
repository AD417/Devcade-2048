using System;
using System.Threading;

using Devcade;

namespace Devcade2048.App;

public static class HighScoreTracker {

    public static int HighScore { get; private set; } = 0;

    public static async void Load() {
        Persistence.SetLocalPath("/tmp/");
        while (!Persistence.initalized) Thread.Sleep(1);
        try {
            HighScore = (await Persistence.Load<int>("2blob48", "highscore", null)).GetObject<int>(null);
        } catch (Exception e) {
            Console.Error.WriteLine("Whoops! Can't load the high score!");
            Console.Error.WriteLine(e);
            HighScore = 0;
        }
    }

    public static async void Save() {
        Persistence.SetLocalPath("/tmp/");
        var _ = await Persistence.Save<int>("2blob48", "highscore", HighScore, null);
        await Persistence.Flush();
    }

    public static void Update(int score) {
        if (score > HighScore) {
            HighScore = score;
        }
    }
}