using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ManagedXAudio2SoundEngine;

using MessageBox = System.Windows.MessageBox;
using ListViewItem = System.Windows.Controls.ListViewItem;
using System.Diagnostics;
using System.ComponentModel;

namespace Underlauncher
{
    /// <summary>
    /// Interaction logic for PresetsWindow.xaml
    /// </summary>
    /// 

    //We create a struct PresetData to store all information about each file for this preset
    public struct PresetData
    {
        public string musicFile;
        public string customFile;
    }

    public partial class MusicEditorWindow : SharpWindow
    {
        public MusicEditorWindow()
        {
            InitializeComponent();
            Window wnd = System.Windows.Application.Current.MainWindow;
            this.Owner = wnd;
            MXA2SE.startup();
            soundEngine = MXA2SE.create_sound_engine();
        }

        private int soundEngine;

        string backupPath = Environment.CurrentDirectory + "\\Backup Game Directory\\";
        string presetsPath = Environment.CurrentDirectory + "\\Presets\\";

        string currentDirectory = "";
        string presetName = "";

        public void orderFilenamesForListView(FileInfo[] files, System.Windows.Controls.ListView list, bool visibleName = false)
        {
            string[] orderedNamedFiles = new string[files.Length];

            if (visibleName)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    orderedNamedFiles[i] = IDs.GameTracks.GetTrackVisibleName(System.IO.Path.GetFileNameWithoutExtension(files[i].ToString()));
                }
            }

            else
            {
                for (int i = 0; i < files.Length; i++)
                {
                    orderedNamedFiles[i] = System.IO.Path.GetFileNameWithoutExtension(files[i].ToString());
                }
            }

            Array.Sort(orderedNamedFiles);

            foreach (var file in orderedNamedFiles)
            {
                ListViewItem item = new ListViewItem();
                item.ToolTip = file;
                item.Content = file;

                if (file.Length > 27)
                {
                    item.Content = file.Substring(0, 24) + "...";
                }

                list.Items.Add(item);
            }
        }

        //readPresetXML is responsible for reading the-- you get the point.
        public PresetData[] readPresetXML()
        {
            //We create an object dataArray as an array of PresetData (size of the number of game tracks) to store data for each track
            PresetData[] dataArray = new PresetData[Constants.GameTracks];

            //We try to load the preset.xml file, and catch an exception
            XmlDocument presetXMLFile = new XmlDocument();
            try
            {
                presetXMLFile.Load(presetsPath + presetName + "\\preset.xml");
            }

            catch (Exception ex)
            {
                MessageBox.Show("An unhandled exception occurred when loading the preset.xml! The exception is: " + ex + ". Please report this bug to me via reddit or Skype!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            int i = 0;

            //We try to loop through the ChildNodes of the root of the presetXMLFile, and catch an exception
            try
            {
                //For each <MUsic File> Node that is a child of the Root node in presetXMLFile
                foreach (XmlNode musicFileNode in presetXMLFile.DocumentElement.ChildNodes)
                {
                    dataArray[i].musicFile = musicFileNode.Attributes["Value"]?.InnerText;  //Add the text of the Value attribute to the musicFile member of dataArray at index i (e.g. abc_123.ogg)
                    dataArray[i].customFile = musicFileNode["CustomFile"].InnerText;    //Add the text of the CustomFile attribute to the customFile member of dataArray at index i (the name of the file we're replacing musicFile with)
                    i++;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("An unhandled exception occurred when reading the preset.xml for values! The exception is: " + ex + ". Please report this bug to me via reddit or Skype!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return dataArray;
        }

        //loadPreset loads -- -_-
        public void loadPreset()
        {
            currentDirectory = "";
            presetName = "";

            FolderBrowser2 directoryFolderBrowser = new FolderBrowser2();
            directoryFolderBrowser.ShowDialog(null);

            if (directoryFolderBrowser.DirectoryPath != null)
            {
                currentDirectory = directoryFolderBrowser.DirectoryPath;

                //If we've gone in 1 folder too deep past preset.xml (so either the Named or Original folder)
                if (System.IO.Path.GetFileName(currentDirectory) == "Named" || System.IO.Path.GetFileName(currentDirectory) == "Original")
                {
                    currentDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(currentDirectory, @"..\"));    //Take Named/Original off currentDirectory
                }

                if (!File.Exists(currentDirectory + "\\preset.xml"))
                {
                    MessageBox.Show("Invalid folder - preset.xml not found! Please ensure your selected folder contains a valid preset!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                else
                {
                    enableUI();

                    string presetName = getCustomFilesForPreset();
                    presetBox.Text = presetName;
                    tagForThisPreset(presetName);
                }

                currentDirectory += "//Original//";
            }

            else
            {
                MessageBox.Show("Please ensure you select a folder!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //getCustomFilesForPreset parses the Original folder for this preset and adds all the custom .ogg files to the customTracksList
        public string getCustomFilesForPreset()
        {
            customTracksList.Items.Clear();

            presetName = new DirectoryInfo(currentDirectory).Name;  //Set presetName to the name of the containing folder of currentDirectory
            
            //For each custom file in the Original folder, add its file name to the content of thisItem, set its tag to the full path to the file and add thisItem to the customTracksList
            foreach (var customMusicFile in Directory.GetFiles(currentDirectory + "\\Original\\", "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".ogg") || s.EndsWith(".wav") || s.EndsWith(".mp3")))
            {
                ListViewItem thisItem = new ListViewItem();
                thisItem.Content = System.IO.Path.GetFileName(customMusicFile);
                thisItem.Tag = System.IO.Path.GetFullPath(customMusicFile);
                customTracksList.Items.Add(thisItem);
            }

            return presetName;
        }

        //resetTags sets all tags of all items in gamesTracksList to nothing
        public void resetTags()
        {
            foreach (ListViewItem thisGameItem in gameTracksList.Items)
            {
                thisGameItem.Tag = null;
            }
        }

        //tagForThisPreset tags all gameTracksList items with their corresponding customTracksList value's index
        public void tagForThisPreset(string presetName)
        {
            resetTags();

            PresetData[] dataArray = new PresetData[Constants.GameTracks];
            dataArray = readPresetXML();

            try
            {
                for (int i = 0; i < dataArray.Length - 1; i++)
                {
                    ListViewItem thisGameItem = (ListViewItem)gameTracksList.Items[i];

                    //If it's default, we don't have a custom file assigned
                    if (dataArray[i].customFile == "Default")
                    {
                        thisGameItem.Tag = null;
                    }

                    else
                    {
                        foreach (ListViewItem thisCustomItem in customTracksList.Items)
                        {
                            //If the custom game track's content equals the custom file at index i in dataArray
                            if (thisCustomItem.Content.ToString() == IDs.GameTracks.GetTrackFilename((dataArray[i].customFile)))
                            {
                                thisGameItem.Tag = customTracksList.Items.IndexOf(thisCustomItem);  //Set the tag of the game track to the index of the custom track
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("An unhandled exception occurred when reading the preset.xml for tags! The exception is: " + ex + ". Please report this bug to me via reddit or Skype!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //savePreset saves the preset. *sigh*
        public void savePreset()
        {
            presetName = presetBox.Text;

            string[] gameFiles = new string[Constants.GameTracks];
            string[] customFiles = new string[Constants.GameTracks];

            for (int i = 0; i < Constants.GameTracks; i++)
            {
                ListViewItem thisGameItem = (ListViewItem)gameTracksList.Items[i];
                gameFiles[i] = IDs.GameTracks.GetTrackFilename(thisGameItem.Content.ToString());
                if (thisGameItem.Tag == null)
                {
                    customFiles[i] = "Default";
                }

                else
                {
                    ListViewItem thisCustomItem = (ListViewItem)customTracksList.Items[Convert.ToInt16(thisGameItem.Tag)];  //We set the custom game track to the content of the item in customTracksList at the index present in the game track's tag
                    customFiles[i] = thisCustomItem.Content.ToString();
                }
            }

            if (!Directory.Exists(presetsPath + presetName))
            {
                Directory.CreateDirectory(presetsPath + presetName + "\\Original");
                Directory.CreateDirectory(presetsPath + presetName + "\\Named");
                writePresetToXML(gameFiles, customFiles);
                MessageBox.Show("Preset " + presetName + " saved successfully! Please restart to load this preset in game.", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            else
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Preset " + presetName + " already exists! Do you want to overwrite?", "Overwrite?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    writePresetToXML(gameFiles, customFiles);
                    MessageBox.Show("Preset " + presetName + " saved successfully! Please restart to load this preset in game.", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        //writePresetToXML writes the content of this preset into an preset.xml file
        public void writePresetToXML(string[] gameFiles, string[] customFiles)
        {
            XmlDocument presetXMLFile = new XmlDocument();
            XmlDeclaration declaration = presetXMLFile.CreateXmlDeclaration("1.0", "UTF-8", null);

            XmlTextWriter presetWriter = new XmlTextWriter(presetsPath + presetName + "\\preset.xml", Encoding.UTF8);
            presetWriter.Formatting = Formatting.Indented;
            presetWriter.Indentation = 4;

            presetWriter.WriteStartDocument();
            presetWriter.WriteStartElement("Root");

            for (int i = 0; i < Constants.GameTracks; i++)
            {
                presetWriter.WriteStartElement("MusicFile", "");
                presetWriter.WriteAttributeString("Value", gameFiles[i]);
                presetWriter.WriteElementString("CustomFile", customFiles[i]);
                presetWriter.WriteEndElement();
            }

            presetWriter.WriteEndElement();
            presetWriter.WriteEndDocument();

            presetXMLFile.Save(presetWriter);
            presetWriter.Flush();
            presetWriter.Close();

            writeAudioToPreset();
        }

        //writeAudioToPreset copies the audio from the selected directory to the preset folder
        public void writeAudioToPreset()
        {
            //We copy the files to the Original folder
            foreach (var customMusicFile in Directory.GetFiles(currentDirectory, "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".ogg") || s.EndsWith(".wav") || s.EndsWith(".mp3")))
            {
                string thisCustomMusicFilename = System.IO.Path.GetFileName(customMusicFile);

                if (!File.Exists(presetsPath + presetName + "\\Original\\" + thisCustomMusicFilename))
                {
                    File.Copy(customMusicFile, presetsPath + presetName + "\\Original\\" + thisCustomMusicFilename, true);
                }
            }

            PresetData[] dataArray = new PresetData[214];
            dataArray = readPresetXML();

            //We copy the files from the Original folder, and name them to the games's filenames then copy them to the Named folder
            for (int i = 0; i < dataArray.Length - 1; i++)
            {
                if (dataArray[i].customFile != "Default")
                {
                    convertToOGG(presetsPath + presetName + "\\Original\\", presetsPath + presetName + "\\Named\\", System.IO.Path.GetFileNameWithoutExtension(dataArray[i].customFile), System.IO.Path.GetFileNameWithoutExtension(dataArray[i].musicFile));
                }
            }
        }

        //Converts wavs and mp3s to OGGs so the game can play them
        public void convertToOGG(string origDir, string outDir, string origFile, string newFile)
        {
            Process ffmpeg = new Process();
            ffmpeg.StartInfo.RedirectStandardOutput = true;
            ffmpeg.StartInfo.RedirectStandardError = true;
            ffmpeg.StartInfo.UseShellExecute = false;
            ffmpeg.StartInfo.CreateNoWindow = true;

            ffmpeg.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\ffmpeg.exe";

            string arguments = "-nostdin -i \"" + origDir + origFile + ".wav" + "\"  \"" + outDir + newFile + ".ogg\"";

            ffmpeg.StartInfo.Arguments = arguments;

            ffmpeg.Start();
            ffmpeg.WaitForExit();
        }

        //Again, the function name speaks for itself.
        public void enableUI()
        {
            gameTracksList.IsEnabled = true;
            customTracksList.IsEnabled = true;
            presetBox.IsEnabled = true;
            saveButton.IsEnabled = true;

            gameTracksList.Items.Clear();
            customTracksList.Items.Clear();

            DirectoryInfo backupDir = new DirectoryInfo(backupPath);
            FileInfo[] files = backupDir.GetFiles("*.ogg");
            orderFilenamesForListView(files, gameTracksList, true);
            
        }

        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowser2 directoryFolderBrowser = new FolderBrowser2();
            directoryFolderBrowser.ShowDialog(null);

            if (directoryFolderBrowser.DirectoryPath != null)
            {
                enableUI();
                currentDirectory = directoryFolderBrowser.DirectoryPath;
                presetName = System.IO.Path.GetFileName(directoryFolderBrowser.DirectoryPath);
                presetBox.Text = presetName;

                DirectoryInfo customDir = new DirectoryInfo(currentDirectory);
                FileInfo[] files = customDir.GetFiles()
                    .Where(f => Constants.SupportedFileTypes.Contains(f.Extension.ToLower()))
                    .ToArray();
                
                orderFilenamesForListView(files, customTracksList);

                /*foreach (var customMusicFile in Directory.GetFiles(currentDirectory, "*.*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".ogg") || s.EndsWith(".wav") || s.EndsWith(".mp3")))
                {
                    ListViewItem thisItem = new ListViewItem();
                    thisItem.Content = System.IO.Path.GetFileName(customMusicFile);
                    thisItem.Tag = System.IO.Path.GetFullPath(customMusicFile);
                    customTracksList.Items.Add(thisItem);
                }*/
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender == gameTracksList)
            {
                if (gameTracksList.SelectedIndex > -1)
                {
                    if (((ListViewItem)gameTracksList.SelectedItem).Tag != null)
                    {
                        customTracksList.SelectedIndex = Convert.ToInt16(((ListViewItem)gameTracksList.SelectedItem).Tag);
                    }

                    else
                    {
                        customTracksList.UnselectAll();
                    }
                }
            }

            else if (sender == customTracksList)
            {
                if (gameTracksList.SelectedIndex > -1)
                {
                    ((ListViewItem)gameTracksList.SelectedItem).Tag = customTracksList.SelectedIndex;
                }
            }
        }

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            if (r.VisualHit.GetType() != typeof(ListBoxItem))
            {
                if (sender == gameTracksList)
                {
                    gameTracksList.UnselectAll();
                    customTracksList.UnselectAll();
                }

                else if (sender == customTracksList)
                {
                    customTracksList.UnselectAll();
                }
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            savePreset();
        }

        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            loadPreset();
        }

        private void SharpWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IDs.GameTracks.populateDictionary();
        }
    }
}
