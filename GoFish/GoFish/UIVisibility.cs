using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFish
{
    public class UIVisibility : INotifyPropertyChanged
    {
        private bool _IsSelectingPlayer;

        public bool IsSelectingPlayer
        {
            get
            {
                return _IsSelectingPlayer;
            }
            set
            {
                if (_IsSelectingPlayer != value)
                {
                    _IsSelectingPlayer = value;
                    RaisePropertyChanged("IsSelectingPlayer");
                }
            }
        }

        private bool _IsSelectingRank;

        public bool IsSelectingRank
        {
            get
            {
                return _IsSelectingRank;
            }
            set
            {
                if (_IsSelectingRank != value)
                {
                    _IsSelectingRank = value;
                    RaisePropertyChanged("IsSelectingRank");
                }
            }
        }

        private bool _IsNotTheirTurn;

        public bool IsNotTheirTurn
        {
            get
            {
                return _IsNotTheirTurn;
            }
            set
            {
                if (_IsNotTheirTurn != value)
                {
                    _IsNotTheirTurn = value;
                    RaisePropertyChanged("IsNotTheirTurn");
                }
            }
        }

        private bool _IsConfirmingSelection;

        public bool IsConfirmingSelection
        {
            get
            {
                return _IsConfirmingSelection;
            }
            set
            {
                if (_IsConfirmingSelection != value)
                {
                    _IsConfirmingSelection = value;
                    RaisePropertyChanged("IsConfirmingSelection");
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
