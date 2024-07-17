using super_tic_tac_toe_api.Logic.Enums;

namespace super_tic_tac_toe_api.Logic
{
    public class BaseBoard
    {
        public CellType[,] Board { get; protected set; }
        public CellType Winner { get; protected set; }
        public bool IsFull => CheckFull();

        public BaseBoard() 
        {
            Board = new CellType[3, 3];
            Winner = CellType.None;
        }
        protected bool CheckWinner(int row, int col)
        {
            if (Board[row, 0] == Board[row, 1] && Board[row, 0] == Board[row, 2] ) return true;
            if (Board[0, col] == Board[1, col] && Board[0, col] == Board[2, col]) return true;

            if (row == col && Board[0, 0] == Board[1, 1] && Board[0, 0] == Board[2, 2]) return true;
            if (row + col == 2 && Board[0, 2] == Board[1, 1] && Board[0, 2] == Board[2, 0]) return true;

            return false;
        }

        protected bool CheckFull()
        {
            foreach (var cell in Board)
            {
                if (cell == CellType.None) return false;
            }
            return true;
        }
    }
}
