using GoFishLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;

namespace GoFish
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ICallback
    {
        public delegate int WindowDelegate(string arg);
        public delegate void HostLeftErrorDelegate();
        private IFishService fishService = null;
        public User user { get; set; }
        public GameLobby gameLobby { get; set; }
        public ObservableCollection<GameLobby> gameList { get; set; }
        public ObservableCollection<GameLobbyPlayer> lobbyPlayers { get; set; }
        public ObservableCollection<GameLobbyPlayer> selectablePlayers { get; set; }
        public ObservableCollection<CardRank> ranksAvaliable { get; set; }
        public GameLobby selectedGame { get; set; }
        public Chat gameChat { get; set; }
        public enum Screen {login, lobby, game_select, game, end_game }
        private Screen currentScreen;
        public UIVisibility ui { get; set; }
        private string selectedPlayer;
        private string selectedRank;
        public MainWindow()
        {
            InitializeComponent();
            user = new User();
            user.Hand = new ObservableCollection<LobbyCard>();
            gameLobby = new GameLobby();
            gameLobby.Players = new ObservableCollection<GameLobbyPlayer>();
            lobbyPlayers = new ObservableCollection<GameLobbyPlayer>();
            for (int i = 0; i < 8; i++)
            {
                lobbyPlayers.Add(new GameLobbyPlayer());
            }
            selectablePlayers = new ObservableCollection<GameLobbyPlayer>();
            gameList = new ObservableCollection<GameLobby>();
            ranksAvaliable = new ObservableCollection<CardRank>();
            ui = new UIVisibility();            
            gameChat = new Chat();
            this.DataContext = this;
            Closing += this.OnWindowClosing;
            currentScreen = Screen.login;
            activateScreen();
            
           
        }

        

        private void turnOffAllScreens()
        {
            login_screen.Visibility = Visibility.Collapsed;            
            lobby_screen.Visibility = Visibility.Collapsed;
            game_select_screen.Visibility = Visibility.Collapsed;
            game_screen.Visibility = Visibility.Collapsed;
            end_game_screen.Visibility = Visibility.Collapsed;
        }

        private void activateScreen()
        {
            turnOffAllScreens();
            switch (currentScreen)
            {
                case Screen.login:
                    login_screen.Visibility = Visibility.Visible;
                    break;                
                case Screen.lobby:
                    lobby_screen.Visibility = Visibility.Visible;
                    gameChat.Message = "";
                    break;
                case Screen.game_select:
                    game_select_screen.Visibility = Visibility.Visible;
                    break;
                case Screen.game:
                    game_screen.Visibility = Visibility.Visible;
                    gameChat.Message = "";
                    break;
                case Screen.end_game:
                    end_game_screen.Visibility = Visibility.Visible;
                    break;
            }              
        }

        private void waitingTurn(string name)
        {
            ui.IsNotTheirTurn = true;
            ui.IsSelectingPlayer = false;
            ui.IsSelectingRank = false;
            ui.IsConfirmingSelection = false;
            DisplayPlayerTurn.Content = "It is currently " + name + "'s turn.";
        }

        private void selectingPlayer()
        {
            ui.IsNotTheirTurn = false;
            ui.IsSelectingPlayer = true;
            ui.IsSelectingRank = false;
            ui.IsConfirmingSelection = false;
        }

        private void selectingRank()
        {
            ui.IsNotTheirTurn = false;
            ui.IsSelectingPlayer = false;
            ui.IsSelectingRank = true;
            ui.IsConfirmingSelection = false;
        }

        private void confirmingSelecton()
        {
            ui.IsNotTheirTurn = false;
            ui.IsSelectingPlayer = false;
            ui.IsSelectingRank = false;
            ui.IsConfirmingSelection = true;
        }

        private void login(object sender, RoutedEventArgs e)
        {
            try
            {
                DuplexChannelFactory<IFishService> channel;
                string hostName = Dns.GetHostName();
                string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();                
                if (endpoint_box.Text != "") {
                    myIP = endpoint_box.Text;
                }
                string endpointStr = "net.tcp://" + myIP + ":13200/CardsLibrary/FishService";
                //channel = new DuplexChannelFactory<IFishService>(this, "FishService", new EndpointAddress( endpointStr));
                //channel.Endpoint.ListenUri = new Uri(endpointStr);
                NetTcpBinding binding = new NetTcpBinding();
                binding.Security.Mode = SecurityMode.None;
                binding.Name = "FishService";
                
                channel = new DuplexChannelFactory<IFishService>(this, binding,
    new EndpointAddress(endpointStr));
                // Activate a MessageBoard object
                fishService = channel.CreateChannel();
            }
            catch (Exception ex)
            {
                
                ErrorWindow win2 = new ErrorWindow(ex.Message);
                win2.Show();
            }
            try
            {
                if (fishService.AddPlayer(login_box.Text))
                {
                    ViewGameList();

                    user.Username = login_box.Text;

                }
                else
                {
                    ErrorWindow win2 = new ErrorWindow("The username selected is allready in use. Please select another username");
                    win2.ShowDialog();
                }
            }
            catch (Exception ex)
            {

                ErrorWindow win2 = new ErrorWindow(ex.Message);
                win2.Show();
            }

        }

        private void logout(object sender, RoutedEventArgs e)
        {
            fishService.RemovePlayer(user.Username);
            user.Username = "";
            currentScreen = Screen.login;
            activateScreen();

        }

        private void createGameButton(object sender, RoutedEventArgs e)
        {
            WindowDelegate func = new WindowDelegate(createGame);
            CreateGame createWin = new CreateGame(func);
            createWin.ShowDialog();
        }

        public int createGame(string name)
        {
            int status = fishService.AddGame(name, user.Username);
            if (status == 0)
            {
                fishService.JoinGame(user.Username, name);
                currentScreen = Screen.lobby;
                activateScreen();
                //Console.WriteLine(gameLobby.name);
            }
            else if(status== 1)
            {
                ErrorWindow win2 = new ErrorWindow("A game with that name already exists. Please select a different name");
                win2.ShowDialog();
            }
            else
            {
                ErrorWindow win2 = new ErrorWindow("An error has occured. Please try again.");
                win2.ShowDialog();
            }
            return status;
        }




        private delegate void GameUpdateDelegate(Game game);
        public void UpdateGameLobby(Game game)
        {
            if (this.Dispatcher.Thread == System.Threading.Thread.CurrentThread)
            {
                if (game == null)
                {
                    quitLobbyPopup();
                    return;
                }
                try
                {                    
                    gameLobby.Name = game.name;
                    GameLobbyPlayer owner = new GameLobbyPlayer();
                    owner.Name = game.owner.name;
                    gameLobby.Owner = owner;
                    gameLobby.Players.Clear();
                   
                    foreach(Player player in game.users)
                    {
                        GameLobbyPlayer newPlayer = new GameLobbyPlayer();
                        newPlayer.Name = player.name;
                        newPlayer.IsReady = player.isReady;
                        gameLobby.Players.Add(newPlayer);
                        
                    }
                    int c = 0;
                    for (int i =0; i< gameLobby.Players.Count; i++)
                    {
                        GameLobbyPlayer newPlayer = new GameLobbyPlayer();
                        newPlayer.Name = gameLobby.Players[i].Name;
                        newPlayer.IsReady = gameLobby.Players[i].IsReady;
                        newPlayer.Disabled = game.owner.name == user.Username ? false : true;
                        lobbyPlayers[i] = newPlayer;
                        c++;
                    }
                    for (int i = c; i < 8; i++)
                    {
                        if (lobbyPlayers.Count <= i)
                        {
                            lobbyPlayers.Add(new GameLobbyPlayer());
                        }
                        else
                        {
                            lobbyPlayers[i] = new GameLobbyPlayer();
                        }
                    }

                    if (gameLobby.Owner.Name == user.Username)
                    {
                        startGameButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        startGameButton.Visibility = Visibility.Collapsed;
                    }

                    if (currentScreen == Screen.game)
                    {
                        for(int i = lobbyPlayers.Count-1; i>=0; i--)
                        {
                            if(lobbyPlayers[i].Name == null || lobbyPlayers[i].Name == "")
                            {
                                lobbyPlayers.RemoveAt(i);
                            }
                        }
                        gameLobby.DeckSize = game.deck.cards.Count;
                        int numBooks = 0;
                        selectablePlayers.Clear();
                        foreach (GameLobbyPlayer lobbyPlayer in lobbyPlayers)
                        {
                            Player tmpPlayer = game.users.Find(player => player.name == lobbyPlayer.Name);
                            lobbyPlayer.HandSize = tmpPlayer.hand.cards.Count;
                            lobbyPlayer.Books = tmpPlayer.books;
                            numBooks += lobbyPlayer.Books;
                            if (lobbyPlayer.Name != user.Username)
                            {
                                selectablePlayers.Add(lobbyPlayer);
                            }
                        }
                        DisplayDeckAmount.Content = "Cards in sea: " + game.deck.cards.Count;
                        DisplayBookAmount.Content = "Books created: " + numBooks;
                        foreach (Player player in game.users)
                        {
                            if (player.name == user.Username)
                            {
                                user.Hand.Clear();
                                ranksAvaliable.Clear();
                                player.hand.cards = player.hand.cards.OrderBy(card => card.rank).ThenBy(card => card.suit).ToList();                                    
                                foreach (Card card in player.hand.cards)
                                {
                                    bool found = false;
                                    foreach (CardRank rank in ranksAvaliable)
                                    {
                                        if (rank.Rank == card.rank.ToString())
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found)
                                    {
                                        CardRank newRank = new CardRank();
                                        newRank.Rank = card.rank.ToString();
                                        ranksAvaliable.Add(newRank);
                                    }
                                    user.Hand.Add(new LobbyCard(card.suit,card.rank));
                                }
                                break;
                                

                            }
                        }
                        if(game.users[game.currentPlayer].name == user.Username)
                        {
                            selectingPlayer();
                        }
                        else
                        {
                            waitingTurn(game.users[game.currentPlayer].name);
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                this.Dispatcher.BeginInvoke(new GameUpdateDelegate(
               UpdateGameLobby), new object[] { game });
            }
        }

        private delegate void GameDoesNotExistUpdateDelegate();
        public void GameDoesNotExist()
        {
            if (this.Dispatcher.Thread == System.Threading.Thread.CurrentThread)
            {
                currentScreen = Screen.game_select;
                activateScreen();
                ErrorWindow win2 = new ErrorWindow("The selected game does not exist. Please refresh the browser.");
                win2.Show();
            }
            else
            {
                this.Dispatcher.BeginInvoke(new GameDoesNotExistUpdateDelegate(
               GameDoesNotExist), new object[] { });
            }
        }

        private delegate void GameIsFullUpdateDelegate();
        public void GameIsFull()
        {
            if (this.Dispatcher.Thread == System.Threading.Thread.CurrentThread)
            {
                currentScreen = Screen.game_select;
                activateScreen();
                ErrorWindow win2 = new ErrorWindow("The selected game is full.");
                win2.Show();
            }
            else
            {
                this.Dispatcher.BeginInvoke(new GameIsFullUpdateDelegate(
               GameIsFull), new object[] { });
            }
        }

        private delegate void GameChatUpdateDelegate(string message);
        public void UpdateGameMessage(string message)
        {
            if (this.Dispatcher.Thread == System.Threading.Thread.CurrentThread)
            {
                gameChat.Message += message+"\n";
                chatOutput.ScrollToEnd();
                gameChatOutput.ScrollToEnd();
            }
            else
            {
                this.Dispatcher.BeginInvoke(new GameChatUpdateDelegate(
               UpdateGameMessage), new object[] { message });
            }
        }

        private delegate void StartGameDelegate();
        public void StartGame()
        {
            if (this.Dispatcher.Thread == System.Threading.Thread.CurrentThread)
            {
                currentScreen = Screen.game;
                activateScreen();
            }
            else
            {
                this.Dispatcher.BeginInvoke(new StartGameDelegate(
               StartGame), new object[] { });
            }
        }

        private delegate void PlayerQuitDelegate();
        public void PlayerHasQuit()
        {
            if (this.Dispatcher.Thread == System.Threading.Thread.CurrentThread)
            {
                playerLeftGamePopup();
            }
            else
            {
                this.Dispatcher.BeginInvoke(new PlayerQuitDelegate(
               PlayerHasQuit), new object[] { });
            }
        }

        private delegate void EndGameUpdateDelegate(Game game);
        public void EndGame(Game game)
        {
            if (this.Dispatcher.Thread == System.Threading.Thread.CurrentThread)
            {
                int books = 0;
                string playerNames = "";
                bool first = true;
                bool changePostfix = false;
                foreach (Player player in game.users)
                {
                    if(player.books >= books)
                    {
                        books = player.books;
                    }
                }
                foreach (Player player in game.users)
                {
                    if(player.books == books)
                    {
                        if (!first)
                        {
                            playerNames += ", ";
                            changePostfix = true;
                        }
                        playerNames += player.name;
                        first = false;
                    }
                }
                string postfix = " has won the game.";
                if (changePostfix)
                {
                    postfix = " have tied for first."; 
                }

                VictoryMessage.Content = playerNames + postfix;
                this.currentScreen = Screen.end_game;
                activateScreen();
            }
            else
            {
                this.Dispatcher.BeginInvoke(new EndGameUpdateDelegate(
               EndGame), new object[] { game });
            }
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (currentScreen == Screen.lobby || currentScreen == Screen.game)
                fishService.QuitGame(user.Username, gameLobby.Name);
            if (user.Username!=null && user.Username.Length>0)
                fishService.RemovePlayer(user.Username);            
        }

        public void quitGame(object sender, RoutedEventArgs e)
        {            
            Close();
        }


        private void ViewGameList(object sender, RoutedEventArgs e)
        {
            ViewGameList();
        }

        private void ViewGameList()
        {
            List<Game> allGames = fishService.GetAllGames();
            gameList.Clear();
            foreach (Game game in allGames)
            {
                gameList.Add(GameToGameLobby(game));
            }
            currentScreen = Screen.game_select;
            activateScreen();
        }

        private GameLobby GameToGameLobby(Game game)
        {
            GameLobby gameL = new GameLobby();
            gameL.Name = game.name;
            GameLobbyPlayer owner = new GameLobbyPlayer();
            owner.Name = game.owner.name;
            gameL.Owner = owner;
            gameL.Players = new ObservableCollection<GameLobbyPlayer>();
            foreach (Player player in game.users)
            {
                GameLobbyPlayer newPlayer = new GameLobbyPlayer();
                newPlayer.Name = player.name;
                gameL.Players.Add(newPlayer);
            }
            gameL.Capacity = game.users.Count + "/" + game.maxPLayers;
            return gameL;
        }


        private void joinSelectedGame(object sender, RoutedEventArgs e)
        {
            if (selectedGame != null)
            {
                fishService.JoinGame(user.Username, selectedGame.Name);
                currentScreen = Screen.lobby;
                activateScreen();
                
            }
        }

        private void quitlobby(object sender, RoutedEventArgs e)
        {
            fishService.QuitGame(user.Username, gameLobby.Name);
            quitLobby();
        }

        private void quitLobby()
        {
            ViewGameList();
        }

        private void quitLobbyPopup()
        {
            currentScreen = Screen.game_select;
            activateScreen();
            HostLeftErrorDelegate func = new HostLeftErrorDelegate(quitLobby);
            ErrorWindow win2 = new ErrorWindow("The game host has left", func);            
            win2.Show();
        }

        private void playerLeftGamePopup()
        {
            currentScreen = Screen.game_select;
            activateScreen();
            HostLeftErrorDelegate func = new HostLeftErrorDelegate(quitLobby);
            ErrorWindow win2 = new ErrorWindow("A player has left the game.", func);
            win2.Show();
        }

        private void refreshGameList(object sender, RoutedEventArgs e)
        {
            ViewGameList();
        }

        private void sendMessage(object sender, RoutedEventArgs e)
        {
            if (chatInput.Text.Length > 0)
            {
                fishService.PostMessage(user.Username + ": " + chatInput.Text, gameLobby.Name);
                chatInput.Text = "";
            }
        }

        private void sendGameMessage(object sender, RoutedEventArgs e)
        {
            if (gameChatInput.Text.Length > 0)
            {
                fishService.PostMessage(user.Username + ": " + gameChatInput.Text, gameLobby.Name);
                gameChatInput.Text = "";
            }
        }


        private void playerIsReady(object sender, RoutedEventArgs e)
        {
            readyButton.Visibility = Visibility.Collapsed;
            unreadyButton.Visibility = Visibility.Visible;
            fishService.IsReady(user.Username, gameLobby.Name, true);
        }

        private void playerIsNotReady(object sender, RoutedEventArgs e)
        {
            readyButton.Visibility = Visibility.Visible;
            unreadyButton.Visibility = Visibility.Collapsed;
            fishService.IsReady(user.Username, gameLobby.Name, false);
        }

        private void startGame(object sender, RoutedEventArgs e)
        {
            bool allReady = true;
            string message = "";
            if (gameLobby.Players.Count<2)
            {
                message = "Not enough players";
                allReady= false;
            }

            foreach (GameLobbyPlayer player in gameLobby.Players)
            {
                if (!player.IsReady)
                {
                    message += player.Name + " is not ready!\n";
                    allReady = false;
                }
            }
            if (allReady)
            {
                message = "Staring Game!";
            }

            fishService.PostMessage(message, gameLobby.Name);
            if (allReady)
            {
                fishService.StartGame(gameLobby.Name);
            }
           
        }

        private void SelectPlayer(object sender, RoutedEventArgs e)
        {
            selectedPlayer = ((Button)e.Source).Content.ToString();
            selectingRank();
        }

        private void BackToSelectPlayer(object sender, RoutedEventArgs e)
        {            
            selectingPlayer();
        }

        private void SelectRank(object sender, RoutedEventArgs e)
        {
            selectedRank = ((Button)e.Source).Content.ToString();
            ConfirmSelectionText.Content = "Ask " + selectedPlayer + " for " + selectedRank + "(s)?";
            confirmingSelecton();
        }

        private void BackToSelectRank(object sender, RoutedEventArgs e)
        {
            selectingRank();
        }

        private void ConfirmSelection(object sender, RoutedEventArgs e)
        {
            fishService.TakeCard(user.Username, gameLobby.Name, (int)((Card.Rank)Enum.Parse(typeof(Card.Rank), selectedRank)), selectedPlayer);
        }

        private void quitToSelectScreen(object sender, RoutedEventArgs e)
        {
            ViewGameList();
        }

        private void showRules(object sender, RoutedEventArgs e)
        {

            RulesWindow window = new RulesWindow();
            window.Show();
        }

    }
}
