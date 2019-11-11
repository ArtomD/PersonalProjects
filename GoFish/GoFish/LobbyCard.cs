using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GoFishLibrary.Card;

namespace GoFish
{
    public class LobbyCard : INotifyPropertyChanged
    {

        public Suit suit;
        public Rank rank;
        private string _Text;

        public LobbyCard(Suit suit, Rank rank)
        {
            this.suit = suit;
            this.rank = rank;
            this.Text = ToString();
        }

        public override string ToString()
        {
            return Enum.GetName(typeof(Rank), rank) + " of " + Enum.GetName(typeof(Suit), suit) + "s";
        }

        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                if (_Text != value)
                {
                    _Text = value;
                    RaisePropertyChanged("Text");
                }
            }
        }

        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
