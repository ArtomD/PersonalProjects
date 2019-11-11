using LiteBrite.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static LiteBrite.ViewModel.MainViewModel;

namespace LiteBrite.View
{
    /// <summary>
    /// Interaction logic for NewBoardWindow.xaml
    /// </summary>
    public partial class NewBoardWindow : Window
    {
        NewBoardDelegate func;
        NewBoardModel model { get; set; }
        public NewBoardWindow(NewBoardDelegate func)
        {
            InitializeComponent();
            this.func = func;
            model = new NewBoardModel();
            this.DataContext = model;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void newBoard(object sender, RoutedEventArgs e)
        {
            func(model);
            this.Close();
        }

        private void closeWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
