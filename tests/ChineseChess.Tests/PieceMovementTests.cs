using ChineseChess;
using NUnit.Framework;
using Side = ChineseChess.Board.Side;

namespace ChineseChess.Tests;

[TestFixture]
public class PieceMovementTests
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
    public void General_AtPalaceCenter_MovesOneSquareOrthogonally([Values] Side side)
    {
        var general = Add("General", side, 1, 4);

        AssertDestinations(general, side, (0, 4), (2, 4), (1, 3), (1, 5));
    }

    [Test]
    public void General_AtPalaceCorner_StaysInsidePalace(
        [Values] Side side, [Values(0, 2)] int row, [Values(3, 5)] int column)
    {
        var general = Add("General", side, row, column);

        AssertDestinations(general, side, (1, column), (row, 4));
    }

    [Test]
    public void Advisor_AtPalaceCenter_MovesOneSquareDiagonally([Values] Side side)
    {
        var advisor = Add("Advisor", side, 1, 4);

        AssertDestinations(advisor, side, (0, 3), (0, 5), (2, 3), (2, 5));
    }

    [Test]
    public void Advisor_AtPalaceCorner_CanOnlyReturnToCenter(
        [Values] Side side, [Values(0, 2)] int row, [Values(3, 5)] int column)
    {
        var advisor = Add("Advisor", side, row, column);

        AssertDestinations(advisor, side, (1, 4));
    }

    [Test]
    public void Elephant_WithClearEyes_MovesTwoSquaresDiagonally([Values] Side side)
    {
        var elephant = Add("Elephant", side, 2, 4);

        AssertDestinations(elephant, side, (0, 2), (0, 6), (4, 2), (4, 6));
    }

    [Test]
    public void Elephant_CannotCrossRiver([Values] Side side)
    {
        var elephant = Add("Elephant", side, 4, 4);

        AssertDestinations(elephant, side, (2, 2), (2, 6));
    }

    [Test]
    public void Elephant_AtBoardEdge_StaysOnBoard(
        [Values] Side side, [Values(0, 8)] int column)
    {
        var elephant = Add("Elephant", side, 2, column);

        AssertDestinations(elephant, side, (0, column == 0 ? 2 : 6), (4, column == 0 ? 2 : 6));
    }

    [Test]
    public void Elephant_BlockedEye_PreventsOnlyThatDiagonal(
        [Values] Side side, [Values(-1, 1)] int rowStep, [Values(-1, 1)] int columnStep,
        [Values] bool friendlyBlocker)
    {
        var elephant = Add("Elephant", side, 2, 4);
        Add("Soldier", friendlyBlocker ? side : Opponent(side), 2 + rowStep, 4 + columnStep, side);
        var blockedDestination = (Row(side, 2 + 2 * rowStep), 4 + 2 * columnStep);

        var destinations = Destinations(elephant);

        Assert.That(destinations, Has.Length.EqualTo(3));
        Assert.That(destinations, Does.Not.Contain(blockedDestination));
    }

    [Test]
    public void Horse_WithClearLegs_HasAllEightLShapedMoves([Values] Side side)
    {
        var horse = Add("Horse", side, 4, 4);

        AssertDestinations(horse, side,
            (2, 3), (2, 5), (3, 2), (3, 6), (5, 2), (5, 6), (6, 3), (6, 5));
    }

    [Test]
    public void Horse_BlockedLeg_PreventsBothMovesInThatDirection(
        [Values] Side side, [Values("north", "south", "west", "east")] string direction,
        [Values] bool friendlyBlocker)
    {
        var horse = Add("Horse", side, 4, 4);
        var (rowStep, columnStep) = Direction(direction);
        Add("Soldier", friendlyBlocker ? side : Opponent(side), 4 + rowStep, 4 + columnStep, side);
        var blocked = rowStep == 0
            ? new[] { (Row(side, 3), 4 + 2 * columnStep), (Row(side, 5), 4 + 2 * columnStep) }
            : new[] { (Row(side, 4 + 2 * rowStep), 3), (Row(side, 4 + 2 * rowStep), 5) };

        var destinations = Destinations(horse);

        Assert.That(destinations, Has.Length.EqualTo(6));
        Assert.That(destinations.Intersect(blocked), Is.Empty);
    }

    [Test]
    public void Horse_AtBoardCorner_HasOnlyTwoOnBoardMoves(
        [Values] Side side, [Values(0, 9)] int row, [Values(0, 8)] int column)
    {
        var horse = Add("Horse", side, row, column);
        int rowStep = row == 0 ? 1 : -1;
        int columnStep = column == 0 ? 1 : -1;

        AssertDestinations(horse, side,
            (row + 2 * rowStep, column + columnStep), (row + rowStep, column + 2 * columnStep));
    }

    [Test]
    public void Chariot_OnEmptyBoard_ReachesEverySquareInItsRankAndFile([Values] Side side)
    {
        var chariot = Add("Chariot", side, 4, 4);

        Assert.That(Destinations(chariot), Is.EquivalentTo(OpenRankAndFile(chariot)));
    }

    [Test]
    public void Chariot_StopsAtFirstPiece_AndCapturesOnlyAnOpponent(
        [Values] Side side, [Values("north", "south", "west", "east")] string direction,
        [Values] bool friendlyBlocker)
    {
        var chariot = Add("Chariot", side, 4, 4);
        var (rowStep, columnStep) = Direction(direction);
        var blocker = Add("Soldier", friendlyBlocker ? side : Opponent(side),
            4 + 2 * rowStep, 4 + 2 * columnStep, side);
        Add("Soldier", Opponent(side), 4 + 3 * rowStep, 4 + 3 * columnStep, side);

        var moves = chariot.FindPossibleMoves();

        Assert.That(moves.Any(move => At(move, Row(side, 4 + rowStep), 4 + columnStep)), Is.True);
        Assert.That(moves.Any(move => At(move, blocker.Rank, blocker.File)), Is.EqualTo(!friendlyBlocker));
        Assert.That(moves.Any(move => At(move, Row(side, 4 + 3 * rowStep), 4 + 3 * columnStep)), Is.False);
        Assert.That(moves.Any(move => At(move, Row(side, 4 + 4 * rowStep), 4 + 4 * columnStep)), Is.False);
        if (!friendlyBlocker)
            Assert.That(moves.Single(move => At(move, blocker.Rank, blocker.File)).CapturedPiece, Is.SameAs(blocker));
    }

    [Test]
    public void Cannon_OnEmptyBoard_MovesLikeAChariotWithoutCapturing([Values] Side side)
    {
        var cannon = Add("Cannon", side, 4, 4);
        var moves = cannon.FindPossibleMoves();

        Assert.That(moves.Select(move => (move.EndRow, move.EndColumn)), Is.EquivalentTo(OpenRankAndFile(cannon)));
        Assert.That(moves.All(move => move.CapturedPiece == null), Is.True);
    }

    [Test]
    public void Cannon_CapturesOnlyAnEnemyBehindExactlyOneScreen(
        [Values] Side side, [Values("north", "south", "west", "east")] string direction,
        [Values(0, 1, 2)] int screens, [Values] bool friendlyTarget)
    {
        var cannon = Add("Cannon", side, 4, 4);
        var (rowStep, columnStep) = Direction(direction);
        for (int distance = 1; distance <= screens; distance++)
            Add("Soldier", side, 4 + distance * rowStep, 4 + distance * columnStep, side);
        var target = Add("Horse", friendlyTarget ? side : Opponent(side),
            4 + 4 * rowStep, 4 + 4 * columnStep, side);

        var captures = cannon.FindPossibleMoves().Where(move => At(move, target.Rank, target.File)).ToArray();

        Assert.That(captures, Has.Length.EqualTo(screens == 1 && !friendlyTarget ? 1 : 0));
        if (captures.Length == 1)
            Assert.That(captures[0].CapturedPiece, Is.SameAs(target));
    }

    [Test]
    public void Cannon_CannotJumpAScreenToAnEmptySquare(
        [Values] Side side, [Values("north", "south", "west", "east")] string direction,
        [Values] bool friendlyScreen)
    {
        var cannon = Add("Cannon", side, 4, 4);
        var (rowStep, columnStep) = Direction(direction);
        Add("Soldier", friendlyScreen ? side : Opponent(side), 4 + 2 * rowStep, 4 + 2 * columnStep, side);

        var moves = cannon.FindPossibleMoves();

        Assert.That(moves.Any(move => At(move, Row(side, 4 + rowStep), 4 + columnStep)), Is.True);
        foreach (int distance in new[] { 2, 3, 4 })
            Assert.That(moves.Any(move => At(move, Row(side, 4 + distance * rowStep), 4 + distance * columnStep)),
                Is.False, $"Distance {distance} must not be a quiet move or screen capture.");
    }

    [Test]
    public void Soldier_BeforeCrossingRiver_CanOnlyMoveForward(
        [Values] Side side, [Values(3, 4)] int row)
    {
        var soldier = Add("Soldier", side, row, 4);

        AssertDestinations(soldier, side, (row + 1, 4));
    }

    [Test]
    public void Soldier_AfterCrossingRiver_CanMoveForwardAndSidewaysButNeverBackward([Values] Side side)
    {
        var soldier = Add("Soldier", side, 5, 4);

        AssertDestinations(soldier, side, (6, 4), (5, 3), (5, 5));
    }

    [Test]
    public void Soldier_AtLastRank_CanOnlyMoveSideways([Values] Side side)
    {
        var soldier = Add("Soldier", side, 9, 4);

        AssertDestinations(soldier, side, (9, 3), (9, 5));
    }

    [Test]
    public void Soldier_AtFarCorner_HasOneSidewaysMove(
        [Values] Side side, [Values(0, 8)] int column)
    {
        var soldier = Add("Soldier", side, 9, column);

        AssertDestinations(soldier, side, (9, column == 0 ? 1 : 7));
    }

    [Test]
    public void EveryPiece_RejectsFriendlyDestinations_AndRecordsEnemyCaptures(
        [Values] Side side,
        [Values("General", "Advisor", "Elephant", "Horse", "Chariot", "Cannon", "Soldier")] string name,
        [Values] bool friendlyTarget)
    {
        var (startRow, startColumn, endRow, endColumn) = name switch
        {
            "General" => (1, 4, 2, 4),
            "Advisor" => (1, 4, 2, 5),
            "Elephant" => (2, 4, 4, 6),
            "Horse" => (4, 4, 6, 5),
            "Soldier" => (4, 4, 5, 4),
            _ => (4, 4, 4, 7)
        };
        var piece = Add(name, side, startRow, startColumn);
        if (name == "Cannon")
            Add("Soldier", side, 4, 5);
        var target = Add("Soldier", friendlyTarget ? side : Opponent(side), endRow, endColumn, side);

        var movesToTarget = piece.FindPossibleMoves().Where(move => At(move, target.Rank, target.File)).ToArray();

        Assert.That(movesToTarget, Has.Length.EqualTo(friendlyTarget ? 0 : 1));
        if (!friendlyTarget)
        {
            var capture = movesToTarget.Single();
            Assert.That(capture.Piece, Is.SameAs(piece));
            Assert.That(capture.CapturedPiece, Is.SameAs(target));
            Assert.That((capture.StartRow, capture.StartColumn), Is.EqualTo((piece.Rank, piece.File)));
        }
    }

    // Test positions use Blue's orientation, then reflect ranks to exercise Red's rules.
    private Piece Add(string name, Side side, int row, int column, Side? orientation = null)
    {
        int rank = Row(orientation ?? side, row);
        Piece piece = name switch
        {
            "General" => new General(board, side, rank, column),
            "Advisor" => new Advisor(board, side, rank, column),
            "Elephant" => new Elephant(board, side, rank, column),
            "Horse" => new Horse(board, side, rank, column),
            "Chariot" => new Chariot(board, side, rank, column),
            "Cannon" => new Cannon(board, side, rank, column),
            "Soldier" => new Soldier(board, side, rank, column),
            _ => throw new ArgumentException($"Unknown piece: {name}", nameof(name))
        };
        board.AddPiece(piece);
        return piece;
    }

    private static int Row(Side side, int blueRow) => side == Side.Blue ? blueRow : 9 - blueRow;
    private static Side Opponent(Side side) => side == Side.Blue ? Side.Red : Side.Blue;
    private static bool At(Move move, int row, int column) => move.EndRow == row && move.EndColumn == column;
    private static (int, int)[] Destinations(Piece piece) =>
        piece.FindPossibleMoves().Select(move => (move.EndRow, move.EndColumn)).ToArray();

    private static void AssertDestinations(Piece piece, Side orientation, params (int Row, int Column)[] expected) =>
        Assert.That(Destinations(piece), Is.EquivalentTo(expected.Select(cell => (Row(orientation, cell.Row), cell.Column))));

    private static IEnumerable<(int, int)> OpenRankAndFile(Piece piece) =>
        Enumerable.Range(0, 10).Where(row => row != piece.Rank).Select(row => (row, piece.File))
            .Concat(Enumerable.Range(0, 9).Where(column => column != piece.File).Select(column => (piece.Rank, column)));

    private static (int Row, int Column) Direction(string direction) => direction switch
    {
        "north" => (-1, 0),
        "south" => (1, 0),
        "west" => (0, -1),
        "east" => (0, 1),
        _ => throw new ArgumentException($"Unknown direction: {direction}", nameof(direction))
    };
}
