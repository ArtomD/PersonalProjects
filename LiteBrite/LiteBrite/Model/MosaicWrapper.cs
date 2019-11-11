using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteBrite.Model
{
    [Serializable]
    public class MosaicWrapper : INotifyPropertyChanged
    {
        
        public MosaicWrapper(Mosaic mosaic)
        {
            this.Mosaic = mosaic;
      
        }
        Mosaic _Mosaic;
        public Mosaic Mosaic
        {
            get
            {
                return _Mosaic;
            }
            set
            {
                if (_Mosaic != value)
                {
                    _Mosaic = value;
                    RaisePropertyChanged("Mosaic");
                }
            }
        }

        public void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
