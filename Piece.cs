using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

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
                    _OnSelectionChanged(EventArgs.Empty);
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
        public event SelectionChangedEventHandler SelectionChanged;
        protected virtual void _OnSelectionChanged(EventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, e);
        }
        #endregion

        #region Contructor
        public Piece(Board board, Board.Side side, int rank, int file)
        {
            this.board = board;
            this.side = side;
            this.rank = rank;
            this.file = file;
            
            isCaptured = false;
            isSelected = false;

            image = new PictureBox();
            image.Width = 32;
            image.Height = 32;
            image.SizeMode = PictureBoxSizeMode.StretchImage;
            image.Cursor = Cursors.Hand;
            image.Top = rank * 42 + 14;
            image.Left = file * 42 + 23;
            image.BackColor = Color.Transparent;
            image.MouseClick += new MouseEventHandler(image_MouseClick);
            image.Tag = this;   //Refer to this piece using tag
        }
        #endregion

        #region Event Handlers
        protected void image_MouseClick(Object sender, MouseEventArgs e)
        {
            PictureBox image = (PictureBox)sender;      
            Piece selectedPiece = (Piece)image.Tag;
            
            //Remove the selected piece in handicap game type
            if ((Program.ChessBoard.Game.Type == Game.GameType.TwoPlayersHandicap || Program.ChessBoard.Game.Type == Game.GameType.TwoPlayersHandicap)
                && Program.ChessBoard.Game.Status == Game.GameStatus.NotStarted)
            {
                board.RemovePiece(selectedPiece);
                board.Cells[selectedPiece.Rank, selectedPiece.File].Piece = null;
                return;
            }

            if (!board.IsSelected)
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
                    if (!cell.IsEmpty)
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
                if (board.SelectedCell.Piece == this) //Reselect the piece
                {
                    //Hide all the move indicators. Enable all the pieces of the current player and disable all the pieces of the opponent
                    foreach (Cell cell in board.Cells)
                    {
                        cell.PossibleMoveIndicator.Visible = false;
                        if (!cell.IsEmpty)
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
                    board.SelectedCell.Piece.Capture(selectedPiece);

                    if (Program.ChessBoard.Game.Status != Game.GameStatus.Ended)
                    {
                        //Switch turn
                        Program.ChessBoard.Game.SwitchTurn();

                        //Enable all the pieces of the current player and disable all the pieces of the opposite side
                        foreach (Cell cell in board.Cells)
                        {
                            if (!cell.IsEmpty)
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
        #endregion

        #region Methods
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
            Program.ChessBoard.Game.Moves.Add(move);
        }
        #endregion
    }
}
