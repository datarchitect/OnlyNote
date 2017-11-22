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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class wndNotes : Window
    {
        string oldValue = string.Empty;
        public wndNotes()
        {
            InitializeComponent();
        }

        private void wndAddNew_Loaded(object sender, RoutedEventArgs e)
        {
            txtNotes.Focus();
            oldValue = txtNotes.Text;
        }

        private void wndAddNew_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                txtNotes.Text = oldValue;
                this.Close();
            }
            if(((Keyboard.IsKeyDown(Key.LeftCtrl)) || (Keyboard.IsKeyDown(Key.RightCtrl))) && (e.Key == Key.Enter))
            {
                this.Close();
            }
        }
    }
}
