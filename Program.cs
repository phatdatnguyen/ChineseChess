using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChineseChess
{
    static class Program
    {
        public static ChineseChess.Main ChessBoard;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ChessBoard = new ChineseChess.Main();
            Application.Run(ChessBoard);
        }
    }
}
