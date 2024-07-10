
using super_tic_tac_toe_logic.Enums;

namespace super_tic_tac_toe_logic
{
    internal class SubGrid
    {
        private CellType[,] _grid;
        public CellType Winner { get; private set; }
        public bool IsFull => CheckFull();

        public SubGrid()
        {
            _grid = new CellType[3, 3];
            Winner = CellType.None;
        }

        public bool MakeMove(int row, int col, CellType player)
        {
            if (_grid[row, col] != CellType.None)
            {
                return false;
            }

            _grid[row, col] = player;

            if (CheckWinner(player)) 
                Winner = player;

            return true;
        }

        private bool CheckWinner(CellType player)
        {
            for (int i = 0; i < 3; i++)
            {
                if (_grid[i, 0] == player && _grid[i, 1] == player && _grid[i, 2] == player)
                    return true;
                if (_grid[0, i] == player && _grid[1, i] == player && _grid[2, i] == player)
                    return true;
            }
            if (_grid[0, 0] == player && _grid[1, 1] == player && _grid[2, 2] == player)
                return true;
            if (_grid[0, 2] == player && _grid[1, 1] == player && _grid[2, 0] == player)
                return true;

            return false;
        }

        private bool CheckFull()
        {
            foreach (var cell in _grid)
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
