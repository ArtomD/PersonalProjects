using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFish
{
    public class GameLobbyPlayer: INotifyPropertyChanged
    {
        bool _IsReady;
        string _Name;
        bool _Disabled;
        int _HandSize;
        int _Books;

        public bool IsReady
        {
            get
            {
                return _IsReady;
            }
            set
            {
                if (_IsReady != value)
                {
                    _IsReady = value;
                    RaisePropertyChanged("IsReady");
                }
            }
        }

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

        public bool Disabled
        {
            get
            {
                return _Disabled;
            }
            set
            {
                if (_Disabled != value)
                {
                    _Disabled = value;
                    RaisePropertyChanged("Disabled");
                }
            }
        }

        public int HandSize
        {
            get
            {
                return _HandSize;
            }
            set
            {
                if (_HandSize != value)
                {
                    _HandSize = value;
                    RaisePropertyChanged("HandSize");
                }
            }
        }

        public int Books
        {
            get
            {
                return _Books;
            }
            set
            {
                if (_Books != value)
                {
                    _Books = value;
                    RaisePropertyChanged("Books");
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
