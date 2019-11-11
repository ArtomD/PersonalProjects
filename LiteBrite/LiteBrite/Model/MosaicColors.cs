using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LiteBrite.Model
{
    [Serializable]
    public class MosaicColors : INotifyPropertyChanged
    {

        
        private string _Color { get; set; }
        private string _ColorName { get; set; }
        private string _FontColor { get; set; }

        public MosaicColors(Brush brush, string name)
        {
            _Color = brush.ToString();
            _ColorName = name;
        }

        public MosaicColors(string color, string name)
        {
            _Color = color;
            _ColorName = name;
        }

        public MosaicColors(MosaicColors color)
        {

            _Color = color.Color;
            _ColorName = color.ColorName;
        }

        public MosaicColors(Brush brush, string name, Brush fontColor)
        {

            _Color = brush.ToString();
            _ColorName = name;
            _FontColor = fontColor.ToString();
        }



        public string Color
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

        public string ColorName
        {
            get
            {
                return _ColorName;
            }
            set
            {
                if (_ColorName != value)
                {
                    _ColorName = value;
                    RaisePropertyChanged("ColorName");
                }
            }
        }

        public string FontColor
        {
            get
            {
                return _FontColor;
            }
            set
            {
                if (_FontColor != value)
                {
                    _FontColor = value;
                    RaisePropertyChanged("FontColor");
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
