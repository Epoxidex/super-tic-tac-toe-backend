using super_tic_tac_toe_api.Logic.Enums;

namespace super_tic_tac_toe_api.Logic
{
    public class Game
    {
        public Sector[,] Sectors { get; private set; }
        public CellType[,] Board { get; private set; }
        public CellType Turn { get; private set; }
        public CellType Winner { get; private set; }
        public bool[,] MoveField { get; private set; }

        public Game()
        {
            Sectors = new Sector[3, 3];
            Board = new CellType[3, 3];
            Turn = CellType.X;
            Winner = CellType.None;
            InitMoveField();
            InitSectors();
        }

        private void InitMoveField(bool value = true)
        {
            MoveField = new bool[3, 3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    MoveField[i, j] = value;
        }
        private void FillMoveField(int row, int col)
        {
            InitMoveField(value: false);
            if (Sectors[row, col].Winner == CellType.None)
                MoveField[row, col] = true;
            else
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        MoveField[i, j] = Sectors[i, j].Winner == CellType.None;
        }
        private void InitSectors()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    Sectors[i, j] = new Sector();
        }

        public bool MakeMove(int sectorRow, int sectorCol, int cellRow, int cellCol)
        {
            var currentGrid = Sectors[sectorRow, sectorCol];

            if (MoveField[sectorRow, sectorCol] == false) return false;
            if (currentGrid.MakeMove(cellRow, cellCol, Turn) == false) return false;

            if (currentGrid.Winner != CellType.None)
                Board[sectorRow, sectorCol] = Turn;

            if (CheckWinner(Turn))
                Winner = Turn;
            else
                if (CheckFull())
                Winner = CellType.Draw;

            FillMoveField(cellRow, cellCol);
            SwitchPlayer();
            return true;
        }

        private void SwitchPlayer()
        {
            Turn = Turn == CellType.X ? CellType.O : CellType.X;
        }

        private bool CheckWinner(CellType player)
        {
            return CheckRowsCols(player) || CheckDiagonals(player);
        }

        private bool CheckRowsCols(CellType player)
        {
            for (int i = 0; i < 3; i++)
            {
                if (Board[i, 0] == player && Board[i, 1] == player && Board[i, 2] == player)
                    return true;
                if (Board[0, i] == player && Board[1, i] == player && Board[2, i] == player)
                    return true;
            }
            return false;
        }
        private bool CheckDiagonals(CellType player)
        {
            if (Board[0, 0] == player && Board[1, 1] == player && Board[2, 2] == player)
                return true;
            if (Board[0, 2] == player && Board[1, 1] == player && Board[2, 0] == player)
                return true;

            return false;
        }
        private bool CheckFull()
        {
            foreach (var grid in Sectors)
            {
                if (grid.Winner == CellType.None)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
