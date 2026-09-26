using NUnit.Framework;

// Pieces create WinForms controls, and Game uses the shared Program.ChessBoard.
// Keep every fixture on an STA thread without parallel access to that state.
[assembly: Apartment(ApartmentState.STA)]
[assembly: NonParallelizable]

namespace ChineseChess.Tests;

[SetUpFixture]
public sealed class TestEnvironment
{
    [OneTimeSetUp]
    public void InitializeWinForms()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Control.CheckForIllegalCrossThreadCalls = true;
    }
}
