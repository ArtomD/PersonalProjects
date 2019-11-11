using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GoFishLibrary
{
    [DataContract]
    public class Deck
    {
        [DataMember] public List<Card> cards { get; set; }
        Random random;

        public Deck()
        {
            cards = new List<Card>();
            random = new Random();
            fillDeck52();
        }

        private void fillDeck52()
        {
            for(int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    cards.Add(new Card((Card.Suit)i, (Card.Rank)j));
                }
            }            
        }

        public Card Draw()
        {
            if (cards.Count > 0)
            {
                int cardIndex = random.Next(cards.Count);
                Card card;
                card = cards.ElementAt(cardIndex);
                cards.RemoveAt(cardIndex);
                return card;
            }
            return null;
        }
    }
}
