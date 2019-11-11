using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LiteBrite.Model
{
    [Serializable]
    public class Mosaic : INotifyPropertyChanged
    {
        public static int maxMosaicSize = 16;
        public static int minMosaicSize = 10;

        public Mosaic(int counter)
        {
            this.Color = new MosaicColors(Brushes.Transparent, "None");
            this.IsCircle = true;
            this.IsPolygon = false;
            this.Size = Mosaic.maxMosaicSize;
            this.Index = counter;
        }

        public Mosaic(int counter, MosaicColors color, bool isCircle, bool isPolygon, int size)
        {
            this.Color = color;
            this.IsCircle = isCircle;
            this.IsPolygon = isPolygon;
            this.Size = size;
            this.Index = counter;
        }


        int _Index;
        public int Index
        {
            get
            {
                return _Index;
            }
            set
            {
                if (_Index != value)
                {
                    _Index = value;
                    RaisePropertyChanged("Index");
                }
            }
        }

        bool _IsCircle;
        public bool IsCircle
        {
            get
            {
                return _IsCircle;
            }
            set
            {
                if (_IsCircle != value)
                {
                    _IsCircle = value;
                    RaisePropertyChanged("IsCircle");
                }
            }
        }

        bool _IsPolygon;
        public bool IsPolygon
        {
            get
            {
                return _IsPolygon;
            }
            set
            {
                if (_IsPolygon != value)
                {
                    _IsPolygon = value;
                    RaisePropertyChanged("IsPolygon");
                }
            }
        }

        int _Size;
        public int Size
        {
            get
            {
                return _Size;
            }
            set
            {
                if (_Size != value)
                {
                    _Size = value;
                    RaisePropertyChanged("Size");
                }
            }
        }

        MosaicColors _Color;
        public MosaicColors Color
        {
            get
            {
                return _Color;
            }
            set
            {
                if (_Color != value)
                {
                    _Color = value;
                    RaisePropertyChanged("Color");
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

