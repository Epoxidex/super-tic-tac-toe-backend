using super_tic_tac_toe_api.Logic.Enums;

namespace super_tic_tac_toe_api.Logic
{
    public class Sector
    {
        public CellType[,] Cells { get; private set; }
        public CellType Winner { get; private set; }

        public Sector()
        {
            Cells = new CellType[3, 3];
            Winner = CellType.None;
        }

        public bool MakeMove(int row, int col, CellType player)
        {
            if (Cells[row, col] != CellType.None)
            {
                return false;
            }

            Cells[row, col] = player;

            if (CheckWinner(player))
                Winner = player;
            else
                if (CheckFull())
                Winner = CellType.Draw;

            return true;
        }
        private bool CheckWinner(CellType player)
        {
            return CheckRowsCols(player) || CheckDiagonals(player);
        }

        private bool CheckRowsCols(CellType player)
        {
            for (int i = 0; i < 3; i++)
            {
                if (Cells[i, 0] == player && Cells[i, 1] == player && Cells[i, 2] == player)
                    return true;
                if (Cells[0, i] == player && Cells[1, i] == player && Cells[2, i] == player)
                    return true;
            }
            return false;
        }
        private bool CheckDiagonals(CellType player)
        {
            if (Cells[0, 0] == player && Cells[1, 1] == player && Cells[2, 2] == player)
                return true;
            if (Cells[0, 2] == player && Cells[1, 1] == player && Cells[2, 0] == player)
                return true;

            return false;
        }

        private bool CheckFull()
        {
            foreach (var cell in Cells)
            {
                if (cell == CellType.None)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
