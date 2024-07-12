using super_tic_tac_toe_logic.Enums;

namespace super_tic_tac_toe_logic
{
    public class Sector
    {
        private CellType[,] _sector;
        public CellType Winner { get; private set; }

        public Sector()
        {
            _sector = new CellType[3, 3];
            Winner = CellType.None;
        }

        public bool MakeMove(int row, int col, CellType player)
        {
            if (_sector[row, col] != CellType.None)
            {
                return false;
            }

            _sector[row, col] = player;

            if (CheckWinner(player))
                Winner = player;
            else
                if (CheckFull()) 
                Winner = CellType.Draw;

            return true;
        }
        public CellType[,] GetSectorState()
        {
            return _sector.Clone() as CellType[,];
        }
        private bool CheckWinner(CellType player)
        {
            return CheckRowsCols(player) || CheckDiagonals(player);
        }

        private bool CheckRowsCols(CellType player)
        {
            for (int i = 0; i < 3; i++)
            {
                if (_sector[i, 0] == player && _sector[i, 1] == player && _sector[i, 2] == player)
                    return true;
                if (_sector[0, i] == player && _sector[1, i] == player && _sector[2, i] == player)
                    return true;
            }
            return false;
        }
        private bool CheckDiagonals(CellType player)
        {
            if (_sector[0, 0] == player && _sector[1, 1] == player && _sector[2, 2] == player)
                return true;
            if (_sector[0, 2] == player && _sector[1, 1] == player && _sector[2, 0] == player)
                return true;

            return false;
        }

        private bool CheckFull()
        {
            foreach (var cell in _sector)
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
