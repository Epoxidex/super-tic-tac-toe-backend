using super_tic_tac_toe_api.Logic.Enums;

namespace super_tic_tac_toe_api.Logic
{
    public class Game
    {
        public Sector[,] Sectors { get; private set; }
        public CellType[,] Board { get; private set; }
        public CellType Turn { get; private set; }
        public CellType Winner { get; private set; }
        public bool[,] OpenSectors { get; private set; }

        public Game()
        {
            Sectors = new Sector[3, 3];
            Board = new CellType[3, 3];
            Turn = CellType.X;
            Winner = CellType.None;
            InitOpenSectors();
            InitSectors();
        }

        private void InitOpenSectors(bool value = true)
        {
            OpenSectors = new bool[3, 3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    OpenSectors[i, j] = value;
        }
        private void FillOpenSectors(int row, int col)
        {
            InitOpenSectors(value: false);
            if (Sectors[row, col].Winner == CellType.None)
                OpenSectors[row, col] = true;
            else
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        OpenSectors[i, j] = Sectors[i, j].Winner == CellType.None;
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

            if (OpenSectors[sectorRow, sectorCol] == false) return false;
            if (currentGrid.MakeMove(cellRow, cellCol, Turn) == false) return false;

            if (currentGrid.Winner != CellType.None)
                Board[sectorRow, sectorCol] = Turn;

            if (CheckWinner(sectorRow, sectorCol))
                Winner = Turn;
            else
                if (CheckFull())
                Winner = CellType.Draw;

            FillOpenSectors(cellRow, cellCol);
            SwitchPlayer();
            return true;
        }

        private void SwitchPlayer()
        {
            Turn = Turn == CellType.X ? CellType.O : CellType.X;
        }

        private bool CheckWinner(int row, int col)
        {
            if (Board[row, 0] == Board[row, 1] && Board[row, 0] == Board[row, 2])
                return true;
            if (Board[0, col] == Board[1, col] && Board[0, col] == Board[2, col])
                return true;

            if (row == col)
                if (Board[0, 0] == Board[1, 1] && Board[0, 0] == Board[2, 2])
                    return true;
            if (row + col == 2)
                if (Board[0, 2] == Board[1, 1] && Board[0, 2] == Board[2, 0])
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
