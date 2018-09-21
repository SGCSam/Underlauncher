using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

//The Stats class stores all weapon, armor, EXP, LOVE and HP values to be used and stored in file0
namespace Underlauncher
{
    public static class Stats
    {
        public static Dictionary<int, int> itemATs = new Dictionary<int, int>();
        public static Dictionary<int, int> itemDFs = new Dictionary<int, int>();

        public static Dictionary<int, int> LOVEHPs = new Dictionary<int, int>();
        public static Dictionary<int, int> LOVEATs = new Dictionary<int, int>();
        public static Dictionary<int, int> LOVEDFs = new Dictionary<int, int>();
        public static Dictionary<int, int> LOVEEXPs = new Dictionary<int, int>();

        //populateDictionary parses the relevant XML Lists and adds their values into the appropriate dictionary
        public static void populateDictionary()
        {
            try
            {
                LOVEHPs.Clear();
                LOVEATs.Clear();
                LOVEDFs.Clear();
                LOVEEXPs.Clear();

                XmlDocument LOVEXMLFile = new XmlDocument();
                LOVEXMLFile.Load("Assets//XML Lists//LOVEList.xml");

                foreach (XmlNode LOVENode in LOVEXMLFile.DocumentElement.ChildNodes)
                {
                    LOVEHPs.Add(Convert.ToInt16(LOVENode.Attributes["Level"]?.InnerText), Convert.ToInt16(LOVENode.Attributes["HP"]?.InnerText));
                    LOVEATs.Add(Convert.ToInt16(LOVENode.Attributes["Level"]?.InnerText), Convert.ToInt16(LOVENode.Attributes["AT"]?.InnerText));
                    LOVEDFs.Add(Convert.ToInt16(LOVENode.Attributes["Level"]?.InnerText), Convert.ToInt16(LOVENode.Attributes["DF"]?.InnerText));
                    LOVEEXPs.Add(Convert.ToInt16(LOVENode.Attributes["Level"]?.InnerText), Convert.ToInt32(LOVENode.Attributes["EXP"]?.InnerText));
                }
            }

            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("An exception occured when populating the LOVE dictionary! Cannot continue. The exception was:\n\n" + ex +
                                ".\n\nPlease make me aware of this issue via reddit or Skype.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //getItemAT gets the total attack for each item selected by the user. We iterate through all weapons AND armors as some armors also increase AT
        public static string getItemAT(int weaponID, int armorID)
        {
            int AT = 0;
            foreach (var item in itemATs)
            {
                if (weaponID == item.Key || armorID == item.Key)
                {
                    AT += item.Value;
                }
            }

            return AT.ToString();
        }

        //getItemDF gets the defense value for the armor selected by the user
        public static string getItemDF(int armorID)
        {
            foreach (var item in itemDFs)
            {
                if (armorID == item.Key)
                {
                    return item.Value.ToString();
                }
            }

            return "ERROR";
        }

        //getLOVEValue acquires the appropriate value from thisDict based on the player's LOVE
        //i.e. when LOVE is changed, we want to get the new EXP value so we pass in EXPDict as thisDict and the new LOVE value to get the new EXP value
        public static string getLOVEValue(int LOVE, Dictionary<int, int> thisDict)
        {
            foreach (var item in thisDict)
            {
                if (LOVE == item.Key)
                {
                    return item.Value.ToString();
                }
            }

            return "ERROR!";
        }
    }
}
