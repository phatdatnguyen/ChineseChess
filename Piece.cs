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
                    OnSelectionChanged(EventArgs.Empty);
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
        public event SelectionChangedEventHandler? SelectionChanged;
        protected virtual void OnSelectionChanged(EventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
        #endregion

        #region Contructor
        public Piece(string name, Board board, Board.Side side, int rank, int file)
        {
            this.name = name;
            this.board = board;
            this.side = side;
            this.rank = rank;
            this.file = file;
            
            isCaptured = false;
            isSelected = false;

            image = new PictureBox
            {
                Width = Board.PieceSize,
                Height = Board.PieceSize,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Cursor = Cursors.Hand,
                Top = rank * Board.VerticalCellDistance + Board.PaddingTop,
                Left = file * Board.HorizontalCellDistance + Board.PaddingLeft,
                BackColor = Color.Transparent
            };
            image.MouseClick += new MouseEventHandler(Image_MouseClick);
            image.Tag = this;   //Refer to this piece using tag
        }
        #endregion

        #region Methods
        protected void Image_MouseClick(object? sender, MouseEventArgs e)
        {
            Main? main = Program.ChessBoard;
            Game? game = main?.Game;
            if (e.Button != MouseButtons.Left || main == null || game == null
                || sender is not PictureBox clickedImage || clickedImage.Tag is not Piece selectedPiece)
                return;

            //Remove the selected piece in handicap game type
            if ((game.Type == Game.GameType.TwoPlayersHandicap || game.Type == Game.GameType.VsAIHandicap)
                && game.Status == Game.GameStatus.NotStarted)
            {
                if (selectedPiece is General)
                    return;

                board.Cells[selectedPiece.Rank, selectedPiece.File].Piece = null;
                if (board.IsCheckDelivered(Board.Side.Red) || board.IsCheckDelivered(Board.Side.Blue))
                {
                    board.Cells[selectedPiece.Rank, selectedPiece.File].Piece = selectedPiece;
                    main.StatusLabel.Text = "That piece must stay to keep the generals protected.";
                    return;
                }

                board.RemovePiece(selectedPiece);
                selectedPiece.IsCaptured = true;
                selectedPiece.Image.Dispose();
                main.StatusLabel.Text = "Choose the pieces to remove and press Start.";
                return;
            }

            if (game.Status != Game.GameStatus.Started || game.CurrentPlayer.IsAI)
                return;

            if (!board.IsSelected)
            {
                if (selectedPiece.Side != game.CurrentPlayer.Side)
                    return;

                //Select the piece, find possible moves
                selectedPiece.IsSelected = true;
                board.SelectedCell = board.Cells[selectedPiece.Rank, selectedPiece.File];
                List<Move> possibleMoves = board.FindLegalMoves(selectedPiece);

                //Disable undo button
                main.UndoButton.Enabled = false;

                //Disable all the pieces
                foreach (Cell cell in board.Cells)
                {
                    if (cell.Piece != null)
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
                if (board.SelectedCell != null && board.SelectedCell.Piece == selectedPiece) //Reselect the piece
                {
                    //Hide all the move indicators.
                    foreach (Cell cell in board.Cells)
                        cell.PossibleMoveIndicator.Visible = false;

                    //Deselect the piece
                    board.SelectedCell.Piece.IsSelected = false;
                    board.SelectedCell = null;
                    game.UpdateTurnControls();
                }
                else //Select different piece
                {
                    Piece? movingPiece = board.SelectedCell?.Piece;
                    if (movingPiece == null || movingPiece.Side != game.CurrentPlayer.Side
                        || !board.FindLegalMoves(movingPiece).Any(move => move.CapturedPiece == selectedPiece))
                        return;

                    movingPiece.Capture(selectedPiece);
                    if (game.Status == Game.GameStatus.Started)
                        game.SwitchTurn();
                    game.UpdateTurnControls();
                }
            }
        }
       
        public virtual List<Move> FindPossibleMoves()
        {
            return new List<Move>();
        }

        public void Move(int row, int column)
        {
            //Create the move
            Move move = new Move(rank, file, row, column, this);
            
            //Record the move before it can end the game and display the result.
            if (Program.ChessBoard != null && Program.ChessBoard.Game != null)
                Program.ChessBoard.Game.Moves.Add(move);

            board.DoMove(move);
        }

        public void Capture(Piece piece)
        {
            //Create the move
            Move move = new Move(rank, file, piece.Rank, piece.File, this, piece);
            
            //Record the move before it can end the game and display the result.
            if (Program.ChessBoard != null && Program.ChessBoard.Game != null)
                Program.ChessBoard.Game.Moves.Add(move);

            board.DoMove(move);
        }
        #endregion
    }
}
