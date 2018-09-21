using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

//The XML class handles all parsing and writing to intended editable XML files
namespace Underlauncher
{
    public class XML
    {
        private static string GamePath = "";

        private static XDocument settingsFile = XDocument.Load("Assets//Settings.xml");

        public static bool characterMessagesSetting;

        //Check ensures that the path.xml file exists and creates it if not
        public static void Check()
        {
            if (!File.Exists("Assets//path.xml"))
            {
                new XDocument(
                    new XElement("Root",
                        new XElement("Path", "")
                    )
                )
                .Save("Assets//path.xml");
            }
        }

        //ReadGamePath parses path.xml to get the stored game path within
        public static void ReadGamePath()
        {
            XDocument pathFile = XDocument.Load("Assets//path.xml");
            var query = from t in pathFile.Descendants("Root")
                        select t;

            foreach (var element in query)
            {
                GamePath = element.Element("Path").Value;
            }
        }

        //WriteGamePath writes the browsed to game path to path.xml
        public static void WriteGamePath(string pathElemVal)
        {
            XDocument pathFile = XDocument.Load("Assets//path.xml");
            var query = from t in pathFile.Descendants("Root")
                        select t;

            foreach (var element in query)
            {
                element.SetElementValue("Path", pathElemVal);
            }
            pathFile.Save("Assets//path.xml");

            ReadGamePath();
        }

        //The public GetGamePath function returns the private variable GamePath
        public static string GetGamePath()
        {
            return GamePath;
        }

        public static void ReadSettingsXML()
        {
            characterMessagesSetting = Convert.ToBoolean(settingsFile.Descendants("Setting").First(element => (string)element.Attribute("Name").Value == "CharacterMessages").Attribute("Value").Value);
        }

        public static void WriteXMLSetting(string setting, string value)
        {
            settingsFile.Descendants("Setting").First(element => (string)element.Attribute("Name").Value == setting).Attribute("Value").Value = value.ToString();
            settingsFile.Save("Assets//Settings.xml");
            ReadSettingsXML();
        }
    }
}
