using super_tic_tac_toe_api.Logic;

namespace super_tic_tac_toe_api.Entities
{
    internal class Lobby
    {
        public int LobbyId { get; private set; }
        public List<Player> Players { get; private set; }
        public Game CurrentGame { get; private set; }

        public Lobby()
        {
            LobbyId = GenerateLobbyId();
            Players = new List<Player>();
            CurrentGame = new Game();
        }

        private int GenerateLobbyId()
        {
            var rnd = new Random();
            return rnd.Next(10000000, 99999999);
        }

        public bool AddPlayer(Player player)
        {
            if (Players.Count >= 2)
                return false;

            Players.Add(player);
            return true;
        }
    }
}
