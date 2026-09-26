using NUnit.Framework;
using Side = ChineseChess.Board.Side;
using static ChineseChess.Tests.TestSupport;

namespace ChineseChess.Tests;

[TestFixture]
public sealed class BoardStateTests
{
    [Test]
    public void Reset_RestoresTheCompleteOpeningAndNeutralEvaluation()
    {
        using var context = new GameTestContext();
        context.Board.Reset();
        var pieces = context.Board.Cells.Cast<Cell>().Where(cell => cell.Piece != null).Select(cell => cell.Piece!).ToArray();
        string[] backRank = ["Chariot", "Horse", "Elephant", "Advisor", "General", "Advisor", "Elephant", "Horse", "Chariot"];

        Assert.That(pieces, Has.Length.EqualTo(32));
        foreach (Side side in Enum.GetValues<Side>())
        {
            int row = side == Side.Blue ? 0 : 9;
            Assert.That(pieces.Count(piece => piece.Side == side), Is.EqualTo(16));
            for (int column = 0; column < 9; column++)
            {
                Assert.That(context.Board.Cells[row, column].Piece!.Name, Is.EqualTo(backRank[column]));
                Assert.That(context.Board.Cells[row, column].Piece!.Side, Is.EqualTo(side));
            }
            Assert.That(context.Board.Cells[side == Side.Blue ? 2 : 7, 1].Piece, Is.TypeOf<Cannon>());
            Assert.That(context.Board.Cells[side == Side.Blue ? 2 : 7, 7].Piece, Is.TypeOf<Cannon>());
            for (int column = 0; column < 9; column += 2)
                Assert.That(context.Board.Cells[side == Side.Blue ? 3 : 6, column].Piece, Is.TypeOf<Soldier>());
            Assert.That(context.Board.Evaluate(side), Is.Zero);
        }
        Assert.That(pieces.All(piece => !piece.IsCaptured && !piece.IsSelected), Is.True);
        Assert.That(context.Board.SelectedCell, Is.Null);
        Assert.That(context.Panel.Controls.Count, Is.EqualTo(90 + 32));
    }

    [TestCase(Side.Red)]
    [TestCase(Side.Blue)]
    public void LegalMoveGenerationAndSimulation_PreservePieceIdentityAndState(Side side)
    {
        using var context = new GameTestContext();
        context.Board.Reset();
        var board = context.Board;
        string before = Snapshot(board);
        var originalPieces = board.Cells.Cast<Cell>().Select(cell => cell.Piece).ToArray();
        var moves = board.FindPossibleMoves(side);

        Assert.That(moves, Is.Not.Empty);
        Assert.That(board.HasLegalMoves(side), Is.True);
        Assert.That(Snapshot(board), Is.EqualTo(before));
        foreach (var move in moves)
        {
            var originalLocation = move.Piece.Image.Location;
            board.DoMove(move, isTestMove: true);
            try
            {
                Assert.That(board.Cells[move.StartRow, move.StartColumn].Piece, Is.Null);
                Assert.That(board.Cells[move.EndRow, move.EndColumn].Piece, Is.SameAs(move.Piece));
                Assert.That(board.IsCheckDelivered(Opponent(side)), Is.False);
                Assert.That(move.Piece.Image.Location, Is.EqualTo(originalLocation), "Search must not move controls.");
                if (move.CapturedPiece != null)
                {
                    Assert.That(move.CapturedPiece.IsCaptured, Is.True);
                    Assert.That(context.Panel.Controls.Contains(move.CapturedPiece.Image), Is.True,
                        "Search must not detach controls.");
                }
            }
            finally
            {
                board.UndoMove(move, isTestMove: true);
            }
            Assert.That(Snapshot(board), Is.EqualTo(before));
            Assert.That(board.Cells.Cast<Cell>().Select(cell => cell.Piece), Is.EqualTo(originalPieces));
            Assert.That(move.CapturedPiece?.IsCaptured ?? false, Is.False);
        }
    }

    [TestCase(Side.Red, false)]
    [TestCase(Side.Red, true)]
    [TestCase(Side.Blue, false)]
    [TestCase(Side.Blue, true)]
    public void SoldierCrossingAndUndo_RestoreMaterialAndCaptureState(Side side, bool capture)
    {
        using var context = new GameTestContext();
        int start = side == Side.Red ? 5 : 4;
        int end = side == Side.Red ? 4 : 5;
        var soldier = context.Add("Soldier", side, start, 0);
        var victim = capture ? context.Add("Horse", Opponent(side), end, 0) : null;
        var move = new Move(start, 0, end, 0, soldier, victim);
        int originalEvaluation = context.Board.Evaluate(side);
        string before = Snapshot(context.Board);

        context.Board.DoMove(move, true);

        Assert.That(soldier.RelativeValue, Is.EqualTo(20));
        Assert.That(context.Board.Evaluate(side), Is.EqualTo(originalEvaluation + 10 + (victim?.RelativeValue ?? 0)));
        Assert.That(context.Board.Evaluate(Opponent(side)), Is.EqualTo(-context.Board.Evaluate(side)));
        if (victim != null) Assert.That(victim.IsCaptured, Is.True);

        context.Board.UndoMove(move, true);

        Assert.That(Snapshot(context.Board), Is.EqualTo(before));
        Assert.That(context.Board.Cells[start, 0].Piece, Is.SameAs(soldier));
        Assert.That(context.Board.Cells[end, 0].Piece, Is.SameAs(victim));
        if (victim != null) Assert.That(victim.IsCaptured, Is.False);
    }

    [Test]
    public void CaptureAndUndo_RestoreControlsAndClearSelection()
    {
        using var context = new GameTestContext();
        var game = context.CreateGame();
        game.Start();
        context.Clear();
        context.Add("General", Side.Red, 9, 3);
        context.Add("General", Side.Blue, 0, 5);
        var mover = context.Add("Chariot", Side.Red, 5, 0);
        var victim = context.Add("Horse", Side.Blue, 5, 3);
        var move = new Move(5, 0, 5, 3, mover, victim);
        var beforeLocation = mover.Image.Location;
        mover.IsSelected = true;
        context.Board.SelectedCell = context.Board.Cells[5, 0];

        context.Board.DoMove(move);

        Assert.That(victim.IsCaptured, Is.True);
        Assert.That(victim.Image.Parent, Is.Null);
        Assert.That(victim.Image.IsDisposed, Is.False, "Captured pieces remain available for undo.");
        Assert.That(mover.Image.Location, Is.EqualTo(new Point(3 * Board.HorizontalCellDistance + Board.PaddingLeft,
            5 * Board.VerticalCellDistance + Board.PaddingTop)));
        Assert.That(mover.IsSelected, Is.False);
        Assert.That(context.Board.IsSelected, Is.False);

        context.Board.UndoMove(move);

        Assert.That(mover.Image.Location, Is.EqualTo(beforeLocation));
        Assert.That(victim.Image.Parent, Is.SameAs(context.Panel));
        Assert.That(victim.IsCaptured, Is.False);
        Assert.That(context.Board.Cells[5, 0].Piece, Is.SameAs(mover));
        Assert.That(context.Board.Cells[5, 3].Piece, Is.SameAs(victim));
    }

    [Test]
    public void UndoMove_RecomputesCheckForTheRestoredPosition()
    {
        using var context = new GameTestContext();
        context.Add("General", Side.Red, 9, 4);
        context.Add("General", Side.Blue, 0, 3);
        context.Add("Chariot", Side.Blue, 4, 4);
        var defender = context.Add("Chariot", Side.Red, 7, 0);
        var block = new Move(7, 0, 7, 4, defender);
        Assert.That(context.Board.IsCheckDelivered(Side.Blue), Is.True);

        context.Board.DoMove(block);
        context.Board.UndoMove(block);

        Assert.That(context.Form.StatusLabel.Text, Is.EqualTo("Check!"));
    }

    [TestCase(17)]
    [TestCase(2026)]
    public void LegalPlayout_AttackDetectionAgreesWithPieceRules_AndAllMovesCanBeUndone(int seed)
    {
        using var context = new GameTestContext();
        var board = context.Board;
        board.Reset();
        string initial = Snapshot(board);
        var initialPieces = board.Cells.Cast<Cell>().Select(cell => cell.Piece).ToArray();
        var random = new Random(seed);
        var history = new List<Move>();
        Side side = Side.Red;
        for (int ply = 0; ply < 60; ply++)
        {
            foreach (Side attacker in Enum.GetValues<Side>())
            {
                bool expectedCheck = board.Cells.Cast<Cell>()
                    .Where(cell => cell.Piece?.Side == attacker)
                    .SelectMany(cell => cell.Piece!.FindPossibleMoves())
                    .Any(move => move.CapturedPiece is General);
                Assert.That(board.IsCheckDelivered(attacker), Is.EqualTo(expectedCheck), $"Seed {seed}, ply {ply}, {attacker}");
            }
            var moves = board.FindPossibleMoves(side);
            if (moves.Count == 0) break;
            var move = moves[random.Next(moves.Count)];
            board.DoMove(move, true);
            history.Add(move);
            Assert.That(board.IsCheckDelivered(Opponent(side)), Is.False, $"Seed {seed}, ply {ply}");
            side = Opponent(side);
        }
        Assert.That(history, Is.Not.Empty);
        for (int index = history.Count - 1; index >= 0; index--)
            board.UndoMove(history[index], true);
        Assert.That(Snapshot(board), Is.EqualTo(initial));
        Assert.That(board.Cells.Cast<Cell>().Select(cell => cell.Piece), Is.EqualTo(initialPieces));
    }

    [Test]
    public void Reset_DisposesPreviousControlsIncludingCapturedPieces()
    {
        using var context = new GameTestContext();
        context.Board.Reset();
        var oldControls = context.Panel.Controls.Cast<Control>().ToArray();
        var detached = context.Board.Cells[3, 0].Piece!;
        context.Board.RemovePiece(detached);
        context.Board.Cells[3, 0].Piece = null;
        Assert.That(detached.Image.IsDisposed, Is.False);

        context.Board.Reset();

        Assert.That(oldControls.All(control => control.IsDisposed), Is.True);
        Assert.That(context.Panel.Controls.Count, Is.EqualTo(122));
        Assert.That(context.Panel.Controls.Cast<Control>().All(control => !control.IsDisposed), Is.True);
    }

    [Test]
    public void DisposingMain_ReleasesBoardAndDialog_AndCanBeRepeated()
    {
        using var context = new GameTestContext();
        context.Board.Reset();
        var controls = context.Panel.Controls.Cast<Control>().ToArray();
        var dialog = GetField<NewGameDialog>(context.Form, "newGameDialog");

        context.Form.Dispose();

        Assert.That(controls.All(control => control.IsDisposed), Is.True);
        Assert.That(dialog.IsDisposed, Is.True);
        Assert.DoesNotThrow(context.Form.Dispose);
        Assert.DoesNotThrow(context.Board.Dispose);
    }
}
