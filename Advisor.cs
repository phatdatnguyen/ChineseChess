using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseChess
{
    class Advisor : Piece
    {
        #region Contructor
        public Advisor(Board board, Board.Side side, int rank, int file) : base(board, side, rank, file)
        {
            name = "Advisor";
            if (side == Board.Side.Blue)
                image.Image = Properties.Resources.BlueAdvisor;
            else
                image.Image = Properties.Resources.RedAdvisor;
            SelectionChanged += new SelectionChangedEventHandler(OnSelectionChanged);
            relativeValue = 20;
        }
        #endregion

        #region Event handlers
        public void OnSelectionChanged(Object sender, EventArgs e)
        {
            if (isSelected)
            {
                if (Side == Board.Side.Blue)
                    image.Image = Properties.Resources.BlueAdvisor_Selected;
                else
                    image.Image = Properties.Resources.RedAdvisor_Selected;
            }
            else
            {
                if (Side == Board.Side.Blue)
                    image.Image = Properties.Resources.BlueAdvisor;
                else
                    image.Image = Properties.Resources.RedAdvisor;
            }
        }
        #endregion

        #region Methods
        public override List<Move> FindPossibleMoves()
        {
            List<Move> possibleMoves = new List<Move>();
            Cell targetCell;

            if (side == Board.Side.Blue) //Blue side
            {
                //Go northwest
                if (rank > 0 && file > 3)
                {
                    targetCell = board.Cells[rank - 1, file - 1];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }

                //Go northeast
                if (rank > 0 && file < 5)
                {
                    targetCell = board.Cells[rank - 1, file + 1];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }

                //Go southeast
                if (rank < 2 && file < 5)
                {
                    targetCell = board.Cells[rank + 1, file + 1];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }

                //Go southweast
                if (rank < 2 && file > 3)
                {
                    targetCell = board.Cells[rank + 1, file - 1];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }
            }
            else //Red side
            {
                //Go northwest
                if (rank > 7 && file > 3)
                {
                    targetCell = board.Cells[rank - 1, file - 1];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }

                //Go northeast
                if (rank > 7 && file < 5)
                {
                    targetCell = board.Cells[rank - 1, file + 1];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }

                //Go southeast
                if (rank < 9 && file < 5)
                {
                    targetCell = board.Cells[rank + 1, file + 1];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }

                //Go southweast
                if (rank < 9 && file > 3)
                {
                    targetCell = board.Cells[rank + 1, file - 1];
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
