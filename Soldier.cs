﻿namespace ChineseChess
{
    class Soldier : Piece
    {
        #region Contructor
        public Soldier(Board board, Board.Side side, int rank, int file) : base("Soldier", board, side, rank, file)
        {
            if (side == Board.Side.Blue)
                image.Image = Properties.Resources.BlueSoldier;
            else
                image.Image = Properties.Resources.RedSoldier;
            SelectionChanged += new SelectionChangedEventHandler(OnSelectionChanged);
            relativeValue = 10;
        }
        #endregion

        #region Methods
        public void OnSelectionChanged(Object sender, EventArgs e)
        {
            if (isSelected)
            {
                if (Side == Board.Side.Blue)
                    image.Image = Properties.Resources.BlueSoldier_Selected;
                else
                    image.Image = Properties.Resources.RedSoldier_Selected;
            }
            else
            {
                if (Side == Board.Side.Blue)
                    image.Image = Properties.Resources.BlueSoldier;
                else
                    image.Image = Properties.Resources.RedSoldier;
            }
        }
        
        public override List<Move> FindPossibleMoves()
        {
            List<Move> possibleMoves = new();
            Cell targetCell;

            if (side == Board.Side.Blue) //Blue side
            {
                if (rank < 5) //Not passed the river
                {
                    //Go south
                    targetCell = board.Cells[rank + 1, file];
                    if (targetCell.Piece == null)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }
                else //Passed the river
                {
                    //Go west
                    if (file > 0)
                    {
                        targetCell = board.Cells[rank, file - 1];
                        if (targetCell.Piece == null)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                        else if (targetCell.Piece.Side != side)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                    }

                    //Go east
                    if (file < 8)
                    {
                        targetCell = board.Cells[rank, file + 1];
                        if (targetCell.Piece == null)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                        else if (targetCell.Piece.Side != side)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                    }

                    //Go south
                    if (rank < 9)
                    {
                        targetCell = board.Cells[rank + 1, file];
                        if (targetCell.Piece == null)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                        else if (targetCell.Piece.Side != side)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                    }
                }
            }
            else //Red side
            {
                if (rank > 4) //Not passed the river
                {
                    //Go north
                    targetCell = board.Cells[rank - 1, file];
                    if (targetCell.Piece == null)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                    else if (targetCell.Piece.Side != side)
                        possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                }
                else //Passed the river
                {
                    //Go west
                    if (file > 0)
                    {
                        targetCell = board.Cells[rank, file - 1];
                        if (targetCell.Piece == null)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                        else if (targetCell.Piece.Side != side)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                    }

                    //Go east
                    if (file < 8)
                    {
                        targetCell = board.Cells[rank, file + 1];
                        if (targetCell.Piece == null)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                        else if (targetCell.Piece.Side != side)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                    }

                    //Go north
                    if (rank > 0)
                    {
                        targetCell = board.Cells[rank - 1, file];
                        if (targetCell.Piece == null)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                        else if (targetCell.Piece.Side != side)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                    }
                }
            }

            return possibleMoves;
        }
        #endregion
    }
}
