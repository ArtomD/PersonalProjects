using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LiteBrite.Model
{
    public class GridProperties : INotifyPropertyChanged
    {
        private int _GridSize { get; set; }
        private int _GridHeight { get; set; }
        private int _GridWidth { get; set; }
        public int GridHeightCount { get; set; }
        public int GridWidthCount { get; set; }
        private int _MinMosaicSize { get; set; }
        private MosaicColors _BackgroundColor { get; set; }
        private bool _Light { get; set; }


        public int GridSize
        {
            get
            {
                return _GridSize;
            }
            set
            {
                if (_GridSize != value)
                {
                    _GridSize = value;
                    RaisePropertyChanged("GridSize");
                }
            }
        }

        public int GridHeight
        {
            get
            {
                return _GridHeight;
            }
            set
            {
                if (_GridHeight != value)
                {
                    _GridHeight = value;
                    RaisePropertyChanged("GridHeight");
                }
            }
        }

        public int GridWidth
        {
            get
            {
                return _GridWidth;
            }
            set
            {
                if (_GridWidth != value)
                {
                    _GridWidth = value;
                    RaisePropertyChanged("GridWidth");
                }
            }
        }

        public int MinMosaicSize
        {
            get
            {
                return _MinMosaicSize;
            }
            set
            {
                if (_MinMosaicSize != value)
                {
                    _MinMosaicSize = value;
                    RaisePropertyChanged("MinMosaicSize");
                }
            }
        }


        public MosaicColors BackgroundColor
        {
            get
            {
                return _BackgroundColor;
            }
            set
            {
                if (_BackgroundColor != value)
                {
                    _BackgroundColor = value;
                    RaisePropertyChanged("BackgroundColor");
                }
            }
        }


        public bool Light
        {
            get
            {
                return _Light;
            }
            set
            {
                if (_Light != value)
                {
                    _Light = value;
                    RaisePropertyChanged("Light");
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
