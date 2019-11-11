using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GoFishLibrary
{
    [DataContract]
    public class Hand
    {
        [DataMember] public List<Card> cards { get; set; }

        public List<Card> RemoveCards(int rank)
        {
            List <Card> removedCards = cards.FindAll(innerCard => innerCard.rank == (Card.Rank)rank);
            foreach(Card removeCard in removedCards)
            {
                cards.Remove(removeCard);
            }
            return removedCards;
        }

    }
}
