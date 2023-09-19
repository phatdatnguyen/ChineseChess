namespace ChineseChess
{
    public class Player
    {
        #region Fields
        private readonly string name;
        private readonly Board.Side side;
        private readonly bool isAI;
        #endregion

        #region Properties
        public string Name
        {
            get { return name; }
        }
        public Board.Side Side
        {
            get { return side; }
        }
        public bool IsAI
        {
            get { return isAI; }
        }
        #endregion

        #region Constructor
        public Player(string name, Board.Side side, bool isAI)
        {
            this.name = name;
            this.side = side;
            this.isAI = isAI;
        }
        #endregion
    }
}
