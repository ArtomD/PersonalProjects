using LiteBrite.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LiteBrite.Model
{
    [Serializable]
    public class BoardFile
    {
        public int maxMosaicSize { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public MosaicColors backgroundColor { get; set; }
        public List<MosaicWrapper> mosaicGrid { get; set; }
        public bool light { get; set; }

        public BoardFile()
        {
            maxMosaicSize = 10;
            width = 50;
            height = 50;
            backgroundColor = new MosaicColors(Brushes.Black,"Black",Brushes.White);
            mosaicGrid = new List<MosaicWrapper>();
            light = true;
            int counter = 0;
            for(int i = 0; i< width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    mosaicGrid.Add(new MosaicWrapper(new Mosaic(counter)));
                    counter++;
                }
            }
        }

        public BoardFile(int maxMosaicSize, int width, int height, bool generateTiles, bool randColor, MosaicColors color, bool randShape, bool squareShape, bool randSize, bool big, bool medium, bool small, bool tiny)
        {
            this.maxMosaicSize = maxMosaicSize;
            this.width = width;
            this.height = height;
            backgroundColor = new MosaicColors(Brushes.Black, "Black", Brushes.White);
            mosaicGrid = new List<MosaicWrapper>();
            light = true;
            int counter = 0;
            Random random = new Random();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (generateTiles){
                        
                        if (randColor)
                        {                                                       
                            color = new MosaicColors(MainViewModel.usedColors[random.Next(MainViewModel.usedColors.Count)]);
                        }
                        
                        if (randShape)
                        {
                            squareShape = random.Next(2) > 0;
                        }
                        int size = maxMosaicSize;
                        if (randSize) {
                            size = maxMosaicSize - ((random.Next(1, 4) * 2));
                        }
                        else
                        {
                           
                            if (big)
                            {
                                size = maxMosaicSize;
                            }
                            else if (medium)
                            {
                                size = maxMosaicSize - 2;
                            }
                            else if (small)
                            {
                                size = maxMosaicSize - 4;
                            }
                            else if (tiny)
                            {
                                size = maxMosaicSize - 6;
                            }
                        }
                        mosaicGrid.Add(new MosaicWrapper(new Mosaic(counter, color, !squareShape, squareShape, size)));
                    }
                    else{
                        mosaicGrid.Add(new MosaicWrapper(new Mosaic(counter)));
                    }
                    counter++;
                }
            }
        }
    }
   
    
}
