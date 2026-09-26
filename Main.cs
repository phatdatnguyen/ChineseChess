namespace ChineseChess
{
    public partial class Main : Form
    {
        #region Fields
        private Board? board;
        private Game? game;
        private readonly NewGameDialog newGameDialog;
        #endregion

        #region Properties
        public Game? Game
        {
            get { return game; }
        }
        public Label CurrentPlayerLabel
        {
            get { return currentPlayerLabel; }
        }
        public Button UndoButton
        {
            get { return undoButton; }
        }
        public Label StatusLabel
        {
            get { return statusLabel; }
        }
        public Button StartButton
        {
            get { return startButton; }
        }
        #endregion

        #region Constructor
        public Main()
        {
            InitializeComponent();

            newGameDialog = new NewGameDialog();
        }
        #endregion

        #region Methods
        private void Main_Load(object sender, EventArgs e)
        {
            NewGameButton_Click(newGameButton, EventArgs.Empty);
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            if (newGameDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (newGameDialog.Player1 != null && newGameDialog.Player2 != null)
                {
                    board?.Dispose();
                    board = new Board(boardPanel);
                    game = new Game(board, newGameDialog.GameType, newGameDialog.Player1, newGameDialog.Player2, newGameDialog.AIDifficulty);
                    game?.Start();
                }
            }
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            game?.UndoLastTurn();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            game?.StartHandicap();
        }
        #endregion
    }
}
