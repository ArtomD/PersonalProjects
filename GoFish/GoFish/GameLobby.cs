using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFish
{
    public class GameLobby: INotifyPropertyChanged
    {
        private string _Name;
        private GameLobbyPlayer _Owner;
        public ObservableCollection<GameLobbyPlayer> Players;
        private string _Capacity;
        int _DeckSize;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        public GameLobbyPlayer Owner
        {
            get
            {
                return _Owner;
            }
            set
            {
                if (_Owner != value)
                {
                    _Owner = value;
                    RaisePropertyChanged("Owner");
                }
            }
        }

        public int DeckSize
        {
            get
            {
                return _DeckSize;
            }
            set
            {
                if (_DeckSize != value)
                {
                    _DeckSize = value;
                    RaisePropertyChanged("DeckSize");
                }
            }
        }

        public string Capacity
        {
            get
            {
                return _Capacity;
            }
            set
            {
                if (_Capacity != value)
                {
                    _Capacity = value;
                    RaisePropertyChanged("Capacity");
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
