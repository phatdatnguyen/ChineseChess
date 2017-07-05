using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ChineseChess
{
    public class Cell
    {
        #region Fields
        private Board board;
        private int row;
        private int column;
        private Piece piece;
        private PictureBox possibleMoveIndicator;
        #endregion

        #region Properties
        public Board Board
        {
            get { return board; }
        }
        public int Row
        {
            get { return row; }
        }
        public int Column
        {
            get { return column; }
        }
        public Piece Piece
        {
            get { return piece; }
            set { piece = value; }
        }
        public bool IsEmpty
        {
            get { return piece == null; }
        }
        public PictureBox PossibleMoveIndicator
        {
            get { return possibleMoveIndicator; }
        }
        #endregion

        #region Constructor
        public Cell(Board board, int row, int column, Piece piece = null)
        {
            this.board = board;
            this.row = row;
            this.column = column;
            this.piece = piece;

            possibleMoveIndicator = new PictureBox();
            possibleMoveIndicator.Width = 20;
            possibleMoveIndicator.Height = 20;
            possibleMoveIndicator.SizeMode = PictureBoxSizeMode.StretchImage;
            possibleMoveIndicator.BackColor = Color.Transparent;
            possibleMoveIndicator.Top = row * 42 + 20;
            possibleMoveIndicator.Left = column * 42 + 30;
            possibleMoveIndicator.Cursor = Cursors.Hand;
            possibleMoveIndicator.Image = Properties.Resources.PossibleMoveIndicator;
            possibleMoveIndicator.Visible = false;
            possibleMoveIndicator.MouseClick += new MouseEventHandler(possibleMoveIndicator_MouseClick);
        }
        #endregion

        #region Event handlers
        private void possibleMoveIndicator_MouseClick(Object sender, MouseEventArgs e)
        {
            //Move
            board.SelectedCell.Piece.Move(row, column);
            
            //Switch turn
            Program.ChessBoard.Game.SwitchTurn();

            //Enable all the pieces of the current player and disable all the pieces of the opponent
            foreach (Cell cell in board.Cells)
            {
                if (!cell.IsEmpty)
                {
                    if (cell.Piece.Side == Program.ChessBoard.Game.CurrentPlayer.Side)
                        cell.Piece.Image.Enabled = true;
                    else
                        cell.Piece.Image.Enabled = false;
                }
            }
        }
        #endregion
    }
}
