using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFishLibrary
{
    public class GameManager
    {
        public static GameManager instance;
        private List<Game> games;

        private GameManager()
        {

        }
        public static GameManager getInstance()
        {
            if (instance == null)
            {
                instance = new GameManager();
                instance.games = new List<Game>();
            }
            return instance;
        }

        public int AddGame(string name, string owner)
        {
            foreach (Game game in games)
            {
                if (game.name == name)
                {
                    return 1;
                }
            }
            PlayerManager playerManager = PlayerManager.getInstance();
            foreach (Player player in playerManager.players)
            {
                if (player.name == owner)
                {
                    
                    games.Add(new Game(name, player));                    
                    return 0;
                }
                
            }
            return 2;
            
        }

        public void RemoveGame(string name)
        {
            foreach (Game game in games)
            {
                if (game.name == name)
                {
                    games.Remove(game);
                    return;
                }
            }
            
        }

        public Game FindGame(string name)
        {
            foreach (Game game in games)
            {
                if (game.name == name)
                {
                    return game;
                }
            }
            return null;
        }

        public Game JoinGame(Player player, string roomName)
        {
            foreach (Game game in games)
            {
                if (game.name == roomName)
                {
                    game.users.Add(player);
                    return game;
                }
            }
            return null;

        }

        public Game QuitGame(Player player, string roomName)
        {
            foreach (Game game in games)
            {
                if (game.name == roomName)
                {
                    game.users.Remove(player);
                    
                    return game;

                }
            }

            return null;

        }

        public List<Game> GetAllGames()
        {
            return games;
        }
    }
}
