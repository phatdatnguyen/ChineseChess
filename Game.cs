namespace ChineseChess
{
    public class Game
    {
        #region Enums
        public enum GameType { TwoPlayers, TwoPlayersHandicap, VsAI, VsAIHandicap }
        public enum GameStatus { NotStarted, Started, Ended }
        public enum AIDifficulty { Easy, Medium, Hard, Extreme }
        #endregion

        #region Fields
        private readonly Board board;
        private readonly GameType type;
        private GameStatus status;
        private readonly Player player1;
        private readonly Player player2;
        private Player currentPlayer;
        private Board.Side aiPlayerSide;
        private Board.Side humanPlayerSide;
        private Move? aiPlayerMove;
        private readonly List<Move> moves;
        private readonly AIDifficulty aiDifficulty;
        #endregion

        #region Properties
        public GameType Type
        {
            get { return type; }
        }
        public GameStatus Status
        {
            get { return status; }
            set { status = value; }
        }
        public Player Player1
        {
            get { return player1; }
        }
        public Player Player2
        {
            get { return player2; }
        }
        public Player CurrentPlayer
        {
            get { return currentPlayer; }
        }
        public Board.Side AIPlayerSide
        {
            get { return aiPlayerSide; }
            set { aiPlayerSide = value; }
        }
        public Board.Side HumanPlayerSide
        {
            get { return humanPlayerSide; }
            set { humanPlayerSide = value; }
        }
        public List<Move> Moves
        {
            get { return moves; }
        }
        public AIDifficulty Difficulty
        {
            get { return aiDifficulty; }
        }
        #endregion

        #region Constructor
        public Game(Board board, GameType type, Player player1, Player player2, AIDifficulty Difficulty)
        {
            this.board = board;
            this.type = type;
            this.player1 = player1;
            this.player2 = player2;
            aiDifficulty = Difficulty;
            currentPlayer = player1;
            status = GameStatus.NotStarted;
            moves = new List<Move>();
            Program.ChessBoard.CurrentPlayerLabel.Text = "";
        }
        #endregion

        #region Methods
        private bool IsAIGame => type == GameType.VsAI || type == GameType.VsAIHandicap;

        public bool CanUndo => status != GameStatus.NotStarted && moves.Count > 0 &&
            (!IsAIGame || moves.Any(move => move.Piece.Side == humanPlayerSide));

        public void Start()
        {
            status = GameStatus.NotStarted;
            moves.Clear();
            aiPlayerMove = null;
            board.Reset();
            currentPlayer = player1;
            aiPlayerSide = player1.IsAI ? player1.Side : player2.Side;
            humanPlayerSide = player1.IsAI ? player2.Side : player1.Side;

            Program.ChessBoard.StartButton.Visible = false;
            Program.ChessBoard.StatusLabel.Text = "";
            Program.ChessBoard.CurrentPlayerLabel.Text = "";
            Program.ChessBoard.UndoButton.Enabled = false;

            if (type == GameType.TwoPlayersHandicap || type == GameType.VsAIHandicap)
            {
                Program.ChessBoard.StatusLabel.Text = "Choose the pieces to remove and press Start.";
                Program.ChessBoard.StartButton.Visible = true;
                foreach (Cell cell in board.Cells)
                {
                    if (cell.Piece != null)
                        cell.Piece.Image.Enabled = cell.Piece is not General;
                }
                return;
            }

            status = GameStatus.Started;
            UpdateTurnControls();
            if (IsAIGame && currentPlayer.IsAI)
                AIPlayerMove();
        }

        public void StartHandicap()
        {
            if (status != GameStatus.NotStarted ||
                (type != GameType.TwoPlayersHandicap && type != GameType.VsAIHandicap))
                return;

            if (board.IsCheckDelivered(Board.Side.Red) || board.IsCheckDelivered(Board.Side.Blue))
            {
                Program.ChessBoard.StatusLabel.Text = "Keep a piece between the generals before starting.";
                return;
            }

            Program.ChessBoard.StartButton.Visible = false;
            Program.ChessBoard.StatusLabel.Text = "";
            status = GameStatus.Started;
            UpdateTurnControls();
            if (!EndIfNoLegalMoves() && IsAIGame && currentPlayer.IsAI)
                AIPlayerMove();
        }

        public void UpdateTurnControls()
        {
            Program.ChessBoard.CurrentPlayerLabel.Text = currentPlayer.Name;
            Program.ChessBoard.CurrentPlayerLabel.ForeColor = currentPlayer.Side == Board.Side.Red ? Color.Red : Color.Blue;
            Program.ChessBoard.UndoButton.Enabled = CanUndo && !board.IsSelected;
            foreach (Cell cell in board.Cells)
            {
                if (cell.Piece != null)
                    cell.Piece.Image.Enabled = status == GameStatus.Started &&
                        (!IsAIGame || !currentPlayer.IsAI) && cell.Piece.Side == currentPlayer.Side;
            }
        }

        public void SwitchTurn()
        {
            if (status != GameStatus.Started)
                return;

            currentPlayer = currentPlayer == player1 ? player2 : player1;
            UpdateTurnControls();
            if (!EndIfNoLegalMoves() && IsAIGame && currentPlayer.IsAI)
                AIPlayerMove();
        }

        private bool EndIfNoLegalMoves()
        {
            if (board.HasLegalMoves(currentPlayer.Side))
                return false;

            // In Xiangqi, having no legal move is a loss, including stalemate.
            End(currentPlayer == player1 ? player2.Side : player1.Side);
            return true;
        }

        public void UndoLastTurn()
        {
            if (!CanUndo)
                return;

            // Against the computer, return to the position before the human's
            // last move, whether the final move belonged to the human or the AI.
            int firstMoveToUndo = IsAIGame
                ? moves.FindLastIndex(move => move.Piece.Side == humanPlayerSide)
                : moves.Count - 1;
            Player restoredPlayer = moves[firstMoveToUndo].Piece.Side == player1.Side ? player1 : player2;
            while (moves.Count > firstMoveToUndo)
            {
                Move move = moves[moves.Count - 1];
                board.UndoMove(move);
                moves.RemoveAt(moves.Count - 1);
            }

            currentPlayer = restoredPlayer;
            status = GameStatus.Started;
            aiPlayerMove = null;
            UpdateTurnControls();
        }

        public void End(Board.Side winSide)
        {
            status = GameStatus.Ended;
            foreach (Cell cell in board.Cells)
                cell.PossibleMoveIndicator.Visible = false;
            if (board.SelectedCell?.Piece != null)
                board.SelectedCell.Piece.IsSelected = false;
            board.SelectedCell = null;
            UpdateTurnControls();
            Player winner = player1.Side == winSide ? player1 : player2;
            Program.ChessBoard.StatusLabel.Text = winner.Name + " won!";
            ShowWinner(winner);
        }

        protected virtual void ShowWinner(Player winner)
        {
            MessageBox.Show(Program.ChessBoard, winner.Name + " won!", "Game over!", MessageBoxButtons.OK);
        }

        private const float MateScore = 100000;

        private float AlphaBeta(Board.Side side, float alpha, float beta, byte depth, byte maxDepth)
        {
            if (depth == 0)
                return board.HasLegalMoves(side) ? board.Evaluate(side) : -MateScore + maxDepth;

            List<Move> possibleMoves = board.FindPossibleMoves(side);
            if (possibleMoves.Count == 0)
                return -MateScore + (maxDepth - depth);

            // Search captures first to improve pruning, especially at higher difficulties.
            possibleMoves.Sort((left, right) =>
                (right.CapturedPiece?.RelativeValue ?? 0).CompareTo(left.CapturedPiece?.RelativeValue ?? 0));
            float bestValue = float.NegativeInfinity;
            foreach (Move testMove in possibleMoves)
            {
                float value;
                if (testMove.CapturedPiece is General)
                {
                    value = MateScore - (maxDepth - depth);
                }
                else
                {
                    board.DoMove(testMove, true);
                    try
                    {
                        Board.Side opponent = side == Board.Side.Red ? Board.Side.Blue : Board.Side.Red;
                        value = -AlphaBeta(opponent, -beta, -alpha, (byte)(depth - 1), maxDepth);
                    }
                    finally
                    {
                        board.UndoMove(testMove, true);
                    }
                }
                if (bestValue < value)
                {
                    bestValue = value;
                    if (depth == maxDepth)
                        aiPlayerMove = testMove;
                }
                alpha = Math.Max(alpha, value);
                if (beta <= alpha)
                    break;
            }
            return bestValue;
        }

        public void AIPlayerMove()
        {
            if (status != GameStatus.Started || !IsAIGame || !currentPlayer.IsAI)
                return;

            byte depth = aiDifficulty switch
            {
                AIDifficulty.Easy => 2,
                AIDifficulty.Medium => 3,
                AIDifficulty.Hard => 4,
                AIDifficulty.Extreme => 5,
                _ => 3
            };

            // A previous search's result must never survive a turn with no moves.
            aiPlayerMove = null;
            AlphaBeta(aiPlayerSide, float.NegativeInfinity, float.PositiveInfinity, depth, depth);
            Move? chosenMove = aiPlayerMove;
            if (chosenMove == null)
            {
                End(humanPlayerSide);
                return;
            }

            moves.Add(chosenMove);
            board.DoMove(chosenMove);
            if (status == GameStatus.Started)
                SwitchTurn();
        }
        #endregion
    }
}
