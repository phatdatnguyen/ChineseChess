namespace ChineseChess
{
    public class Board : IDisposable
    {
        #region Enums
        public enum Side { Red, Blue }
        #endregion

        #region Fields
        private readonly Panel boardPanel;
        private readonly Cell[,] cells;
        private readonly HashSet<Piece> ownedPieces = new();
        private Cell? selectedCell;
        #endregion

        #region Properties
        public static int PieceSize { get { return 32; } }
        public static int PossibleMoveIndicatorSize { get { return 20; } }
        public static int PaddingTop { get { return 12; } }
        public static int PaddingLeft { get { return 32; } }
        public static int VerticalCellDistance { get { return 43; } }
        public static int HorizontalCellDistance { get { return 49; } }
        public Cell? SelectedCell
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
            DisposeControls();
            boardPanel.Controls.Clear();
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 9; j++)
                {
                    cells[i, j] = new Cell(this, i, j);
                    boardPanel.Controls.Add(cells[i, j].PossibleMoveIndicator);
                }
            selectedCell = null;
        }

        private void DisposeControls()
        {
            // Captured pieces are detached from the panel but remain alive for undo.
            foreach (Piece piece in ownedPieces)
                piece.Image.Dispose();
            ownedPieces.Clear();

            foreach (Cell? cell in cells)
                cell?.PossibleMoveIndicator.Dispose();
        }

        public void Dispose()
        {
            DisposeControls();
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
                ownedPieces.Add(piece);
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
                move.Piece.Image.Top = move.Piece.Rank * Board.VerticalCellDistance + Board.PaddingTop;
                move.Piece.Image.Left = move.Piece.File * Board.HorizontalCellDistance + Board.PaddingLeft;
                move.Piece.Image.Invalidate();
                move.Piece.Image.Update();

                if (Program.ChessBoard != null)
                    Program.ChessBoard.UndoButton.Enabled = Program.ChessBoard.Game?.CanUndo == true;

                //Check whether a check is delivered
                if (Program.ChessBoard != null && IsCheckDelivered(move.Piece.Side))
                    Program.ChessBoard.StatusLabel.Text = "Check!";
                else if (Program.ChessBoard != null)
                    Program.ChessBoard.StatusLabel.Text = "";

                //Check end game
                if (Program.ChessBoard != null && Program.ChessBoard.Game != null && move.CapturedPiece != null && move.CapturedPiece.GetType() == typeof(General))
                {
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
                if (selectedCell?.Piece != null)
                    selectedCell.Piece.IsSelected = false;
                move.Piece.IsSelected = false;
                selectedCell = null;

                foreach (Cell cell in cells)
                    cell.PossibleMoveIndicator.Visible = false;

                //Position the images
                move.Piece.Image.Top = move.Piece.Rank * Board.VerticalCellDistance + Board.PaddingTop;
                move.Piece.Image.Left = move.Piece.File * Board.HorizontalCellDistance + Board.PaddingLeft;

                //Add the captured piece
                if (move.CapturedPiece != null)
                    AddPiece(move.CapturedPiece);

                //Update check status label to reflect restored position
                if (Program.ChessBoard != null)
                {
                    Side opponentSide = move.Piece.Side == Side.Red ? Side.Blue : Side.Red;
                    Program.ChessBoard.StatusLabel.Text = IsCheckDelivered(opponentSide) ? "Check!" : "";
                }
            }
        }

        public List<Move> FindPossibleMoves(Side side)
        {
            List<Move> possibleMoves = new();
            Piece? general = FindGeneral(side);
            if (general == null)
                return possibleMoves;

            foreach (Cell cell in cells)
            {
                if (cell.Piece != null && cell.Piece.Side == side)
                {
                    foreach (Move move in cell.Piece.FindPossibleMoves())
                    {
                        if (IsLegalMove(move, general))
                            possibleMoves.Add(move);
                    }
                }
            }

            return possibleMoves;
        }

        public bool HasLegalMoves(Side side)
        {
            Piece? general = FindGeneral(side);
            if (general == null)
                return false;

            foreach (Cell cell in cells)
            {
                if (cell.Piece != null && cell.Piece.Side == side)
                {
                    foreach (Move move in cell.Piece.FindPossibleMoves())
                    {
                        if (IsLegalMove(move, general))
                            return true;
                    }
                }
            }
            return false;
        }

        public List<Move> FindLegalMoves(Piece piece)
        {
            List<Move> legalMoves = new();
            if (piece.IsCaptured || cells[piece.Rank, piece.File].Piece != piece)
                return legalMoves;

            Piece? general = FindGeneral(piece.Side);
            if (general == null)
                return legalMoves;

            foreach (Move move in piece.FindPossibleMoves())
            {
                if (IsLegalMove(move, general))
                    legalMoves.Add(move);
            }

            return legalMoves;
        }

        private bool IsLegalMove(Move move, Piece general)
        {
            Side opponent = move.Piece.Side == Side.Red ? Side.Blue : Side.Red;
            DoMove(move, true);
            try
            {
                return !IsGeneralAttacked(general, opponent);
            }
            finally
            {
                UndoMove(move, true);
            }
        }
                
        public int Evaluate(Side side)
        {
            int value = 0;

            foreach (Cell cell in cells)
            {
                if (cell.Piece != null)
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
            Piece? general = FindGeneral(side == Side.Red ? Side.Blue : Side.Red);
            return general != null && IsGeneralAttacked(general, side);
        }

        private Piece? FindGeneral(Side side)
        {
            foreach (Cell cell in cells)
            {
                if (cell.Piece is General && cell.Piece.Side == side)
                    return cell.Piece;
            }
            return null;
        }

        private bool IsGeneralAttacked(Piece general, Side attackingSide)
        {
            // Test attacks directly, without legal-move recursion or allocating a
            // move list for every enemy piece at every node of the AI search.
            foreach (Cell cell in cells)
            {
                Piece? attacker = cell.Piece;
                if (attacker == null || attacker.Side != attackingSide)
                    continue;

                int rowDistance = general.Rank - attacker.Rank;
                int columnDistance = general.File - attacker.File;
                int rows = Math.Abs(rowDistance);
                int columns = Math.Abs(columnDistance);

                switch (attacker)
                {
                    case Chariot:
                    case Cannon:
                        if (rows != 0 && columns != 0)
                            break;
                        int screens = CountPiecesBetween(attacker, general);
                        if (screens == (attacker is Cannon ? 1 : 0))
                            return true;
                        break;
                    case General:
                        if (columns == 0 && CountPiecesBetween(attacker, general) == 0)
                            return true;
                        if (rows + columns == 1 && IsInPalace(general.Rank, general.File, attackingSide))
                            return true;
                        break;
                    case Horse:
                        if (((rows == 2 && columns == 1) || (rows == 1 && columns == 2)) &&
                            cells[attacker.Rank + rowDistance / 2, attacker.File + columnDistance / 2].Piece == null)
                            return true;
                        break;
                    case Soldier:
                        if (columnDistance == 0 && rowDistance == (attackingSide == Side.Blue ? 1 : -1))
                            return true;
                        bool crossedRiver = attackingSide == Side.Blue ? attacker.Rank >= 5 : attacker.Rank <= 4;
                        if (crossedRiver && rows == 0 && columns == 1)
                            return true;
                        break;
                    case Advisor:
                        if (rows == 1 && columns == 1 && IsInPalace(general.Rank, general.File, attackingSide))
                            return true;
                        break;
                    case Elephant:
                        bool onOwnSide = attackingSide == Side.Blue ? general.Rank <= 4 : general.Rank >= 5;
                        if (rows == 2 && columns == 2 && onOwnSide &&
                            cells[attacker.Rank + rowDistance / 2, attacker.File + columnDistance / 2].Piece == null)
                            return true;
                        break;
                }
            }
            return false;
        }

        private static bool IsInPalace(int row, int column, Side side)
        {
            return column >= 3 && column <= 5 &&
                (side == Side.Blue ? row >= 0 && row <= 2 : row >= 7 && row <= 9);
        }

        private int CountPiecesBetween(Piece start, Piece end)
        {
            int rowStep = Math.Sign(end.Rank - start.Rank);
            int columnStep = Math.Sign(end.File - start.File);
            int count = 0;
            for (int row = start.Rank + rowStep, column = start.File + columnStep;
                row != end.Rank || column != end.File;
                row += rowStep, column += columnStep)
            {
                if (cells[row, column].Piece != null)
                    count++;
            }
            return count;
        }
        #endregion
    }
}
