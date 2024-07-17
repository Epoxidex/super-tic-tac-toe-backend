using super_tic_tac_toe_api.Logic.Enums;

namespace super_tic_tac_toe_api.Logic
{
    public class Game : BaseBoard
    {
        public Sector[,] Sectors { get; private set; }
        public CellType Turn { get; private set; }
        public bool[,] OpenSectors { get; private set; }
        public Game() : base()
        {
            Sectors = InitializeSectors();
            Turn = CellType.X;
            OpenSectors = InitializeOpenSectors();
        }
        private Sector[,] InitializeSectors()
        {
            var sectors = new Sector[3, 3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    sectors[i, j] = new Sector();
            return sectors;
        }
        private bool[,] InitializeOpenSectors(bool value = true)
        {
            var openSectors = new bool[3, 3];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    openSectors[i, j] = value;
            return openSectors;
        }
        private void UpdateOpenSectors(int row, int col)
        {
            OpenSectors = InitializeOpenSectors(false);

            if (!Sectors[row, col].HasWinner)
                OpenSectors[row, col] = true;
            else
            {
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        OpenSectors[i, j] = !Sectors[i, j].HasWinner;
            }
        }
        public bool MakeMove(int sectorRow, int sectorCol, int cellRow, int cellCol)
        {
            if (!OpenSectors[sectorRow, sectorCol]) return false;

            var currentGrid = Sectors[sectorRow, sectorCol];
            if (!currentGrid.MakeMove(cellRow, cellCol, Turn)) return false;

            if (currentGrid.HasWinner)
                Board[sectorRow, sectorCol] = Turn;

            if (CheckWinner(sectorRow, sectorCol))
                Winner = Turn;
            else if (IsFull)
                Winner = CellType.Draw;

            UpdateOpenSectors(cellRow, cellCol);
            SwitchPlayer();
            return true;
        }

        private void SwitchPlayer()
        {
            Turn = Turn == CellType.X ? CellType.O : CellType.X;
        }
    }
}
