using ChineseChess;
using NUnit.Framework;
using Side = ChineseChess.Board.Side;

namespace ChineseChess.Tests;

[TestFixture]
public class BoardRulesTests
{
    private Panel panel = null!;
    private Board board = null!;

    [SetUp]
    public void SetUp()
    {
        panel = new Panel();
        board = new Board(panel);
    }

    [TearDown]
    public void TearDown()
    {
        board.Dispose();
        panel.Dispose();
    }

    [Test]
    public void FacingGenerals_CheckEachOther_AndAllowFlyingCapture([Values] Side side)
    {
        var general = AddGeneral(side, 0, 4, side);
        var opponent = AddGeneral(Opponent(side), 9, 4, side);

        Assert.That(board.IsCheckDelivered(side), Is.True);
        Assert.That(board.IsCheckDelivered(Opponent(side)), Is.True);
        var capture = board.FindLegalMoves(general).Single(move => move.CapturedPiece == opponent);
        Assert.That((capture.EndRow, capture.EndColumn), Is.EqualTo((opponent.Rank, opponent.File)));
    }

    [Test]
    public void FacingGenerals_WithInterveningPiece_DoNotCheckOrCaptureEachOther([Values] Side side)
    {
        var general = AddGeneral(side, 0, 4, side);
        var opponent = AddGeneral(Opponent(side), 9, 4, side);
        Add(new Soldier(board, side, Row(side, 4), 4));

        Assert.That(board.IsCheckDelivered(side), Is.False);
        Assert.That(board.IsCheckDelivered(Opponent(side)), Is.False);
        Assert.That(general.FindPossibleMoves().Any(move => move.CapturedPiece == opponent), Is.False);
        Assert.That(opponent.FindPossibleMoves().Any(move => move.CapturedPiece == general), Is.False);
    }

    [Test]
    public void SoleBlockerBetweenGenerals_CannotMoveOffTheirFile([Values] Side side)
    {
        AddGeneral(side, 0, 4, side);
        AddGeneral(Opponent(side), 9, 4, side);
        var blocker = Add(new Chariot(board, side, Row(side, 4), 4));

        Assert.That(blocker.FindPossibleMoves().Any(move => move.EndColumn != 4), Is.True);
        var legalMoves = board.FindLegalMoves(blocker);
        Assert.That(legalMoves, Is.Not.Empty);
        Assert.That(legalMoves.All(move => move.EndColumn == 4), Is.True);
        Assert.That(board.FindPossibleMoves(side).Where(move => move.Piece == blocker).All(move => move.EndColumn == 4),
            Is.True, "The AI's board-wide list must also reject facing generals.");
    }

    [Test]
    public void PinnedChariot_CanKeepBlockingOrCaptureAttacker_ButCannotExposeGeneral([Values] Side side)
    {
        AddGeneral(side, 0, 4, side);
        AddGeneral(Opponent(side), 9, 4, side);
        var attacker = Add(new Chariot(board, Opponent(side), Row(side, 4), 4));
        var pinned = Add(new Chariot(board, side, Row(side, 2), 4));

        var moves = board.FindLegalMoves(pinned);

        Assert.That(moves.Any(move => At(move, Row(side, 3), 4)), Is.True);
        Assert.That(moves.Any(move => move.CapturedPiece == attacker), Is.True);
        Assert.That(moves.Any(move => move.EndColumn != 4), Is.False);
        Assert.That(board.FindPossibleMoves(side).Where(move => move.Piece == pinned).All(move => move.EndColumn == 4), Is.True);
    }

    [Test]
    public void InCheck_OnlyABlockingMoveFromAnUnrelatedChariotIsLegal([Values] Side side)
    {
        AddGeneral(side, 0, 4, side);
        AddGeneral(Opponent(side), 9, 4, side);
        Add(new Chariot(board, Opponent(side), Row(side, 3), 4));
        var defender = Add(new Chariot(board, side, Row(side, 1), 0));

        Assert.That(board.IsCheckDelivered(Opponent(side)), Is.True);
        var destinations = board.FindLegalMoves(defender).Select(move => (move.EndRow, move.EndColumn));
        Assert.That(destinations, Is.EquivalentTo(new[] { (Row(side, 1), 4) }));
    }

    [Test]
    public void InCheck_CapturingTheAttackerIsLegal([Values] Side side)
    {
        AddGeneral(side, 0, 4, side);
        AddGeneral(Opponent(side), 9, 4, side);
        var attacker = Add(new Chariot(board, Opponent(side), Row(side, 3), 4));
        var defender = Add(new Chariot(board, side, Row(side, 3), 0));

        var moves = board.FindLegalMoves(defender);

        Assert.That(moves, Has.Count.EqualTo(1));
        Assert.That(moves[0].CapturedPiece, Is.SameAs(attacker));
    }

    [Test]
    public void General_InCheck_CannotMoveIntoAttackedSquare([Values] Side side)
    {
        var general = AddGeneral(side, 0, 4, side);
        AddGeneral(Opponent(side), 9, 4, side);
        Add(new Chariot(board, Opponent(side), Row(side, 3), 4));

        Assert.That(general.FindPossibleMoves().Any(move => At(move, Row(side, 1), 4)), Is.True);
        var destinations = board.FindLegalMoves(general).Select(move => (move.EndRow, move.EndColumn));
        Assert.That(destinations, Is.EquivalentTo(new[] { (Row(side, 0), 3), (Row(side, 0), 5) }));
    }

    [Test]
    public void General_CannotCaptureAProtectedEnemy([Values] Side side)
    {
        var general = AddGeneral(side, 0, 4, side);
        AddGeneral(Opponent(side), 9, 3, side);
        var target = Add(new Soldier(board, Opponent(side), Row(side, 1), 4));
        Add(new Chariot(board, Opponent(side), Row(side, 3), 4));

        Assert.That(general.FindPossibleMoves().Any(move => move.CapturedPiece == target), Is.True);
        Assert.That(board.FindLegalMoves(general).Any(move => move.CapturedPiece == target), Is.False);
    }

    [Test]
    public void ChariotCheck_RequiresAnUnobstructedRankOrFile(
        [Values] Side side, [Values] bool horizontal, [Values] bool blocked)
    {
        AddGeneral(Opponent(side), 9, 4, side);
        Add(new Chariot(board, side, Row(side, horizontal ? 9 : 5), horizontal ? 0 : 4));
        if (blocked)
            Add(new Soldier(board, Opponent(side), Row(side, horizontal ? 9 : 7), horizontal ? 2 : 4));

        Assert.That(board.IsCheckDelivered(side), Is.EqualTo(!blocked));
    }

    [Test]
    public void CannonCheck_RequiresExactlyOneScreen(
        [Values] Side side, [Values(0, 1, 2)] int screens, [Values] bool friendlyScreen)
    {
        AddGeneral(Opponent(side), 9, 4, side);
        Add(new Cannon(board, side, Row(side, 5), 4));
        for (int offset = 0; offset < screens; offset++)
            Add(new Soldier(board, friendlyScreen ? side : Opponent(side), Row(side, 6 + offset), 4));

        Assert.That(board.IsCheckDelivered(side), Is.EqualTo(screens == 1));
    }

    [Test]
    public void HorseCheck_RequiresAClearLeg([Values] Side side, [Values] bool blocked)
    {
        AddGeneral(Opponent(side), 9, 4, side);
        Add(new Horse(board, side, Row(side, 7), 3));
        if (blocked)
            Add(new Soldier(board, Opponent(side), Row(side, 8), 3));

        Assert.That(board.IsCheckDelivered(side), Is.EqualTo(!blocked));
    }

    [Test]
    public void SoldierCheck_AttacksForwardAndSidewaysAfterRiver_ButNeverBackwards(
        [Values] Side side, [Values("forward", "sideways", "backward")] string attack)
    {
        int generalRow = attack == "backward" ? 8 : 9;
        AddGeneral(Opponent(side), generalRow, 4, side);
        int soldierRow = attack == "forward" ? 8 : 9;
        int soldierColumn = attack == "sideways" ? 3 : 4;
        Add(new Soldier(board, side, Row(side, soldierRow), soldierColumn));

        Assert.That(board.IsCheckDelivered(side), Is.EqualTo(attack != "backward"));
    }

    [Test]
    public void NoGeneral_MeansNoLegalMoves_EvenWhenOtherPiecesCanMove([Values] Side side)
    {
        AddGeneral(Opponent(side), 9, 4, side);
        var chariot = Add(new Chariot(board, side, Row(side, 4), 0));

        Assert.That(chariot.FindPossibleMoves(), Is.Not.Empty);
        Assert.That(board.FindLegalMoves(chariot), Is.Empty);
        Assert.That(board.FindPossibleMoves(side), Is.Empty);
        Assert.That(board.HasLegalMoves(side), Is.False);
        Assert.That(board.IsCheckDelivered(Opponent(side)), Is.False);
    }

    [Test]
    public void CapturedPiece_HasNoLegalMoves([Values] Side side)
    {
        AddGeneral(side, 0, 4, side);
        AddGeneral(Opponent(side), 9, 3, side);
        var captured = Add(new Horse(board, side, Row(side, 4), 0));
        captured.IsCaptured = true;
        board.Cells[captured.Rank, captured.File].Piece = null;

        Assert.That(board.FindLegalMoves(captured), Is.Empty);
    }

    [Test]
    public void PieceAbsentFromItsCell_HasNoLegalMoves([Values] Side side)
    {
        AddGeneral(side, 0, 4, side);
        AddGeneral(Opponent(side), 9, 3, side);
        var detached = Add(new Horse(board, side, Row(side, 4), 0));
        board.Cells[detached.Rank, detached.File].Piece = null;

        Assert.That(detached.IsCaptured, Is.False);
        Assert.That(board.FindLegalMoves(detached), Is.Empty);
    }

    private General AddGeneral(Side side, int row, int column, Side orientation) =>
        Add(new General(board, side, Row(orientation, row), column));

    private T Add<T>(T piece) where T : Piece
    {
        board.AddPiece(piece);
        return piece;
    }

    private static int Row(Side side, int blueRow) => side == Side.Blue ? blueRow : 9 - blueRow;
    private static Side Opponent(Side side) => side == Side.Blue ? Side.Red : Side.Blue;
    private static bool At(Move move, int row, int column) => move.EndRow == row && move.EndColumn == column;
}
