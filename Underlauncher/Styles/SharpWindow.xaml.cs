using System.Windows;
using ManagedXAudio2SoundEngine;

namespace Underlauncher.Styles
{
    public partial class SharpWindow : ResourceDictionary
    {
        int soundEngine;

        public SharpWindow()
        {
            bool isOk = MXA2SE.startup();
            if (isOk)
            {
                soundEngine = MXA2SE.create_sound_engine();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.Close();
        }

        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            if (window.WindowState == WindowState.Normal)
            {
                window.WindowState = WindowState.Maximized;
            }
            else
            {
                window.WindowState = WindowState.Normal;
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.WindowState = WindowState.Minimized;
        }

        public void About_Click(object sender, RoutedEventArgs e)
        {
            MXA2SE.play_sound(soundEngine, "Assets//Sounds//bark.wav");
            AboutWindow window = new AboutWindow();
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }
    }
}
