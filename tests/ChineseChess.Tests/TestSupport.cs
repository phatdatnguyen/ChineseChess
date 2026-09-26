using System.Reflection;
using System.Runtime.ExceptionServices;
using Side = ChineseChess.Board.Side;

namespace ChineseChess.Tests;

// Private form wiring stays here; rule tests can use the real piece types directly.
internal static class TestSupport
{
    public static string Snapshot(Board board) => string.Join("|", board.Cells.Cast<Cell>().Select(cell =>
        cell.Piece is { } piece
            ? $"{cell.Row},{cell.Column}:{piece.Name},{piece.Side},{piece.Rank},{piece.File},{piece.RelativeValue},{piece.IsCaptured}"
            : $"{cell.Row},{cell.Column}:-"));

    public static bool HasMove(IEnumerable<Move> moves, int row, int column) =>
        moves.Any(move => move.EndRow == row && move.EndColumn == column);

    public static Side Opponent(Side side) => side == Side.Red ? Side.Blue : Side.Red;

    public static void ClickPiece(Piece piece, MouseButtons button = MouseButtons.Left) =>
        Invoke(piece, "Image_MouseClick", piece.Image, new MouseEventArgs(button, 1, 0, 0, 0));

    public static T GetField<T>(object instance, string name) => (T)FindField(instance.GetType(), name).GetValue(instance)!;

    public static void SetField(object instance, string name, object? value) =>
        FindField(instance.GetType(), name).SetValue(instance, value);

    public static object? Invoke(object instance, string name, params object[] args)
    {
        MethodInfo? method = null;
        for (Type? type = instance.GetType(); type != null && method == null; type = type.BaseType)
            method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        if (method == null)
            throw new MissingMethodException(instance.GetType().FullName, name);
        try
        {
            return method.Invoke(instance, args);
        }
        catch (TargetInvocationException exception) when (exception.InnerException != null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            throw;
        }
    }

    private static FieldInfo FindField(Type type, string name)
    {
        for (Type? current = type; current != null; current = current.BaseType)
        {
            FieldInfo? field = current.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (field != null) return field;
        }
        throw new MissingFieldException(type.FullName, name);
    }
}

internal sealed class GameTestContext : IDisposable
{
    private readonly Main? previousForm = Program.ChessBoard;
    public Main Form { get; } = new();
    public Board Board { get; }
    public Panel Panel { get; }

    public GameTestContext()
    {
        Program.ChessBoard = Form;
        Panel = TestSupport.GetField<Panel>(Form, "boardPanel");
        Board = new Board(Panel);
        TestSupport.SetField(Form, "board", Board);
    }

    public Piece Add(string name, Side side, int row, int column)
    {
        Piece piece = name switch
        {
            "Advisor" => new Advisor(Board, side, row, column),
            "Cannon" => new Cannon(Board, side, row, column),
            "Chariot" => new Chariot(Board, side, row, column),
            "Elephant" => new Elephant(Board, side, row, column),
            "General" => new General(Board, side, row, column),
            "Horse" => new Horse(Board, side, row, column),
            "Soldier" => new Soldier(Board, side, row, column),
            _ => throw new ArgumentException("Unknown piece type.", nameof(name))
        };
        Board.AddPiece(piece);
        return piece;
    }

    public void Clear() => TestSupport.Invoke(Board, "Clear");

    public TestGame CreateGame(Game.GameType type = Game.GameType.TwoPlayers, bool aiFirst = false,
        Game.AIDifficulty difficulty = Game.AIDifficulty.Easy)
    {
        bool versusAI = type is Game.GameType.VsAI or Game.GameType.VsAIHandicap;
        var game = new TestGame(Board, type,
            new Player("Red", Side.Red, versusAI && aiFirst),
            new Player("Blue", Side.Blue, versusAI && !aiFirst), difficulty);
        TestSupport.SetField(Form, "game", game);
        return game;
    }

    public void Dispose()
    {
        try
        {
            Form.Dispose(); // Main owns and disposes its board and dialog.
        }
        finally
        {
            Program.ChessBoard = previousForm!;
        }
    }
}

internal sealed class TestGame(Board board, Game.GameType type, Player first, Player second, Game.AIDifficulty difficulty)
    : Game(board, type, first, second, difficulty)
{
    public Player? Winner { get; private set; }
    public int WinnerNotifications { get; private set; }

    protected override void ShowWinner(Player winner)
    {
        Winner = winner;
        WinnerNotifications++;
    }
}
