using LiteBrite.Model;
using LiteBrite.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LiteBrite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Point startPoint;
        public MainViewModel model { get; set; }
        private bool dragSelected;

        public MainWindow()
        {
            InitializeComponent();
            dragSelected = false;
            DataTemplate mosaicTemplate = this.FindResource("polygonTemplate") as DataTemplate;
            model = new MainViewModel(wrapPanel, mosaicTemplate);
            this.DataContext = model;

        }


        private void PolygonSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get the starting point of initiating the drag
            startPoint = e.GetPosition(null);  // absolute position

        }
        private void PolygonSource_MouseMove(object sender, MouseEventArgs e)
        {

            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;
            if (e.LeftButton == MouseButtonState.Pressed && !dragSelected)
            {
                dragSelected = true;
                Grid source = sender as Grid;
                Mosaic mosaic = (Mosaic)source.DataContext;
                DragDrop.DoDragDrop(source, new DataObject(typeof(Mosaic), mosaic), DragDropEffects.Move);

            }
            

        }

        
            private static T findAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetChild(current,0);
            }
            while (current != null);
            return null;
        }       

        private void PolygonDestination_DragEnter(object sender, DragEventArgs e)
        {
            // Don't allow if the Data is not a string or the 
            // source is the same as the sender
            // (Not allowed to drag and drop on same item
            if (!e.Data.GetDataPresent(typeof(Mosaic)) || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
            //dragSelected = false;
        }

        private void PolygonDestination_Drop(object sender, DragEventArgs e)
        {
           
            if (e.Data.GetDataPresent(typeof(Mosaic)))
            {
               
                Mosaic theItem = (Mosaic)e.Data.GetData(typeof(Mosaic));
                Grid grid = sender as Grid;

                MosaicColors tmpColor = new MosaicColors( ((Mosaic)grid.DataContext).Color );
                bool tmpIsCircle = ((Mosaic)grid.DataContext).IsCircle;
                bool tmpIsPolygon = ((Mosaic)grid.DataContext).IsPolygon;
                int tmpSize = ((Mosaic)grid.DataContext).Size;
                if (grid.Name == "PolygonSource")
                {
                    model.selectedMosaic.Color = model.Colors.Find(x => x.Color == theItem.Color.Color);
                    //model.selectedMosaic.Color.ColorName = theItem.Color.ColorName;
                    model.selectedMosaic.IsCircle = theItem.IsCircle;
                    model.selectedMosaic.IsPolygon = theItem.IsPolygon;
                    model.selectedMosaic.Size = theItem.Size;                  

                }
                else
                {
                    
                    ((Mosaic)grid.DataContext).Color.Color = theItem.Color.Color;
                    ((Mosaic)grid.DataContext).Color.ColorName = theItem.Color.ColorName;
                    ((Mosaic)grid.DataContext).IsCircle = theItem.IsCircle;
                    ((Mosaic)grid.DataContext).IsPolygon = theItem.IsPolygon;
                    ((Mosaic)grid.DataContext).Size = theItem.Size;
                    if (theItem.Index != int.MaxValue)
                    {
                        if (model.gridSettingSwap)
                        {
                            theItem.Color.Color = tmpColor.Color;
                            theItem.Color.ColorName = tmpColor.ColorName;
                            theItem.IsCircle = tmpIsCircle;
                            theItem.IsPolygon = tmpIsPolygon;
                            theItem.Size = tmpSize;
                        }
                        else if (model.gridSettingMove)
                        {
                            theItem.Color.Color = Brushes.Transparent.ToString();
                            theItem.Color.ColorName = "None";
                        }
                    }
                }
                               
                dragSelected = false;
            }
        }

        private void PolygonClear_Drop(object sender, DragEventArgs e)
        {
            

            if (e.Data.GetDataPresent(typeof(Mosaic)))
            {
               
                Mosaic theItem = (Mosaic)e.Data.GetData(typeof(Mosaic));
                if (theItem.Index != int.MaxValue)
                {
                    theItem.Color.Color = Brushes.Transparent.ToString();
                    theItem.Color.ColorName = "None";
                }
                dragSelected = false;
            }
        }
        

        private void MouseIsUp(object sender, MouseButtonEventArgs e)
        {

            dragSelected = false;
        }

       
        

    }
}
