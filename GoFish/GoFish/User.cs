using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GoFish
{
    public class User : INotifyPropertyChanged
    {
        private string _Username;
        private ObservableCollection<LobbyCard> _Hand;

        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                if (_Username != value)
                {
                    _Username = value;
                    RaisePropertyChanged("Username");
                }
            }
        }

        public ObservableCollection<LobbyCard> Hand
        {
            get
            {
                return _Hand;
            }
            set
            {
                if (_Hand != value)
                {
                    _Hand = value;
                    RaisePropertyChanged("Hand");
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
