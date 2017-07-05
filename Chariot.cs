using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseChess
{
    public class Chariot : Piece
    {
        #region Contructor
        public Chariot(Board board, Board.Side side, int rank, int file) : base(board, side, rank, file)
        {
            name = "Chariot";
            if (side == Board.Side.Blue)
                image.Image = Properties.Resources.BlueChariot;
            else
                image.Image = Properties.Resources.RedChariot;
            SelectionChanged += new SelectionChangedEventHandler(OnSelectionChanged);
            relativeValue = 90;
        }
        #endregion

        #region Event handlers
        public void OnSelectionChanged(Object sender, EventArgs e)
        {
            if (isSelected)
            {
                if (Side == Board.Side.Blue)
                    image.Image = Properties.Resources.BlueChariot_Selected;
                else
                    image.Image = Properties.Resources.RedChariot_Selected;
            }
            else
            {
                if (Side == Board.Side.Blue)
                    image.Image = Properties.Resources.BlueChariot;
                else
                    image.Image = Properties.Resources.RedChariot;
            }
        }
        #endregion

        #region Methods
        public override List<Move> FindPossibleMoves()
        {
            List<Move> possibleMoves = new List<Move>();
            Cell targetCell;
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
                        possibleMoves.Add(new Move(rank, file, rank, targetColumn, this));
                    }
                    else
                    {
                        if (targetCell.Piece.Side != side)
                            possibleMoves.Add(new Move(rank, file, rank, targetColumn, this, targetCell.Piece));
                        break;
                    }
                    targetColumn -= 1;
                }
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
                        possibleMoves.Add(new Move(rank, file, rank, targetColumn, this));
                    }
                    else
                    {
                        if (targetCell.Piece.Side != side)
                            possibleMoves.Add(new Move(rank, file, rank, targetColumn, this, targetCell.Piece));
                        break;
                    }
                    targetColumn += 1;
                }
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
                        possibleMoves.Add(new Move(rank, file, targetRow, file, this));
                    }
                    else
                    {
                        if (targetCell.Piece.Side != side)
                            possibleMoves.Add(new Move(rank, file, targetRow, file, this, targetCell.Piece));
                        break;
                    }
                    targetRow -= 1;
                }
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
                        possibleMoves.Add(new Move(rank, file, targetRow, file, this));
                    }
                    else
                    {
                        if (targetCell.Piece.Side != side)
                            possibleMoves.Add(new Move(rank, file, targetRow, file, this, targetCell.Piece));
                        break;
                    }
                    targetRow += 1;
                }
            }

            return possibleMoves;
        }
        #endregion
    }
}
