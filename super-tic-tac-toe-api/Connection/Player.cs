using super_tic_tac_toe_api.Logic.Enums;

namespace super_tic_tac_toe_api
{
    internal class Player
    {
        public string Name { get; set; }
        public CellType PlayerType { get; set; }

        public Player(string name, CellType playerType)
        {
            Name = name;
            PlayerType = playerType;
        }
    }
}
