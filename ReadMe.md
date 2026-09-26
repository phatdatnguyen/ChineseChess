Chinese Chess
=======

### *Author:* Dat Nguyen
  
### Introduction
  
Chinese Chess (or Xiangqi) is a strategy board game for two players. The game represents a battle between two armies, with the objective of capturing the enemy's general.

![Main window](/Images/MainWindow.png)

In this program, we can play 1 vs. 1 or against AI. The AI is based on minimax algorithm with alpha-beta pruning.

### Build and tests

On Windows with a .NET SDK that supports .NET 9:

```powershell
dotnet build ChineseChess.csproj -c Release
dotnet test tests/ChineseChess.Tests/ChineseChess.Tests.csproj -c Release
```

The NUnit suite replaces the original console regression runner. NuGet restores the NUnit framework, Visual Studio adapter, .NET test SDK, and Coverlet collector automatically. The test project is included in the local solution and can also run from Visual Studio's **Test Explorer**.

Tests require Windows and the .NET 9 Desktop Runtime because the app's pieces and board create WinForms controls. They use hidden controls and intercept winner notifications, so no windows or dialogs need to be dismissed. Tests run on an STA thread without parallel execution because the app shares `Program.ChessBoard`; each game test disposes its controls and restores that shared state.

Coverage includes every piece's movement and captures for both sides, blockers and boundaries, legal moves and checks, checkmate/stalemate, board evaluation and simulation/undo, all AI difficulties, game and handicap setup, mouse input, dialog settings, and control disposal. Board and piece tests are unit tests; tests that connect the game to forms are integration tests. The suite does not automate a visible desktop or test visual layout.

To run just one fixture:

```powershell
dotnet test tests/ChineseChess.Tests/ChineseChess.Tests.csproj -c Release --filter FullyQualifiedName~PieceMovementTests
```

To collect a Cobertura coverage report under `TestResults`:

```powershell
dotnet test tests/ChineseChess.Tests/ChineseChess.Tests.csproj -c Release --collect "XPlat Code Coverage" --results-directory TestResults
```

To time an AI opening at each difficulty:

```powershell
dotnet test tests/ChineseChess.Tests/ChineseChess.Tests.csproj -c Release --filter TestCategory=Benchmark --logger "console;verbosity=normal"
```

The four benchmark cases are explicitly selected measurements and are skipped in ordinary runs. They report timings without a machine-dependent speed threshold. Threading is configured using NUnit's [STA support](https://docs.nunit.org/articles/nunit/writing-tests/attributes/apartment.html) and [nonparallel execution](https://docs.nunit.org/articles/nunit/writing-tests/attributes/nonparallelizable.html).
 
For more information, please visit the links below.

### Links

* [Download](https://github.com/phatdatnguyen/ChineseChess/releases/)
* [Xiangqi](https://en.wikipedia.org/wiki/Xiangqi)
* [Minimax algorithm](https://en.wikipedia.org/wiki/Minimax)
* [Alpha–beta pruning](https://en.wikipedia.org/wiki/Alpha%E2%80%93beta_pruning)
