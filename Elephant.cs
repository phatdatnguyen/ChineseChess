namespace ChineseChess
{
    class Elephant : Piece
    {
        #region Contructor
        public Elephant(Board board, Board.Side side, int rank, int file) : base("Elephant", board, side, rank, file)
        {
            if (side == Board.Side.Blue)
                image.Image = Properties.Resources.BlueElephant;
            else
                image.Image = Properties.Resources.RedElephant;
            SelectionChanged += new SelectionChangedEventHandler(OnSelectionChanged);
            relativeValue = 20;
        }
        #endregion

        #region Methods
        public void OnSelectionChanged(Object sender, EventArgs e)
        {
            if (isSelected)
            {
                if (Side == Board.Side.Blue)
                    image.Image = Properties.Resources.BlueElephant_Selected;
                else
                    image.Image = Properties.Resources.RedElephant_Selected;
            }
            else
            {
                if (Side == Board.Side.Blue)
                    image.Image = Properties.Resources.BlueElephant;
                else
                    image.Image = Properties.Resources.RedElephant;
            }
        }
        
        public override List<Move> FindPossibleMoves()
        {
            List<Move> possibleMoves = new();
            Cell targetCell;
            Cell diagonallyAdjacentCell;
            
            if (side == Board.Side.Blue) //Blue side
            {
                //Go northwest
                if (rank > 1 && file > 1)
                {
                    diagonallyAdjacentCell = board.Cells[rank - 1, file - 1];
                    if (diagonallyAdjacentCell.Piece == null)
                    {
                        targetCell = board.Cells[rank - 2, file - 2];
                        if (targetCell.Piece == null)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                        else if (targetCell.Piece.Side != side)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                    }
                }

                //Go northeast
                if (rank > 1 && file < 7)
                {
                    diagonallyAdjacentCell = board.Cells[rank - 1, file + 1];
                    if (diagonallyAdjacentCell.Piece == null)
                    {
                        targetCell = board.Cells[rank - 2, file + 2];
                        if (targetCell.Piece == null)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                        else if (targetCell.Piece.Side != side)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                    }
                }

                //Go southeast
                if (rank < 3 && file < 7)
                {
                    diagonallyAdjacentCell = board.Cells[rank + 1, file + 1];
                    if (diagonallyAdjacentCell.Piece == null)
                    {
                        targetCell = board.Cells[rank + 2, file + 2];
                        if (targetCell.Piece == null)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                        else if (targetCell.Piece.Side != side)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                    }
                }

                //Go southweast
                if (rank < 3 && file > 1)
                {
                    diagonallyAdjacentCell = board.Cells[rank + 1, file - 1];
                    if (diagonallyAdjacentCell.Piece == null)
                    {
                        targetCell = board.Cells[rank + 2, file - 2];
                        if (targetCell.Piece == null)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                        else if (targetCell.Piece.Side != side)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                    }
                }
            }
            else //Red side
            {
                //Go northwest
                if (rank > 6 && file > 1)
                {
                    diagonallyAdjacentCell = board.Cells[rank - 1, file - 1];
                    if (diagonallyAdjacentCell.Piece == null)
                    {
                        targetCell = board.Cells[rank - 2, file - 2];
                        if (targetCell.Piece == null)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                        else if (targetCell.Piece.Side != side)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                    }
                }

                //Go northeast
                if (rank > 6 && file < 7)
                {
                    diagonallyAdjacentCell = board.Cells[rank - 1, file + 1];
                    if (diagonallyAdjacentCell.Piece == null)
                    {
                        targetCell = board.Cells[rank - 2, file + 2];
                        if (targetCell.Piece == null)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                        else if (targetCell.Piece.Side != side)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                    }
                }

                //Go southeast
                if (rank < 8 && file < 7)
                {
                    diagonallyAdjacentCell = board.Cells[rank + 1, file + 1];
                    if (diagonallyAdjacentCell.Piece == null)
                    {
                        targetCell = board.Cells[rank + 2, file + 2];
                        if (targetCell.Piece == null)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this));
                        else if (targetCell.Piece.Side != side)
                            possibleMoves.Add(new Move(rank, file, targetCell.Row, targetCell.Column, this, targetCell.Piece));
                    }
                }

                //Go southweast
                if (rank < 8 && file > 1)
                {
                    diagonallyAdjacentCell = board.Cells[rank + 1, file - 1];
                    if (diagonallyAdjacentCell.Piece == null)
                    {
                        targetCell = board.Cells[rank + 2, file - 2];
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
