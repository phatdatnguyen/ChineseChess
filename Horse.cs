using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseChess
{
    class Horse : Piece
    {
        #region Contructor
        public Horse(Board board, Board.Side side, int rank, int file) : base(board, side, rank, file)
        {
            name = "Horse";
            if (side == Board.Side.Blue)
                image.Image = Properties.Resources.BlueHorse;
            else
                image.Image = Properties.Resources.RedHorse;
            SelectionChanged += new SelectionChangedEventHandler(OnSelectionChanged);
            relativeValue = 40;
        }
        #endregion

        #region Event handlers
        public void OnSelectionChanged(Object sender, EventArgs e)
        {
            if (isSelected)
            {
                if (Side == Board.Side.Blue)
                    image.Image = Properties.Resources.BlueHorse_Selected;
                else
                    image.Image = Properties.Resources.RedHorse_Selected;
            }
            else
            {
                if (Side == Board.Side.Blue)
                    image.Image = Properties.Resources.BlueHorse;
                else
                    image.Image = Properties.Resources.RedHorse;
            }
        }
        #endregion

        #region Methods
        public override List<Move> FindPossibleMoves()
        {
            List<Move> possibleMoves = new List<Move>();
            Cell adjacentCell;
            Cell targetCell;

            //Go northwest
            if (rank > 0 && file > 1)
            {
                adjacentCell = board.Cells[rank, file - 1];
                if (adjacentCell.IsEmpty)
                {
                    targetCell = board.Cells[rank - 1, file - 2];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }
            }
            if (rank > 1 && file > 0)
            {
                adjacentCell = board.Cells[rank - 1, file];
                if (adjacentCell.IsEmpty)
                {
                    targetCell = board.Cells[rank - 2, file - 1];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }
            }
            
            //Go northeast
            if (rank > 1 && file < 8)
            {
                adjacentCell = board.Cells[rank - 1, file];
                if (adjacentCell.IsEmpty)
                {
                    targetCell = board.Cells[rank - 2, file + 1];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }
            }
            if (rank > 0 && file < 7)
            {
                adjacentCell = board.Cells[rank, file + 1];
                if (adjacentCell.IsEmpty)
                {
                    targetCell = board.Cells[rank - 1, file + 2];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }
            }

            //Go southeast
            if (rank < 9 && file < 7)
            {
                adjacentCell = board.Cells[rank, file + 1];
                if (adjacentCell.IsEmpty)
                {
                    targetCell = board.Cells[rank + 1, file + 2];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }
            }
            if (rank < 8 && file < 8)
            {
                adjacentCell = board.Cells[rank + 1, file];
                if (adjacentCell.IsEmpty)
                {
                    targetCell = board.Cells[rank + 2, file + 1];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }
            }

            //Go southweast
            if (rank < 8 && file > 0)
            {
                adjacentCell = board.Cells[rank + 1, file];
                if (adjacentCell.IsEmpty)
                {
                    targetCell = board.Cells[rank + 2, file - 1];
                    if (targetCell.IsEmpty)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }
            }
            if (rank < 9 && file > 1)
            {
                adjacentCell = board.Cells[rank, file - 1];
                if (adjacentCell.IsEmpty)
                {
                    targetCell = board.Cells[rank + 1, file - 2];
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
