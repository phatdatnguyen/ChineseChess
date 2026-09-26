using System.Diagnostics;
using NUnit.Framework;

namespace ChineseChess.Tests;

[TestFixture]
[Category("Benchmark")]
[Explicit("Optional timing measurement; select the Benchmark category to run.")]
public sealed class BenchmarkTests
{
    [TestCase(Game.AIDifficulty.Easy)]
    [TestCase(Game.AIDifficulty.Medium)]
    [TestCase(Game.AIDifficulty.Hard)]
    [TestCase(Game.AIDifficulty.Extreme)]
    public void AIOpening_ReportsElapsedTime(Game.AIDifficulty difficulty)
    {
        using var context = new GameTestContext();
        var game = context.CreateGame(Game.GameType.VsAI, aiFirst: true, difficulty);
        var timer = Stopwatch.StartNew();
        game.Start();
        timer.Stop();

        Assert.That(game.Moves, Has.Count.EqualTo(1));
        Assert.That(game.CurrentPlayer, Is.SameAs(game.Player2));
        TestContext.Out.WriteLine($"{difficulty} AI opening: {timer.Elapsed.TotalMilliseconds:N0} ms");
    }
}
