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
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : SharpWindow
    {
        public UpdateWindow()
        {
            InitializeComponent();
        }

        private async void SharpWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IsExitEnabled = false;

            ProgressManager.downloadProg.ProgressChanged += (send, progressPercentage) =>
            {
                downloadBar.Value = progressPercentage;
                percBlock.Text = progressPercentage.ToString() + "%";
                descBlock.Text = ProgressManager.progressDescription;
            };

            await SwiftUpdate.DownloadUpdate(downloadBar, percBlock, descBlock);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            SwiftUpdate.AbortDownload();
            Close();
        }
    }
}
