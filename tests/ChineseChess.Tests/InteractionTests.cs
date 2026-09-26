using System.Windows.Forms;
using NUnit.Framework;
using Side = ChineseChess.Board.Side;
using static ChineseChess.Tests.TestSupport;

namespace ChineseChess.Tests;

[TestFixture]
public sealed class InteractionTests
{
    private GameTestContext context = null!;

    [SetUp]
    public void SetUp() => context = new GameTestContext();

    [TearDown]
    public void TearDown() => context.Dispose();

    [Test]
    public void ClickingWithoutAGameDoesNothing()
    {
        var piece = context.Add("Soldier", Side.Red, 6, 0);
        string before = Snapshot(context.Board);

        ClickPiece(piece);
        ClickIndicator(context.Board.Cells[5, 0]);

        Assert.Multiple(() =>
        {
            Assert.That(context.Board.IsSelected, Is.False);
            Assert.That(Snapshot(context.Board), Is.EqualTo(before));
        });
    }

    [TestCase(Game.GameStatus.NotStarted)]
    [TestCase(Game.GameStatus.Ended)]
    public void ClickingPieceOutsideActiveGameDoesNotSelectIt(Game.GameStatus status)
    {
        var game = context.CreateGame();
        game.Start();
        game.Status = status;

        ClickPiece(context.Board.Cells[6, 0].Piece!);

        Assert.Multiple(() =>
        {
            Assert.That(context.Board.IsSelected, Is.False);
            Assert.That(game.Moves, Is.Empty);
        });
    }

    [Test]
    public void ClickingOpponentPieceDoesNotSelectIt()
    {
        var game = context.CreateGame();
        game.Start();

        ClickPiece(context.Board.Cells[3, 0].Piece!);

        Assert.Multiple(() =>
        {
            Assert.That(context.Board.IsSelected, Is.False);
            Assert.That(game.CurrentPlayer, Is.SameAs(game.Player1));
            Assert.That(game.Moves, Is.Empty);
        });
    }

    [Test]
    public void ClickingDuringComputerTurnDoesNothing()
    {
        var game = context.CreateGame(Game.GameType.VsAI, aiFirst: true);
        context.Board.Reset();
        game.Status = Game.GameStatus.Started;
        var piece = context.Board.Cells[6, 0].Piece!;

        ClickPiece(piece);
        // A stale selection must not bypass the computer-turn guard on indicators.
        context.Board.SelectedCell = context.Board.Cells[6, 0];
        ClickIndicator(context.Board.Cells[5, 0]);

        Assert.Multiple(() =>
        {
            Assert.That(piece.IsSelected, Is.False);
            Assert.That(context.Board.Cells[6, 0].Piece, Is.SameAs(piece));
            Assert.That(game.Moves, Is.Empty);
        });
    }

    [TestCase(MouseButtons.Right)]
    [TestCase(MouseButtons.Middle)]
    public void NonLeftClicksDoNotSelectOrMovePieces(MouseButtons button)
    {
        var game = context.CreateGame();
        game.Start();
        var piece = context.Board.Cells[6, 0].Piece!;

        ClickPiece(piece, button);
        Assert.That(context.Board.IsSelected, Is.False);
        ClickPiece(piece);
        ClickIndicator(context.Board.Cells[5, 0], button);

        Assert.Multiple(() =>
        {
            Assert.That(context.Board.SelectedCell?.Piece, Is.SameAs(piece));
            Assert.That(context.Board.Cells[6, 0].Piece, Is.SameAs(piece));
            Assert.That(game.Moves, Is.Empty);
        });
    }

    [Test]
    public void ClickingIndicatorWithoutASelectionDoesNothing()
    {
        var game = context.CreateGame();
        game.Start();
        string before = Snapshot(context.Board);

        ClickIndicator(context.Board.Cells[5, 0]);

        Assert.Multiple(() =>
        {
            Assert.That(Snapshot(context.Board), Is.EqualTo(before));
            Assert.That(game.Moves, Is.Empty);
            Assert.That(game.CurrentPlayer, Is.SameAs(game.Player1));
        });
    }

    [TestCase(false)]
    [TestCase(true)]
    public void IllegalDestinationDoesNotMoveOrChangeSelection(bool clickFriendlyPiece)
    {
        var game = context.CreateGame();
        var general = context.Add("General", Side.Red, 9, 4);
        context.Add("General", Side.Blue, 0, 3);
        context.Add("Chariot", Side.Blue, 0, 4);
        var pinned = context.Add("Chariot", Side.Red, 5, 4);
        game.Status = Game.GameStatus.Started;
        ClickPiece(pinned);
        string before = Snapshot(context.Board);

        if (clickFriendlyPiece)
            ClickPiece(general);
        else
            ClickIndicator(context.Board.Cells[5, 3]);

        Assert.Multiple(() =>
        {
            Assert.That(Snapshot(context.Board), Is.EqualTo(before));
            Assert.That(context.Board.SelectedCell?.Piece, Is.SameAs(pinned));
            Assert.That(game.CurrentPlayer, Is.SameAs(game.Player1));
            Assert.That(game.Moves, Is.Empty);
        });
    }

    [Test]
    public void LegalIndicatorClickMovesOnceAndSwitchesTurn()
    {
        var game = context.CreateGame();
        game.Start();
        var piece = context.Board.Cells[6, 0].Piece!;
        ClickPiece(piece);

        ClickIndicator(context.Board.Cells[5, 0]);

        Assert.Multiple(() =>
        {
            Assert.That(context.Board.Cells[6, 0].Piece, Is.Null);
            Assert.That(context.Board.Cells[5, 0].Piece, Is.SameAs(piece));
            Assert.That(context.Board.IsSelected, Is.False);
            Assert.That(piece.IsSelected, Is.False);
            Assert.That(game.Moves, Has.Count.EqualTo(1));
            Assert.That(game.CurrentPlayer, Is.SameAs(game.Player2));
            Assert.That(piece.Image.Enabled, Is.False);
            Assert.That(context.Board.Cells[3, 0].Piece!.Image.Enabled, Is.True);
            Assert.That(context.Form.UndoButton.Enabled, Is.True);
        });
    }

    [Test]
    public void SelectingAndDeselectingPieceRestoresUndoWithoutChangingPosition()
    {
        var game = context.CreateGame();
        game.Start();
        context.Board.Cells[6, 0].Piece!.Move(5, 0);
        game.SwitchTurn();
        var piece = context.Board.Cells[3, 0].Piece!;
        string before = Snapshot(context.Board);
        Assert.That(context.Form.UndoButton.Enabled, Is.True);

        ClickPiece(piece);
        Assert.Multiple(() =>
        {
            Assert.That(piece.IsSelected, Is.True);
            Assert.That(context.Board.SelectedCell?.Piece, Is.SameAs(piece));
            Assert.That(context.Form.UndoButton.Enabled, Is.False);
        });
        ClickPiece(piece);

        Assert.Multiple(() =>
        {
            Assert.That(piece.IsSelected, Is.False);
            Assert.That(context.Board.IsSelected, Is.False);
            Assert.That(context.Form.UndoButton.Enabled, Is.True);
            Assert.That(Snapshot(context.Board), Is.EqualTo(before));
            Assert.That(game.Moves, Has.Count.EqualTo(1));
            Assert.That(game.CurrentPlayer, Is.SameAs(game.Player2));
            Assert.That(context.Board.Cells[3, 2].Piece!.Image.Enabled, Is.True);
            Assert.That(context.Board.Cells[5, 0].Piece!.Image.Enabled, Is.False);
        });
    }

    [TestCase(Game.GameStatus.NotStarted)]
    [TestCase(Game.GameStatus.Ended)]
    public void StaleIndicatorClickOutsideActiveGameDoesNotMove(Game.GameStatus status)
    {
        var game = context.CreateGame();
        game.Start();
        var piece = context.Board.Cells[6, 0].Piece!;
        ClickPiece(piece);
        game.Status = status;

        ClickIndicator(context.Board.Cells[5, 0]);

        Assert.Multiple(() =>
        {
            Assert.That(context.Board.Cells[6, 0].Piece, Is.SameAs(piece));
            Assert.That(context.Board.Cells[5, 0].Piece, Is.Null);
            Assert.That(game.Moves, Is.Empty);
            Assert.That(game.CurrentPlayer, Is.SameAs(game.Player1));
        });
    }

    [Test]
    public void CapturingGeneralByClickEndsGameAndUndoButtonRestoresPosition()
    {
        var game = context.CreateGame();
        context.Add("General", Side.Red, 9, 3);
        var blue = context.Add("General", Side.Blue, 0, 4);
        var chariot = context.Add("Chariot", Side.Red, 0, 8);
        game.Status = Game.GameStatus.Started;
        game.UpdateTurnControls();

        ClickPiece(chariot);
        ClickPiece(blue);

        Assert.Multiple(() =>
        {
            Assert.That(game.Status, Is.EqualTo(Game.GameStatus.Ended));
            Assert.That(game.Winner, Is.SameAs(game.Player1));
            Assert.That(game.WinnerNotifications, Is.EqualTo(1));
            Assert.That(game.CurrentPlayer, Is.SameAs(game.Player1));
            Assert.That(chariot.Image.Enabled, Is.False);
            Assert.That(game.Moves, Has.Count.EqualTo(1));
            Assert.That(context.Form.UndoButton.Enabled, Is.True);
        });

        Invoke(context.Form, "UndoButton_Click", context.Form.UndoButton, EventArgs.Empty);

        Assert.Multiple(() =>
        {
            Assert.That(game.Status, Is.EqualTo(Game.GameStatus.Started));
            Assert.That(game.Moves, Is.Empty);
            Assert.That(context.Board.Cells[0, 4].Piece, Is.SameAs(blue));
            Assert.That(context.Board.Cells[0, 8].Piece, Is.SameAs(chariot));
            Assert.That(blue.IsCaptured, Is.False);
            Assert.That(game.CurrentPlayer, Is.SameAs(game.Player1));
            Assert.That(chariot.Image.Enabled, Is.True);
            Assert.That(blue.Image.Enabled, Is.False);
            Assert.That(context.Form.UndoButton.Enabled, Is.False);
        });
    }

    [TestCase(Game.GameType.TwoPlayersHandicap, Side.Red)]
    [TestCase(Game.GameType.TwoPlayersHandicap, Side.Blue)]
    [TestCase(Game.GameType.VsAIHandicap, Side.Red)]
    [TestCase(Game.GameType.VsAIHandicap, Side.Blue)]
    public void HandicapClickCannotRemoveAGeneral(Game.GameType type, Side side)
    {
        var game = context.CreateGame(type);
        game.Start();
        int row = side == Side.Red ? 9 : 0;
        var general = context.Board.Cells[row, 4].Piece!;

        ClickPiece(general);

        Assert.Multiple(() =>
        {
            Assert.That(context.Board.Cells[row, 4].Piece, Is.SameAs(general));
            Assert.That(general.IsCaptured, Is.False);
            Assert.That(general.Image.IsDisposed, Is.False);
            Assert.That(game.Status, Is.EqualTo(Game.GameStatus.NotStarted));
            Assert.That(game.Moves, Is.Empty);
        });
    }

    [Test]
    public void HandicapClickCannotExposeFacingGenerals()
    {
        var game = context.CreateGame(Game.GameType.TwoPlayersHandicap);
        game.Start();
        context.Clear();
        context.Add("General", Side.Red, 9, 4);
        context.Add("General", Side.Blue, 0, 4);
        var blocker = context.Add("Soldier", Side.Red, 5, 4);

        ClickPiece(blocker);

        Assert.Multiple(() =>
        {
            Assert.That(context.Board.Cells[5, 4].Piece, Is.SameAs(blocker));
            Assert.That(blocker.Image.IsDisposed, Is.False);
            Assert.That(blocker.IsCaptured, Is.False);
            Assert.That(context.Board.IsCheckDelivered(Side.Red), Is.False);
            Assert.That(context.Board.IsCheckDelivered(Side.Blue), Is.False);
        });
    }

    [TestCase(Side.Red)]
    [TestCase(Side.Blue)]
    public void HandicapClickCannotExposeEitherGeneralToCheck(Side protectedSide)
    {
        var game = context.CreateGame(Game.GameType.TwoPlayersHandicap);
        game.Start();
        context.Clear();
        int homeRow = protectedSide == Side.Red ? 9 : 0;
        int opponentRow = 9 - homeRow;
        context.Add("General", protectedSide, homeRow, 4);
        context.Add("General", Opponent(protectedSide), opponentRow, 3);
        context.Add("Chariot", Opponent(protectedSide), opponentRow, 4);
        var blocker = context.Add("Soldier", protectedSide, 5, 4);

        ClickPiece(blocker);

        Assert.Multiple(() =>
        {
            Assert.That(context.Board.Cells[5, 4].Piece, Is.SameAs(blocker));
            Assert.That(blocker.Image.IsDisposed, Is.False);
            Assert.That(context.Board.IsCheckDelivered(Opponent(protectedSide)), Is.False);
        });
    }

    [TestCase(Game.GameType.TwoPlayersHandicap)]
    [TestCase(Game.GameType.VsAIHandicap)]
    public void HandicapClickRemovesSafePieceAndDisposesItsControl(Game.GameType type)
    {
        var game = context.CreateGame(type);
        game.Start();
        var piece = context.Board.Cells[9, 1].Piece!;

        ClickPiece(piece);

        Assert.Multiple(() =>
        {
            Assert.That(context.Board.Cells[9, 1].Piece, Is.Null);
            Assert.That(piece.IsCaptured, Is.True);
            Assert.That(piece.Image.IsDisposed, Is.True);
            Assert.That(context.Panel.Controls.Contains(piece.Image), Is.False);
            Assert.That(game.Moves, Is.Empty);
            Assert.That(game.Status, Is.EqualTo(Game.GameStatus.NotStarted));
        });
    }

    [TestCase(Game.GameType.TwoPlayersHandicap)]
    [TestCase(Game.GameType.VsAIHandicap)]
    public void StartButtonBeginsPreparedHandicapGame(Game.GameType type)
    {
        var game = context.CreateGame(type);
        game.Start();
        ClickPiece(context.Board.Cells[9, 1].Piece!);

        Invoke(context.Form, "StartButton_Click", context.Form.StartButton, EventArgs.Empty);

        Assert.Multiple(() =>
        {
            Assert.That(game.Status, Is.EqualTo(Game.GameStatus.Started));
            Assert.That(game.CurrentPlayer, Is.SameAs(game.Player1));
            Assert.That(context.Board.Cells[9, 1].Piece, Is.Null);
            Assert.That(context.Board.Cells[6, 0].Piece!.Image.Enabled, Is.True);
            Assert.That(context.Board.Cells[3, 0].Piece!.Image.Enabled, Is.False);
            Assert.That(game.Moves, Is.Empty);
        });
    }

    private static void ClickIndicator(Cell cell, MouseButtons button = MouseButtons.Left) =>
        Invoke(cell, "PossibleMoveIndicator_MouseClick", cell.PossibleMoveIndicator,
            new MouseEventArgs(button, 1, 0, 0, 0));
}

[TestFixture]
public sealed class NewGameDialogTests
{
    [TestCase(Game.GameType.TwoPlayers, false, false)]
    [TestCase(Game.GameType.TwoPlayersHandicap, false, true)]
    [TestCase(Game.GameType.VsAI, true, false)]
    [TestCase(Game.GameType.VsAIHandicap, true, true)]
    public void AcceptMapsSelectedGameType(Game.GameType expected, bool versusAI, bool handicap)
    {
        using var dialog = new NewGameDialog();
        GetField<RadioButton>(dialog, "gameType_VsAI").Checked = versusAI;
        GetField<CheckBox>(dialog, "gameType_Handicap").Checked = handicap;

        CloseWithResult(dialog, DialogResult.OK);

        Assert.That(dialog.GameType, Is.EqualTo(expected));
    }

    [TestCase("", "", "Player 1", "Player 2")]
    [TestCase("Alice", "Bob", "Alice", "Bob")]
    [TestCase("Alice", "", "Alice", "Player 2")]
    [TestCase("", "Bob", "Player 1", "Bob")]
    public void AcceptUsesPlayerNamesOrDefaults(string firstName, string secondName,
        string expectedFirstName, string expectedSecondName)
    {
        using var dialog = new NewGameDialog();
        GetField<TextBox>(dialog, "playersInformation_Player1Name").Text = firstName;
        GetField<TextBox>(dialog, "playersInformation_Player2Name").Text = secondName;

        CloseWithResult(dialog, DialogResult.OK);

        Assert.Multiple(() =>
        {
            Assert.That(dialog.Player1!.Name, Is.EqualTo(expectedFirstName));
            Assert.That(dialog.Player2!.Name, Is.EqualTo(expectedSecondName));
            Assert.That(dialog.Player1.Side, Is.EqualTo(Side.Red));
            Assert.That(dialog.Player2.Side, Is.EqualTo(Side.Blue));
            Assert.That(dialog.Player1.IsAI, Is.False);
            Assert.That(dialog.Player2.IsAI, Is.False);
        });
    }

    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public void AcceptConfiguresExactlyTheSelectedComputerPlayer(bool aiFirst, bool handicap)
    {
        using var dialog = new NewGameDialog();
        GetField<RadioButton>(dialog, "gameType_VsAI").Checked = true;
        GetField<CheckBox>(dialog, "gameType_Handicap").Checked = handicap;
        GetField<RadioButton>(dialog, aiFirst ? "playersInformation_Player1AI" : "playersInformation_Player2AI")
            .Checked = true;

        CloseWithResult(dialog, DialogResult.OK);

        Assert.Multiple(() =>
        {
            Assert.That(dialog.Player1!.IsAI, Is.EqualTo(aiFirst));
            Assert.That(dialog.Player2!.IsAI, Is.EqualTo(!aiFirst));
        });
    }

    [TestCase(0, Game.AIDifficulty.Easy)]
    [TestCase(1, Game.AIDifficulty.Medium)]
    [TestCase(2, Game.AIDifficulty.Hard)]
    [TestCase(3, Game.AIDifficulty.Extreme)]
    public void AcceptMapsDifficultySelection(int index, Game.AIDifficulty expected)
    {
        using var dialog = new NewGameDialog();
        GetField<RadioButton>(dialog, "gameType_VsAI").Checked = true;
        GetField<ComboBox>(dialog, "gameType_AIDifficulty").SelectedIndex = index;

        CloseWithResult(dialog, DialogResult.OK);

        Assert.That(dialog.AIDifficulty, Is.EqualTo(expected));
    }

    [Test]
    public void NewDialogDefaultsToMediumDifficulty()
    {
        using var dialog = new NewGameDialog();

        Assert.Multiple(() =>
        {
            Assert.That(dialog.AIDifficulty, Is.EqualTo(Game.AIDifficulty.Medium));
            Assert.That(GetField<ComboBox>(dialog, "gameType_AIDifficulty").SelectedIndex, Is.EqualTo(1));
        });
    }

    [TestCase(false)]
    [TestCase(true)]
    public void SwitchingGameTypeUpdatesComputerControls(bool versusAI)
    {
        using var dialog = new NewGameDialog();
        GetField<RadioButton>(dialog, "gameType_VsAI").Checked = true;
        GetField<RadioButton>(dialog, "gameType_TwoPlayer").Checked = !versusAI;

        Assert.Multiple(() =>
        {
            Assert.That(GetField<RadioButton>(dialog, "playersInformation_Player1AI").Enabled, Is.EqualTo(versusAI));
            Assert.That(GetField<RadioButton>(dialog, "playersInformation_Player2AI").Enabled, Is.EqualTo(versusAI));
            Assert.That(GetField<ComboBox>(dialog, "gameType_AIDifficulty").Enabled, Is.EqualTo(versusAI));
        });
    }

    [Test]
    public void SwitchingBackToTwoPlayersIgnoresComputerSelection()
    {
        using var dialog = new NewGameDialog();
        GetField<RadioButton>(dialog, "gameType_VsAI").Checked = true;
        GetField<RadioButton>(dialog, "playersInformation_Player1AI").Checked = true;
        GetField<RadioButton>(dialog, "gameType_TwoPlayer").Checked = true;

        CloseWithResult(dialog, DialogResult.OK);

        Assert.Multiple(() =>
        {
            Assert.That(dialog.Player1!.IsAI, Is.False);
            Assert.That(dialog.Player2!.IsAI, Is.False);
        });
    }

    [TestCase(DialogResult.None)]
    [TestCase(DialogResult.Cancel)]
    public void ClosingWithoutAcceptingDoesNotCreatePlayers(DialogResult result)
    {
        using var dialog = new NewGameDialog();

        CloseWithResult(dialog, result);

        Assert.Multiple(() =>
        {
            Assert.That(dialog.Player1, Is.Null);
            Assert.That(dialog.Player2, Is.Null);
        });
    }

    [Test]
    public void CancellingReusedDialogPreservesLastAcceptedSettings()
    {
        using var dialog = new NewGameDialog();
        CloseWithResult(dialog, DialogResult.OK);
        var first = dialog.Player1;
        var second = dialog.Player2;
        GetField<RadioButton>(dialog, "gameType_VsAI").Checked = true;
        GetField<TextBox>(dialog, "playersInformation_Player1Name").Text = "Changed";
        GetField<ComboBox>(dialog, "gameType_AIDifficulty").SelectedIndex = 3;

        CloseWithResult(dialog, DialogResult.Cancel);

        Assert.Multiple(() =>
        {
            Assert.That(dialog.Player1, Is.SameAs(first));
            Assert.That(dialog.Player2, Is.SameAs(second));
            Assert.That(dialog.GameType, Is.EqualTo(Game.GameType.TwoPlayers));
            Assert.That(dialog.AIDifficulty, Is.EqualTo(Game.AIDifficulty.Medium));
        });
    }

    private static void CloseWithResult(NewGameDialog dialog, DialogResult result)
    {
        dialog.DialogResult = result;
        // Exercise the commit handler without showing a window or entering a modal message loop.
        Invoke(dialog, "NewGameDialog_FormClosed", dialog,
            new FormClosedEventArgs(CloseReason.UserClosing));
    }
}
