using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChineseChess
{
    public partial class NewGameDialog : Form
    {
        #region Fields
        private Game.GameType gameType;
        private Player player1;
        private Player player2;
        #endregion

        #region Properties
        public Game.GameType GameType
        {
            get { return gameType; }
        }
        public Player Player1
        {
            get { return player1; }
        }
        public Player Player2
        {
            get { return player2; }
        }
        #endregion

        #region Constructor
        public NewGameDialog()
        {
            InitializeComponent();
        }
        #endregion

        #region Event handlers
        private void NewGameDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                //Set game type
                if (!gameType_Handicap.Checked)
                {
                    if (gameType_TwoPlayer.Checked)
                        gameType = Game.GameType.TwoPlayers;
                    else
                        gameType = Game.GameType.VsAI;
                }
                else
                {
                    if (gameType_TwoPlayer.Checked)
                        gameType = Game.GameType.TwoPlayersHandicap;
                    else
                        gameType = Game.GameType.VsAIHandicap;
                }

                //Set players information
                if (gameType == Game.GameType.TwoPlayers || gameType == Game.GameType.TwoPlayersHandicap)
                {
                    if (playersInformation_Player1Name.Text != "")
                        player1 = new Player(playersInformation_Player1Name.Text, Board.Side.Red, false);
                    else
                        player1 = new Player("Player 1", Board.Side.Red, false);

                    if (playersInformation_Player2Name.Text != "")
                        player2 = new Player(playersInformation_Player2Name.Text, Board.Side.Blue, false);
                    else
                        player2 = new Player("Player 2", Board.Side.Blue, false);
                }
                else
                {
                    if (playersInformation_Player1Name.Text != "")
                        player1 = new Player(playersInformation_Player1Name.Text, Board.Side.Red, playersInformation_Player1AI.Checked);
                    else
                        player1 = new Player("Player 1", Board.Side.Red, playersInformation_Player1AI.Checked);

                    if (playersInformation_Player2Name.Text != "")
                        player2 = new Player(playersInformation_Player2Name.Text, Board.Side.Blue, playersInformation_Player2AI.Checked);
                    else
                        player2 = new Player("Player 2", Board.Side.Blue, playersInformation_Player2AI.Checked);
                }
            }
        }

        private void gameType_CheckedChanged(object sender, EventArgs e)
        {
            playersInformation_Player1AI.Enabled = gameType_VsAI.Checked;
            playersInformation_Player2AI.Enabled = gameType_VsAI.Checked;
        }
        #endregion
    }
}
