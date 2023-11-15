using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Devcade2048.App.Render;

public static class ScoreContainer {
    public static List<ScoreDelta> scores = new List<ScoreDelta>();

    public static void Add(ScoreDelta score) => scores.Add(score);
    public static void Add(int score) => Add(new ScoreDelta(score));

    public static void Increment(GameTime gt) {
        for (int i = scores.Count - 1; i >= 0; i--) {
            scores[i].Increment(gt);
        }
        _ = scores.RemoveAll(score => score.IsDone());
    }
}