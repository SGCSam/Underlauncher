using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Underlauncher
{
    public class MusicPreset
    {
        public Constants.MusicPresetData[] presetData { get; private set; }
        public string customTrackDirectory { get; private set; }
        public string presetName { get; set; }
        private string _OutputPath;

        public MusicPreset(string customDir)
        {
            customTrackDirectory = customDir;
            presetData = new Constants.MusicPresetData[Constants.GameTracksCount];
        }

        //Loads a preset
        public bool Load()
        {
            _OutputPath = customTrackDirectory;
            customTrackDirectory += "//Original//";
            return ReadPresetXMLFile();
        }

        //ReadPresetXMLFile reads the preset.xml in the specified directory and populates the presetData array
        public bool ReadPresetXMLFile()
        {
            try
            {
                XDocument presetXML = XDocument.Load(_OutputPath + "//preset.xml");
                int i = 0;

                presetName = presetXML.Root.Element("Name").Value.ToString();

                foreach (XElement element in presetXML.Root.Elements("MusicTrack"))
                {
                    presetData[i].GameTrack = element.Attribute("GameTrack").Value.ToString();
                    presetData[i].CustomTrack = element.Attribute("CustomTrack").Value.ToString();
                    i++;
                }

                return true;
            }

            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Caught exception when reading the preset XML file. It was " + ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        //Write writes the selected audio into a preset, with a list of game and custom tracks passed in
        public void Write(string[] game, string[] custom)
        {
            _OutputPath = Constants.PresetsPath + presetName;

            for (int i = 0; i < Constants.GameTracksCount; i++)
            {
                presetData[i].GameTrack = game[i];
                presetData[i].CustomTrack = custom[i];
            }

            CopyAllAudioToOriginal();
            CreatePresetXMLFile();
            CopyAudioToNamed();
        }

        //CopyAllAudioToOriginal copies all selected custom tracks from their directory to the Original folder in the preset
        private void CopyAllAudioToOriginal()
        {
            foreach (var file in MiscFunctions.GetFilesWithExtensions(customTrackDirectory, Constants.SupportedFileTypes))
            {
                foreach (var data in presetData)
                {
                    if (Path.GetFileName(file) == Path.GetFileName(data.CustomTrack))
                    {
                        File.Copy(file, _OutputPath + "//Original//" + Path.GetFileName(data.CustomTrack), true);
                    }
                }
            }
        }

        //CreatePresetXMLFile generates the preset.xml file which specifies all the track information
        private void CreatePresetXMLFile()
        {
            try
            {
                XDocument presetXML = new XDocument(
                                    new XDeclaration("1.0", "utf-8", "yes"),
                                        new XElement("Root",
                                            new XElement("Name", presetName))
                                    );

                for (int i = 0; i < Constants.GameTracksCount; i++)
                {
                    presetXML.Root.Add(new XElement("MusicTrack",
                                        new XAttribute("GameTrack", presetData[i].GameTrack),
                                        new XAttribute("CustomTrack", presetData[i].CustomTrack)
                                        )
                        );
                }

                presetXML.Save(_OutputPath + "//preset.xml");
            }

            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Caught exception when writing the preset XML file. It was " + ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //CopyAudioToNamed copies the audio file from original, converts it to OGG, then outputs it to the Named folder
        private void CopyAudioToNamed()
        {
            for (int i = 0; i < Constants.GameTracksCount; i++)
            {
                if (presetData[i].CustomTrack != "Default")
                {
                    MiscFunctions.convertToOGG(_OutputPath + "//Original//", _OutputPath + "//Named//", presetData[i].CustomTrack, presetData[i].GameTrack);
                }
            }
        }
    }
}
