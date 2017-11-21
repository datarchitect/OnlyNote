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

namespace OnlyNote
{
    /// <summary>
    /// Interaction logic for AddNew.xaml
    /// </summary>
    public partial class AddNew : Window
    {
        public AddNew()
        {
            InitializeComponent();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtNewValue.Focus();
        }
    }
}
