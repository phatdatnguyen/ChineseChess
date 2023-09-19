namespace ChineseChess
{
    static class Program
    {
        public static Main ChessBoard;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ChessBoard = new Main();
            Application.Run(ChessBoard);
        }
    }
}
