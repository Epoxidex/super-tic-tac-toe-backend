using super_tic_tac_toe_api.Logic.Enums;

namespace super_tic_tac_toe_api.Logic
{
    public class Sector : BaseBoard
    {
        public bool HasWinner => Winner != CellType.None;

        public bool MakeMove(int row, int col, CellType player)
        {
            if (Board[row, col] != CellType.None) return false;

            Board[row, col] = player;

            if (CheckWinner(row, col))
                Winner = player;
            else if (IsFull)
                Winner = CellType.Draw;

            return true;
        }
    }
}
