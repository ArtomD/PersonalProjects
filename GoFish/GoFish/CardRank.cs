using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFish
{
    public class CardRank : INotifyPropertyChanged
    {
        private string _Rank;

        public string Rank
        {
            get
            {
                return _Rank;
            }
            set
            {
                if (_Rank != value)
                {
                    _Rank = value;
                    RaisePropertyChanged("Rank");
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
