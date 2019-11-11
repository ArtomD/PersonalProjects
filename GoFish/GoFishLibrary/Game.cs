using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GoFishLibrary
{
    [DataContract]
    public class Game
    {
        [DataMember] public string name { get; set; }
        [DataMember] public List<Player> users { get; set; }
        [DataMember] public Player owner { get; set; }
        [DataMember] public Deck deck { get; set; }
        [DataMember] public int[] order { get; set; }
        [DataMember] public int currentPlayer { get; set; }
        [DataMember] public int cardsToDraw { get; set; }
        [DataMember] public bool isOver { get; set; }
        [DataMember] public int maxPLayers { get; set; }
        public Game(string name, Player owner)
        {
            this.name = name;
            this.owner = owner;
            this.owner.isReady = true;
            this.users = new List<Player>();
            this.maxPLayers = 8;
            this.isOver = true;
        }

        public void SetPlayerOrder()
        {
            Random rand = new Random();
            order = new int[users.Count];
            for(int i =0; i< order.Length; i++)
            {
                order[i] = i;
            }
            for (int i = 0; i < order.Length - 1; i++)
            {
                int j = rand.Next(i, order.Length);
                int temp = order[i];
                order[i] = order[j];
                order[j] = temp;
            }
        }

        public void processGameState(Player player)
        {
            Card.Rank[] ranks = {Card.Rank.Ace, Card.Rank.Two, Card.Rank.Three, Card.Rank.Four, Card.Rank.Five, Card.Rank.Six,
            Card.Rank.Seven, Card.Rank.Eight, Card.Rank.Nine, Card.Rank.Ten, Card.Rank.Jack, Card.Rank.Queen, Card.Rank.King};

            if (player.hand.cards.Count < 1)
            {
                int numCards = this.cardsToDraw;
                if (numCards > this.deck.cards.Count)
                {
                    numCards = this.deck.cards.Count;
                }
                for (int i = 0; i < numCards; i++)
                {
                    player.hand.cards.Add(this.deck.Draw());
                }
                string message = player.name + " has run out of cards and draws " + numCards + " more cards out of the sea.";
                if (numCards <= 0)
                {
                    message = player.name + " has run out of cards and no more are in the sea. They can make no more moves.";
                    player.canPlay = false;
                }
                foreach (Player gamePlayer in this.users)
                {
                    gamePlayer.callback.UpdateGameMessage(message);
                }
            }

            for (int i = 0; i < ranks.Length; i++)
            {
                List<Card> book = player.hand.cards.FindAll(innerCard => innerCard.rank == ranks[i]);
                if (book.Count == 4)
                {
                    player.books++;
                    foreach (Card card in book)
                    {
                        player.hand.cards.Remove(card);
                    }

                    foreach (Player gamePlayer in this.users)
                    {
                        gamePlayer.callback.UpdateGameMessage(player.name + " has made a book of " + ranks[i].ToString() + "s");
                    }
                    processGameState(player);
                }
            }
            bool cardsRemaining = false;
            foreach (Player gamePlayer in this.users)
            {
                if (gamePlayer.hand.cards.Count > 0)
                {
                    cardsRemaining = true;
                }
            }
            if (!cardsRemaining)
            {
                this.isOver = true;
            }

        }
    }
}
