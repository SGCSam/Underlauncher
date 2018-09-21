using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

//The Constants class stores constant values made before runtime to be used globally through all classes and scopes
namespace Underlauncher
{
    public class Constants
    {
        public const string GameVersion = "1.0.8.0";

        public const int GameTracksCount = 212;
        public const int NumberOfEndings = 22;

        public static string BackupPath = Environment.CurrentDirectory + "//Backup Game Directory//";
        public static string PresetsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//Underlauncher//Presets//";
        public static string SavesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//Underlauncher//Saves//";
        public static string AppdataPath = Environment.GetEnvironmentVariable("LocalAppData") + "\\UNDERTALE\\";

        public static string[] SupportedFileTypes = { ".wav", ".mp3", ".ogg" };

        public struct MusicPresetData
        {
            public string GameTrack;
            public string CustomTrack;
        }

        public enum CharacterReactions
        {
            Positive,
            Normal,
            Negative,
            None
        }
    }
}
