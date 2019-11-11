using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace GoFishLibrary
{

    public interface ICallback
    {        
        [OperationContract(IsOneWay = true)]
        void UpdateGameLobby(Game game);
        [OperationContract(IsOneWay = true)]
        void GameDoesNotExist();
        [OperationContract(IsOneWay = true)]
        void GameIsFull();
        [OperationContract(IsOneWay = true)]
        void UpdateGameMessage(string message);
        [OperationContract(IsOneWay = true)]
        void StartGame();
        [OperationContract(IsOneWay = true)]
        void PlayerHasQuit();
        [OperationContract(IsOneWay = true)]
        void EndGame(Game game);
    }

    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IFishService
    {
        [OperationContract] bool AddPlayer(string name);
        [OperationContract(IsOneWay = true)] void RemovePlayer(string name);
        [OperationContract] int AddGame(string name, string owner);
        [OperationContract(IsOneWay = true)] void RemoveGame(string name);
        [OperationContract(IsOneWay = true)] void JoinGame(string name, string roomName);
        [OperationContract(IsOneWay = true)] void QuitGame(string name, string roomName);
        [OperationContract] List<Game> GetAllGames();
        [OperationContract(IsOneWay = true)] void PostMessage(string message, string room);
        [OperationContract(IsOneWay = true)] void IsReady(string name, string room, bool isReady);
        [OperationContract(IsOneWay = true)] void StartGame(string room);
        [OperationContract(IsOneWay = true)] void TakeCard(string name, string room, int rank, string targetName);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class FishService : IFishService
    {
        private Dictionary<string, ICallback> callbacks = new Dictionary<string, ICallback>();
        public PlayerManager playerManager;
        public GameManager gameManager;

        public FishService()
        {
            playerManager = PlayerManager.getInstance();
            gameManager = GameManager.getInstance();
        }

        public bool AddPlayer(string name)
        {
            ICallback cb = OperationContext.Current.GetCallbackChannel<ICallback>();
            return playerManager.AddPlayer(name, cb);            
        }

        public void RemovePlayer(string name)
        {
            playerManager.RemovePlayer(name);
            if (callbacks.ContainsKey(name))
            {
                callbacks.Remove(name);
            }
        }

        public int AddGame(string name,string owner)
        {
            return gameManager.AddGame(name,owner);
        }

        public void RemoveGame(string name)
        {
            gameManager.RemoveGame(name);
        }

        public void JoinGame(string name, string roomName)
        {
            Player player = playerManager.FindPlayer(name);
            Game game = gameManager.FindGame(roomName);            
            if(game.users.Count >= 8)
            {
                player.callback.GameIsFull();
                return;
            }
            game = gameManager.JoinGame(player, roomName);


            if (game != null)
            {
                foreach (Player gamePlayer in game.users)
                {
                    gamePlayer.callback.UpdateGameLobby(game);
                    gamePlayer.callback.UpdateGameMessage(name + " has entered the room.");
                }
            }
            else
            {
                player.callback.GameDoesNotExist();
            }

        }

        public void QuitGame(string name, string roomName)
        {
            Player player = playerManager.FindPlayer(name);
            Game game = gameManager.QuitGame(player, roomName);
            if (!game.isOver)
            {
                foreach (Player gamePlayer in game.users)
                {
                    gamePlayer.callback.PlayerHasQuit();
                }
                gameManager.RemoveGame(game.name);
                return;
            }
            if (game.owner.name == name)
            {
                foreach (Player gamePlayer in game.users)
                {
                    gamePlayer.callback.UpdateGameLobby(null);
                }
                gameManager.RemoveGame(roomName);
            }
            else
            {
                foreach (Player gamePlayer in game.users)
                {
                    gamePlayer.callback.UpdateGameLobby(game);
                    gamePlayer.callback.UpdateGameMessage(name+" has left the room.");
                }
            }
        }

        public List<Game> GetAllGames()
        {
            return gameManager.GetAllGames();
        }

        public void PostMessage(string message, string room)
        {
            Game game = gameManager.FindGame(room);
            foreach(Player gamePlayer in game.users)
            {
                gamePlayer.callback.UpdateGameMessage(message);
            }
        }

        public void IsReady(string name, string room, bool isReady)
        {
            Game game = gameManager.FindGame(room);
            foreach (Player gamePlayer in game.users)
            {
                if (gamePlayer.name == name)
                {
                    gamePlayer.isReady = isReady;
                    break;
                    
                }
            }
            string msg = isReady ? name + " is ready." : name + " is not ready.";
            foreach (Player gamePlayer in game.users)
            {                
                gamePlayer.callback.UpdateGameLobby(game);
                gamePlayer.callback.UpdateGameMessage(msg);
            }           
        }

        public void StartGame(string room)
        {
            Game game = gameManager.FindGame(room);
            game.SetPlayerOrder();
            game.deck = new Deck();
            game.cardsToDraw = 7;
            game.isOver = false;
            if (game.users.Count > 3)
            {
                game.cardsToDraw = 5;
            }
            foreach (Player gamePlayer in game.users)
            {
                gamePlayer.hand.cards = new List<Card>();
                gamePlayer.hand.cards.Clear();
                gamePlayer.canPlay = true;
            }

            for (int i = 0; i < game.cardsToDraw; i++)
            {
                foreach (Player gamePlayer in game.users)
                {
                    gamePlayer.hand.cards.Add(game.deck.Draw());
                }
            }
            string message = "Game started.\nIt is " + game.users[game.currentPlayer].name + "'s turn.";
            foreach (Player gamePlayer in game.users)
            {               
                gamePlayer.callback.StartGame();
                gamePlayer.callback.UpdateGameLobby(game);
                gamePlayer.callback.UpdateGameMessage(message);
            }
            foreach (Player gamePlayer in game.users)
            {
                game.processGameState(gamePlayer);
            }
        }

        public void TakeCard(string name, string room, int rank, string targetName)
        {
            
            Player target = playerManager.FindPlayer(targetName);
            Player player = playerManager.FindPlayer(name);
            Game game = gameManager.FindGame(room);
            foreach (Player gamePlayer in game.users)
            {
                gamePlayer.callback.UpdateGameMessage(name+ " asked " + targetName + " for " + ((Card.Rank)rank).ToString() + "s");
            }
            Hand tempHand = new Hand();
            tempHand.cards = target.hand.RemoveCards(rank);

            if(tempHand.cards.Count > 0)
            {
                foreach(Card card in tempHand.cards)
                {
                    player.hand.cards.Add(card);
                }
                foreach (Player gamePlayer in game.users)
                {
                    gamePlayer.callback.UpdateGameMessage(targetName + " gave " + tempHand.cards.Count + " " + ((Card.Rank)rank).ToString() + "(s) to " + name);
                }
                game.processGameState(target);
            }
            else
            {
                foreach (Player gamePlayer in game.users)
                {
                    gamePlayer.callback.UpdateGameMessage(targetName + " had none and "+name + " is drawing a card from the sea.");
                }
                Card newCard = game.deck.Draw();
                if (newCard!=null) {
                    if (newCard.rank == (Card.Rank)rank)
                    {
                        game.currentPlayer--;
                        foreach (Player gamePlayer in game.users)
                        {
                            gamePlayer.callback.UpdateGameMessage(name + " drew a " + ((Card.Rank)rank).ToString() + " from the sea and gets to go again.");
                        }
                    }
                    else
                    {
                        player.callback.UpdateGameMessage("You drew a " + ((Card.Rank)newCard.rank).ToString() + " from the sea.");
                    }
                    player.hand.cards.Add(newCard);
                }
                else
                {
                    foreach (Player gamePlayer in game.users)
                    {
                        gamePlayer.callback.UpdateGameMessage(name + " cannot draw a card because the sea is empty.");
                    }
                }
            }
            game.processGameState(player);
            if (game.isOver)
            {
                foreach (Player gamePlayer in game.users)
                {
                    gamePlayer.callback.EndGame(game);
                }
                gameManager.RemoveGame(game.name);
                return;
            }
            do
            {
                game.currentPlayer++;
                if(game.currentPlayer >= game.users.Count)
                {
                    game.currentPlayer = 0;
                }
            }
            while (!game.users[game.currentPlayer].canPlay);
            

            foreach (Player gamePlayer in game.users)
            {
                gamePlayer.callback.UpdateGameLobby(game);
            }
            
           
        }

       
    }
}
