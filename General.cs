using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseChess
{
    class General : Piece
    {
        #region Contructor
        public General(Board board, Board.Side side, int rank, int file) : base(board, side, rank, file)
        {
            name = "General";
            if (side == Board.Side.Blue)
                image.Image = Properties.Resources.BlueGeneral;
            else
                image.Image = Properties.Resources.RedGeneral;
            SelectionChanged += new SelectionChangedEventHandler(OnSelectionChanged);
            relativeValue = 1000;
        }
        #endregion

        #region Event handlers
        public void OnSelectionChanged(Object sender, EventArgs e)
        {
            if (isSelected)
            {
                if (Side == Board.Side.Blue)
                    image.Image = Properties.Resources.BlueGeneral_Selected;
                else
                    image.Image = Properties.Resources.RedGeneral_Selected;
            }
            else
            {
                if (Side == Board.Side.Blue)
                    image.Image = Properties.Resources.BlueGeneral;
                else
                    image.Image = Properties.Resources.RedGeneral;
            }
        }
        #endregion

        #region Methods
        public override List<Move> FindPossibleMoves()
        {
            List<Move> possibleMoves = new List<Move>();
            Cell targetCell;
            int targetRow;

            if (side == Board.Side.Blue) //Blue side
            {
                //Go west
                if (file > 3)
                {
                    targetCell = board.Cells[rank, file - 1];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }

                //Go east
                if (file < 5)
                {
                    targetCell = board.Cells[rank, file + 1];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }

                //Go north
                if (rank > 0)
                {
                    targetCell = board.Cells[rank - 1, file];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }

                //Go south
                targetRow = rank + 1;
                while (targetRow <= 9)
                {
                    targetCell = board.Cells[targetRow, file];
                    if (!targetCell.IsEmpty)
                    {
                        if ((targetCell.Piece.GetType() == typeof(General)) && !(targetCell.Piece.Side == side))
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                        break;
                    }
                    targetRow += 1;
                }
                if (rank < 2)
                {
                    targetCell = board.Cells[rank + 1, file];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }
            }
            else //Red side
            {
                //Go west
                if (file > 3)
                {
                    targetCell = board.Cells[rank, file - 1];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }

                //Go east
                if (file < 5)
                {
                    targetCell = board.Cells[rank, file + 1];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }

                //Go north
                targetRow = rank - 1;
                while (targetRow >= 0)
                {
                    targetCell = board.Cells[targetRow, file];
                    if (!targetCell.IsEmpty)
                    {
                        if ((targetCell.Piece.GetType() == typeof(General)) && !(targetCell.Piece.Side == side))
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                        break;
                    }
                    targetRow -= 1;
                }
                if (rank > 7)
                {
                    targetCell = board.Cells[rank - 1, file];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }

                //Go south
                if (rank < 9)
                {
                    targetCell = board.Cells[rank + 1, file];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }
            }

            return possibleMoves;
        }
        #endregion
    }
}
