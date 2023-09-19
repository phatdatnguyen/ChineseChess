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
            if (Program.ChessBoard == null || Program.ChessBoard.Game == null)
                return;

            //Move
            if (board.SelectedCell != null && board.SelectedCell.Piece != null)
            board.SelectedCell.Piece.Move(row, column);
            
            //Switch turn
            Program.ChessBoard.Game.SwitchTurn();

            //Enable all the pieces of the current player and disable all the pieces of the opponent
            foreach (Cell cell in board.Cells)
            {
                if (cell.Piece != null)
                {
                    if (cell.Piece.Side == Program.ChessBoard.Game.CurrentPlayer.Side)
                        cell.Piece.Image.Enabled = true;
                    else
                        cell.Piece.Image.Enabled = false;
                }
            }
        }
        #endregion
    }
}
