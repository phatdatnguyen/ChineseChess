using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ChineseChess
{
    public class Board
    {
        #region Enums
        public enum Side { Red, Blue }
        #endregion

        #region Fields
        private Panel boardPanel;
        private Cell[,] cells;
        private Cell selectedCell;
        #endregion

        #region Properties
        public Cell SelectedCell
        {
            get { return selectedCell; }
            set { selectedCell = value; }
        }
        public bool IsSelected
        {
            get { return selectedCell != null; }
        }
        public Cell[,] Cells
        {
            get { return cells; }
        }
        #endregion

        #region Contructor
        public Board(Panel boardPanel)
        {
            this.boardPanel = boardPanel;
            cells = new Cell[10, 9];
            Clear();
        }
        #endregion

        #region Methods
        private void Clear()
        {
            boardPanel.Controls.Clear();
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 9; j++)
                {
                    cells[i, j] = new Cell(this, i, j);
                    boardPanel.Controls.Add(cells[i, j].PossibleMoveIndicator);
                }
            selectedCell = null;
        }

        public void Reset()
        {
            Clear();

            //Create pieces for blue side
            AddPiece(new Chariot(this, Board.Side.Blue, 0, 0));
            AddPiece(new Horse(this, Board.Side.Blue, 0, 1));
            AddPiece(new Elephant(this, Board.Side.Blue, 0, 2));
            AddPiece(new Advisor(this, Board.Side.Blue, 0, 3));
            AddPiece(new General(this, Board.Side.Blue, 0, 4));
            AddPiece(new Advisor(this, Board.Side.Blue, 0, 5));
            AddPiece(new Elephant(this, Board.Side.Blue, 0, 6));
            AddPiece(new Horse(this, Board.Side.Blue, 0, 7));
            AddPiece(new Chariot(this, Board.Side.Blue, 0, 8));
            AddPiece(new Cannon(this, Board.Side.Blue, 2, 1));
            AddPiece(new Cannon(this, Board.Side.Blue, 2, 7));
            AddPiece(new Soldier(this, Board.Side.Blue, 3, 0));
            AddPiece(new Soldier(this, Board.Side.Blue, 3, 2));
            AddPiece(new Soldier(this, Board.Side.Blue, 3, 4));
            AddPiece(new Soldier(this, Board.Side.Blue, 3, 6));
            AddPiece(new Soldier(this, Board.Side.Blue, 3, 8));

            //Create pieces for red side
            AddPiece(new Soldier(this, Board.Side.Red, 6, 0));
            AddPiece(new Soldier(this, Board.Side.Red, 6, 2));
            AddPiece(new Soldier(this, Board.Side.Red, 6, 4));
            AddPiece(new Soldier(this, Board.Side.Red, 6, 6));
            AddPiece(new Soldier(this, Board.Side.Red, 6, 8));
            AddPiece(new Cannon(this, Board.Side.Red, 7, 1));
            AddPiece(new Cannon(this, Board.Side.Red, 7, 7));
            AddPiece(new Chariot(this, Board.Side.Red, 9, 0));
            AddPiece(new Horse(this, Board.Side.Red, 9, 1));
            AddPiece(new Elephant(this, Board.Side.Red, 9, 2));
            AddPiece(new Advisor(this, Board.Side.Red, 9, 3));
            AddPiece(new General(this, Board.Side.Red, 9, 4));
            AddPiece(new Advisor(this, Board.Side.Red, 9, 5));
            AddPiece(new Elephant(this, Board.Side.Red, 9, 6));
            AddPiece(new Horse(this, Board.Side.Red, 9, 7));
            AddPiece(new Chariot(this, Board.Side.Red, 9, 8));
        }

        public void AddPiece(Piece piece)
        {
            if (piece != null)
            {
                cells[piece.Rank, piece.File].Piece = piece;
                boardPanel.Controls.Add(piece.Image);
            }
        }
        
        public void RemovePiece(Piece piece)
        {
            if (piece != null)
            {
                boardPanel.Controls.Remove(piece.Image);
            }
        }

        public void DoMove(Move move, bool isTestMove = false)
        {
            //Capture the piece
            if (move.CapturedPiece != null)
                move.CapturedPiece.IsCaptured = true;

            //Move
            cells[move.EndRow, move.EndColumn].Piece = move.Piece;
            cells[move.StartRow, move.StartColumn].Piece = null;
            move.Piece.Rank = move.EndRow;
            move.Piece.File = move.EndColumn;
            
            //Change relative values for soldiers that cross the river
            if (move.Piece.GetType() == typeof(Soldier))
            {
                if (move.Piece.Side == Side.Blue && move.StartRow == 4)
                    move.Piece.RelativeValue = 20;

                if (move.Piece.Side == Side.Red && move.StartRow == 5)
                    move.Piece.RelativeValue = 20;
            }

            //If this is a real move, make visual changes and check end game
            if(!isTestMove)
            {
                //Hide all the move indicators
                foreach (Cell cell in cells)
                    cell.PossibleMoveIndicator.Visible = false;

                //Deselect
                move.Piece.IsSelected = false;
                selectedCell = null;

                //Remove the captured piece
                if (move.CapturedPiece != null)
                    RemovePiece(move.CapturedPiece);

                //Position the images
                move.Piece.Image.Top = move.Piece.Rank * 42 + 14;
                move.Piece.Image.Left = move.Piece.File * 42 + 23;
                move.Piece.Image.Invalidate();
                move.Piece.Image.Update();

                //Enable undo button
                if (Program.ChessBoard.Game.Type == Game.GameType.TwoPlayers || Program.ChessBoard.Game.Type == Game.GameType.TwoPlayersHandicap)
                {
                    Program.ChessBoard.UndoButton.Enabled = true;
                }
                else
                {
                    if (Program.ChessBoard.Game.CurrentPlayer.IsAI && Program.ChessBoard.Game.Moves.Count > 0)
                        Program.ChessBoard.UndoButton.Enabled = true;
                    else
                        Program.ChessBoard.UndoButton.Enabled = false;
                }

                //Check whether a check is delivered
                if (IsCheckDelivered(move.Piece.Side))
                    Program.ChessBoard.StatusLabel.Text = "Check!";
                else
                    Program.ChessBoard.StatusLabel.Text = "";

                //Check end game
                if (move.CapturedPiece != null && move.CapturedPiece.GetType() == typeof(General))
                {
                    Program.ChessBoard.Game.Status = Game.GameStatus.Ended;
                    Program.ChessBoard.UndoButton.Enabled = true;
                    Program.ChessBoard.Game.End(move.Piece.Side);
                }
            }
        }

        public void UndoMove(Move move, bool isTestMove = false)
        {
            //Unmove
            cells[move.StartRow, move.StartColumn].Piece = move.Piece;
            move.Piece.Rank = move.StartRow;
            move.Piece.File = move.StartColumn;

            //Restore captured piece
            if (move.CapturedPiece != null)
            {
                move.CapturedPiece.IsCaptured = false;
                cells[move.EndRow, move.EndColumn].Piece = move.CapturedPiece;
            }
            else
            {
                cells[move.EndRow, move.EndColumn].Piece = null;
            }

            //Unchange relative values
            if (move.Piece.GetType() == typeof(Soldier))
            {
                if (move.Piece.Side == Side.Blue && move.StartRow == 4)
                    move.Piece.RelativeValue = 10;

                if (move.Piece.Side == Side.Red && move.StartRow == 5)
                    move.Piece.RelativeValue = 10;
            }

            //If this is a real move, undo visual changes
            if (!isTestMove)
            {
                //Deselect
                move.Piece.IsSelected = false;
                selectedCell = null;
                
                //Position the images
                move.Piece.Image.Top = move.Piece.Rank * 42 + 14;
                move.Piece.Image.Left = move.Piece.File * 42 + 23;

                //Add the captured piece
                if (move.CapturedPiece != null)
                    AddPiece(move.CapturedPiece);
            }
        }

        public List<Move> FindPossibleMoves(Side side)
        {
            List<Move> possibleMoves = new List<Move>();

            foreach (Cell cell in cells)
            {
                if (!cell.IsEmpty && cell.Piece.Side == side)
                {
                    List<Move> moves = cell.Piece.FindPossibleMoves();
                    foreach (Move move in moves)
                        possibleMoves.Add(move);
                }
            }

            return possibleMoves;
        }
                
        public int Evaluate(Side side)
        {
            int value = 0;

            foreach (Cell cell in cells)
            {
                if (!cell.IsEmpty)
                {
                    if (cell.Piece.Side == side)
                        value += cell.Piece.RelativeValue;
                    else
                        value -= cell.Piece.RelativeValue;
                }
            }

            return value;
        }

        public bool IsCheckDelivered(Side side)
        {
            foreach (Cell cell in cells)
            {
                if (!cell.IsEmpty && cell.Piece.Side == side)
                {
                    if (cell.Piece.GetType() == typeof(Chariot) || cell.Piece.GetType() == typeof(Cannon) ||
                        cell.Piece.GetType() == typeof(Horse) || cell.Piece.GetType() == typeof(Soldier))
                    {
                        List<Move> moves = cell.Piece.FindPossibleMoves();
                        foreach (Move move in moves)
                        {
                            if (move.CapturedPiece != null && move.CapturedPiece.GetType() == typeof(General))
                                return true;
                        }
                    }
                }
            }

            return false;
        }
        #endregion
    }
}
