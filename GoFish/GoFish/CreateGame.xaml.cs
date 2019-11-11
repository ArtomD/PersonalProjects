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
using System.Windows.Shapes;
using static GoFish.MainWindow;

namespace GoFish
{
    /// <summary>
    /// Interaction logic for CreateGame.xaml
    /// </summary>
    public partial class CreateGame : Window
    {
        WindowDelegate func;
        public CreateGame(WindowDelegate func)
        {
            InitializeComponent();
            this.func = func;            
        }


        private void quit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void createGame(object sender, RoutedEventArgs e)
        {
            if (func(game_name.Text) == 0)
            {
                Close();
            }
        }
    }
}
