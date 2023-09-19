namespace ChineseChess
{
    [Serializable]
    public class Move
    {
        #region Fields
        private readonly int startRow;
        private readonly int startColumn;
        private readonly int endRow;
        private readonly int endColumn;
        private readonly Piece piece;
        private readonly Piece? capturedPiece;
        #endregion

        #region Properties
        public int StartRow
        {
            get { return startRow; }
        }
        public int StartColumn
        {
            get { return startColumn; }
        }
        public int EndRow
        {
            get { return endRow; }
        }
        public int EndColumn
        {
            get { return endColumn; }
        }
        public Piece Piece
        {
            get { return piece; }
        }
        public Piece? CapturedPiece
        {
            get { return capturedPiece; }
        }
        #endregion

        #region Constructor
        public Move(int startRow, int startColumn, int endRow, int endColumn, Piece piece, Piece? capturedPiece = null)
        {
            this.startRow = startRow;
            this.startColumn = startColumn;
            this.endRow = endRow;
            this.endColumn = endColumn;
            this.piece = piece;
            this.capturedPiece = capturedPiece;
        }
        #endregion
    }
}
