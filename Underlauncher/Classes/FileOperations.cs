using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO.Compression;

//The FileOperations class handles all file manipulation, copying and pasting of files to and from the application's working directory and other
//locations on the user's PC
namespace Underlauncher
{
    public class FileOperations
    {
        //goodFolderIntegrity checks that all application folders and files are present and creates them if this is not the case
        public static bool goodFolderIntegrity()
        {
            if (!Directory.Exists(Constants.BackupPath))
            {
                Directory.CreateDirectory(Constants.BackupPath);

                if (Directory.Exists(XML.GetGamePath()))
                {
                    foreach (var file in Directory.GetFiles(XML.GetGamePath()))
                    {
                        File.Copy(file, Constants.BackupPath + "//" + System.IO.Path.GetFileName(file), true);
                    }
                }
            }

            if (!Directory.Exists(Constants.PresetsPath))
            {
                Directory.CreateDirectory(Constants.PresetsPath);
            }

            if (!Directory.Exists(Constants.SavesPath))
            {
                Directory.CreateDirectory(Constants.SavesPath);
            }

            if (!Directory.Exists(XML.GetGamePath()))   //If the path of the game in path.xml is empty or is invalid
            {
                //We delete any possible backup files in the backup directory, in case they were backed up from a modded install previously
                System.IO.DirectoryInfo di = new DirectoryInfo(Constants.BackupPath);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }

                return false;
            }

            return true;
        }

        //backupFilesFromGameDirectory copies all files from the game's install directory to Backup Game Directory in the application working folder
        public static bool backupFilesFromGameDirectory(string dir)
        {
            try
            {
                foreach (var thisFile in Directory.GetFiles(dir))
                {
                    string thisFilename = System.IO.Path.GetFileName(thisFile);

                    if (!File.Exists(Constants.BackupPath + thisFile))
                    {
                        File.Copy(thisFile, Constants.BackupPath + thisFilename, true);
                    }
                }
            }

            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("A fatal exception occurred when attempting to backup your game files. The exception was: \n\n" + ex +
                                "\n\n Please notify me of this issue via reddit or Skype. Exiting now...", "Fatal Error!",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
                return false;
            }

            return true;
        }

        //disableDogcheck is responsible for hex editing data.win to disable Dog Check and allow more advanced save manipulation
        public static bool disableDogcheck(string gameVersion, string winPath)
        {
            List<int> hexPositions = new List<int>();

            try
            {
                if (gameVersion != "1.0.8.0")
                {
                    System.Windows.MessageBox.Show("Found Game Version of " + gameVersion + ". Please ensure you update your game version to 1.0.8.0.");
                    return false;
                }

                else
                {
                    BinaryWriter writer = new BinaryWriter(File.OpenWrite(winPath));
                    byte[] byteToWrite = { 0x01 };

                    hexPositions.Add(0x76DF38);
                    hexPositions.Add(0x76DF58);
                    hexPositions.Add(0x76DF78);
                    hexPositions.Add(0x76DF98);
                    hexPositions.Add(0x76DFB8);
                    hexPositions.Add(0x76DFD8);
                    hexPositions.Add(0x76DFF8);
                    hexPositions.Add(0x76E018);

                    foreach (var position in hexPositions)
                    {
                        writer.BaseStream.Position = position;
                        writer.Write(byteToWrite);
                    }

                    writer.Close();
                    writer.Dispose();

                }
            }

            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("A fatal exception occurred when attempting to disable Dogcheck. The exception was: \n\n" + ex +
                                "\n\n Please notify me of this issue via reddit or Skype. Exiting now...", "Fatal Error!",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        //copyMusicPresetToGame copies all named music from the presetPath to the game's install directory
        public static void copyMusicPresetToGame(string presetPath)
        {
            //We copy over all music files from the backup directory to the game directory to ensure we don't have any leftover music files from other presets
            foreach (var musicFile in Directory.GetFiles(Constants.BackupPath))
            {
                if (Path.GetExtension(musicFile) == ".ogg")
                {
                    string thisMusicFilename = System.IO.Path.GetFileName(musicFile);
                    FileInfo gameMusicFile = new FileInfo(XML.GetGamePath() + "\\" + thisMusicFilename);
                    var gameMusicFileLength = gameMusicFile.Length;

                    if (musicFile.Length != gameMusicFileLength)
                    {
                        File.Copy(musicFile, XML.GetGamePath() + "\\" + thisMusicFilename, true);
                    }
                }
            }

            if (presetPath != "Default Music")
            {
                presetPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Underlauncher\\Presets\\" + presetPath + "\\Named\\";

                foreach (var customMusicFile in Directory.GetFiles(presetPath))
                {
                    string thisMusicFilename = System.IO.Path.GetFileName(customMusicFile);

                    if (Path.GetExtension(thisMusicFilename) == ".ogg")
                    {
                        File.Copy(customMusicFile, XML.GetGamePath() + "\\" + thisMusicFilename, true);
                    }
                }
            }
        }

        //copySAVEToGame copies file0 from the saveToCopy directory to Undertale's %localappdata% folder
        public static void copySAVEToGame(string saveToCopy)
        {
            if (saveToCopy != "Local Appdata Save")
            {
                foreach (var file in Directory.GetFiles(Constants.SavesPath + saveToCopy))
                {
                    string thisFilename = System.IO.Path.GetFileName(file);
                    File.Copy(file, Constants.AppdataPath + "\\" + thisFilename, true);
                }
            }
        }

        //setDebugMode is responsible for hex editing data.win to enable debug mode
        public static void setDebugMode(bool enableDebug)
        {
            try
            {
                BinaryWriter writer = new BinaryWriter(File.OpenWrite(XML.GetGamePath() + "//data.win"));
                byte[] byteToWrite;

                if (enableDebug)
                {
                    byteToWrite = new byte[] { 0x01 };
                }

                else
                {
                    byteToWrite = new byte[] { 0x00 };
                }

                writer.BaseStream.Position = 0x7748C4;
                writer.Write(byteToWrite);

                writer.Close();
                writer.Dispose();
            }

            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("An exception occurred when attempting to set Debug Mode. The exception was: \n\n" + ex +
                                "\n\n Please notify me of this issue via reddit or Skype. " +
                                "\n\n The game will still function but the setting of Debug Mode cannot be verfied.", "Error!",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //setGenocide creates the files in Local Appdata to set the game into a Genocide state
        public static void setGenocide(GenocideStates state)
        {
            switch(state)
            {
                case GenocideStates.Soulless:
                    if (!File.Exists(Constants.AppdataPath + "system_information_963"))
                    {
                        File.Create(Constants.AppdataPath + "system_information_963").Dispose();
                    }
                    goto case GenocideStates.Abyss;

                case GenocideStates.Abyss:
                    if (!File.Exists(Constants.AppdataPath + "system_information_962"))
                    {
                        File.Create(Constants.AppdataPath + "system_information_962").Dispose();
                    }
                    break;
            }
        }
    }
}
