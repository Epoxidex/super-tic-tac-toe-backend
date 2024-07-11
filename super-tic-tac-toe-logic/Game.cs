using super_tic_tac_toe_logic.Enums;

namespace super_tic_tac_toe_logic
{
    internal class Game
    {
        public SubGrid[,] SubGrids { get; private set; }
        public CellType[,] MacroGrid { get; private set; }
        public CellType CurrentPlayer { get; private set; }
        public CellType Winner { get; private set; }
        public bool[,] MoveField { get; private set; }

        public Game()
        {
            SubGrids = new SubGrid[3, 3];
            MacroGrid = new CellType[3, 3];
            CurrentPlayer = CellType.X;
            Winner = CellType.None;
            InitMoveField();
            FillSubGrids();
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
            if (SubGrids[row, col].Winner == CellType.None)
                MoveField[row, col] = true;
            else
                for (int i = 0;i < 3;i++)
                    for (int j = 0;j < 3;j++)
                        MoveField[i, j] = SubGrids[i, j].Winner == CellType.None;
        }
        private void FillSubGrids()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    SubGrids[i, j] = new SubGrid();
        }

        public bool MakeMove(int subGridRow, int subGridCol, int cellRow, int cellCol)
        {
            var currentGrid = SubGrids[subGridRow, subGridCol];

            if (MoveField[subGridRow, subGridCol] == false) return false;
            if (currentGrid.MakeMove(cellRow, cellCol, CurrentPlayer) == false) return false;

            if (currentGrid.Winner != CellType.None)
                MacroGrid[subGridRow, subGridCol] = CurrentPlayer;

            if (CheckWinner(CurrentPlayer))
                Winner = CurrentPlayer;
            else
                if (CheckFull())
                Winner = CellType.Draw;

            FillMoveField(subGridRow, subGridCol);
            SwitchPlayer();
            return true;
        }

        private void SwitchPlayer()
        {
            CurrentPlayer = CurrentPlayer == CellType.X ? CellType.O : CellType.X;
        }

        private bool CheckWinner(CellType player)
        {
            return CheckRowsCols(player) || CheckDiagonals(player);
        }

        private bool CheckRowsCols(CellType player)
        {
            for (int i = 0; i < 3; i++)
            {
                if (MacroGrid[i, 0] == player && MacroGrid[i, 1] == player && MacroGrid[i, 2] == player)
                    return true;
                if (MacroGrid[0, i] == player && MacroGrid[1, i] == player && MacroGrid[2, i] == player)
                    return true;
            }
            return false;
        }
        private bool CheckDiagonals(CellType player)
        {
            if (MacroGrid[0, 0] == player && MacroGrid[1, 1] == player && MacroGrid[2, 2] == player)
                return true;
            if (MacroGrid[0, 2] == player && MacroGrid[1, 1] == player && MacroGrid[2, 0] == player)
                return true;

            return false;
        }
        private bool CheckFull()
        {
            foreach (var grid in SubGrids)
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
