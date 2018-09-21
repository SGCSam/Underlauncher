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

namespace Underlauncher
{
    /// <summary>
    /// Interaction logic for ConvertingWindow.xaml
    /// </summary>
    public partial class WaitingWindow : SharpWindow
    {
        public WaitingWindow(string titleText, string waitText)
        {
            InitializeComponent();
            waitBlock.Text = waitText;
            Title = titleText;
        }

        private void SharpWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IsExitEnabled = false;

            if (Owner != null)
            {
                Owner.IsEnabled = false;
            }
        }

        private void SharpWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Owner != null)
            {
                Owner.IsEnabled = true;
            }
        }
    }
}
