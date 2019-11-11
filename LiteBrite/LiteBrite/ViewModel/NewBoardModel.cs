using LiteBrite.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LiteBrite.ViewModel
{
    public class NewBoardModel
    {
        public int mosaicSize { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public bool generate { get; set; }
        public bool randomColor { get; set; }
        public bool notRandomColor { get; set; }
        public List<MosaicColors> Colors {get;set;}
        public MosaicColors selectedColor { get; set; }
        public bool randomShape { get; set; }
        public bool circle { get; set; }
        public bool square { get; set; }
        public bool randomSize { get; set; }
        public bool big { get; set; }
        public bool medium { get; set; }
        public bool small { get; set; }
        public bool tiny { get; set; }
        public NewBoardModel()
        {
            Colors = new List<MosaicColors>();
            foreach(MosaicColors color in MainViewModel.usedColors)
            {
                Colors.Add(new MosaicColors(color));
            }
            

            mosaicSize = 20;
            width = 25;
            height = 25;
            generate = false;
            randomColor = true;
            randomShape = true;
            randomSize = true;
            selectedColor = new MosaicColors(MainViewModel.usedColors[0]);
        }
    }
}
