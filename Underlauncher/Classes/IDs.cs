using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

//The IDs class stores all item, armor, weapon and location values in the game in their corresponding classes's dictionary. It does this by parsing the 
//included XML lists in the Assets folder. It also stores most enums used through out other classes, such as Characters, CharacterStates and DateStates

namespace Underlauncher
{
    public class IDs
    {
        public static Dictionary<string, int> allItemsDict = new Dictionary<string, int>();

        public class Weapons
        {
            public static Dictionary<string, int> weaponDict = new Dictionary<string, int>();

            public static void populateDictionary()
            {
                try
                {
                    weaponDict.Clear();
                    Stats.itemATs.Clear();

                    XmlDocument weaponsXMLFile = new XmlDocument();
                    weaponsXMLFile.Load("Assets//XML Lists//weaponsList.xml");

                    foreach (XmlNode weaponNode in weaponsXMLFile.DocumentElement.ChildNodes)
                    {
                        weaponDict.Add(weaponNode.Attributes["Name"]?.InnerText, Convert.ToInt16(weaponNode.Attributes["ID"]?.InnerText));
                        Stats.itemATs.Add(Convert.ToInt16(weaponNode.Attributes["ID"]?.InnerText), Convert.ToInt16(weaponNode.Attributes["AT"]?.InnerText));
                    }
                }

                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("An exception occured when populating the weapons dictionary! Cannot continue. The exception was:\n\n" + ex +
                                    ".\n\nPlease make me aware of this issue via reddit or Skype.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            public static string SetCurrentWeapon(int ID)
            {
                foreach (var item in weaponDict)
                {
                    if (item.Value == ID)
                    {
                        return item.Key;
                    }
                }

                return "Nothing";
            }
        }

        public class Armors
        {
            public static Dictionary<string, int> armorsDict = new Dictionary<string, int>();

            public static void populateDictionary()
            {
                try
                {
                    armorsDict.Clear();
                    Stats.itemDFs.Clear();

                    XmlDocument armorsXMLFile = new XmlDocument();
                    armorsXMLFile.Load("Assets//XML Lists//armorsList.xml");

                    foreach (XmlNode armorNode in armorsXMLFile.DocumentElement.ChildNodes)
                    {
                        armorsDict.Add(armorNode.Attributes["Name"]?.InnerText, Convert.ToInt16(armorNode.Attributes["ID"]?.InnerText));
                        Stats.itemDFs.Add(Convert.ToInt16(armorNode.Attributes["ID"]?.InnerText), Convert.ToInt16(armorNode.Attributes["DF"]?.InnerText));

                        if (armorNode.Attributes["AT"] != null)
                        {
                            Stats.itemATs.Add(Convert.ToInt16(armorNode.Attributes["ID"]?.InnerText), Convert.ToInt16(armorNode.Attributes["AT"]?.InnerText));
                        }

                    }
                }

                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("An exception occured when populating the armors dictionary! Cannot continue. The exception was:\n\n" + ex +
                                    ".\n\nPlease make me aware of this issue via reddit or Skype.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            public static string SetCurrentArmor(int ID)
            {
                foreach (var item in armorsDict)
                {
                    if (item.Value == ID)
                    {
                        return item.Key;
                    }
                }

                return "Nothing";
            }
        };

        public class Items
        {
            public static Dictionary<string, int> itemsDict = new Dictionary<string, int>();

            public static void populateDictionary()
            {
                try
                {
                    itemsDict.Clear();
                    XmlDocument itemsXMLFile = new XmlDocument();
                    itemsXMLFile.Load("Assets//XML Lists//itemsList.xml");

                    foreach (XmlNode weaponNode in itemsXMLFile.DocumentElement.ChildNodes)
                    {
                        itemsDict.Add(weaponNode.Attributes["Name"]?.InnerText, Convert.ToInt16(weaponNode.Attributes["ID"]?.InnerText));
                    }
                }

                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("An exception occured when populating the items dictionary! Cannot continue. The exception was:\n\n" + ex +
                                    ".\n\nPlease make me aware of this issue via reddit or Skype.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            public static string SetCurrentItem(int ID)
            {
                foreach (var item in itemsDict)
                {
                    if (item.Value == ID)
                    {
                        return item.Key;
                    }
                }

                return "Nothing";
            }
        };

        public class InventoryItems
        {
            public static void populateDictionary()
            {
                foreach (var weapon in Weapons.weaponDict)
                {
                    if (!allItemsDict.ContainsKey(weapon.Key))
                    {
                        allItemsDict.Add(weapon.Key, weapon.Value);
                    }
                }

                foreach (var armor in Armors.armorsDict)
                {
                    if (!allItemsDict.ContainsKey(armor.Key))
                    {
                        allItemsDict.Add(armor.Key, armor.Value);
                    }
                }

                foreach (var item in Items.itemsDict)
                {
                    if (!allItemsDict.ContainsKey(item.Key))
                    {
                        allItemsDict.Add(item.Key, item.Value);
                    }
                }

                foreach (var phoneItem in PhoneItems.phoneItemsDict)
                {
                    if (!allItemsDict.ContainsKey(phoneItem.Key))
                    {
                        allItemsDict.Add(phoneItem.Key, phoneItem.Value);
                    }
                }
            }

            public static string SetItem(int ID)
            {
                foreach (var item in allItemsDict)
                {
                    if (item.Value == ID)
                    {
                        return item.Key;
                    }
                }

                return "Nothing";
            }
        }

        public class PhoneItems
        {
            public static Dictionary<string, int> phoneItemsDict = new Dictionary<string, int>();

            public static void populateDictionary()
            {
                try
                {
                    phoneItemsDict.Clear();
                    XmlDocument phoneItemsXMLFile = new XmlDocument();
                    phoneItemsXMLFile.Load("Assets//XML Lists//phoneItemsList.xml");

                    foreach (XmlNode phoneItemNode in phoneItemsXMLFile.DocumentElement.ChildNodes)
                    {
                        phoneItemsDict.Add(phoneItemNode.Attributes["Name"]?.InnerText, Convert.ToInt16(phoneItemNode.Attributes["ID"]?.InnerText));
                    }
                }

                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("An exception occured when populating the phone items dictionary! Cannot continue. The exception was:\n\n" + ex +
                                    ".\n\nPlease make me aware of this issue via reddit or Skype.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            public static string SetItem(int ID)
            {
                foreach (var item in phoneItemsDict)
                {
                    if (item.Value == ID)
                    {
                        return item.Key;
                    }
                }

                return "Nothing";
            }

            public static int GetPhoneItemID(string name)
            {
                foreach (var item in phoneItemsDict)
                {
                    if (item.Key == name)
                    {
                        return item.Value;
                    }
                }

                return 0;
            }
        }

        public class Locations
        {
            public static Dictionary<string, int> locationsDict = new Dictionary<string, int>();
            public static Dictionary<int, Tuple<double, double>> plotDict = new Dictionary<int, Tuple<double, double>>();

            public static void populateDictionary()
            {
                try
                {
                    locationsDict.Clear();
                    plotDict.Clear();
                    XmlDocument locationsXMLFile = new XmlDocument();
                    locationsXMLFile.Load("Assets//XML Lists//roomsList.xml");

                    foreach (XmlNode roomNode in locationsXMLFile.DocumentElement.ChildNodes)
                    {
                        locationsDict.Add(roomNode.Attributes["Name"]?.InnerText, Convert.ToInt16(roomNode.Attributes["ID"]?.InnerText));
                    }

                    XmlDocument plotXMLFile = new XmlDocument();
                    plotXMLFile.Load("Assets//XML Lists//plotList.xml");

                    foreach (XmlNode plotNode in plotXMLFile.DocumentElement.ChildNodes)
                    {
                        plotDict.Add(Convert.ToInt16(plotNode.Attributes["Room"]?.InnerText), Tuple.Create(Convert.ToDouble(plotNode.Attributes["PacifistValue"]?.InnerText), Convert.ToDouble(plotNode.Attributes["GenocideValue"]?.InnerText)));
                    }
                }

                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("An exception occured when populating the locations dictionary! Cannot continue. The exception was:\n\n" + ex +
                                    ".\n\nPlease make me aware of this issue via reddit or Skype.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            public static string SetCurrentLocation(int ID)
            {
                foreach (var room in locationsDict)
                {
                    if (room.Value == ID)
                    {
                        return room.Key;
                    }
                }

                return "Nothing";
            }

            public static int GetRoomID(string name)
            {
                foreach (var room in locationsDict)
                {
                    if (room.Key == name)
                    {
                        return room.Value;
                    }
                }

                return 0;
            }

            public static string GetRoomString(int roomID)
            {
                foreach (var room in locationsDict)
                {
                    if (room.Value == roomID)
                    {
                        return room.Key;
                    }
                }

                return "";
            }

            public static double GetPlotForRoom(int ID, Routes.GameRoutes route)
            {
                double plot = 0;

                foreach (var room in plotDict)
                {
                    if (room.Key <= ID)
                    {
                        if (route != Routes.GameRoutes.Genocide)
                        {
                            plot = room.Value.Item1;
                        }

                        else
                        {
                            plot = room.Value.Item2;
                        }

                    }

                    else if (room.Key > ID)
                    {
                        break;
                    }
                }

                return plot;
            }
        }

        public class GameTracks
        {
            public static Dictionary<string, string> gameTracksDict = new Dictionary<string, string>();

            public static void populateDictionary()
            {
                try
                {
                    gameTracksDict.Clear();
                    XmlDocument gameTracksXMLFile = new XmlDocument();
                    gameTracksXMLFile.Load("Assets//XML Lists//tracksList.xml");

                    
                    foreach (XmlNode roomNode in gameTracksXMLFile.DocumentElement.ChildNodes)
                    {
                        gameTracksDict.Add(roomNode.Attributes["Filename"]?.InnerText, roomNode.Attributes["VisibleName"]?.InnerText);
                    }
                }

                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("An exception occured when populating the game tracks dictionary! Cannot continue. The exception was:\n\n" + ex +
                                    ".\n\nPlease make me aware of this issue via reddit or Skype.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            public static string GetTrackFilename(string file)
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                foreach (var track in gameTracksDict)
                {
                    if (track.Value == filename)
                    {
                        return track.Key;
                    }
                }

                return "";
            }

            public static string GetTrackVisibleName(string file)
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                foreach (var track in gameTracksDict)
                {
                    if (track.Key == filename)
                    {
                        return track.Value;
                    }
                }

                return "";
            }
        }

        public static int GetID(string itemToSearch)
        {
            foreach(var item in IDs.allItemsDict)
            {
                if (item.Key == itemToSearch)
                {
                    return item.Value;
                }
            }

            return 0;
        }

        public static string GetStringValue(int itemID)
        {
            foreach (var item in IDs.allItemsDict)
            {
                if (item.Value == itemID)
                {
                    return item.Key;
                }
            }

            return "";
        }
    }

    public enum CharacterStates
    {
        Default = 0,
        Spared = 0,
        Killed = 1,
        ComedianKilled = 2,
        TorielKilled = 4
    };

    public enum DateStates
    {
        DateNotStarted = 0,
        PapyrusOutHouse = 1,
        PapyrusInHouse = 2,
        PapyrusComplete = 4,
        UndyneAtDoor = 1,
        UndyneComplete = 5,
        AlphysDateTrigger = 1,
        AlphysDateComplete = 2,
        SansWaiting = 0,
        SansComplete = 2
    };

    public enum GenocideStates
    {
        Abyss,
        Soulless,
        None
    }

    public enum Characters
    {
        All,
        None,
        Flowey,
        Papyrus,
        Undyne,
        Mettaton,
        Toriel,
        RuinsDummy,
        Doggo,
        Dogamy,
        GreaterDog,
        Comedian,
        Shyren,
        MadDummy,
        Muffet,
        BroGuards,
        Alphys,
        Asgore,
        Asriel,
        Sans
    };
}
