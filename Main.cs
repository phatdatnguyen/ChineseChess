namespace ChineseChess
{
    public partial class Main : Form
    {
        #region Fields
        private Board? board;
        private Game? game;
        private readonly NewGameDialog newGameDialog;
        #endregion

        #region Properties
        public Game? Game
        {
            get { return game; }
        }
        public Label CurrentPlayerLabel
        {
            get { return currentPlayerLabel; }
        }
        public Button UndoButton
        {
            get { return undoButton; }
        }
        public Label StatusLabel
        {
            get { return statusLabel; }
        }
        public Button StartButton
        {
            get { return startButton; }
        }
        #endregion

        #region Constructor
        public Main()
        {
            InitializeComponent();

            newGameDialog = new NewGameDialog();
        }
        #endregion

        #region Methods
        private void Main_Load(object sender, EventArgs e)
        {
            NewGameButton_Click(newGameButton, EventArgs.Empty);
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            if (newGameDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (newGameDialog.Player1 != null && newGameDialog.Player2 != null)
                {
                    board = new Board(boardPanel);
                    game = new Game(board, newGameDialog.GameType, newGameDialog.Player1, newGameDialog.Player2, newGameDialog.AIDifficulty);
                    game?.Start();
                }
            }
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            if (board == null || game == null || game.Moves.Count == 0)
                return;

            if (game.Type == Game.GameType.TwoPlayers || game.Type == Game.GameType.TwoPlayersHandicap)
            {
                if (game.Status != Game.GameStatus.Ended)
                {
                    board.UndoMove(game.Moves.Last());
                    game.Moves.Remove(game.Moves.Last());

                    game.SwitchTurn();
                }
                else
                {
                    board.UndoMove(game.Moves.Last());
                    game.Moves.Remove(game.Moves.Last());
                    board.UndoMove(game.Moves.Last());
                    game.Moves.Remove(game.Moves.Last());

                    game.SwitchTurn();
                }

                if (game.Moves.Count == 0)
                {
                    undoButton.Enabled = false;
                }
            }
            else
            {
                if (game.Status != Game.GameStatus.Ended)
                {
                    board.UndoMove(game.Moves.Last());
                    game.Moves.Remove(game.Moves.Last());
                    board.UndoMove(game.Moves.Last());
                    game.Moves.Remove(game.Moves.Last());
                }
                else
                {
                    if (game.CurrentPlayer.Side == game.AIPlayerSide)
                    {
                        board.UndoMove(game.Moves.Last());
                        game.Moves.Remove(game.Moves.Last());
                        board.UndoMove(game.Moves.Last());
                        game.Moves.Remove(game.Moves.Last());

                        game.SwitchTurn();
                    }
                    else
                    {
                        board.UndoMove(game.Moves.Last());
                        game.Moves.Remove(game.Moves.Last());
                    }
                }

                if ((game.Moves.Count == 0) || (game.Moves.Count == 1 && game.Player1.IsAI))
                {
                    undoButton.Enabled = false;
                }
            }

            game.Status = Game.GameStatus.Started;

            //Enable all the pieces of the current player and disable all the pieces of the opposite side
            foreach (Cell cell in board.Cells)
            {
                if (cell.Piece != null)
                {
                    if (cell.Piece.Side == game.CurrentPlayer.Side)
                        cell.Piece.Image.Enabled = true;
                    else
                        cell.Piece.Image.Enabled = false;
                }
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (board == null || game == null)
                return;

            Program.ChessBoard.CurrentPlayerLabel.Text = game.Player1.Name;
            Program.ChessBoard.CurrentPlayerLabel.ForeColor = Color.Red;
            statusLabel.Text = "";
            startButton.Visible = false;
            game.Status = Game.GameStatus.Started;

            switch (game.Type)
            {
                case Game.GameType.TwoPlayersHandicap:
                    //Enable all the pieces of the current player and disable all the pieces of the opposite side
                    foreach (Cell cell in board.Cells)
                    {
                        if (cell.Piece != null)
                        {
                            if (cell.Piece.Side == game.CurrentPlayer.Side)
                                cell.Piece.Image.Enabled = true;
                            else
                                cell.Piece.Image.Enabled = false;
                        }
                    }
                    break;
                case Game.GameType.VsAIHandicap:
                    if (game.Player1.IsAI)
                    {
                        game.AIPlayerSide = game.Player1.Side;
                        game.HumanPlayerSide = game.Player2.Side;

                        game.AIPlayerMove();
                    }
                    else
                    {
                        game.AIPlayerSide = game.Player2.Side;
                        game.HumanPlayerSide = game.Player1.Side;
                    }

                    //Enable all the pieces of the current player and disable all the pieces of the opposite side
                    foreach (Cell cell in board.Cells)
                    {
                        if (cell.Piece != null)
                        {
                            if (cell.Piece.Side == game.CurrentPlayer.Side)
                                cell.Piece.Image.Enabled = true;
                            else
                                cell.Piece.Image.Enabled = false;
                        }
                    }
                    break;
            }
        }
        #endregion
    }
}
