namespace ChineseChess
{
    static class Program
    {
        // Assigned during startup before the form begins processing events.
        public static Main ChessBoard = null!;
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
