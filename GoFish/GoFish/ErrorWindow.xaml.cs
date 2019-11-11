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
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        HostLeftErrorDelegate func;
        public ErrorWindow(string messageStr)
        {
            InitializeComponent();
            message.Text = messageStr;
        }

        public ErrorWindow(string messageStr, HostLeftErrorDelegate func)
        {
            InitializeComponent();
            message.Text = messageStr;
            this.func = func;
        }

        private void quit(object sender, RoutedEventArgs e)
        {
            if (func != null)
            {
                func();
            }
            Close();
        }
    }
}
