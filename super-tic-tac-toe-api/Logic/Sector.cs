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

            if (CheckWinner(row, col))
                Winner = player;
            else
                if (CheckFull())
                Winner = CellType.Draw;

            return true;
        }

        private bool CheckWinner(int row, int col)
        {
            if (Cells[row, 0] == Cells[row, 1] && Cells[row, 0] == Cells[row, 2])
                return true;
            if (Cells[0, col] == Cells[1, col] && Cells[0, col] == Cells[2, col])
                return true;

            if (row == col)
                if (Cells[0, 0] == Cells[1, 1] && Cells[0, 0] == Cells[2, 2])
                    return true;
            if (row + col == 2)
                if (Cells[0, 2] == Cells[1, 1] && Cells[0, 2] == Cells[2, 0])
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
