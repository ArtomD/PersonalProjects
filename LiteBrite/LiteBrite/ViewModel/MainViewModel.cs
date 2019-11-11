using LiteBrite.Model;
using LiteBrite.Utils;
using LiteBrite.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;


namespace LiteBrite.ViewModel
{
    public class MainViewModel
    {

        public delegate void NewBoardDelegate(NewBoardModel board);
        public Mosaic selectedMosaic { get; set; }
        private DataTemplate mosaicTemplate;
        private List<MosaicWrapper> listMosaic;
        private WrapPanel mosaicGrid { get; set; }
        public GridProperties GridProp { get; set; }
        public List<MosaicColors> Colors { get; set; }
        public static List<MosaicColors> usedColors;
        public List<MosaicColors> backgroundColors { get; set; }

        public bool gridSettingMove { get; set; }
        public bool gridSettingCopy { get; set; }
        public bool gridSettingSwap { get; set; }


        public RelayCommand OpenFileCommand { get; set; }
        public RelayCommand SaveFileCommand { get; set; }
        public RelayCommand CloseAppCommand { get; set; }

        public RelayCommand NewGridCommand { get; set; }
        public RelayCommand NewRandomGridCommand { get; set; }

        public RelayCommand ShowHelpCommand { get; set; }
        public RelayCommand ShowAboutCommand { get; set; }



        public MainViewModel(WrapPanel mosaicGrid, DataTemplate template)
        {
            MainViewModel.usedColors = new List<MosaicColors>();
            MainViewModel.usedColors.Add(new MosaicColors(Brushes.DarkRed, "Dark Red"));
            MainViewModel.usedColors.Add(new MosaicColors(Brushes.Red, "Red"));
            MainViewModel.usedColors.Add(new MosaicColors(Brushes.Pink, "Pink"));
            MainViewModel.usedColors.Add(new MosaicColors(Brushes.OrangeRed, "Dark Orange"));
            MainViewModel.usedColors.Add(new MosaicColors(Brushes.Orange, "Orange"));
            MainViewModel.usedColors.Add(new MosaicColors(Brushes.Yellow, "Yellow"));
            MainViewModel.usedColors.Add(new MosaicColors(Brushes.SeaGreen, "Dark Green"));
            MainViewModel.usedColors.Add(new MosaicColors(Brushes.LimeGreen, "Green"));
            MainViewModel.usedColors.Add(new MosaicColors(Brushes.LawnGreen, "Light Green"));
            MainViewModel.usedColors.Add(new MosaicColors(Brushes.Navy, "Dark Blue"));
            MainViewModel.usedColors.Add(new MosaicColors(Brushes.RoyalBlue, "Blue"));
            MainViewModel.usedColors.Add(new MosaicColors(Brushes.DeepSkyBlue, "Light Blue"));
            MainViewModel.usedColors.Add(new MosaicColors(Brushes.BlueViolet, "Violet"));
            MainViewModel.usedColors.Add(new MosaicColors(Brushes.Purple, "Purple"));
            MainViewModel.usedColors.Add(new MosaicColors(Brushes.Black, "Black"));
            MainViewModel.usedColors.Add(new MosaicColors(Brushes.White, "White"));
            MainViewModel.usedColors.Add(new MosaicColors(Brushes.Transparent, "None"));

            backgroundColors = new List<MosaicColors>();
            backgroundColors.Add(new MosaicColors(Brushes.Black, "Black", Brushes.White));
            backgroundColors.Add(new MosaicColors(Brushes.Maroon, "Dark Red", Brushes.White));
            backgroundColors.Add(new MosaicColors(Brushes.DarkGreen, "Dark Green", Brushes.White));
            backgroundColors.Add(new MosaicColors(Brushes.MidnightBlue, "Dark Blue", Brushes.White));
            backgroundColors.Add(new MosaicColors(Brushes.White, "White", Brushes.Black));
            backgroundColors.Add(new MosaicColors(Brushes.LightSalmon, "Light Red", Brushes.Black));
            backgroundColors.Add(new MosaicColors(Brushes.YellowGreen, "Light Green", Brushes.Black));
            backgroundColors.Add(new MosaicColors(Brushes.PowderBlue, "Light Blue", Brushes.Black));

            gridSettingMove = false;            
            gridSettingCopy = false;
            gridSettingSwap = true;
            GridProp = new GridProperties();
            this.mosaicGrid = mosaicGrid;
            mosaicTemplate = template;
            listMosaic = new List<MosaicWrapper>();
           

            Colors = new List<MosaicColors>();
            foreach(MosaicColors color in usedColors)
            {
                Colors.Add(new MosaicColors(color.Color, color.ColorName));
            }
            selectedMosaic = new Mosaic(int.MaxValue);
            selectedMosaic.Color.Color = Brushes.Red.ToString();
            selectedMosaic.Color.ColorName = "Red";
            selectedMosaic.IsCircle = true;
            selectedMosaic.IsPolygon = false;

            BoardFile newFile = new BoardFile();
            populate(newFile);

            OpenFileCommand = new RelayCommand(OpenFile);
            SaveFileCommand = new RelayCommand(SaveFile);
            CloseAppCommand = new RelayCommand(CloseApp);

            NewGridCommand = new RelayCommand(NewBoard);
            NewRandomGridCommand = new RelayCommand(generateNewRandomBoard);

            ShowHelpCommand = new RelayCommand(ShowHelpWindow);
            ShowAboutCommand = new RelayCommand(ShowAboutWindow);
        }

        void OpenFile(object parameter)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".binary";
            dlg.Filter = "Binary Files|*.binary";
            dlg.InitialDirectory = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory());
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                populate(FileUtils.loadFile(dlg.FileName));
            }
        }

        void SaveFile(object parameter)
        {
            BoardFile newFile = new BoardFile();
            newFile.backgroundColor = GridProp.BackgroundColor;
            newFile.height = GridProp.GridHeightCount;
            newFile.width = GridProp.GridWidthCount;
            newFile.maxMosaicSize = GridProp.GridSize;
            newFile.mosaicGrid = listMosaic;
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".binary";
            dlg.Filter = "Binary Files|*.binary";
            dlg.InitialDirectory = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory());
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                FileUtils.saveFile(newFile, dlg.FileName);
            }
        }

        void CloseApp(object parameter)
        {
            Application.Current.MainWindow.Close();
        }

        public void populate(BoardFile file)
        {

            Mosaic.maxMosaicSize = file.maxMosaicSize;
            Mosaic.minMosaicSize = Mosaic.maxMosaicSize - 6;
            GridProp.GridSize = Mosaic.maxMosaicSize;
            GridProp.MinMosaicSize = Mosaic.minMosaicSize;
            GridProp.GridWidthCount = file.width;
            GridProp.GridHeightCount = file.height;
            GridProp.GridWidth = GridProp.GridWidthCount * GridProp.GridSize;
            GridProp.GridHeight = GridProp.GridHeightCount * GridProp.GridSize;
            GridProp.BackgroundColor = backgroundColors.Find(x => x.ColorName == file.backgroundColor.ColorName);
            GridProp.Light = file.light;

            selectedMosaic.Size = Mosaic.maxMosaicSize;

            
            mosaicGrid.Children.Clear();
            listMosaic.Clear();
            for (int i = 0; i < file.mosaicGrid.Count; i++)
            { 
                Grid grid = new Grid();
                grid.Width = GridProp.GridSize;
                grid.Height = GridProp.GridSize;
                ContentControl contentControl = new ContentControl();
                contentControl.ContentTemplate = mosaicTemplate;
                Binding myBinding = new Binding("Mosaic");
                listMosaic.Add(file.mosaicGrid[i]);                
                myBinding.Source = listMosaic.Last();
                myBinding.Path = new PropertyPath("Mosaic");
                contentControl.SetBinding(ContentControl.ContentProperty, myBinding);
                grid.Children.Add(contentControl);
                mosaicGrid.Children.Add(grid);
            }
            
        }

        private void generateNewBoard(NewBoardModel board)
        {
            populate(new BoardFile(board.mosaicSize, board.width, board.height, board.generate, board.randomColor, board.selectedColor, board.randomShape, board.square, board.randomSize, board.big, board.medium, board.small, board.tiny));
        }

        private void generateNewRandomBoard(object parameter)
        {
            populate(new BoardFile(GridProp.GridSize, GridProp.GridWidthCount, GridProp.GridHeightCount, true, true, null, true, false, true, false, false, false,false));
        }

        void NewBoard(object parameter)
        {
            NewBoardDelegate func = new NewBoardDelegate(generateNewBoard);
            NewBoardWindow aboutWindow = new NewBoardWindow(func);
            aboutWindow.ShowDialog();
        }

        void ShowHelpWindow(object parameter)
        {
            HelpWindow helpWindow = new HelpWindow();
            helpWindow.Show();
        }

        void ShowAboutWindow(object parameter)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }

    }
}
