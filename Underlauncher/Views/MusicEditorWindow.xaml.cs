using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Underlauncher
{
    public partial class MusicEditorWindow : SharpWindow
    {
        MusicPreset musicPreset;

        public MusicEditorWindow()
        {
            InitializeComponent();
        }

        //PopulateGameTracks adds all OGG vorbis game tracks from the backup directory to the list
        private void PopulateGameTracks()
        {
            string[] gameTracks = MiscFunctions.GetFilesWithExtensions(Constants.BackupPath, Constants.SupportedFileTypes);
            for (int i = 0; i < gameTracks.Length; i++)
            {
                gameTracks[i] = IDs.GameTracks.GetTrackVisibleName(gameTracks[i]);
            }

            Array.Sort(gameTracks);
            AddListViewItems(gameTracksList, gameTracks);
        }

        //PopulateGameTracks adds all audio files from the specified dir to the list
        private void PopulateCustomTracks(string dir)
        {
            AddListViewItems(customTracksList, MiscFunctions.GetFilesWithExtensions(dir, Constants.SupportedFileTypes));
        }

        //AddListViewItems adds the source items to the specified list view, while setting tags and ellipsis
        private void AddListViewItems(ListView listView, string[] source)
        {
            foreach (var sourceFile in source)
            {
                ListViewItem thisItem = new ListViewItem();
                thisItem.Content = System.IO.Path.GetFileName(sourceFile);
                thisItem.ToolTip = thisItem.Content;
                thisItem.PreviewMouseDown += PreviewMouseDown;
                thisItem.Tag = "-1";
                listView.Items.Add(thisItem);
            }

            ElipticateItems(listView);
        }

        //ElipticateItems loops through every item in the listView and substrings it to add ellipsis if its length is too long
        private void ElipticateItems(ListView listView)
        {
            foreach (ListViewItem item in listView.Items)
            {
                string contentString = item.Content.ToString();

                if (contentString.Length > 27)
                {
                    item.Content = contentString.Substring(0, 24) + "...";
                }

            }
        }

        //ResetUI clears and resets the list views
        private void ResetUI()
        {
            gameTracksList.Items.Clear();
            customTracksList.Items.Clear();
            PopulateGameTracks();
        }

        //TagFromLoad tags all the game track items in the list with their corresponding index value of their selected custom track
        private void TagFromLoad()
        {
            for (int i = 0; i < musicPreset.presetData.Count(); i++)
            {
                if (musicPreset.presetData[i].CustomTrack != "Default")
                {
                    foreach (ListViewItem customTrackItem in customTracksList.Items)
                    {
                        if (customTrackItem.ToolTip.ToString() == musicPreset.presetData[i].CustomTrack)
                        {
                            foreach (ListViewItem gameTrackItem in gameTracksList.Items)
                            {
                                string gameTrackName = IDs.GameTracks.GetTrackVisibleName(musicPreset.presetData[i].GameTrack);
                                if (gameTrackItem.ToolTip.ToString() == gameTrackName)
                                {
                                    gameTrackItem.Tag = customTracksList.Items.IndexOf(customTrackItem);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowser2 directoryFolderBrowser = new FolderBrowser2();
            directoryFolderBrowser.ShowDialog(null);

            if (directoryFolderBrowser.DirectoryPath != null)
            {
                ResetUI();
                presetNameBox.Text = "New Preset";
                musicPreset = new MusicPreset(directoryFolderBrowser.DirectoryPath);
                musicPreset.presetName = presetNameBox.Text;
                PopulateCustomTracks(directoryFolderBrowser.DirectoryPath);
                saveButton.IsEnabled = true;
            }
        }

        private async void saveButton_Click(object sender, RoutedEventArgs e)
        {
            bool shouldWrite = false;

            musicPreset.presetName = presetNameBox.Text;

            if (!Directory.Exists(Constants.PresetsPath + musicPreset.presetName))
            {
                Directory.CreateDirectory(Constants.PresetsPath + musicPreset.presetName + "\\Original");
                Directory.CreateDirectory(Constants.PresetsPath + musicPreset.presetName + "\\Named");
                shouldWrite = true;
            }

            else
            {
                Dictionary<Characters, string> messageDict = new Dictionary<Characters, string>()
                {
                    {Characters.Alphys, "Hey! So, preset \"" + musicPreset.presetName + "\" already exists. Would you like me to overwrite it?" },
                    {Characters.Asgore, "Well human, the preset \"" + musicPreset.presetName + "\" already exists. Shall I overwrite it?" },
                    {Characters.Asriel, "Sorry, but the preset \"" + musicPreset.presetName + "\" already exists. Do you want me to overwrite it? It's no trouble, really!" },
                    {Characters.Flowey, "YOU. IDIOT! The preset \"" + musicPreset.presetName + "\" already exists. Of course, I've got to do all the work - do you want me to overwrite it?" },
                    {Characters.Papyrus, "NYEH, SORRY HUMAN. THE PRESET \"" + musicPreset.presetName + "\" ALREADY EXISTS. WOULD YOU LIKE ME TO ASSIST YOU AND OVERWRITE IT?" },
                    {Characters.Sans, "yo, buddy. the preset \"" + musicPreset.presetName + "\" already exists. i'm normally quite lazy, but i'll make an exception this time. want me to overwrite it?" },
                    {Characters.Toriel, "My child, I apologize, but the preset \"" + musicPreset.presetName + "\" already exists. Would you like me to overwrite it for you?" },
                    {Characters.Undyne, "OH COME ON.  The preset \"" + musicPreset.presetName + "\" already exists. Want me to overwrite it?" },
                    {Characters.None, "Preset \"" + musicPreset.presetName + "\" already exists! Do you want to overwrite?" }
                };

                MessageBoxResult res = UTMessageBox.Show(messageDict, Constants.CharacterReactions.Negative, MessageBoxButton.YesNo);

                if (res == MessageBoxResult.Yes)
                {
                    shouldWrite = true;
                }
            }

            if (shouldWrite)
            {
                string[] gameTracks = new string[Constants.GameTracksCount];
                string[] customTracks = new string[Constants.GameTracksCount];

                for (int i = 0; i < Constants.GameTracksCount; i++)
                {
                    ListViewItem thisGameItem = (ListViewItem)gameTracksList.Items[i];
                    gameTracks[i] = IDs.GameTracks.GetTrackFilename(thisGameItem.ToolTip.ToString());
                    if (thisGameItem.Tag.ToString() == "-1")
                    {
                        customTracks[i] = "Default";
                    }

                    else
                    {
                        ListViewItem thisCustomItem = (ListViewItem)customTracksList.Items[Convert.ToInt16(thisGameItem.Tag)];  //We set the custom game track to the content of the item in customTracksList at the index present in the game track's tag
                        customTracks[i] = thisCustomItem.ToolTip.ToString();
                    }
                }

                WaitingWindow waitWindow = new WaitingWindow("Converting Audio", "Converting audio for preset, please wait.This may take some time.");
                waitWindow.Owner = this;
                waitWindow.Show();
                await Task.Run(() => musicPreset.Write(gameTracks, customTracks));
                waitWindow.Close();
                ((MainWindow)this.Owner).populatePresetsCombo();

                Dictionary<Characters, string> messageDict = new Dictionary<Characters, string>()
                {
                    {Characters.Alphys, "Alright, sure! The \"" + musicPreset.presetName + "\" preset has been saved successfully!" },
                    {Characters.Asgore, "Of course human! I've made the \"" + musicPreset.presetName + "\" preset successfully!" },
                    {Characters.Asriel, "No problem, glad to help! I've made the \"" + musicPreset.presetName + "\" preset successfully" },
                    {Characters.Flowey, "Fine. I've made the stupid \"" + musicPreset.presetName + "\" preset successfully. Now go find something better to do." },
                    {Characters.Papyrus, "OF COURSE FRIEND, I'VE CREATED THE \"" + musicPreset.presetName + "\" PRESET SUCCESSFULLY. NYEH HEH HEH!" },
                    {Characters.Sans, "sure thing kiddo. the \"" + musicPreset.presetName + "\" preset has been made successfully." },
                    {Characters.Toriel, "No problem dear. I have created the \"" + musicPreset.presetName + "\" preset for you successfully." },
                    {Characters.Undyne, "Yeah, sure thing punk! The \"" + musicPreset.presetName + "\" preset has been created successfully! FUHUHUH!" },
                    {Characters.None, "Preset \"" + musicPreset.presetName + "\" saved successsfully!" }
                };

                UTMessageBox.Show(messageDict, Constants.CharacterReactions.Positive, MessageBoxButton.OK);
                Close();
            }

        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog presetDialog = new OpenFileDialog();
            presetDialog.Title = "Open preset.xml file";
            presetDialog.Filter = "XML files|*.xml";
            presetDialog.InitialDirectory = Environment.CurrentDirectory;
            presetDialog.ShowDialog();

            if (System.IO.Path.GetFileName(presetDialog.FileName) == "preset.xml")
            {
                ResetUI();
                saveButton.IsEnabled = true;
                musicPreset = new MusicPreset(Constants.PresetsPath + new DirectoryInfo(System.IO.Path.GetDirectoryName(presetDialog.FileName)).Name);

                if (!musicPreset.Load())
                {
                    Application.Current.Shutdown();
                    return;
                }

                PopulateCustomTracks(musicPreset.customTrackDirectory);
                TagFromLoad();
                presetNameBox.Text = musicPreset.presetName;
            }

            else
            {
                Dictionary<Characters, string> messageDict = new Dictionary<Characters, string>()
                {
                    {Characters.Alphys, "S-s-sorry. but you're g-g-going to have to navigate to a valid preset folder." },
                    {Characters.Asgore, "I'm afraid you shall have to navigate to a valid preset folder human." },
                    {Characters.Asriel, "I'm real sorry, but please navigate to a valid preset folder." },
                    {Characters.Flowey, "Oh come on you idiot! Can't you navigate to a valid preset folder?" },
                    {Characters.Papyrus, "I AM SORRY MY FRIEND, BUT I NEED YOU TO NAVIGATE TO A VALID PRESET FOLDER." },
                    {Characters.Sans, "heyo, come on pal. navigate to a valid preset folder would ya?" },
                    {Characters.Toriel, "I'm sorry child, but please navigate to a valid preset folder." },
                    {Characters.Undyne, "HEY! Can you navigate to a valid preset folder already?" },
                    {Characters.None, "Invalid preset folder selected, please navigate to a valid preset folder!" }
                };

                UTMessageBox.Show(messageDict, Constants.CharacterReactions.Negative, MessageBoxButton.OK);
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == gameTracksList)
            {
                if (gameTracksList.SelectedIndex > -1)
                {
                    if (((ListViewItem)gameTracksList.SelectedItem).Tag.ToString() != "-1")    //Select the custom track as per the tag
                    {
                        customTracksList.SelectedIndex = Convert.ToInt16(((ListViewItem)gameTracksList.SelectedItem).Tag);
                    }

                    else    //No tag for this one, unselect the previous item
                    {
                        customTracksList.UnselectAll();
                    }
                }
            }

            else if (sender == customTracksList)
            {
                if (gameTracksList.SelectedIndex > -1)
                {
                    ((ListViewItem)gameTracksList.SelectedItem).Tag = customTracksList.SelectedIndex;   //Set the tag of the game track to the index of the selected custom track
                }
            }
        }

        private void PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = (ListViewItem)sender;
            
            if (item != null && item.IsSelected)
            {
                item.IsSelected = false;
                e.Handled = true;
            }
        }
    }
}
