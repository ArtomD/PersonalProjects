using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace GoFishLibrary
{

    [DataContract]
    public class Card
    {
        public enum Suit {Heart, Diamond, Spade, Club }
        public enum Rank { Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King}

        [DataMember] public Suit suit { get; private set; }
        [DataMember] public Rank rank { get; private set; }

        public Card(Suit suit, Rank rank)
        {
            this.suit = suit;
            this.rank = rank;
        }

        public override string ToString()
        {
            return Enum.GetName(typeof(Rank), rank) + " of " + Enum.GetName(typeof(Suit), suit) + "s";
        }
    }
}
