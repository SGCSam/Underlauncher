using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Underlauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : SharpWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            IDs.GameTracks.populateDictionary();
        }
        
        //populatePresetsCombo populates the preset combobox with a list of music presets
        public void populatePresetsCombo()
        {
            presetsCombo.Items.Clear();
            presetsCombo.Items.Add("Default Music");
            foreach (string presetsFolderName in Directory.GetDirectories(Constants.PresetsPath))
            {
                ComboBoxItem item = new ComboBoxItem();
                if (presetsFolderName.Remove(0, Constants.PresetsPath.Length).Length > 32)
                {
                    item.Content = presetsFolderName.Remove(0, Constants.PresetsPath.Length).Substring(0, 32) + "...";
                }

                else
                {
                    item.Content = presetsFolderName.Remove(0, Constants.PresetsPath.Length);
                }

                item.ToolTip = presetsFolderName.Remove(0, Constants.PresetsPath.Length);
                presetsCombo.Items.Add(item.Content);
            }

            presetsCombo.SelectedIndex = 0;
        }

        //populateSavesCombo populates the saves combobox with a list of save files
        public void populateSavesCombo()
        {
            savesCombo.Items.Clear();
            savesCombo.Items.Add("Local Appdata Save");

            foreach (string saveFolderName in Directory.GetDirectories(Constants.SavesPath))
            {
                ComboBoxItem item = new ComboBoxItem();
                if (saveFolderName.Remove(0, Constants.SavesPath.Length).Length > 32)
                {
                    item.Content = saveFolderName.Remove(0, Constants.SavesPath.Length).Substring(0, 32) + "...";
                }

                else
                {
                    item.Content = saveFolderName.Remove(0, Constants.SavesPath.Length);
                }

                item.ToolTip = saveFolderName.Remove(0, Constants.SavesPath.Length);
                savesCombo.Items.Add(item.Content);
            }

            savesCombo.SelectedIndex = 0;
        }

        //The Window_Loaded event is resposible for checking all working folder directories and contents to ensure they are valid
        //This includes checking the install path in path.xml as well as ensuring we a have good working folder structure and populating the UI Comboboxes
        private async void SharpWindow_Loaded(object sender, RoutedEventArgs e)
        {
            XML.Check();
            XML.ReadGamePath();
            XML.ReadSettingsXML();

            if (await SwiftUpdate.CheckForUpdates("Underlauncher", "http://apps.sgcsam.com/Underlauncher/"))
            {
                Dictionary<Characters, string> messageDict = new Dictionary<Characters, string>()
                {
                    {Characters.Alphys, "H-h-hey, so an update is r-r-ready for Underlauncher. Want to download it?" },
                    {Characters.Asgore, "Greetings human, it appears that an update is available for Underlauncher. Shall we download it?" },
                    {Characters.Asriel, "Howdy! Looks like there's an update available for Underlauncher. Want to download it?" },
                    {Characters.Flowey, "HEY. IDIOT. There's an update available to Underlauncher. Can we just download it already?" },
                    {Characters.Papyrus, "NYEH, HUMAN! I BRING NEWS! AN UPDATE IS AVAILABLE FOR UNDERLAUNCHER. WANT TO DOWNLOAD IT?" },
                    {Characters.Sans, "heya buddy. it looks like there's an update for underlauncher. want to download it?" },
                    {Characters.Toriel, "Excuse me child, but an update is available for Underlauncher. Shall we download it?" },
                    {Characters.Undyne, "HEY PUNK! Looks like there's an update available for Underlauncher. LET'S DOWNLOAD IT!" },
                    {Characters.None, "An update is available for Underlauncher. Do you want to download it?" }
                };
                
                if (UTMessageBox.Show(messageDict, Constants.CharacterReactions.Normal, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Hide();
                    UpdateWindow updateWindow = new UpdateWindow();
                    updateWindow.ShowDialog();
                    Show();
                }
            }

            if (FileOperations.goodFolderIntegrity())
            {
                directoryBlock.Text = "Path: " + XML.GetGamePath();
                directoryBlock.ToolTip = XML.GetGamePath();
                versionBlock.Text = "Version: " + Constants.GameVersion + ", Dog Check DISABLED.";

                browseButton.IsEnabled = false;
                saveEditorButton.IsEnabled = true;
                musicEditorButton.IsEnabled = true;
                optionsButton.IsEnabled = true;
                savesCombo.IsEnabled = true;
                presetsCombo.IsEnabled = true;
                debugCheck.IsEnabled = true;
                launchButton.IsEnabled = true;

                populateSavesCombo();
                populatePresetsCombo();
            }
        }

        //browseButton_Click event allows the user to browse for an designate a new Undertale install directory
        private async void browseButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowser2 directoryFolderBrowser = new FolderBrowser2();
            directoryFolderBrowser.ShowDialog((System.Windows.Forms.IWin32Window)this.Owner);

            if (File.Exists(directoryFolderBrowser.DirectoryPath + "//UNDERTALE.exe"))
            {
                WaitingWindow waitWindow = new WaitingWindow("Backing Up", "Backing up game files, please wait. This may take some time.");
                waitWindow.Owner = this;
                Hide();
                waitWindow.Show();
                await Task.Run(() => FileOperations.backupFilesFromGameDirectory(directoryFolderBrowser.DirectoryPath));
                waitWindow.Close();
                Show();

                browseButton.IsEnabled = false;
                saveEditorButton.IsEnabled = true;
                musicEditorButton.IsEnabled = true;
                optionsButton.IsEnabled = true;
                savesCombo.IsEnabled = true;
                presetsCombo.IsEnabled = true;
                debugCheck.IsEnabled = true;
                launchButton.IsEnabled = true;

                populateSavesCombo();
                populatePresetsCombo();

                try
                {
                    string gameVersion = FileVersionInfo.GetVersionInfo(directoryFolderBrowser.DirectoryPath + "//UNDERTALE.exe").ProductVersion;
                    gameVersion = gameVersion.Replace(" ", String.Empty);   //Remove any spaces at the end of the version

                    XML.WriteGamePath(directoryFolderBrowser.DirectoryPath);

                    directoryBlock.Text = "Path: " + XML.GetGamePath();
                    directoryBlock.ToolTip = XML.GetGamePath();

                    if (FileOperations.disableDogcheck(gameVersion, directoryFolderBrowser.DirectoryPath + "//data.win"))   //We disable dogcheck at the game directory
                    {
                        versionBlock.Text = "Game Version: " + gameVersion + ", Dog Check DISABLED.";
                    }

                    else
                    {
                        this.Close();
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Unable to verify game version. This could indicate the Undertale.exe file is invalid or corrupt. Please try again and ensure the selected directory is correct. If the issue persists, please redownload the game. The exception is: " + ex, "Invalid Game Version!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            else if (directoryFolderBrowser.DirectoryPath != null)
            {
                Dictionary<Characters, string> messageDict = new Dictionary<Characters, string>()
                {
                    {Characters.Alphys, "I'm real s-s-sorry, but I c-c-couldn't find the Undertale.exe in that folder." },
                    {Characters.Asgore, "I'm greatly sorry human, but I was unable to find the Undertale.exe in that folder." },
                    {Characters.Asriel, "I'm so sorry, but I can't find the Undertale.exe in that folder." },
                    {Characters.Flowey, "YOU. IDIOT. I can't find the Undertale.exe in that folder!" },
                    {Characters.Papyrus, "NYOO HOO HOO. I AM SORRY, BUT I'VE FAILED TO FIND THE UNDERTALE.EXE IN THAT FOLDER." },
                    {Characters.Sans, "c'mon pal, help me out here. i can't find the undertale.exe in that folder." },
                    {Characters.Toriel, "I'm afraid that I was unable to locate the Undertale.exe in that folder, my child." },
                    {Characters.Undyne, "HEY, COME ON PUNK! I can't find the Undertale.exe in that folder." },
                    {Characters.None, "Cannot locate Undertale.exe in the folder specified. Please navigate to a valid Undertale installation folder." }
                };

                UTMessageBox.Show(messageDict, Constants.CharacterReactions.Negative, MessageBoxButton.OK);
            }
        }
        private void saveEditorButton_Click(object sender, RoutedEventArgs e)
        {
            SaveEditorWindow saveEditorWindow = new SaveEditorWindow();
            saveEditorWindow.ShowDialog();
        }

        private void musicEditorButton_Click(object sender, RoutedEventArgs e)
        {
            MusicEditorWindow musicEditorWindow = new MusicEditorWindow();
            musicEditorWindow.Owner = this;
            musicEditorWindow.ShowDialog();
        }

        private void optionsButton_Click(object sender, RoutedEventArgs e)
        {
            GameOptionsWindow gameOptionsWindow = new GameOptionsWindow();
            gameOptionsWindow.ShowDialog();
        }

        private void launchButton_Click(object sender, RoutedEventArgs e)
        {
            FileOperations.copyMusicPresetToGame(presetsCombo.SelectedItem.ToString());
            FileOperations.copySAVEToGame(savesCombo.SelectedItem.ToString());
            FileOperations.setDebugMode((bool)debugCheck.IsChecked);
            Process.Start(XML.GetGamePath() + "//UNDERTALE.exe");
        }
    }
}
