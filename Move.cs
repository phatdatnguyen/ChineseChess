using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChineseChess
{
    [Serializable]
    public class Move
    {
        #region Fields
        private int startRow;
        private int startColumn;
        private int endRow;
        private int endColumn;
        private Piece piece;
        private Piece capturedPiece;
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
        public Piece CapturedPiece
        {
            get { return capturedPiece; }
        }
        #endregion

        #region Constructors
        public Move(int startRow, int startColumn, int endRow, int endColumn, Piece piece, Piece capturedPiece = null, bool isTestMove = false)
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
