using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseChess
{
    class Cannon : Piece
    {
        #region Contructor
        public Cannon(Board board, Board.Side side, int rank, int file) : base(board, side, rank, file)
        {
            name = "Cannon";
            if (side == Board.Side.Blue)
                image.Image = Properties.Resources.BlueCannon;
            else
                image.Image = Properties.Resources.RedCannon;
            SelectionChanged += new SelectionChangedEventHandler(OnSelectionChanged);
            relativeValue = 45;
        }
        #endregion

        #region Event handlers
        public void OnSelectionChanged(Object sender, EventArgs e)
        {
            if (IsSelected)
            {
                if (Side == Board.Side.Blue)
                    image.Image = Properties.Resources.BlueCannon_Selected;
                else
                    image.Image = Properties.Resources.RedCannon_Selected;
            }
            else
            {
                if (Side == Board.Side.Blue)
                    image.Image = Properties.Resources.BlueCannon;
                else
                    image.Image = Properties.Resources.RedCannon;
            }
        }
        #endregion

        #region Methods
        public override List<Move> FindPossibleMoves()
        {
            List<Move> possibleMoves = new List<Move>();
            Cell targetCell;
            Piece screen = null;
            int targetColumn;
            int targetRow;

            //Go west
            if (file > 0)
            {
                targetColumn = file - 1;
                while (targetColumn >= 0)
                {
                    targetCell = board.Cells[rank, targetColumn];
                    if (targetCell.IsEmpty)
                    {
                        possibleMoves.Add(new Move(rank, file, rank, targetColumn, this, targetCell.Piece));
                    }
                    else
                    {
                        screen = targetCell.Piece;
                        break;
                    }
                    
                    targetColumn -= 1;
                }
                if (screen != null && screen.File > 0)
                {
                    targetColumn = screen.File - 1;
                    while (targetColumn >= 0)
                    {
                        targetCell = board.Cells[rank, targetColumn];
                        if (!targetCell.IsEmpty)
                        {
                            if (targetCell.Piece.Side != side)
                                possibleMoves.Add(new Move(rank, file, rank, targetColumn, this, targetCell.Piece));
                            break;
                        }
                        targetColumn -= 1;
                    }
                }
                screen = null;
            }

            //Go east
            if (file < 8)
            {
                targetColumn = file + 1;
                while (targetColumn <= 8)
                {
                    targetCell = board.Cells[rank, targetColumn];
                    if (targetCell.IsEmpty)
                    {
                        possibleMoves.Add(new Move(rank, file, rank, targetColumn, this, targetCell.Piece));
                    }
                    else
                    {
                        screen = targetCell.Piece;
                        break;
                    }
                    targetColumn += 1;
                }
                if (screen != null && screen.File < 8)
                {
                    targetColumn = screen.File + 1;
                    while (targetColumn <= 8)
                    {
                        targetCell = board.Cells[rank, targetColumn];
                        if (!targetCell.IsEmpty)
                        {
                            if (targetCell.Piece.Side != side)
                                possibleMoves.Add(new Move(rank, file, rank, targetColumn, this, targetCell.Piece));
                            break;
                        }
                        targetColumn += 1;
                    }
                }
                screen = null;
            }

            //Go north
            if (rank > 0)
            {
                targetRow = rank - 1;
                while (targetRow >= 0)
                {
                    targetCell = board.Cells[targetRow, file];
                    if (targetCell.IsEmpty)
                    {
                        possibleMoves.Add(new Move(rank, file, targetRow, file, this, targetCell.Piece));
                    }
                    else
                    {
                        screen = targetCell.Piece;
                        break;
                    }
                    targetRow -= 1;
                }
                if (screen != null && screen.Rank > 0)
                {
                    targetRow = screen.Rank - 1;
                    while (targetRow >= 0)
                    {
                        targetCell = board.Cells[targetRow, file];
                        if (!targetCell.IsEmpty)
                        {
                            if (targetCell.Piece.Side != side)
                                possibleMoves.Add(new Move(rank, file, targetRow, file, this, targetCell.Piece));
                            break;
                        }
                        targetRow -= 1;
                    }
                }
                screen = null;
            }

            //Go south
            if (rank < 9)
            {
                targetRow = rank + 1;
                while (targetRow <= 9)
                {
                    targetCell = board.Cells[targetRow, file];
                    if (targetCell.IsEmpty)
                    {
                        possibleMoves.Add(new Move(rank, file, targetRow, file, this, targetCell.Piece));
                    }
                    else
                    {
                        screen = targetCell.Piece;
                        break;
                    }
                    targetRow += 1;
                }
                if (screen != null && screen.Rank < 9)
                {
                    targetRow = screen.Rank + 1;
                    while (targetRow <= 9)
                    {
                        targetCell = board.Cells[targetRow, file];
                        if (!targetCell.IsEmpty)
                        {
                            if (targetCell.Piece.Side != side)
                                possibleMoves.Add(new Move(rank, file, targetRow, file, this, targetCell.Piece));
                            break;
                        }
                        targetRow += 1;
                    }
                }
                screen = null;
            }

            return possibleMoves;
        }
        #endregion
    }
}
