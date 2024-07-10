using super_tic_tac_toe_logic.Enums;

namespace super_tic_tac_toe_logic
{
    internal class Game
    {
        private SubGrid[,] _subGrids;
        private CellType[,] _macroGrid;
        private CellType _currentPlayer;
        public CellType Winner { get; private set; }

        public Game()
        {
            _subGrids = new SubGrid[3, 3];
            _macroGrid = new CellType[3, 3];
            _currentPlayer = CellType.X;
            Winner = CellType.None;

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    _subGrids[i, j] = new SubGrid();
        }

        public bool MakeMove(int subGridRow, int subGridCol, int cellRow, int cellCol)
        {
            var currentGrid = _subGrids[subGridRow, subGridCol];
            if (!currentGrid.MakeMove(cellRow, cellCol, _currentPlayer))
            {
                return false;
            }

            if (currentGrid.Winner == _currentPlayer)
                _macroGrid[subGridRow, subGridCol] = _currentPlayer;

            if (CheckWinner(_currentPlayer))
                Winner = _currentPlayer;

            SwitchPlayer();
            return true;
        }

        private void SwitchPlayer()
        {
            _currentPlayer = _currentPlayer == CellType.X ? CellType.O : CellType.X;
        }

        private bool CheckWinner(CellType player)
        {
            for (int i = 0; i < 3; i++)
            {
                if (_macroGrid[i, 0] == player && _macroGrid[i, 1] == player && _macroGrid[i, 2] == player)
                    return true;
                if (_macroGrid[0, i] == player && _macroGrid[1, i] == player && _macroGrid[2, i] == player)
                    return true;
            }
            if (_macroGrid[0, 0] == player && _macroGrid[1, 1] == player && _macroGrid[2, 2] == player)
                return true;
            if (_macroGrid[0, 2] == player && _macroGrid[1, 1] == player && _macroGrid[2, 0] == player)
                return true;

            return false;
        }
    }
}
