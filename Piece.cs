using System.Data.Common;

namespace ChineseChess
{
    public class Piece
    {
        #region Fields
        protected string name;
        protected Board board;
        protected Board.Side side;
        protected int rank;
        protected int file;
        protected bool isCaptured;
        protected bool isSelected;
        protected int relativeValue;
        protected PictureBox image;
        #endregion

        #region Properties
        public string Name
        {
            get { return name; }
        }
        public Board Board
        {
            get { return board; }
        }
        public Board.Side Side
        {
            get { return side; }
        }
        public int Rank
        {
            get { return rank; }
            set { rank = value; }
        }
        public int File
        {
            get { return file; }
            set { file = value; }
        }
        public bool IsCaptured
        {
            get { return isCaptured; }
            set { isCaptured = value; }
        }
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    OnSelectionChanged(EventArgs.Empty);
                }
            }
        }
        public int RelativeValue
        {
            get { return relativeValue; }
            set { relativeValue = value; }
        }
        public PictureBox Image
        {
            get { return image; }
        }
        #endregion;

        #region Events
        public delegate void SelectionChangedEventHandler(object sender, EventArgs e);
        public event SelectionChangedEventHandler? SelectionChanged;
        protected virtual void OnSelectionChanged(EventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
        #endregion

        #region Contructor
        public Piece(string name, Board board, Board.Side side, int rank, int file)
        {
            this.name = name;
            this.board = board;
            this.side = side;
            this.rank = rank;
            this.file = file;
            
            isCaptured = false;
            isSelected = false;

            image = new PictureBox
            {
                Width = Board.PieceSize,
                Height = Board.PieceSize,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Cursor = Cursors.Hand,
                Top = rank * Board.VerticalCellDistance + Board.PaddingTop,
                Left = file * Board.HorizontalCellDistance + Board.PaddingLeft,
                BackColor = Color.Transparent
            };
            image.MouseClick += new MouseEventHandler(Image_MouseClick);
            image.Tag = this;   //Refer to this piece using tag
        }
        #endregion

        #region Methods
        protected void Image_MouseClick(object? sender, MouseEventArgs e)
        {
            if (Program.ChessBoard == null || Program.ChessBoard.Game == null || sender == null)
                return;

            PictureBox image = (PictureBox)sender;      
            Piece? selectedPiece = (Piece?)image.Tag;
            
            //Remove the selected piece in handicap game type
            if (selectedPiece != null &&
                (Program.ChessBoard.Game.Type == Game.GameType.TwoPlayersHandicap || Program.ChessBoard.Game.Type == Game.GameType.TwoPlayersHandicap)
                && Program.ChessBoard.Game.Status == Game.GameStatus.NotStarted)
            {
                board.RemovePiece(selectedPiece);
                board.Cells[selectedPiece.Rank, selectedPiece.File].Piece = null;
                return;
            }

            if (selectedPiece != null && !board.IsSelected)
            {
                //Select the piece, find possible moves
                selectedPiece.IsSelected = true;
                board.SelectedCell = board.Cells[selectedPiece.Rank, selectedPiece.File];
                List<Move> possibleMoves = selectedPiece.FindPossibleMoves();

                //Disable undo button
                Program.ChessBoard.UndoButton.Enabled = false;

                //Disable all the pieces
                foreach (Cell cell in board.Cells)
                {
                    if (cell.Piece != null)
                        cell.Piece.Image.Enabled = false;
                }

                //Enable the selected piece (for deselection)
                selectedPiece.Image.Enabled = true;

                //Show the indicators for possible moves and enable the possible pieces for capturing
                foreach (Move move in possibleMoves)
                {
                    if (move.CapturedPiece == null)
                        board.Cells[move.EndRow, move.EndColumn].PossibleMoveIndicator.Visible = true;
                    else
                        move.CapturedPiece.Image.Enabled = true;
                }
            }
            else
            {
                if (board.SelectedCell != null && board.SelectedCell.Piece == this) //Reselect the piece
                {
                    //Hide all the move indicators. Enable all the pieces of the current player and disable all the pieces of the opponent
                    foreach (Cell cell in board.Cells)
                    {
                        cell.PossibleMoveIndicator.Visible = false;
                        if (cell.Piece != null && selectedPiece != null)
                        {
                            if (cell.Piece.Side == selectedPiece.Side)
                                cell.Piece.Image.Enabled = true;
                            else
                                cell.Piece.Image.Enabled = false;
                        }
                    }

                    //Deselect the piece
                    board.SelectedCell.Piece.IsSelected = false;
                    board.SelectedCell = null;

                    //Enable undo button
                    if (Program.ChessBoard.Game.Type == Game.GameType.TwoPlayers || Program.ChessBoard.Game.Type == Game.GameType.TwoPlayersHandicap)
                    {
                        if (Program.ChessBoard.Game.Moves.Count > 0)
                            Program.ChessBoard.UndoButton.Enabled = true;
                    }
                    else
                    {
                        if ((Program.ChessBoard.Game.Moves.Count == 0) || (Program.ChessBoard.Game.Moves.Count == 1 && Program.ChessBoard.Game.Player1.IsAI))
                            Program.ChessBoard.UndoButton.Enabled = false;
                        else
                            Program.ChessBoard.UndoButton.Enabled = true;
                    }
                }
                else //Select different piece
                {
                    //Capture
                    if (board.SelectedCell != null && board.SelectedCell.Piece!= null && selectedPiece != null)
                        board.SelectedCell.Piece.Capture(selectedPiece);

                    if (Program.ChessBoard.Game.Status != Game.GameStatus.Ended)
                    {
                        //Switch turn
                        Program.ChessBoard.Game.SwitchTurn();

                        //Enable all the pieces of the current player and disable all the pieces of the opposite side
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
                }
            }
        }
       
        public virtual List<Move> FindPossibleMoves()
        {
            return new List<Move>();
        }

        public void Move(int row, int column)
        {
            //Create the move
            Move move;
            if (side == Board.Side.Blue)
                move = new Move(rank, file, row, column, this);
            else
                move = new Move(rank, file, row, column, this);
            
            //Perform the move
            board.DoMove(move);

            //Add to moves list
            if (Program.ChessBoard != null && Program.ChessBoard.Game != null)
                Program.ChessBoard.Game.Moves.Add(move);
        }

        public void Capture(Piece piece)
        {
            //Create the move
            Move move;
            if (side == Board.Side.Blue)
                move = new Move(rank, file, piece.Rank, piece.File, this, piece);
            else
                move = new Move(rank, file, piece.Rank, piece.File, this, piece);
            
            //Perform the move
            board.DoMove(move);

            //Add to moves list
            if (Program.ChessBoard != null && Program.ChessBoard.Game != null)
                Program.ChessBoard.Game.Moves.Add(move);
        }
        #endregion
    }
}
