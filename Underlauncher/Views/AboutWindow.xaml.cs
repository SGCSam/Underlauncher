using System.Reflection;
using System.Windows;

namespace Underlauncher
{
    public partial class AboutWindow : SharpWindow
    {
        public AboutWindow()
        {
            InitializeComponent();

            characterMessageCheck.IsChecked = XML.characterMessagesSetting;
        }

        private void SharpWindow_Loaded(object sender, RoutedEventArgs e)
        {
            versionTextBlock.Text = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            XML.WriteXMLSetting("CharacterMessages", characterMessageCheck.IsChecked.ToString());
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
