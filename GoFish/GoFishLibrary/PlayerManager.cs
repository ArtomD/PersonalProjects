using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFishLibrary
{
    public class PlayerManager
    {
        private static PlayerManager instance;
        public List<Player> players;

        private PlayerManager()
        {

        }
        public static PlayerManager getInstance()
        {
            if(instance == null)
            {
                instance = new PlayerManager();
                instance.players = new List<Player>();
            }            
            return instance;
        }

        public bool AddPlayer(string name, ICallback callback)
        {
            foreach(Player player in players)
            {
                if (player.name == name)
                {
                    return false;
                }
            }
            players.Add(new Player(name, callback));
            return true;
        }

        public Player FindPlayer(string name)
        {
            foreach (Player player in players)
            {
                if (player.name == name)
                {
                    return player;
                }
            }
            return null;
        }

        public void RemovePlayer(string name)
        {
            foreach (Player player in players)
            {
                if (player.name == name)
                {
                    players.Remove(player);
                    return;
                }
            }
            
        }
    }
}
