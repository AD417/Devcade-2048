using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.Render;

public static class ScoreContainer {
    public static List<ScoreDelta> Scores { get; private set; } = new List<ScoreDelta>();

    public static void Add(ScoreDelta score) => Scores.Add(score);
    public static void Add(int score) => Add(new ScoreDelta(score));

    public static void Increment(GameTime gt) {
        for (int i = Scores.Count - 1; i >= 0; i--) {
            Scores[i].Increment(gt);
        }
        _ = Scores.RemoveAll(score => score.IsDone());
    }

    public static IEnumerator<ScoreDelta> GetEnumerator()
    {
        foreach (ScoreDelta score in Scores) {
            yield return score;
        }
    }
}