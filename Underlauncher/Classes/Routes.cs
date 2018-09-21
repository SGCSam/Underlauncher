using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

//The Routes class is used to store and compare values associated with the different game routes to be used in file0 and undertale.ini
namespace Underlauncher
{
    public static class Routes
    {
        //The enum GameRoutes stores all GameRoutes as well as their string description used to populate Comboboxes as items
        public enum GameRoutes
        {
            [Description("Genocide")]
            Genocide,
            [Description("Family")]
            Family,
            [Description("Betrayed Undyne (No Date)")]
            BetrayedUndyneNoDate,
            [Description("Betrayed Undyne (Killed Mettaton)")]
            BetrayedUndyneMettaton,
            [Description("Betrayed Undyne (Dated)")]
            BetrayedUndyneDated,
            [Description("Exiled Queen (10+ Monsters Killed)")]
            ExiledMonsters,
            [Description("Exiled Queen (Killed Papyrus)")]
            ExiledUndyneNoPapyrus,
            [Description("Exiled Queen (Killed Undyne)")]
            ExiledNoUndynePapyrus,
            [Description("Exiled Queen (Killed Undyne and Papyrus)")]
            ExiledNoUndyneNoPapyrus,
            [Description("Queen Undyne")]
            QueenUndyne,
            [Description("Queen Undyne (Killed Papyrus)")]
            QueenUndyneNoPapyrus,
            [Description("King Mettaton")]
            KingMettaton,
            [Description("King Mettaton (Killed Papyrus)")]
            KingMettatonNoPapyrus,
            [Description("King Papyrus")]
            KingPapyrus,
            [Description("King Dog")]
            KingDog,
            [Description("Leaderless")]
            Leaderless,
            [Description("Queen Alphys")]
            QueenAlphys,
            [Description("True Pacifist Alphys Date")]
            TruePacifistDate,
            [Description("True Pacifist True Lab")]
            TruePacifistLab,
            [Description("True Pacifist Asriel Battle")]
            TruePacifistAsriel,
            [Description("True Pacifist Asriel Talk")]
            TruePacifistAsrielTalk,
            [Description("True Pacifist Epilogue")]
            TruePacifistEpilogue
        }

        //GetEnumDescription is used to get the description for this enum value, to be used as a comparison value or to be added to a Combobox
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        //GetEnumValue is used to convert a string from a Combobox to its appropriate enum value
        public static GameRoutes GetEnumValue(string selectedItem)
        {
            foreach (GameRoutes ending in Enum.GetValues(typeof(Routes.GameRoutes)))
            {
                if (selectedItem == GetEnumDescription(ending))
                {
                    return ending;
                }
            }

            return GameRoutes.TruePacifistDate;
        }

        //CheckRoomForState checks the player's currentRoom and allows the character's state to be set if it is greater than or equal to the triggerRoom
        public static bool CheckRoomForState(int currentRoom, Characters character)
        {
            int triggerRoom = -1;

            switch (character)
            {
                case Characters.Papyrus:
                    triggerRoom = 82;   //Waterfall Entrance
                    break;

                case Characters.Undyne:
                    triggerRoom = 139;  //After Water Cooler - Alphys's Lab
                    break;

                case Characters.Mettaton:
                    triggerRoom = 212;  //Just before elevator to Castle
                    break;

                case Characters.Toriel:
                    triggerRoom = 42;   //Ruins Exit Corridor
                    break;

                case Characters.Comedian:
                    triggerRoom = 46;
                    break;

            }

            if (currentRoom >= triggerRoom)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        //CheckRoomForDate checks the player's currentRoom and allows the character's date state to be set if it is greater than or equal to the triggerRoom
        public static bool CheckRoomForDate(int currentRoom, Characters character, GameRoutes Route)
        {
            int triggerRoom = -1;

            switch (character)
            {
                case Characters.Papyrus:
                    triggerRoom = 82;   //Papyrus's Boss Battle
                    break;

                case Characters.Undyne:
                    triggerRoom = 139; //Water Cooler (i.e. I assume we go to Undyne's house immediately after)
                    break;
            }

            if (currentRoom >= triggerRoom)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        //GetMurderLevelForLocation checks if the intendedLevel is valid for the currentLocation and then returns the highest possible level
        public static int GetMurderLevelForLocation(int currentLocation, int intendedLevel)
        {
            int levelToReturn = 0;

            bool checkLocation(int triggerRoom)
            {
                if (currentLocation <= triggerRoom)
                {
                    return false;
                }

                else
                {
                    return true;
                }
            }

            bool levelValid = false;

            while (!levelValid)
            {
                switch (intendedLevel)
                {
                    case 0:
                        levelValid = true;
                        break;

                    case 1:
                        levelValid = checkLocation(26);
                        break;

                    case 2:
                        levelValid = checkLocation(41);
                        break;

                    case 3:
                        levelValid = checkLocation(49);
                        break;

                    case 4:
                        levelValid = checkLocation(54);
                        break;

                    case 5:
                    case 6:
                    case 7:
                        levelValid = checkLocation(66);
                        break;

                    case 8:
                        levelValid = checkLocation(81);
                        break;

                    case 9:
                        levelValid = checkLocation(101);
                        break;

                    case 10:
                        levelValid = checkLocation(115);
                        break;

                    case 11:
                        levelValid = checkLocation(129);
                        break;

                    case 12:
                        levelValid = checkLocation(132);
                        break;

                    case 13:
                        levelValid = checkLocation(165);
                        break;

                    case 14:
                        levelValid = checkLocation(177);
                        break;

                    case 15:
                    case 16:
                        levelValid = checkLocation(211);
                        break;
                }

                if (!levelValid)
                {
                    //We're still in the loop so we continue to decrement intended level until checkLocation returns true
                    intendedLevel = intendedLevel - 1;
                }

                else
                {
                    //checkLocation has returned true, so set the variable and break from the while loop
                    levelToReturn = intendedLevel;
                    break;
                }
            }

            return levelToReturn;
        }
    }
}



