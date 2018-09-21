using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

//MiscFunctions contains miscellaneous functions used throughout all classes and scopes
namespace Underlauncher
{
    class MiscFunctions
    {
        //SetBooleanValueFromLocation compares the currentLocation and the triggerLocation, returning the appropriate boolean value
        //This is used quite often for setting flag values for file0 depending on the player's location
        public static bool SetBooleanValueFromLocation(int currentLocation, int triggerLocation)
        {
            if (currentLocation >= triggerLocation)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        //GetFilesWithExtensions returns a list of files in the specified dir that have one of the extensions in the extensions list
        public static string[] GetFilesWithExtensions(string dir, string[] extensions)
        {
            return Directory.GetFiles(dir, "*.*", SearchOption.TopDirectoryOnly).Where(s => extensions.Contains(Path.GetExtension(s))).ToArray();
        }

        //convertToOgg takes a file and converts it to the OGG vorbis audio format
        public static void convertToOGG(string origDir, string outDir, string origFile, string newFile)
        {
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }

            Process ffmpeg = new Process();
            ffmpeg.StartInfo.RedirectStandardOutput = true;
            ffmpeg.StartInfo.RedirectStandardError = false;
            ffmpeg.StartInfo.UseShellExecute = false;
            ffmpeg.StartInfo.CreateNoWindow = true;

            ffmpeg.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\Assets\\ffmpeg.exe";

            string arguments = "-nostdin -y -i \"" + origDir + origFile + "\"  \"" + outDir + newFile + ".ogg\"";

            ffmpeg.StartInfo.Arguments = arguments;
            
            ffmpeg.Start();
            ffmpeg.WaitForExit();
            ffmpeg.Close();
        }

        //Returns a random Character from the Characters enum to be used in the MessageBoxes
        public static Characters GetRandomMessageBoxCharacter(List<Characters> charaList)
        {
            Random randObj = new Random(Guid.NewGuid().GetHashCode());
            return charaList[randObj.Next(charaList.Count - 1)];
        }
    }
}
