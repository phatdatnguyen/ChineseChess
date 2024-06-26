﻿using System.Diagnostics;
using System.Net.Http.Headers;

namespace ChineseChess
{
    public class Game
    {
        #region Enums
        public enum GameType { TwoPlayers, TwoPlayersHandicap, VsAI, VsAIHandicap }
        public enum GameStatus { NotStarted, Started, Ended }
        public enum AIDifficulty { Easy, Medium, Hard, Extreme }
        #endregion

        #region Fields
        private readonly Board board;
        private readonly GameType type;
        private GameStatus status;
        private readonly Player player1;
        private readonly Player player2;
        private Player currentPlayer;
        private Board.Side aiPlayerSide;
        private Board.Side humanPlayerSide;
        private Move? aiPlayerMove;
        private readonly List<Move> moves;
        private AIDifficulty aiDifficulty;
        #endregion

        #region Properties
        public GameType Type
        {
            get { return type; }
        }
        public GameStatus Status
        {
            get { return status; }
            set { status = value; }
        }
        public Player Player1
        {
            get { return player1; }
        }
        public Player Player2
        {
            get { return player2; }
        }
        public Player CurrentPlayer
        {
            get { return currentPlayer; }
        }
        public Board.Side AIPlayerSide
        {
            get { return aiPlayerSide; }
            set { aiPlayerSide = value; }
        }
        public Board.Side HumanPlayerSide
        {
            get { return humanPlayerSide; }
            set { humanPlayerSide = value; }
        }
        public List<Move> Moves
        {
            get { return moves; }
        }
        public AIDifficulty Difficulty
        {
            get { return aiDifficulty; }
        }
        #endregion

        #region Constructor
        public Game(Board board, GameType type, Player player1, Player player2, AIDifficulty Difficulty)
        {
            this.board = board;
            this.type = type;
            this.player1 = player1;
            this.player2 = player2;
            aiDifficulty = Difficulty;
            currentPlayer = player1;
            status = GameStatus.NotStarted;
            moves = new List<Move>();
            Program.ChessBoard.CurrentPlayerLabel.Text = "";
        }
        #endregion

        #region Methods
        public void Start()
        {
            board.Reset();
            currentPlayer = player1;
            Program.ChessBoard.UndoButton.Enabled = false;

            switch (type)
            {
                case GameType.TwoPlayers:
                    Program.ChessBoard.CurrentPlayerLabel.Text = player1.Name;
                    Program.ChessBoard.CurrentPlayerLabel.ForeColor = Color.Red;
                    status = GameStatus.Started;

                    //Enable all the pieces of the current player and disable all the pieces of the opposite side
                    foreach (Cell cell in board.Cells)
                    {
                        if (cell.Piece != null)
                        {
                            if (cell.Piece.Side == currentPlayer.Side)
                                cell.Piece.Image.Enabled = true;
                            else
                                cell.Piece.Image.Enabled = false;
                        }
                    }

                    break;
                case GameType.TwoPlayersHandicap:
                    //Show start button
                    Program.ChessBoard.StatusLabel.Text = "Choose the pieces to remove and press Start.";
                    Program.ChessBoard.StartButton.Visible = true;

                    //Enable all the pieces except generals
                    foreach (Cell cell in board.Cells)
                    {
                        if (cell.Piece != null)
                        {
                            if (cell.Piece.GetType() != typeof(General))
                                cell.Piece.Image.Enabled = true;
                            else
                                cell.Piece.Image.Enabled = false;
                        }
                    }

                    break;
                case GameType.VsAI:
                    Program.ChessBoard.CurrentPlayerLabel.Text = player1.Name;
                    Program.ChessBoard.CurrentPlayerLabel.ForeColor = Color.Red;
                    status = GameStatus.Started;

                    if (player1.IsAI)
                    {
                        aiPlayerSide = player1.Side;
                        humanPlayerSide = player2.Side;

                        AIPlayerMove();
                    }
                    else
                    {
                        aiPlayerSide = player2.Side;
                        humanPlayerSide = player1.Side;
                    }

                    //Enable all the pieces of the current player and disable all the pieces of the opposite side
                    foreach (Cell cell in board.Cells)
                    {
                        if (cell.Piece != null)
                        {
                            if (cell.Piece.Side == currentPlayer.Side)
                                cell.Piece.Image.Enabled = true;
                            else
                                cell.Piece.Image.Enabled = false;
                        }
                    }

                    break;
                default: //GameType.VsAIHandicap
                    //Show start button
                    Program.ChessBoard.StatusLabel.Text = "Choose the pieces to remove and press Start.";
                    Program.ChessBoard.StartButton.Visible = true;

                    //Enable all the pieces except generals
                    foreach (Cell cell in board.Cells)
                    {
                        if (cell.Piece != null)
                        {
                            if (cell.Piece.GetType() != typeof(General))
                                cell.Piece.Image.Enabled = true;
                            else
                                cell.Piece.Image.Enabled = false;
                        }
                    }

                    break;
            }
        }

        public void SwitchTurn()
        {
            if (currentPlayer == player1)
            {
                currentPlayer = player2;
                Program.ChessBoard.CurrentPlayerLabel.Text = player2.Name;
                Program.ChessBoard.CurrentPlayerLabel.ForeColor = Color.Blue;
            }
            else
            {
                currentPlayer = player1;
                Program.ChessBoard.CurrentPlayerLabel.Text = player1.Name;
                Program.ChessBoard.CurrentPlayerLabel.ForeColor = Color.Red;
            }
            
            //If current player is AI, do the AI move
            if ((type == GameType.VsAI || type == GameType.VsAIHandicap) && currentPlayer.Side == aiPlayerSide)
                AIPlayerMove();
        }

        public void End(Board.Side winSide)
        {
            //Declare the winner
            if (player1.Side == winSide)
            {
                MessageBox.Show(player1.Name + " won!", "Game over!", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(player2.Name + " won!", "Game over!", MessageBoxButtons.OK);
            }

            //Disable all the pieces
            foreach (Cell cell in board.Cells)
            {
                if (cell.Piece != null)
                {
                    cell.Piece.Image.Enabled = false;
                }
            }
        }
        
        private float AlphaBeta(Board.Side side, float alpha, float beta, byte depth, byte maxDepth)
        {
            if (depth == 0)
            {
                return board.Evaluate(side);
            }

            float bestValue;
            bestValue = float.NegativeInfinity;
            List<Move> moves = board.FindPossibleMoves(side);
            foreach (Move testMove in moves)
            {
                if (bestValue >= beta)
                    break;

                float value;
                if (testMove.CapturedPiece != null && testMove.CapturedPiece.GetType() == typeof(General))
                {
                    value = 1000 - (maxDepth - depth);
                }
                else
                {
                    board.DoMove(testMove, true);
                    if (side == aiPlayerSide)
                    {
                        value = -AlphaBeta(humanPlayerSide, -beta, -alpha, (byte)(depth - 1), maxDepth);
                    }
                    else
                    {
                        value = -AlphaBeta(aiPlayerSide, -beta, -alpha, (byte)(depth - 1), maxDepth);
                    }
                    board.UndoMove(testMove, true);
                }
                if (bestValue < value)
                {
                    bestValue = value;
                    if (depth == maxDepth)
                        aiPlayerMove = new Move(testMove.StartRow, testMove.StartColumn, testMove.EndRow, testMove.EndColumn, testMove.Piece, testMove.CapturedPiece);
                }
                alpha = Math.Max(alpha, value);
                if (beta <= alpha)
                    break;
            }

            return bestValue;
        }

        public void AIPlayerMove()
        {
            //Set the depth
            byte depth = 0;
            switch (aiDifficulty)
            {
                case AIDifficulty.Easy:
                    depth = 2; break;
                case AIDifficulty.Medium:
                    depth = 3; break;
                case AIDifficulty.Hard:
                    depth = 4; break;
                case AIDifficulty.Extreme:
                    depth = 5; break;
            }

            //Perform the move
            AlphaBeta(aiPlayerSide, float.NegativeInfinity, float.PositiveInfinity, depth, depth);
            if (aiPlayerMove != null)
            {
                board.DoMove(aiPlayerMove);

                //Add to moves list
                moves.Add(aiPlayerMove);
            }
            
            //Switch turn
            if (Program.ChessBoard != null && Program.ChessBoard.Game != null && Program.ChessBoard.Game.Status != Game.GameStatus.Ended)
                Program.ChessBoard.Game.SwitchTurn();
        }
        #endregion
    }
}
