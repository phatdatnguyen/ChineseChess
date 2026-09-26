using NUnit.Framework;
using Side = ChineseChess.Board.Side;
using static ChineseChess.Tests.TestSupport;

namespace ChineseChess.Tests;

[TestFixture]
public class GameTests
{
    [TestCase(Game.GameType.TwoPlayers, false)]
    [TestCase(Game.GameType.VsAI, false)]
    [TestCase(Game.GameType.TwoPlayersHandicap, true)]
    [TestCase(Game.GameType.VsAIHandicap, true)]
    public void Start_ResetsStartButtonVisibility_AndHandicapStartHidesIt(Game.GameType type, bool handicap)
    {
        using var context = new GameTestContext();
        var game = context.CreateGame(type);
        var button = context.Form.StartButton;
        Control? parent = button.Parent;
        // A hidden parent's visibility masks the child's own Visible state.
        button.Parent = null;
        try
        {
            button.Visible = !handicap;
            game.Start();
            Assert.That(button.Visible, Is.EqualTo(handicap));
            if (handicap)
            {
                game.StartHandicap();
                Assert.That(button.Visible, Is.False);
            }
        }
        finally
        {
            button.Parent = parent;
        }
    }

    [TestCase(Game.GameType.TwoPlayers, false)]
    [TestCase(Game.GameType.VsAI, false)]
    [TestCase(Game.GameType.TwoPlayersHandicap, true)]
    [TestCase(Game.GameType.VsAIHandicap, true)]
    public void Start_ReplacesOldGameStateWithOpening(Game.GameType type, bool handicap)
    {
        using var context = new GameTestContext();
        var game = context.CreateGame(type);
        game.Start();
        if (handicap) game.StartHandicap();
        context.Board.Cells[6, 0].Piece!.Move(5, 0);
        game.Status = Game.GameStatus.Ended;
        context.Form.StatusLabel.Text = "Old result";

        game.Start();

        Assert.Multiple(() =>
        {
            Assert.That(game.Status, Is.EqualTo(handicap ? Game.GameStatus.NotStarted : Game.GameStatus.Started));
            Assert.That(game.Moves, Is.Empty);
            Assert.That(game.CurrentPlayer, Is.SameAs(game.Player1));
            Assert.That(game.CanUndo, Is.False);
            Assert.That(context.Form.UndoButton.Enabled, Is.False);
            Assert.That(context.Board.Cells.Cast<Cell>().Count(cell => cell.Piece != null), Is.EqualTo(32));
            Assert.That(context.Board.Cells[6, 0].Piece?.Name, Is.EqualTo("Soldier"));
            Assert.That(context.Board.Cells[5, 0].Piece, Is.Null);
            Assert.That(context.Form.StatusLabel.Text, Is.EqualTo(handicap
                ? "Choose the pieces to remove and press Start." : ""));
            Assert.That(context.Form.CurrentPlayerLabel.Text, Is.EqualTo(handicap ? "" : game.Player1.Name));
        });
        foreach (var piece in Pieces(context.Board))
            Assert.That(piece.Image.Enabled, Is.EqualTo(handicap ? piece.Name != "General" : piece.Side == Side.Red));
    }

    [TestCase(Game.GameType.TwoPlayersHandicap, false)]
    [TestCase(Game.GameType.VsAIHandicap, false)]
    [TestCase(Game.GameType.VsAIHandicap, true)]
    public void StartHandicap_BeginsPlayAndHandlesAiOpening(Game.GameType type, bool aiFirst)
    {
        using var context = new GameTestContext();
        var game = context.CreateGame(type, aiFirst);
        game.Start();

        Assert.That(game.Status, Is.EqualTo(Game.GameStatus.NotStarted));
        Assert.That(game.Moves, Is.Empty);
        game.StartHandicap();

        Assert.Multiple(() =>
        {
            Assert.That(game.Status, Is.EqualTo(Game.GameStatus.Started));
            Assert.That(game.Moves, Has.Count.EqualTo(aiFirst ? 1 : 0));
            Assert.That(game.CurrentPlayer, Is.SameAs(aiFirst ? game.Player2 : game.Player1));
            Assert.That(game.HumanPlayerSide, Is.EqualTo(aiFirst ? Side.Blue : Side.Red));
            Assert.That(game.AIPlayerSide, Is.EqualTo(aiFirst ? Side.Red : Side.Blue));
            Assert.That(context.Form.StatusLabel.Text, Is.Empty);
            Assert.That(context.Form.CurrentPlayerLabel.Text, Is.EqualTo(game.CurrentPlayer.Name));
            Assert.That(game.CanUndo, Is.False);
        });
        AssertTurnControls(context, game);
    }

    [TestCase(Game.GameType.TwoPlayersHandicap)]
    [TestCase(Game.GameType.VsAIHandicap)]
    public void StartHandicap_WithFacingGenerals_WaitsUntilPositionIsProtected(Game.GameType type)
    {
        using var context = new GameTestContext();
        var game = context.CreateGame(type);
        game.Start();
        context.Clear();
        context.Add("General", Side.Red, 9, 4);
        context.Add("General", Side.Blue, 0, 4);
        string before = Snapshot(context.Board);

        game.StartHandicap();

        Assert.Multiple(() =>
        {
            Assert.That(game.Status, Is.EqualTo(Game.GameStatus.NotStarted));
            Assert.That(game.Moves, Is.Empty);
            Assert.That(game.WinnerNotifications, Is.Zero);
            Assert.That(context.Form.StatusLabel.Text, Is.Not.Empty);
            Assert.That(Snapshot(context.Board), Is.EqualTo(before));
        });
        context.Add("Soldier", Side.Red, 5, 4);
        game.StartHandicap();
        Assert.That(game.Status, Is.EqualTo(Game.GameStatus.Started));
        Assert.That(game.CurrentPlayer, Is.SameAs(game.Player1));
        Assert.That(context.Form.StatusLabel.Text, Is.Empty);
    }

    [TestCase(Side.Red)]
    [TestCase(Side.Blue)]
    public void StartHandicap_WithEitherGeneralInCheck_RejectsPosition(Side attackingSide)
    {
        using var context = new GameTestContext();
        var game = context.CreateGame(Game.GameType.VsAIHandicap, aiFirst: true);
        game.Start();
        context.Clear();
        context.Add("General", attackingSide, RowForAttacker(attackingSide, 9), 3);
        context.Add("General", Opponent(attackingSide), RowForAttacker(attackingSide, 0), 4);
        context.Add("Chariot", attackingSide, RowForAttacker(attackingSide, 4), 4);
        string before = Snapshot(context.Board);

        game.StartHandicap();

        Assert.Multiple(() =>
        {
            Assert.That(game.Status, Is.EqualTo(Game.GameStatus.NotStarted));
            Assert.That(game.Moves, Is.Empty);
            Assert.That(game.WinnerNotifications, Is.Zero);
            Assert.That(Snapshot(context.Board), Is.EqualTo(before));
        });
    }

    [TestCase(Game.GameType.TwoPlayersHandicap, false)]
    [TestCase(Game.GameType.VsAIHandicap, false)]
    [TestCase(Game.GameType.VsAIHandicap, true)]
    public void StartHandicap_WithNoFirstPlayerMoves_EndsWithoutMakingAMove(Game.GameType type, bool aiFirst)
    {
        using var context = new GameTestContext();
        var game = context.CreateGame(type, aiFirst);
        game.Start();
        context.Clear();
        AddTerminalPosition(context, Side.Blue, checkmate: false);
        string before = Snapshot(context.Board);

        game.StartHandicap();

        Assert.Multiple(() =>
        {
            Assert.That(game.Status, Is.EqualTo(Game.GameStatus.Ended));
            Assert.That(game.Winner, Is.SameAs(game.Player2));
            Assert.That(game.WinnerNotifications, Is.EqualTo(1));
            Assert.That(game.Moves, Is.Empty);
            Assert.That(game.CanUndo, Is.False);
            Assert.That(Pieces(context.Board).All(piece => !piece.Image.Enabled), Is.True);
            Assert.That(Snapshot(context.Board), Is.EqualTo(before));
        });
    }

    [TestCase(Game.GameType.TwoPlayers, Game.GameStatus.NotStarted)]
    [TestCase(Game.GameType.VsAI, Game.GameStatus.NotStarted)]
    [TestCase(Game.GameType.TwoPlayersHandicap, Game.GameStatus.Started)]
    [TestCase(Game.GameType.TwoPlayersHandicap, Game.GameStatus.Ended)]
    [TestCase(Game.GameType.VsAIHandicap, Game.GameStatus.Started)]
    [TestCase(Game.GameType.VsAIHandicap, Game.GameStatus.Ended)]
    public void StartHandicap_OutsideSetup_IsNoOp(Game.GameType type, Game.GameStatus status)
    {
        using var context = new GameTestContext();
        var game = context.CreateGame(type);
        context.Board.Reset();
        game.Status = status;
        string before = Snapshot(context.Board);

        game.StartHandicap();

        Assert.Multiple(() =>
        {
            Assert.That(game.Status, Is.EqualTo(status));
            Assert.That(game.CurrentPlayer, Is.SameAs(game.Player1));
            Assert.That(game.Moves, Is.Empty);
            Assert.That(game.WinnerNotifications, Is.Zero);
            Assert.That(Snapshot(context.Board), Is.EqualTo(before));
        });
    }

    [TestCase(Game.GameStatus.NotStarted)]
    [TestCase(Game.GameStatus.Ended)]
    public void SwitchTurn_OutsideStartedGame_IsNoOp(Game.GameStatus status)
    {
        using var context = new GameTestContext();
        var game = context.CreateGame();
        context.Board.Reset();
        game.Status = status;
        string before = Snapshot(context.Board);

        game.SwitchTurn();

        Assert.Multiple(() =>
        {
            Assert.That(game.Status, Is.EqualTo(status));
            Assert.That(game.CurrentPlayer, Is.SameAs(game.Player1));
            Assert.That(game.Moves, Is.Empty);
            Assert.That(game.WinnerNotifications, Is.Zero);
            Assert.That(Snapshot(context.Board), Is.EqualTo(before));
        });
    }

    [TestCase(Game.GameType.VsAI, true, Game.GameStatus.NotStarted)]
    [TestCase(Game.GameType.VsAI, true, Game.GameStatus.Ended)]
    [TestCase(Game.GameType.VsAI, false, Game.GameStatus.Started)]
    [TestCase(Game.GameType.TwoPlayers, false, Game.GameStatus.Started)]
    public void AIPlayerMove_OutsideAnActiveAiTurn_IsNoOp(Game.GameType type, bool aiFirst, Game.GameStatus status)
    {
        using var context = new GameTestContext();
        var game = context.CreateGame(type, aiFirst);
        context.Board.Reset();
        game.Status = status;
        string before = Snapshot(context.Board);

        game.AIPlayerMove();

        Assert.Multiple(() =>
        {
            Assert.That(game.Status, Is.EqualTo(status));
            Assert.That(game.CurrentPlayer, Is.SameAs(game.Player1));
            Assert.That(game.Moves, Is.Empty);
            Assert.That(game.WinnerNotifications, Is.Zero);
            Assert.That(Snapshot(context.Board), Is.EqualTo(before));
        });
    }

    [TestCase(false)]
    [TestCase(true)]
    public void UndoLastTurn_TwoPlayers_RestoresOnlyLastMove(bool ended)
    {
        using var context = new GameTestContext();
        var game = context.CreateGame();
        game.Start();
        context.Board.Cells[6, 0].Piece!.Move(5, 0);
        game.SwitchTurn();
        string afterFirstMove = Snapshot(context.Board);
        var secondPiece = context.Board.Cells[3, 0].Piece!;
        secondPiece.Move(4, 0);
        game.SwitchTurn();
        if (ended) game.End(Side.Blue);

        game.UndoLastTurn();

        Assert.Multiple(() =>
        {
            Assert.That(game.Moves, Has.Count.EqualTo(1));
            Assert.That(game.CurrentPlayer, Is.SameAs(game.Player2));
            Assert.That(game.Status, Is.EqualTo(Game.GameStatus.Started));
            Assert.That(Snapshot(context.Board), Is.EqualTo(afterFirstMove));
            Assert.That(context.Board.Cells[3, 0].Piece, Is.SameAs(secondPiece));
            Assert.That(game.CanUndo, Is.True);
            Assert.That(context.Form.StatusLabel.Text, Is.Empty);
        });
        AssertTurnControls(context, game);
    }

    [TestCase(false)]
    [TestCase(true)]
    public void UndoLastTurn_AfterFinalHumanMove_UndoesOnce(bool aiFirst)
    {
        using var context = new GameTestContext();
        var game = context.CreateGame(Game.GameType.VsAI, aiFirst);
        game.Start();
        context.Clear();
        var human = game.CurrentPlayer;
        context.Add("General", human.Side, RowForAttacker(human.Side, 9), 3);
        var target = context.Add("General", game.AIPlayerSide, RowForAttacker(human.Side, 0), 4);
        var attacker = context.Add("Chariot", human.Side, RowForAttacker(human.Side, 0), 8);
        string before = Snapshot(context.Board);
        int openingMoves = game.Moves.Count;
        attacker.Capture(target);
        Assert.That(game.Status, Is.EqualTo(Game.GameStatus.Ended));
        Assert.That(game.Winner, Is.SameAs(human));

        game.UndoLastTurn();

        Assert.Multiple(() =>
        {
            Assert.That(game.Moves, Has.Count.EqualTo(openingMoves));
            Assert.That(game.CurrentPlayer, Is.SameAs(human));
            Assert.That(game.Status, Is.EqualTo(Game.GameStatus.Started));
            Assert.That(Snapshot(context.Board), Is.EqualTo(before));
            Assert.That(game.CanUndo, Is.False);
            Assert.That(target.IsCaptured, Is.False);
            Assert.That(target.Image.IsDisposed, Is.False);
        });
        AssertTurnControls(context, game);
    }

    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public void UndoLastTurn_AfterAiReply_RestoresPreviousHumanTurn(bool aiFirst, bool ended)
    {
        using var context = new GameTestContext();
        var game = context.CreateGame(Game.GameType.VsAI, aiFirst);
        game.Start();
        string before = Snapshot(context.Board);
        int openingMoves = game.Moves.Count;
        var human = game.CurrentPlayer;
        MakeHumanMove(context, game);
        game.SwitchTurn();
        Assert.That(game.Moves, Has.Count.EqualTo(openingMoves + 2));
        Assert.That(game.CurrentPlayer, Is.SameAs(human));
        if (ended) game.End(game.AIPlayerSide);

        game.UndoLastTurn();

        Assert.Multiple(() =>
        {
            Assert.That(game.Moves, Has.Count.EqualTo(openingMoves));
            Assert.That(game.CurrentPlayer, Is.SameAs(human));
            Assert.That(game.Status, Is.EqualTo(Game.GameStatus.Started));
            Assert.That(Snapshot(context.Board), Is.EqualTo(before));
            Assert.That(game.CanUndo, Is.False);
        });
        AssertTurnControls(context, game);
    }

    [Test]
    public void UndoLastTurn_WithOnlyAiOpening_IsNoOp()
    {
        using var context = new GameTestContext();
        var game = context.CreateGame(Game.GameType.VsAI, aiFirst: true);
        game.Start();
        string before = Snapshot(context.Board);

        game.UndoLastTurn();

        Assert.Multiple(() =>
        {
            Assert.That(game.Moves, Has.Count.EqualTo(1));
            Assert.That(game.CurrentPlayer, Is.SameAs(game.Player2));
            Assert.That(game.Status, Is.EqualTo(Game.GameStatus.Started));
            Assert.That(game.CanUndo, Is.False);
            Assert.That(context.Form.UndoButton.Enabled, Is.False);
            Assert.That(Snapshot(context.Board), Is.EqualTo(before));
        });
    }

    [TestCase(Game.GameStatus.NotStarted)]
    [TestCase(Game.GameStatus.Started)]
    [TestCase(Game.GameStatus.Ended)]
    public void UndoLastTurn_WithNoMoves_IsNoOp(Game.GameStatus status)
    {
        using var context = new GameTestContext();
        var game = context.CreateGame();
        context.Board.Reset();
        game.Status = status;
        string before = Snapshot(context.Board);

        game.UndoLastTurn();
        game.UndoLastTurn();

        Assert.Multiple(() =>
        {
            Assert.That(game.Moves, Is.Empty);
            Assert.That(game.Status, Is.EqualTo(status));
            Assert.That(game.CanUndo, Is.False);
            Assert.That(game.CurrentPlayer, Is.SameAs(game.Player1));
            Assert.That(Snapshot(context.Board), Is.EqualTo(before));
        });
    }

    [TestCase(Side.Red, false)]
    [TestCase(Side.Blue, false)]
    [TestCase(Side.Red, true)]
    [TestCase(Side.Blue, true)]
    public void SwitchTurn_WithNoLegalReply_EndsGameAndUndoRestoresMover(Side winningSide, bool checkmate)
    {
        using var context = new GameTestContext();
        var game = context.CreateGame();
        AddTerminalPosition(context, winningSide, checkmate, beforeFinalMove: true);
        var winner = winningSide == Side.Red ? game.Player1 : game.Player2;
        SetField(game, "currentPlayer", winner);
        game.Status = Game.GameStatus.Started;
        string before = Snapshot(context.Board);
        int sourceRow = RowForAttacker(winningSide, 1);
        var finalPiece = context.Board.Cells[sourceRow, checkmate ? 0 : 1].Piece!;
        finalPiece.Move(RowForAttacker(winningSide, checkmate ? 0 : 1), 0);
        Assert.That(context.Board.IsCheckDelivered(winningSide), Is.EqualTo(checkmate));
        Assert.That(context.Board.HasLegalMoves(Opponent(winningSide)), Is.False);

        game.SwitchTurn();

        Assert.Multiple(() =>
        {
            Assert.That(game.Status, Is.EqualTo(Game.GameStatus.Ended));
            Assert.That(game.Winner, Is.SameAs(winner));
            Assert.That(game.WinnerNotifications, Is.EqualTo(1));
            Assert.That(Pieces(context.Board).All(piece => !piece.Image.Enabled), Is.True);
            Assert.That(game.CanUndo, Is.True);
        });
        game.UndoLastTurn();
        Assert.Multiple(() =>
        {
            Assert.That(game.Status, Is.EqualTo(Game.GameStatus.Started));
            Assert.That(game.CurrentPlayer, Is.SameAs(winner));
            Assert.That(game.Moves, Is.Empty);
            Assert.That(Snapshot(context.Board), Is.EqualTo(before));
            Assert.That(game.CanUndo, Is.False);
        });
        AssertTurnControls(context, game);
    }

    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public void AIPlayerMove_WithNoMoves_DoesNotReplayPreviousSearch(bool aiFirst, bool checkmate)
    {
        using var context = new GameTestContext();
        var game = context.CreateGame(Game.GameType.VsAI, aiFirst);
        game.Start();
        if (!aiFirst)
        {
            MakeHumanMove(context, game);
            game.SwitchTurn();
        }
        Assert.That(game.Moves.Any(move => move.Piece.Side == game.AIPlayerSide), Is.True);
        context.Clear();
        AddTerminalPosition(context, game.HumanPlayerSide, checkmate);
        SetField(game, "currentPlayer", aiFirst ? game.Player1 : game.Player2);
        game.Status = Game.GameStatus.Started;
        int moveCount = game.Moves.Count;
        string before = Snapshot(context.Board);
        Assert.That(context.Board.IsCheckDelivered(game.HumanPlayerSide), Is.EqualTo(checkmate));
        Assert.That(context.Board.HasLegalMoves(game.AIPlayerSide), Is.False);

        game.AIPlayerMove();
        game.AIPlayerMove();

        Assert.Multiple(() =>
        {
            Assert.That(game.Status, Is.EqualTo(Game.GameStatus.Ended));
            Assert.That(game.Winner, Is.SameAs(aiFirst ? game.Player2 : game.Player1));
            Assert.That(game.WinnerNotifications, Is.EqualTo(1));
            Assert.That(game.Moves, Has.Count.EqualTo(moveCount));
            Assert.That(Snapshot(context.Board), Is.EqualTo(before));
            Assert.That(Pieces(context.Board).All(piece => !piece.Image.Enabled), Is.True);
        });
    }

    [TestCase(Game.AIDifficulty.Easy, false)]
    [TestCase(Game.AIDifficulty.Medium, false)]
    [TestCase(Game.AIDifficulty.Hard, false)]
    [TestCase(Game.AIDifficulty.Extreme, false)]
    [TestCase(Game.AIDifficulty.Easy, true)]
    [TestCase(Game.AIDifficulty.Medium, true)]
    [TestCase(Game.AIDifficulty.Hard, true)]
    [TestCase(Game.AIDifficulty.Extreme, true)]
    public void AIPlayerMove_EveryDifficulty_ProducesOneLegalMove(Game.AIDifficulty difficulty, bool aiFirst)
    {
        using var context = new GameTestContext();
        // Handicap setup allows us to record the legal candidates before the AI starts.
        var game = context.CreateGame(Game.GameType.VsAIHandicap, aiFirst, difficulty);
        game.Start();
        if (!aiFirst)
        {
            game.StartHandicap();
            MakeHumanMove(context, game);
        }
        var legalMoves = context.Board.FindPossibleMoves(game.AIPlayerSide);
        int previousMoves = game.Moves.Count;
        string before = Snapshot(context.Board);

        if (aiFirst) game.StartHandicap();
        else game.SwitchTurn();

        Assert.That(game.Moves, Has.Count.EqualTo(previousMoves + 1));
        Move actual = game.Moves[^1];
        Assert.Multiple(() =>
        {
            Assert.That(legalMoves.Any(move => move.Piece == actual.Piece
                && move.StartRow == actual.StartRow && move.StartColumn == actual.StartColumn
                && move.EndRow == actual.EndRow && move.EndColumn == actual.EndColumn
                && move.CapturedPiece == actual.CapturedPiece), Is.True, "AI must choose a legal candidate.");
            Assert.That(game.Status, Is.EqualTo(Game.GameStatus.Started));
            Assert.That(game.CurrentPlayer.Side, Is.EqualTo(game.HumanPlayerSide));
            Assert.That(game.CurrentPlayer.IsAI, Is.False);
            Assert.That(context.Board.IsCheckDelivered(game.HumanPlayerSide), Is.False);
            Assert.That(game.WinnerNotifications, Is.Zero);
        });
        AssertTurnControls(context, game);
        context.Board.UndoMove(actual, isTestMove: true);
        Assert.That(Snapshot(context.Board), Is.EqualTo(before), "Search must change only the selected move.");
    }

    [TestCase(Side.Red)]
    [TestCase(Side.Blue)]
    public void AIPlayerMove_CapturingGeneral_EndsWithoutTakingAnotherTurn(Side aiSide)
    {
        using var context = new GameTestContext();
        bool aiFirst = aiSide == Side.Red;
        var game = context.CreateGame(Game.GameType.VsAIHandicap, aiFirst);
        game.Start();
        context.Clear();
        context.Add("General", aiSide, RowForAttacker(aiSide, 9), 3);
        var opposingGeneral = context.Add("General", Opponent(aiSide), RowForAttacker(aiSide, 0), 4);
        var chariot = context.Add("Chariot", aiSide, RowForAttacker(aiSide, 0), 8);
        var ai = aiFirst ? game.Player1 : game.Player2;
        SetField(game, "currentPlayer", ai);
        game.Status = Game.GameStatus.Started;

        game.AIPlayerMove();

        Assert.Multiple(() =>
        {
            Assert.That(game.Status, Is.EqualTo(Game.GameStatus.Ended));
            Assert.That(game.Winner, Is.SameAs(ai));
            Assert.That(game.CurrentPlayer, Is.SameAs(ai));
            Assert.That(game.Moves, Has.Count.EqualTo(1));
            Assert.That(game.Moves[0].CapturedPiece, Is.SameAs(opposingGeneral));
            Assert.That(context.Board.Cells[opposingGeneral.Rank, opposingGeneral.File].Piece, Is.SameAs(chariot));
            Assert.That(opposingGeneral.IsCaptured, Is.True);
            Assert.That(game.WinnerNotifications, Is.EqualTo(1));
            Assert.That(Pieces(context.Board).All(piece => !piece.Image.Enabled), Is.True);
        });
    }

    private static IEnumerable<Piece> Pieces(Board board) => board.Cells.Cast<Cell>()
        .Where(cell => cell.Piece != null).Select(cell => cell.Piece!);

    private static void AssertTurnControls(GameTestContext context, Game game)
    {
        Assert.That(context.Form.CurrentPlayerLabel.Text, Is.EqualTo(game.CurrentPlayer.Name));
        Assert.That(context.Form.UndoButton.Enabled, Is.EqualTo(game.CanUndo));
        foreach (var piece in Pieces(context.Board))
            Assert.That(piece.Image.Enabled, Is.EqualTo(piece.Side == game.CurrentPlayer.Side));
    }

    private static void MakeHumanMove(GameTestContext context, Game game)
    {
        Move move = context.Board.FindPossibleMoves(game.HumanPlayerSide)
            .First(candidate => candidate.Piece.Name == "Soldier" && candidate.CapturedPiece == null);
        move.Piece.Move(move.EndRow, move.EndColumn);
    }

    // Coordinates describe a Red win. Mirroring rows and swapping sides gives the Blue case.
    private static int RowForAttacker(Side side, int row) => side == Side.Red ? row : 9 - row;

    private static void AddTerminalPosition(GameTestContext context, Side winningSide, bool checkmate,
        bool beforeFinalMove = false)
    {
        int Row(int row) => RowForAttacker(winningSide, row);
        context.Add("General", winningSide, Row(9), 3);
        context.Add("General", Opponent(winningSide), Row(0), 4);
        context.Add("Chariot", winningSide, Row(2), 3);
        context.Add("Chariot", winningSide, Row(2), 5);
        if (checkmate)
        {
            context.Add("Soldier", winningSide, Row(2), 4);
            context.Add("Chariot", winningSide, Row(beforeFinalMove ? 1 : 0), 0);
        }
        else
        {
            context.Add("Chariot", winningSide, Row(1), beforeFinalMove ? 1 : 0);
        }
    }
}
