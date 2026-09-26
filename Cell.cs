namespace ChineseChess
{
    public class Cell
    {
        #region Fields
        private readonly Board board;
        private readonly int row;
        private readonly int column;
        private Piece? piece;
        private readonly PictureBox possibleMoveIndicator;
        #endregion

        #region Properties
        public Board Board
        {
            get { return board; }
        }
        public int Row
        {
            get { return row; }
        }
        public int Column
        {
            get { return column; }
        }
        public Piece? Piece
        {
            get { return piece; }
            set { piece = value; }
        }
        public PictureBox PossibleMoveIndicator
        {
            get { return possibleMoveIndicator; }
        }
        #endregion

        #region Constructor
        public Cell(Board board, int row, int column, Piece? piece = null)
        {
            this.board = board;
            this.row = row;
            this.column = column;
            this.piece = piece;

            possibleMoveIndicator = new PictureBox
            {
                Width = Board.PossibleMoveIndicatorSize,
                Height = Board.PossibleMoveIndicatorSize,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent,
                Top = row * Board.VerticalCellDistance + Board.PaddingTop + (Board.PieceSize - Board.PossibleMoveIndicatorSize) / 2,
                Left = column * Board.HorizontalCellDistance + Board.PaddingLeft + (Board.PieceSize - Board.PossibleMoveIndicatorSize) / 2,
                Cursor = Cursors.Hand,
                Image = Properties.Resources.PossibleMoveIndicator,
                Visible = false
            };
            possibleMoveIndicator.MouseClick += new MouseEventHandler(PossibleMoveIndicator_MouseClick);
        }
        #endregion

        #region Method
        private void PossibleMoveIndicator_MouseClick(object? sender, MouseEventArgs e)
        {
            Game? game = Program.ChessBoard?.Game;
            Piece? selectedPiece = board.SelectedCell?.Piece;
            if (e.Button != MouseButtons.Left || game == null || game.Status != Game.GameStatus.Started
                || game.CurrentPlayer.IsAI || selectedPiece == null || selectedPiece.Side != game.CurrentPlayer.Side)
                return;

            if (!board.FindLegalMoves(selectedPiece).Any(move => move.EndRow == row
                && move.EndColumn == column && move.CapturedPiece == null))
                return;

            selectedPiece.Move(row, column);
            if (game.Status == Game.GameStatus.Started)
                game.SwitchTurn();
            game.UpdateTurnControls();
        }
        #endregion
    }
}
