using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Ini;

//The SAVE class is responsible for all manipulation, editing and creation of data stored within undertale.ini, the game's INI file.
//It often derives some data from file0 so an instance of a SAVEFile object is used here occasionally
namespace Underlauncher
{
    public class INI
    {
        //We create a child class of class INI, called INIFile which stores all variables and values of undertale.ini to be written
        public class INIFile
        {
            public bool defaultINI = true;

            public int timePlayed;
            public int deaths;
            public int FUN;
            public bool skipAsrielStory;

            public bool trueResetted;
            public bool doorUnlocked;

            public int timesMetFlowey;
            public int floweyChatProgress;

            public int piePreference;

            public int timesMetSans;
            public bool sansMetInJudgment;
            public int sansPasswordProgress;
            public int timesReachedMid;
            public int timesHeardIntro;
            public int timesFoughtSans;
            public bool killedSans;

            public int timesMetPapyrus;

            public bool mettatonSkip;

            public bool photoshopFight;
            public int fightStage;
            public bool skipFight;

            public bool beatNeutralRun;
            public bool canTrueReset;
            public bool barrierDestroyed;

            //We use the INIFile class constructor to initalize and set default values
            public INIFile()
            {
                timePlayed = 0;
                deaths = 0;
                FUN = 66;
                timesMetFlowey = 0;
                floweyChatProgress = 0;
                piePreference = 1;
                timesMetSans = 0;
                timesMetPapyrus = 0;
                fightStage = 0;
            }
        }

        //We create a static instance of an INIFile object staticINIFile to be associated to the current save file
        private static INIFile staticINIFile = new INIFile();

        //SetIntegerValue compares location values and returns the valueToSet if currentLocation is greater than triggerLocation, defaulting to 0 if false
        private static int SetIntegerValue(int valueToSet, int currentLocation, int triggerLocation)
        {
            if (currentLocation > triggerLocation)
            {
                return valueToSet;
            }

            else
            {
                return 0;
            }
        }

        //ConvertTimePlayedToFrames converts a minute based period of time to an amount of frames seen in game to be stored in the INI file
        private static string ConvertTimePlayedToFrames(int timePlayedMins)
        {
            return (((timePlayedMins) * 60) * 29.97).ToString();
        }

        //ConvertFramesToTimePlayed converts a number of frames seen in game from the INI file to a minute based time to be displayed within the UI
        private static int ConvertFramesToTimePlayed(string frames)
        {
            double doubleVal = 0;
            if (Double.TryParse(frames, out doubleVal))
            {
                return Convert.ToInt32((((doubleVal) / 60) / 29.97));
            }

            else
            {
                return 0;
            }
        }

        //ToININumber converts an int value to a string with the appropriate decimal places afterward to be written to the INI file
        private static string ToININumber(int value)
        {
            return value.ToString() + ".000000";
        }

        //ToINIBool converts a boolean value to its appropriate string value with decimal places to be written to the INI file
        private static string ToINIBool(bool boolean)
        {
            if (boolean)
            {
                return "1.000000";
            }

            else
            {
                return "0.000000";
            }
        }

        //ToINIDateState converts a date state to a string with its appropriate decimal places to be written to the INI file
        private static string ToINIDateState(DateStates dateState)
        {
            if ((int)dateState >= 1)
            {
                return "1.000000";
            }

            else
            {
                return "0.000000";
            }
        }

        //FromININumber converts an INI file type number from a string to an integer value
        private static int FromININumber(string ININumber)
        {
            double doubleVal = 0;
            if (Double.TryParse(ININumber, out doubleVal))
            {
                return Convert.ToInt16(Math.Floor(doubleVal));
            }

            else
            {
                return 0;
            }
        }

        //FromINIBool converts an INI file type number from a string to a boolean value
        private static bool FromINIBool(string INIBool)
        {
            double doubleVal;
            Double.TryParse(INIBool, out doubleVal);

            if (Convert.ToInt16(doubleVal) >= 1)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        //FromINICharacterState casts an INI file type number to a CharacterStates enum value
        private static CharacterStates FromINICharacterState(string INIState)
        {
            double doubleVal = 0;
            if (Double.TryParse(INIState, out doubleVal))
            {
                short intVal = Convert.ToInt16(Math.Floor(Convert.ToDouble(INIState)));
                return (CharacterStates)intVal;
            }

            else
            {
                return CharacterStates.Default;
            }
        }

        //FromINIDateState casts an INI file type number to a DateStates enum value
        private static DateStates FromINIDateState(string INIState)
        {
            double doubleVal = 0;
            if (Double.TryParse(INIState, out doubleVal))
            {
                short intVal = Convert.ToInt16(Math.Floor(Convert.ToDouble(INIState)));
                return (DateStates)intVal;
            }

            else
            {
                return DateStates.DateNotStarted;
            }
        }

        //setINI sets the staticINIFile to the INItoSet
        public static void setINI(INIFile INItoSet)
        {
            staticINIFile = INItoSet;
        }

        //GetINI returns the staticINIFile
        public static INIFile GetINI()
        {
            return staticINIFile;
        }

        //WriteINI writes, well, the INI.
        public static bool WriteINI(INIFile INItoWrite, SAVE.SAVEFile save)
        {
            try
            {
                var iniWriter = new Ini.IniFile(Constants.SavesPath + save.SaveName + "//undertale.ini");

                iniWriter.IniWriteValue("General", "Name", save.PlayerName);
                iniWriter.IniWriteValue("General", "Time", ConvertTimePlayedToFrames(INItoWrite.timePlayed));
                iniWriter.IniWriteValue("General", "Room", save.Location.ToString());
                iniWriter.IniWriteValue("General", "Gameover", INItoWrite.deaths.ToString());
                iniWriter.IniWriteValue("General", "Kills", save.KillCount.ToString());
                iniWriter.IniWriteValue("General", "Love", save.LOVE.ToString());
                iniWriter.IniWriteValue("General", "fun", INItoWrite.FUN.ToString());
                iniWriter.IniWriteValue("General", "Tale", ToINIBool(INItoWrite.skipAsrielStory));
                iniWriter.IniWriteValue("General", "Won", ToININumber(Convert.ToInt16(INItoWrite.beatNeutralRun) + Convert.ToInt16(INItoWrite.floweyChatProgress)));

                iniWriter.IniWriteValue("reset", "reset", ToINIBool(INItoWrite.trueResetted));
                iniWriter.IniWriteValue("reset", "s_key", ToINIBool(INItoWrite.doorUnlocked));

                iniWriter.IniWriteValue("Flowey", "Met1", ToININumber(INItoWrite.timesMetFlowey));

                if (save.floweyState == CharacterStates.Killed)
                {
                    iniWriter.IniWriteValue("Flowey", "IK", "1.000000");
                }

                else if (INItoWrite.beatNeutralRun)
                {
                    iniWriter.IniWriteValue("Flowey", "NK", "1.000000");
                }

                iniWriter.IniWriteValue("Flowey", "EX", ToININumber(INItoWrite.floweyChatProgress));

                iniWriter.IniWriteValue("Toriel", "Bscotch", ToININumber(INItoWrite.piePreference));

                if (save.torielState == CharacterStates.TorielKilled)
                {
                    iniWriter.IniWriteValue("Toriel", "TK", "1.000000");
                }

                else if (save.torielState == CharacterStates.Spared)
                {
                    iniWriter.IniWriteValue("Toriel", "TS", "1.000000");
                }

                else if (save.torielState == CharacterStates.Default)
                {
                    iniWriter.IniWriteValue("Toriel", "TS", "0.000000");
                }

                iniWriter.IniWriteValue("Sans", "M1", ToININumber(INItoWrite.timesMetSans));
                iniWriter.IniWriteValue("Sans", "EndMet", ToINIBool(INItoWrite.sansMetInJudgment));


                if (save.LOVE == 1)
                {
                    iniWriter.IniWriteValue("Sans", "MeetLv1", ToININumber(INItoWrite.timesMetSans));
                }

                else
                {
                    iniWriter.IniWriteValue("Sans", "MeetLv2", ToININumber(INItoWrite.timesMetSans));
                }

                iniWriter.IniWriteValue("Sans", "Pass", ToININumber(INItoWrite.sansPasswordProgress));
                iniWriter.IniWriteValue("Sans", "SK", ToINIBool(INItoWrite.killedSans));
                iniWriter.IniWriteValue("Sans", "MP", ToININumber(INItoWrite.timesReachedMid));
                iniWriter.IniWriteValue("Sans", "F", ToININumber(INItoWrite.timesFoughtSans));
                iniWriter.IniWriteValue("Sans", "Intro", ToININumber(INItoWrite.timesHeardIntro));

                iniWriter.IniWriteValue("Papyrus", "M1", ToININumber(INItoWrite.timesMetPapyrus));

                if (save.papyrusState == CharacterStates.Killed)
                {
                    iniWriter.IniWriteValue("Papyrus", "PK", "1.000000");
                }

                else if (save.papyrusState == CharacterStates.Spared)
                {
                    iniWriter.IniWriteValue("Papyrus", "PS", "1.000000");
                }

                else if (save.papyrusState == CharacterStates.Default)
                {
                    iniWriter.IniWriteValue("Papyrus", "PS", "0.000000");
                }

                iniWriter.IniWriteValue("Papyrus", "PD", ToINIDateState(save.papyrusDated));

                iniWriter.IniWriteValue("Undyne", "UD", ToINIDateState(save.undyneDated));

                iniWriter.IniWriteValue("Mettaton", "BossMet", ToINIBool(INItoWrite.mettatonSkip));

                iniWriter.IniWriteValue("Alphys", "AD", ToINIDateState(save.alphysDated));

                iniWriter.IniWriteValue("F7", "F7", ToINIBool(INItoWrite.barrierDestroyed));

                iniWriter.IniWriteValue("EndF", "EndF", ToINIBool(INItoWrite.canTrueReset));

                iniWriter.IniWriteValue("FFFFF", "F", ToINIBool(INItoWrite.photoshopFight));
                iniWriter.IniWriteValue("FFFFF", "P", ToININumber(INItoWrite.fightStage));
                iniWriter.IniWriteValue("FFFFF", "E", ToINIBool(INItoWrite.skipFight));

                return true;
            }

            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("An exception occurred when attempting to write the INI file. Cannot continue. The exception was: \n\n" + ex +
                                "\n\n Please notify me of this issue via reddit or Skype. ", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        //ReadINI reads, well, the INI.
        public static INIFile ReadINI(SAVE.SAVEFile save)
        {
            var iniReader = new Ini.IniFile(Constants.SavesPath + save.SaveName + "//undertale.ini");

            staticINIFile.defaultINI = false;

            staticINIFile.timePlayed = ConvertFramesToTimePlayed(iniReader.IniReadValue("General", "Time"));
            staticINIFile.deaths = FromININumber(iniReader.IniReadValue("General", "Gameover"));
            staticINIFile.FUN = FromININumber(iniReader.IniReadValue("General", "fun"));
            staticINIFile.skipAsrielStory = FromINIBool(iniReader.IniReadValue("General", "Tale"));
            staticINIFile.beatNeutralRun = FromINIBool(iniReader.IniReadValue("General", "Won"));

            staticINIFile.trueResetted = FromINIBool(iniReader.IniReadValue("reset", "reset"));
            staticINIFile.doorUnlocked = FromINIBool(iniReader.IniReadValue("reset", "s_key"));

            staticINIFile.timesMetFlowey = FromININumber(iniReader.IniReadValue("Flowey", "Met1"));

            if (FromINIBool(iniReader.IniReadValue("Flowey", "IK")))
            {
                save.floweyState = CharacterStates.Killed;
            }

            else
            {
                save.floweyState = CharacterStates.Default;
            }

            staticINIFile.floweyChatProgress = FromININumber(iniReader.IniReadValue("Flowey", "EX"));

            staticINIFile.piePreference = FromININumber(iniReader.IniReadValue("Toriel", "Bscotch"));

            if (FromINIBool(iniReader.IniReadValue("Toriel", "TK")))
            {
                save.torielState = CharacterStates.Killed;
            }

            else
            {
                save.torielState = CharacterStates.Default;
            }

            staticINIFile.timesMetSans = FromININumber(iniReader.IniReadValue("Sans", "M1"));
            staticINIFile.sansMetInJudgment = FromINIBool(iniReader.IniReadValue("Sans", "EndMet"));
            staticINIFile.sansPasswordProgress = FromININumber(iniReader.IniReadValue("Sans", "Pass"));
            staticINIFile.killedSans = FromINIBool(iniReader.IniReadValue("Sans", "SK"));
            staticINIFile.timesReachedMid = FromININumber(iniReader.IniReadValue("Sans", "MP"));
            staticINIFile.timesFoughtSans = FromININumber(iniReader.IniReadValue("Sans", "F"));
            staticINIFile.timesHeardIntro = FromININumber(iniReader.IniReadValue("Sans", "Intro"));

            staticINIFile.timesMetPapyrus = FromININumber(iniReader.IniReadValue("Papyrus", "M1"));
            save.papyrusState = FromINICharacterState(iniReader.IniReadValue("Papyrus", "PK"));
            save.papyrusDated = FromINIDateState(iniReader.IniReadValue("Papyrus", "PD"));

            save.undyneDated = FromINIDateState(iniReader.IniReadValue("Undyne", "UD"));

            staticINIFile.mettatonSkip = FromINIBool(iniReader.IniReadValue("Mettaton", "BossMet"));

            save.alphysDated = FromINIDateState(iniReader.IniReadValue("Alphys", "AD"));

            staticINIFile.fightStage = FromININumber(iniReader.IniReadValue("FFFFF", "P"));
            staticINIFile.skipFight = FromINIBool(iniReader.IniReadValue("FFFFF", "E"));

            staticINIFile.barrierDestroyed = FromINIBool(iniReader.IniReadValue("F7", "F7"));
            staticINIFile.canTrueReset = FromINIBool(iniReader.IniReadValue("EndF", "EndF"));

            return staticINIFile;
        }

        //SetINIForRoute sets the appropriate INI file values for the player's route and location
        public static INIFile SetINIForRoute(int locationID, Routes.GameRoutes routeToSet)
        {
            if (routeToSet >= Routes.GameRoutes.TruePacifistDate)
            {
                staticINIFile.skipAsrielStory = true;
                staticINIFile.floweyChatProgress = 1;
                staticINIFile.mettatonSkip = true;
                staticINIFile.fightStage = 7;

                if (routeToSet == Routes.GameRoutes.TruePacifistEpilogue)
                {
                    staticINIFile.barrierDestroyed = true;
                    staticINIFile.canTrueReset = true; 
                }
            }

            else if (routeToSet == Routes.GameRoutes.Genocide)
            {
                staticINIFile.killedSans = MiscFunctions.SetBooleanValueFromLocation(locationID, 231);
                staticINIFile.timesReachedMid = SetIntegerValue(1, locationID, 231);
                staticINIFile.timesFoughtSans = SetIntegerValue(1, locationID, 231);
                staticINIFile.timesHeardIntro = SetIntegerValue(1, locationID, 231);
            }

            staticINIFile.timesMetFlowey = SetIntegerValue(1, locationID, 5);
            staticINIFile.timesMetSans = SetIntegerValue(1, locationID, 45);
            staticINIFile.timesMetPapyrus = SetIntegerValue(1, locationID, 45);
            staticINIFile.mettatonSkip = Convert.ToBoolean(SetIntegerValue(1, locationID, 211));
            staticINIFile.sansMetInJudgment = Convert.ToBoolean(SetIntegerValue(1, locationID, 231));

            return staticINIFile;
        }
    }
}
