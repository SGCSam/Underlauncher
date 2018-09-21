using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Collections.Specialized;

using MessageBox = System.Windows.MessageBox;
using System.Collections;

//The SAVE class is responsible for all manipulation, editing and creation of data stored within file0, the game's save file.
namespace Underlauncher
{
    public class SAVE
    {
        private static SAVEFile SAVEInstance;

        //We create a child class of class SAVE, called SAVEFile which stores all variables and values of file0 to be written
        public class SAVEFile
        {
            public string PlayerName;
            public int LOVE;
            public int HP;
            public int friskAT;
            public int weaponAT;
            public int friskDF;
            public int armorDF;
            public int Location;
            public int EXP;
            public int GOLD;
            public int KillCount;
            public int[] inventoryItems = new int[8];
            public int[] phoneItems = new int[4];
            public int Weapon;
            public int Armor;

            private CharacterStates ruinsDummyState;
            public CharacterStates torielState;
            private int ruinsKills;

            private CharacterStates doggoState;
            private CharacterStates dogamyState;
            private CharacterStates greaterDogState;
            private CharacterStates comedianState;
            public CharacterStates papyrusState;
            private int snowdinKills;

            private CharacterStates shyrenState;
            private CharacterStates undyneState;
            private CharacterStates madDummyState;
            private int waterfallKills;

            private bool undyneOnPhone;

            private CharacterStates muffetState;
            private CharacterStates broGuardsState;
            private CharacterStates mettatonState;
            private int hotlandKills;
            private bool disableAlphysCalls;
            private bool disableHotlandPuzzles;
            private bool coreElevatorUnlocked;

            private bool dateAlphysFlag;
            private bool beatTrueLab;

            private bool rodeCastleLift;
            private bool unlockedCastleChain;
            private bool insideCastle;

            public CharacterStates floweyState;

            public DateStates papyrusDated;
            public DateStates undyneDated;
            public DateStates alphysDated;

            private double Plot;
            private int pacifistStage;
            private bool asrielDefeated;
            private bool randomCountersDisabled;
            public string SaveName;

            //readSaveFile is responsible for reading file0 and converting or casting its string values to our variable data types
            //We can then use this to setup the editor UI as well as edit data for this specific save
            private static SAVEFile readSaveFile(string saveFileDirectory)
            {
                string ReadLine(int lineNumber)
                {
                    return File.ReadLines(saveFileDirectory + "//file0").Skip(lineNumber - 1).Take(1).First().Replace(" ", String.Empty);
                }

                SAVEFile currentSave = new SAVEFile();

                //We read the line passed into ReadLine as a parameter and set the variable to the value read, as well as casting/converting as needed
                currentSave.PlayerName = ReadLine(1);
                currentSave.LOVE = Convert.ToInt16(ReadLine(2));
                currentSave.HP = Convert.ToInt16(ReadLine(3));
                currentSave.friskAT = Convert.ToInt16(ReadLine(5));
                currentSave.weaponAT = Convert.ToInt16(ReadLine(6));
                currentSave.friskDF = Convert.ToInt16(ReadLine(7));
                currentSave.armorDF = Convert.ToInt16(ReadLine(8));
                currentSave.EXP = Convert.ToInt32(ReadLine(10));
                currentSave.GOLD = Convert.ToInt16(ReadLine(11));
                currentSave.KillCount = Convert.ToInt16(ReadLine(12));
                currentSave.inventoryItems[0] = Convert.ToInt16(ReadLine(13));
                currentSave.phoneItems[0] = Convert.ToInt16(ReadLine(14));
                currentSave.inventoryItems[1] = Convert.ToInt16(ReadLine(15));
                currentSave.phoneItems[1] = Convert.ToInt16(ReadLine(16));
                currentSave.inventoryItems[2] = Convert.ToInt16(ReadLine(17));
                currentSave.phoneItems[2] = Convert.ToInt16(ReadLine(18));
                currentSave.inventoryItems[3] = Convert.ToInt16(ReadLine(19));
                currentSave.phoneItems[3] = Convert.ToInt16(ReadLine(20));
                currentSave.inventoryItems[4] = Convert.ToInt16(ReadLine(21));
                currentSave.inventoryItems[5] = Convert.ToInt16(ReadLine(23));
                currentSave.inventoryItems[6] = Convert.ToInt16(ReadLine(25));
                currentSave.inventoryItems[7] = Convert.ToInt16(ReadLine(27));
                currentSave.Weapon = IDs.GetID(ReadLine(29));
                currentSave.Armor = IDs.GetID(ReadLine(30));
                currentSave.asrielDefeated = Convert.ToBoolean(Convert.ToInt16(ReadLine(38)));
                currentSave.randomCountersDisabled = Convert.ToBoolean(Convert.ToInt16(ReadLine(39)));
                currentSave.ruinsDummyState = (CharacterStates)(Convert.ToInt16(ReadLine(45)));
                currentSave.torielState = (CharacterStates)(Convert.ToInt16(ReadLine(76)));
                currentSave.doggoState = (CharacterStates)(Convert.ToInt16(ReadLine(83)));
                currentSave.dogamyState = (CharacterStates)(Convert.ToInt16(ReadLine(84)));
                currentSave.greaterDogState = (CharacterStates)(Convert.ToInt16(ReadLine(85)));
                currentSave.comedianState = (CharacterStates)(Convert.ToInt16(ReadLine(88)));
                currentSave.papyrusState = (CharacterStates)(Convert.ToInt16(ReadLine(98)));
                currentSave.shyrenState = (CharacterStates)(Convert.ToInt16(ReadLine(112)));
                currentSave.papyrusDated = (DateStates)(Convert.ToInt16(ReadLine(119)));
                currentSave.KillCount = Convert.ToInt16(ReadLine(232));
                currentSave.ruinsKills = Convert.ToInt16(ReadLine(233));
                currentSave.snowdinKills = Convert.ToInt16(ReadLine(234));
                currentSave.waterfallKills = Convert.ToInt16(ReadLine(235));
                currentSave.hotlandKills = Convert.ToInt16(ReadLine(236));
                currentSave.undyneState = (CharacterStates)(Convert.ToInt16(ReadLine(282)));
                currentSave.madDummyState = (CharacterStates)(Convert.ToInt16(ReadLine(283)));
                currentSave.undyneState = (CharacterStates)(Convert.ToInt16(ReadLine(381)));
                currentSave.undyneDated = (DateStates)(Convert.ToInt16(ReadLine(420)));
                currentSave.disableAlphysCalls = Convert.ToBoolean(Convert.ToInt16(ReadLine(399)));
                currentSave.muffetState = (CharacterStates)(Convert.ToInt16(ReadLine(428)));
                currentSave.disableHotlandPuzzles = Convert.ToBoolean(Convert.ToInt16(ReadLine(431)));
                currentSave.broGuardsState = (CharacterStates)(Convert.ToInt16(ReadLine(433)));
                currentSave.coreElevatorUnlocked = Convert.ToBoolean(Convert.ToInt16(449));
                currentSave.mettatonState = (CharacterStates)(Convert.ToInt16(ReadLine(456)));
                currentSave.insideCastle = Convert.ToBoolean(Convert.ToInt16(ReadLine(462)));
                currentSave.rodeCastleLift = Convert.ToBoolean(Convert.ToInt16(ReadLine(463)));
                currentSave.unlockedCastleChain = Convert.ToBoolean(Convert.ToInt16(ReadLine(485)));
                currentSave.undyneOnPhone = Convert.ToBoolean(Convert.ToInt16(ReadLine(496)));
                currentSave.beatTrueLab = Convert.ToBoolean(Convert.ToInt16(ReadLine(522)));
                currentSave.dateAlphysFlag = Convert.ToBoolean(Convert.ToInt16(ReadLine(523)));
                currentSave.pacifistStage = Convert.ToInt16(ReadLine(524));
                currentSave.Plot = Convert.ToInt16(ReadLine(543));
                currentSave.Location = Convert.ToInt16(ReadLine(548));
                currentSave.SaveName = new DirectoryInfo(saveFileDirectory).Name;

                return currentSave;
            }

            //LoadSaveFile is the public function we can call from other classes and scopes to read file0 at the location of the saveFileDirectory parameter
            public static SAVEFile LoadSaveFile(string saveFileDirectory)
            {
                SAVEFile saveFile = readSaveFile(saveFileDirectory);
                return saveFile;
            }

            //GetClosestEnding checks variables within the SAVEFile and returns the route that most closely matches the SAVEFile content
            public static Routes.GameRoutes GetClosestRoute(SAVEFile actualSave)
            {
                Dictionary<Routes.GameRoutes, int> likenessDict = new Dictionary<Routes.GameRoutes, int>();

                int likeness = 0;

                if (actualSave.pacifistStage == 2)
                {
                    return Routes.GameRoutes.TruePacifistDate;
                }

                else if (actualSave.pacifistStage == 11)
                {
                    return Routes.GameRoutes.TruePacifistLab;
                }

                else if (actualSave.pacifistStage == 12)
                {
                    if (actualSave.Location == 331)
                    {
                        return Routes.GameRoutes.TruePacifistAsrielTalk;
                    }

                    else if (actualSave.asrielDefeated)
                    {
                        return Routes.GameRoutes.TruePacifistEpilogue;
                    }

                    else
                    {
                        return Routes.GameRoutes.TruePacifistAsriel;
                    }
                }

                else if (actualSave.disableAlphysCalls && actualSave.disableHotlandPuzzles)
                {
                    return Routes.GameRoutes.Genocide;
                }

                else
                {
                    Routes.GameRoutes[] routesToIgnore = new Routes.GameRoutes[] { Routes.GameRoutes.TruePacifistDate, Routes.GameRoutes.TruePacifistLab, Routes.GameRoutes.TruePacifistAsriel, Routes.GameRoutes.TruePacifistAsrielTalk, Routes.GameRoutes.TruePacifistEpilogue };
                    foreach (Routes.GameRoutes route in Enum.GetValues(typeof(Routes.GameRoutes)))
                    {
                        if (!routesToIgnore.Contains(route))
                        {
                            SAVEFile compareSave = new SAVEFile();
                            compareSave = SetSaveForRoute(compareSave, actualSave.Location, route);

                            foreach (var actualCharacterState in compareSave.GetType().GetProperties())
                            {
                                foreach (CharacterStates stateToCheck in Enum.GetValues(typeof(CharacterStates)))
                                {
                                    if ((CharacterStates)actualCharacterState.GetValue(actualCharacterState) == stateToCheck)
                                    {
                                        likeness++;
                                    }
                                }
                            }

                            foreach (var actualDateState in compareSave.GetType().GetProperties())
                            {
                                foreach (DateStates stateToCheck in Enum.GetValues(typeof(DateStates)))
                                {
                                    if ((DateStates)actualDateState.GetValue(actualDateState) == stateToCheck)
                                    {
                                        likeness++;
                                    }
                                }
                            }

                            if (compareSave.KillCount <= actualSave.KillCount)
                            {
                                likeness++;
                            }

                            if (Routes.GetMurderLevelForLocation(compareSave.Location, 16) == Routes.GetMurderLevelForLocation(actualSave.Location, 16))
                            {
                                likeness++;
                            }

                            likenessDict.Add(route, likeness);
                            likeness = 0;
                        }
                    }
                }

                return likenessDict.Where(key => key.Value == likenessDict.Max(value => value.Value)).Select(key => key.Key).FirstOrDefault();

            }

            //AskToWrite is called when we want to write our SAVEFile classes's variables to file0. It checks if the directory to write to 
            //already exists and asks the user if they wish to overwrite. It then calls WriteSaveFile and WriteINI with the relevant arguments passed
            public static bool AskToWrite(SAVEFile thisSave, INI.INIFile thisINI)
            {
                if (!Directory.Exists(Constants.SavesPath + thisSave.SaveName))
                {
                    Directory.CreateDirectory(Constants.SavesPath + thisSave.SaveName);

                    if (!WriteSaveFile(thisSave, thisINI) || !INI.WriteINI(thisINI, thisSave))
                    {
                        return false;
                    }

                    return true;
                }

                else
                {
                    Dictionary<Characters, string> messageDict = new Dictionary<Characters, string>()
                    {
                        {Characters.Alphys, "Hey! So, the save \"" + thisSave.SaveName + "\" already exists. Would you like me to overwrite it?" },
                        {Characters.Asgore, "Well human, the save \"" + thisSave.SaveName + "\" already exists. Shall I overwrite it?" },
                        {Characters.Asriel, "Sorry, but the save \"" + thisSave.SaveName + "\" already exists. Do you want me to overwrite it? It's no trouble, really!" },
                        {Characters.Flowey, "YOU. IDIOT! The save \"" + thisSave.SaveName + "\" already exists. Of course, I've got to do all the work - do you want me to overwrite it?" },
                        {Characters.Papyrus, "NYEH, SORRY HUMAN. THE SAVE \"" + thisSave.SaveName + "\" ALREADY EXISTS. WOULD YOU LIKE ME TO ASSIST YOU AND OVERWRITE IT?" },
                        {Characters.Sans, "yo, buddy. the save \"" + thisSave.SaveName + "\" already exists. i'm normally quite lazy, but i'll make an exception for ya. want me to overwrite it?" },
                        {Characters.Toriel, "My child, I apologize, but the save \"" + thisSave.SaveName + "\" already exists. Would you like me to overwrite it for you?" },
                        {Characters.Undyne, "OH COME ON.  The save \"" + thisSave.SaveName + "\" already exists. Want me to overwrite it?" },
                        {Characters.None, "Save \"" + thisSave.SaveName + "\" already exists! Do you want to overwrite?" }
                    };

                    MessageBoxResult res = UTMessageBox.Show(messageDict, Constants.CharacterReactions.Normal, MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.Yes)
                    {
                        if (!WriteSaveFile(thisSave, thisINI) || !INI.WriteINI(thisINI, thisSave))
                        {
                            return false;
                        }

                        return true;
                    }

                    return false;
                }
            }

            //WriteSaveFile writes, well, the save file. 
            public static bool WriteSaveFile(SAVEFile fileToWrite, INI.INIFile ini)
            {
                //SetFlagFromBool converts a boolean variable or value to a string to be written as a flag in file0
                string SetFlagFromBool(bool booleanToCheck, int valueToSet = 1)
                {
                    if (booleanToCheck)
                    {
                        return valueToSet.ToString();
                    }

                    else
                    {
                        return "0";
                    }
                }

                //We use an ordered dictionary with the key as the line number in file0 to write the value to
                OrderedDictionary file0Contents = new OrderedDictionary
                {
                    {1, fileToWrite.PlayerName},
                    {2, fileToWrite.LOVE.ToString()},
                    {3, fileToWrite.HP.ToString()},
                    {4, "0"},
                    {5, fileToWrite.friskAT.ToString()},
                    {6, fileToWrite.weaponAT.ToString()},
                    {7, fileToWrite.friskDF.ToString()},
                    {8, fileToWrite.armorDF.ToString()},
                    {9, "4" },
                    {10, fileToWrite.EXP.ToString()},
                    {11, fileToWrite.GOLD.ToString()},
                    {12, fileToWrite.KillCount.ToString()},
                    {13, fileToWrite.inventoryItems[0].ToString()},
                    {14, fileToWrite.phoneItems[0].ToString()},
                    {15, fileToWrite.inventoryItems[1].ToString()},
                    {16, fileToWrite.phoneItems[1].ToString()},
                    {17, fileToWrite.inventoryItems[2].ToString()},
                    {18, fileToWrite.phoneItems[2].ToString()},
                    {19, fileToWrite.inventoryItems[3].ToString()},
                    {20, fileToWrite.phoneItems[3].ToString()},
                    {21, fileToWrite.inventoryItems[4].ToString()},
                    {22, "0" },
                    {23, fileToWrite.inventoryItems[5].ToString()},
                    {24, "0" },
                    {25, fileToWrite.inventoryItems[6].ToString()},
                    {26, "0" },
                    {27, fileToWrite.inventoryItems[7].ToString()},
                    {28, "0" },
                    {29, fileToWrite.Weapon},
                    {30, fileToWrite.Armor},
                    {38, Convert.ToInt16(fileToWrite.asrielDefeated)},
                    {39, Convert.ToInt16(fileToWrite.randomCountersDisabled)},
                    {45, (int)fileToWrite.ruinsDummyState},
                    {76, (int)fileToWrite.torielState},
                    {83, (int)fileToWrite.doggoState},
                    {84, (int)fileToWrite.dogamyState},
                    {85, (int)fileToWrite.greaterDogState},
                    {88, (int)fileToWrite.comedianState},
                    {98, (int)fileToWrite.papyrusState},
                    {112, (int)fileToWrite.shyrenState},
                    {119, (int)fileToWrite.papyrusDated},
                    {232, fileToWrite.KillCount},
                    {233, fileToWrite.ruinsKills},
                    {234, fileToWrite.snowdinKills},
                    {235, fileToWrite.waterfallKills},
                    {236, fileToWrite.hotlandKills},
                    {282, (int)fileToWrite.undyneState},
                    {283, (int)fileToWrite.madDummyState},
                    {381, (int)fileToWrite.undyneState},
                    {398, Convert.ToInt16(fileToWrite.disableAlphysCalls)},
                    {399, Convert.ToInt16(fileToWrite.disableAlphysCalls)},
                    {402, Convert.ToInt16(fileToWrite.disableHotlandPuzzles)},
                    {404, Convert.ToInt16(fileToWrite.disableHotlandPuzzles)},
                    {405, Convert.ToInt16(fileToWrite.disableHotlandPuzzles)},
                    {406, Convert.ToInt16(fileToWrite.disableHotlandPuzzles)},
                    {420, (int)fileToWrite.undyneDated},
                    {428, (int)fileToWrite.muffetState},
                    {430, Convert.ToInt16(fileToWrite.disableHotlandPuzzles)},
                    {431, Convert.ToInt16(fileToWrite.disableHotlandPuzzles)},
                    {433, (int)fileToWrite.broGuardsState},
                    {449, SetFlagFromBool(fileToWrite.coreElevatorUnlocked) },
                    {456, (int)fileToWrite.mettatonState},
                    {462, SetFlagFromBool(fileToWrite.insideCastle)},
                    {463, SetFlagFromBool(fileToWrite.rodeCastleLift)},
                    {481, SetFlagFromBool(ini.skipAsrielStory, 17)},
                    {483, SetFlagFromBool(fileToWrite.unlockedCastleChain)},
                    {484, SetFlagFromBool(fileToWrite.unlockedCastleChain)},
                    {485, SetFlagFromBool(fileToWrite.unlockedCastleChain)},
                    {486, SetFlagFromBool(ini.skipAsrielStory)},
                    {487, SetFlagFromBool(ini.skipAsrielStory, 2)},
                    {496, Convert.ToInt16(fileToWrite.undyneOnPhone)},
                    {510, SetFlagFromBool(fileToWrite.asrielDefeated, 2)},
                    {512, SetFlagFromBool(fileToWrite.beatTrueLab, 3)},
                    {513, SetFlagFromBool(fileToWrite.beatTrueLab, 3)},
                    {514, SetFlagFromBool(fileToWrite.beatTrueLab, 3)},
                    {515, SetFlagFromBool(fileToWrite.beatTrueLab, 3)},
                    {516, SetFlagFromBool(fileToWrite.beatTrueLab)},
                    {517, SetFlagFromBool(fileToWrite.beatTrueLab, 10)},
                    {518, SetFlagFromBool(fileToWrite.beatTrueLab, 0)},
                    {519, SetFlagFromBool(fileToWrite.beatTrueLab)},
                    {520, SetFlagFromBool(fileToWrite.beatTrueLab)},
                    {521, SetFlagFromBool(fileToWrite.beatTrueLab)},
                    {522, SetFlagFromBool(fileToWrite.beatTrueLab)},
                    {523, SetFlagFromBool(fileToWrite.dateAlphysFlag)},
                    {524, fileToWrite.pacifistStage},
                    {543, fileToWrite.Plot },
                    {544, "1" },
                    {545, "1" },
                    {546, SetFlagFromBool(MiscFunctions.SetBooleanValueFromLocation(fileToWrite.Location, 12))}, //Cell Phone
                    {547, "0" },
                    {548, fileToWrite.Location.ToString()},
                    {549, "0"}
                };

                if (fileToWrite.Location == 998)
                {
                    fileToWrite.Location = 235;
                    FileOperations.setGenocide(GenocideStates.Abyss);
                }

                else if (fileToWrite.Location == 999)
                {
                    fileToWrite.Location = 237;
                }

                try
                {
                    StreamWriter file0Writer = new StreamWriter(File.Open(Constants.SavesPath + fileToWrite.SaveName + "//file0", FileMode.Create), Encoding.ASCII);

                    //We iterate through every line and key in file0Contents and if the key value matches lineNumber, we write the corresponding value
                    for (int lineNumber = 1; lineNumber <= 549; lineNumber++)
                    {
                        foreach (DictionaryEntry file0Entry in file0Contents)
                        {
                            if ((int)file0Entry.Key == lineNumber)
                            {
                                file0Writer.WriteLine(file0Entry.Value.ToString());
                                break;
                            }

                            else if ((int)file0Entry.Key > lineNumber)
                            {
                                file0Writer.WriteLine("0");
                                break;
                            }
                        }
                    }

                    file0Writer.Close();

                    if (fileToWrite.Location >= 238 || fileToWrite.Plot >= 208)
                    {
                        File.Copy(Constants.SavesPath + fileToWrite.SaveName + "//file0", Constants.SavesPath + fileToWrite.SaveName + "//file8", true);
                    }
                    return true;
                }

                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("An exception occurred when writing the save file. Cannot continue. The exception was: \n\n" + ex, "Error!",
                                     MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            //SetCharacterStates sets the corresponding character to the state passed in. Consult IDs.cs to see the CharactersStates enum
            private static void SetCharacterStates(int currentLocation, CharacterStates stateToSet, params Characters[] charactersList)
            {
                foreach (var character in charactersList)
                {
                    if (character == Characters.All)
                    {
                        //We use recursion here to set the state passed in to every character in the Charaters enum
                        foreach (Characters enumVal in Enum.GetValues(typeof(Characters)))
                        {
                            if (enumVal != Characters.All)
                            {
                                SetCharacterStates(currentLocation, CharacterStates.Spared, enumVal);
                            }
                        }
                    }

                    //We call CheckRoomForState to see if the character's state would be invalid at the player's currentLocation
                    //i.e. Papyrus cannot possibly be killed if we're still in the Ruins, so if that's the case, we set the stateToSet to Default
                    else if (!Routes.CheckRoomForState(currentLocation, character))
                    {
                        stateToSet = CharacterStates.Default;
                    }

                    switch (character)
                    {
                        case Characters.Papyrus:
                            SAVEInstance.papyrusState = stateToSet;
                            break;

                        case Characters.Undyne:
                            SAVEInstance.undyneState = stateToSet;
                            break;

                        case Characters.Mettaton:
                            SAVEInstance.mettatonState = stateToSet;
                            break;

                        case Characters.Toriel:
                            SAVEInstance.torielState = stateToSet;

                            if (stateToSet == CharacterStates.Killed)
                            {
                                SAVEInstance.torielState = CharacterStates.TorielKilled;
                            }
                            break;

                        case Characters.RuinsDummy:
                            SAVEInstance.ruinsDummyState = stateToSet;
                            break;

                        case Characters.Doggo:
                            SAVEInstance.doggoState = stateToSet;
                            break;

                        case Characters.Dogamy:
                            SAVEInstance.dogamyState = stateToSet;
                            break;

                        case Characters.GreaterDog:
                            SAVEInstance.greaterDogState = stateToSet;
                            break;

                        case Characters.Comedian:
                            SAVEInstance.comedianState = stateToSet;

                            if (stateToSet == CharacterStates.Killed)
                            {
                                SAVEInstance.comedianState = CharacterStates.ComedianKilled;
                            }
                            break;

                        case Characters.Shyren:
                            SAVEInstance.shyrenState = stateToSet;
                            break;

                        case Characters.MadDummy:
                            SAVEInstance.madDummyState = stateToSet;
                            break;

                        case Characters.Muffet:
                            SAVEInstance.muffetState = stateToSet;
                            break;

                        case Characters.BroGuards:
                            SAVEInstance.broGuardsState = stateToSet;
                            break;
                    }
                }
            }

            //SetDateValue sets the corresponding character to the date state passed in. Consult IDs.cs to see the DateStates enum
            private static void SetDateValue(int currentLocation, Characters character, DateStates stateToSet, Routes.GameRoutes route = Routes.GameRoutes.Family)
            {
                //We call CheckRoomForDate to see if the character's date state would be invalid at the player's currentLocation
                //i.e. Undyne cannot possibly be dated if we're still in Snowdin, so if that's the case, we set the stateToSet to DateNotStarted
                if (!Routes.CheckRoomForDate(currentLocation, character, route))
                {
                    stateToSet = DateStates.DateNotStarted;
                }

                switch (character)
                {
                    case Characters.Papyrus:
                        SAVEInstance.papyrusDated = stateToSet;
                        break;

                    case Characters.Undyne:
                        SAVEInstance.undyneDated = stateToSet;
                        break;
                }
            }

            //SetLOVE sets the player's LOVE value based upon their Murder Level. This is only used when a Genocide run is selected
            private static int SetLOVE(int murderLevel)
            {
                switch (murderLevel)
                {
                    case int i when (i >= 15):
                        return 19;

                    case int i when (i >= 14):
                        return 13;

                    case int i when (i >= 12):
                        return 12;

                    case int i when (i >= 11):
                        return 10;

                    case int i when (i >= 9):
                        return 9;

                    case int i when (i >= 3):
                        return 8;

                    case int i when (i >= 2):
                        return 6;

                    case int i when (i >= 1):
                        return 4;

                    default:
                        return 1;
                }
            }

            //clearStates is used when we load a new save file to set all variable values back to their defaults
            private static void clearStates(int currentLocation)
            {
                //It is critical we do this, otherwise if we change locations to a lower murder level, the higher level values will still be set
                //So here we clear them
                SetCharacterStates(999, CharacterStates.Default, Characters.All);
                SetDateValue(999, Characters.Papyrus, DateStates.DateNotStarted);
                SetDateValue(999, Characters.Undyne, DateStates.DateNotStarted);
                SetDateValue(999, Characters.Alphys, DateStates.DateNotStarted);
                SAVEInstance.hotlandKills = 0;
                SAVEInstance.waterfallKills = 0;
                SAVEInstance.snowdinKills = 0;
                SAVEInstance.ruinsKills = 0;
                SAVEInstance.LOVE = 1;
                SAVEInstance.EXP = 0;
                SAVEInstance.asrielDefeated = false;
                SAVEInstance.disableAlphysCalls = false;
                SAVEInstance.disableHotlandPuzzles = false;
                SAVEInstance.KillCount = 0;
                SAVEInstance.Plot = 0;
                SAVEInstance.undyneOnPhone = false;
                SAVEInstance.dateAlphysFlag = false;
                SAVEInstance.pacifistStage = 0;
            }

            //SetMurderLevel sets the appropriate levelToSet for the currentLocation in Genocide runs
            private static void SetMurderLevel(int currentLocation, int levelToSet)
            {
                clearStates(currentLocation);

                //We call GetMurderLevelForLocation to get the highest possible levelToSet for the currentLocation
                //i.e. We can't possibly have a Murder Level of 15 where we kill Mettaton and exhaust the Hotland Kills Counter if we're still in Waterfall
                levelToSet = Routes.GetMurderLevelForLocation(currentLocation, levelToSet);
                SAVEInstance.LOVE = SetLOVE(levelToSet);

                //We set the appropriate values required for the murder level
                //The reason goto is used here is so we set all the previous values to be true as well, so if we have a level of 14 where Muffet is killed
                    //then that means the BroGuards must also be killed as well as Undyne, Mad Dummy, etc...
                    //This is also because C# does not support case fall-through, but goto isn't considered harmful in this specific usage
                switch (levelToSet)
                {
                    case 16:
                    case 15:
                        SetCharacterStates(currentLocation, CharacterStates.Killed, Characters.Mettaton);
                        SAVEInstance.hotlandKills = 40;
                        goto case 14;

                    case 14:
                        SetCharacterStates(currentLocation, CharacterStates.Killed, Characters.Muffet);
                        goto case 13;

                    case 13:
                        SetCharacterStates(currentLocation, CharacterStates.Killed, Characters.BroGuards);
                        goto case 12;

                    case 12:
                        SetCharacterStates(currentLocation, CharacterStates.Killed, Characters.Undyne);
                        goto case 11;

                    case 11:
                        SetCharacterStates(currentLocation, CharacterStates.Killed, Characters.Undyne);
                        SAVEInstance.waterfallKills = 18;
                        goto case 10;

                    case 10:
                        SetCharacterStates(currentLocation, CharacterStates.Killed, Characters.MadDummy);
                        goto case 9;

                    case 9:
                        SetCharacterStates(currentLocation, CharacterStates.Killed, Characters.Shyren);
                        goto case 8;

                    case 8:
                        SetCharacterStates(currentLocation, CharacterStates.Killed, Characters.Papyrus);
                        goto case 7;

                    case 7:
                        SAVEInstance.snowdinKills = 16;
                        goto case 6;

                    case 6:
                        SetCharacterStates(currentLocation, CharacterStates.Killed, Characters.Comedian);
                        goto case 5;

                    case 5:
                        SetCharacterStates(currentLocation, CharacterStates.Killed, Characters.GreaterDog);
                        goto case 4;

                    case 4:
                        SetCharacterStates(currentLocation, CharacterStates.Killed, Characters.Dogamy);
                        goto case 3;

                    case 3:
                        SetCharacterStates(currentLocation, CharacterStates.Killed, Characters.Doggo);
                        goto case 2;

                    case 2:
                        SetCharacterStates(currentLocation, CharacterStates.Killed, Characters.Toriel);
                        goto case 1;

                    case 1:
                        SAVEInstance.ruinsKills = 20;
                        break;
                }

                SAVEInstance.KillCount = SAVEInstance.ruinsKills + SAVEInstance.snowdinKills + SAVEInstance.waterfallKills + SAVEInstance.hotlandKills;
            }

            //SetPlotValue sets the appropriate plot value based on the player's route and their currentLocation
            private static void SetPlotValue(Routes.GameRoutes route, int currentLocation)
            {
                if (route >= Routes.GameRoutes.TruePacifistDate && route < Routes.GameRoutes.TruePacifistEpilogue)
                {
                    SAVEInstance.Plot = 208;
                }

                else if (route == Routes.GameRoutes.TruePacifistEpilogue)
                {
                    SAVEInstance.Plot = 999;
                }

                else
                {
                    SAVEInstance.Plot = IDs.Locations.GetPlotForRoom(currentLocation, route);
                }
            }

            //SetPhoneItems sets the appropriate phone items for the player's currentLocation and their currentRoute
            //This can be overridden by the user if they change the CheckBoxes manually in the Editor UI
            private static void SetPhoneItems(int currentLocation, Routes.GameRoutes currentRoute)
            {
                int i = 0;
                Array.Clear(SAVEInstance.phoneItems, 0, SAVEInstance.phoneItems.Length);
                if (currentLocation > 11)
                {
                    //We are past the Tension room so we add Toriel's number
                    SAVEInstance.phoneItems[i] = 206;
                    i++;

                    if ((currentLocation > 142 && currentRoute != Routes.GameRoutes.QueenAlphys && currentRoute != Routes.GameRoutes.Genocide) || currentRoute >= Routes.GameRoutes.TruePacifistDate)
                    {
                        //We've passed the Hotland Lab on an appropriate route so we add the Dimensional Boxes
                        SAVEInstance.phoneItems[i] = 220;
                        i++;
                        SAVEInstance.phoneItems[i] = 221;
                        i++;
                    }
                }

                if ((SAVEInstance.papyrusDated == DateStates.PapyrusComplete && SAVEInstance.papyrusState != CharacterStates.Killed) || currentRoute >= Routes.GameRoutes.TruePacifistDate)
                {
                    //Papyrus isn't dead and has been dated so we add his number
                    SAVEInstance.phoneItems[i] = 210;
                }
            }

            //SetSaveForRoute sets the variable values needed for each route's requirements to be met up to the player's currentLocation
            public static SAVEFile SetSaveForRoute(SAVEFile save, int locationID, Routes.GameRoutes routeToSet)
            {
                //It is key to note we assume the maximum kill count allowed for each route, regardless of location
                //This shouldn't be too big of an issue as the user can manually change their kills via the Editor UI
                SAVEInstance = save;
                clearStates(locationID);

                SAVEInstance.rodeCastleLift = MiscFunctions.SetBooleanValueFromLocation(locationID, 216);
                SAVEInstance.unlockedCastleChain = MiscFunctions.SetBooleanValueFromLocation(locationID, 226);
                SAVEInstance.insideCastle = MiscFunctions.SetBooleanValueFromLocation(locationID, 216);
                SAVEInstance.coreElevatorUnlocked = MiscFunctions.SetBooleanValueFromLocation(locationID, 210);

                switch (routeToSet)
                {
                    case Routes.GameRoutes.Family:
                        SetCharacterStates(locationID, CharacterStates.Spared, Characters.Toriel, Characters.Papyrus);
                        SetDateValue(locationID, Characters.Papyrus, DateStates.PapyrusComplete);
                        SetDateValue(locationID, Characters.Undyne, DateStates.UndyneComplete);
                        SAVEInstance.undyneOnPhone = MiscFunctions.SetBooleanValueFromLocation(locationID, 139);
                        SAVEInstance.KillCount = 0;
                        break;

                    case Routes.GameRoutes.BetrayedUndyneDated:
                        SetCharacterStates(locationID, CharacterStates.Spared, Characters.Toriel, Characters.Papyrus);
                        SetDateValue(locationID, Characters.Undyne, DateStates.UndyneComplete);
                        SetDateValue(locationID, Characters.Papyrus, DateStates.PapyrusComplete);
                        SAVEInstance.undyneOnPhone = MiscFunctions.SetBooleanValueFromLocation(locationID, 139);
                        SAVEInstance.KillCount = 9;
                        break;

                    case Routes.GameRoutes.BetrayedUndyneNoDate:
                        SetCharacterStates(locationID, CharacterStates.Spared, Characters.Toriel, Characters.Papyrus);
                        SetDateValue(locationID, Characters.Undyne, DateStates.DateNotStarted);
                        SetDateValue(locationID, Characters.Papyrus, DateStates.PapyrusComplete);
                        SAVEInstance.KillCount = 9;
                        break;

                    case Routes.GameRoutes.BetrayedUndyneMettaton:
                        SetDateValue(locationID, Characters.Undyne, DateStates.DateNotStarted);
                        SetCharacterStates(locationID, CharacterStates.Spared, Characters.Toriel, Characters.Papyrus);
                        SetCharacterStates(locationID, CharacterStates.Killed, Characters.Mettaton);
                        SAVEInstance.KillCount = 1;
                        break;

                    case Routes.GameRoutes.ExiledNoUndyneNoPapyrus:
                        SetCharacterStates(locationID, CharacterStates.Spared, Characters.Toriel);
                        SetCharacterStates(locationID, CharacterStates.Killed, Characters.Papyrus, Characters.Undyne);
                        SAVEInstance.KillCount = 2;
                        break;

                    case Routes.GameRoutes.ExiledNoUndynePapyrus:
                        SetCharacterStates(locationID, CharacterStates.Spared, Characters.Toriel, Characters.Papyrus);
                        SetCharacterStates(locationID, CharacterStates.Killed, Characters.Undyne);
                        SAVEInstance.KillCount = 1;
                        break;

                    case Routes.GameRoutes.ExiledUndyneNoPapyrus:
                        SetCharacterStates(locationID, CharacterStates.Spared, Characters.Toriel);
                        SetCharacterStates(locationID, CharacterStates.Killed, Characters.Papyrus);
                        SAVEInstance.KillCount = 1;
                        break;

                    case Routes.GameRoutes.ExiledMonsters:
                        SAVEInstance.KillCount = 10;
                        break;

                    case Routes.GameRoutes.QueenUndyne:
                        SetCharacterStates(locationID, CharacterStates.Spared, Characters.Papyrus);
                        SetCharacterStates(locationID, CharacterStates.Killed, Characters.Toriel);
                        SAVEInstance.KillCount = 1;
                        break;

                    case Routes.GameRoutes.QueenUndyneNoPapyrus:
                        SetCharacterStates(locationID, CharacterStates.Killed, Characters.Toriel, Characters.Papyrus);
                        SAVEInstance.KillCount = 2;
                        break;

                    case Routes.GameRoutes.KingMettaton:
                        SetCharacterStates(locationID, CharacterStates.Spared, Characters.Papyrus);
                        SetCharacterStates(locationID, CharacterStates.Killed, Characters.Toriel, Characters.Undyne);
                        SAVEInstance.KillCount = 2;
                        break;

                    case Routes.GameRoutes.KingMettatonNoPapyrus:
                        SetCharacterStates(locationID, CharacterStates.Killed, Characters.Toriel, Characters.Undyne, Characters.Papyrus);
                        SAVEInstance.KillCount = 3;
                        break;

                    case Routes.GameRoutes.KingPapyrus:
                        SetCharacterStates(locationID, CharacterStates.Spared, Characters.Papyrus);
                        SetCharacterStates(locationID, CharacterStates.Killed, Characters.Toriel, Characters.Undyne, Characters.Mettaton);
                        SAVEInstance.KillCount = 3;
                        break;

                    case Routes.GameRoutes.KingDog:
                        SetCharacterStates(locationID, CharacterStates.Killed, Characters.Toriel, Characters.Undyne, Characters.Mettaton, Characters.Papyrus);
                        SAVEInstance.KillCount = 4;
                        break;

                    case Routes.GameRoutes.Leaderless:
                        SetCharacterStates(locationID, CharacterStates.Killed, Characters.Toriel, Characters.Undyne, Characters.Mettaton, Characters.Papyrus, Characters.RuinsDummy, Characters.Doggo, Characters.Shyren, Characters.BroGuards);
                        SAVEInstance.KillCount = 8;
                        break;

                    case Routes.GameRoutes.QueenAlphys:
                        SetMurderLevel(locationID, 12);
                        SetCharacterStates(locationID, CharacterStates.Killed, Characters.Mettaton);
                        SAVEInstance.KillCount = SAVEInstance.KillCount + 1; //We add one to account for Mettaton being killed
                        break;

                    case Routes.GameRoutes.Genocide:
                        SetMurderLevel(locationID, 16);
                        SAVEInstance.disableAlphysCalls = MiscFunctions.SetBooleanValueFromLocation(locationID, 133);
                        SAVEInstance.disableHotlandPuzzles = MiscFunctions.SetBooleanValueFromLocation(locationID, 133);
                        SAVEInstance.coreElevatorUnlocked = MiscFunctions.SetBooleanValueFromLocation(locationID, 133);
                        SAVEInstance.randomCountersDisabled = MiscFunctions.SetBooleanValueFromLocation(locationID, 212);
                        break;

                    case Routes.GameRoutes.TruePacifistDate:
                        SetCharacterStates(locationID, CharacterStates.Spared, Characters.All); //These used to be just Toriel and Papyrus! Any reason?
                        SetDateValue(236, Characters.Papyrus, DateStates.PapyrusComplete);
                        SetDateValue(236, Characters.Undyne, DateStates.UndyneComplete);
                        SAVEInstance.pacifistStage = 2;
                        SAVEInstance.Location = 236;
                        SAVEInstance.dateAlphysFlag = true;
                        SAVEInstance.rodeCastleLift = true;
                        SAVEInstance.unlockedCastleChain = true;
                        SAVEInstance.undyneOnPhone = true;
                        break;

                    case Routes.GameRoutes.TruePacifistLab:
                        SetCharacterStates(locationID, CharacterStates.Spared, Characters.All);
                        SetDateValue(236, Characters.Papyrus, DateStates.PapyrusComplete);
                        SetDateValue(236, Characters.Undyne, DateStates.UndyneComplete);
                        SAVEInstance.pacifistStage = 11;
                        SAVEInstance.Location = 139;
                        SAVEInstance.rodeCastleLift = true;
                        SAVEInstance.unlockedCastleChain = true;
                        SAVEInstance.undyneOnPhone = true;
                        SAVEInstance.randomCountersDisabled = true;
                        break;

                    case Routes.GameRoutes.TruePacifistAsriel:
                        SetCharacterStates(locationID, CharacterStates.Spared, Characters.All);
                        SetDateValue(236, Characters.Papyrus, DateStates.PapyrusComplete);
                        SetDateValue(236, Characters.Undyne, DateStates.UndyneComplete);
                        SAVEInstance.pacifistStage = 12;
                        SAVEInstance.Location = 216;
                        SAVEInstance.rodeCastleLift = true;
                        SAVEInstance.unlockedCastleChain = true;
                        SAVEInstance.beatTrueLab = true;
                        SAVEInstance.undyneOnPhone = true;
                        SAVEInstance.randomCountersDisabled = true;
                        break;

                    case Routes.GameRoutes.TruePacifistAsrielTalk:
                        SetCharacterStates(locationID, CharacterStates.Spared, Characters.All);
                        SetDateValue(236, Characters.Papyrus, DateStates.PapyrusComplete);
                        SetDateValue(236, Characters.Undyne, DateStates.UndyneComplete);
                        SAVEInstance.pacifistStage = 12;
                        SAVEInstance.Location = 331;
                        SAVEInstance.rodeCastleLift = true;
                        SAVEInstance.unlockedCastleChain = true;
                        SAVEInstance.beatTrueLab = true;
                        SAVEInstance.undyneOnPhone = true;
                        SAVEInstance.randomCountersDisabled = true;
                        break;

                    case Routes.GameRoutes.TruePacifistEpilogue:
                        SetCharacterStates(locationID, CharacterStates.Spared, Characters.All);
                        SetDateValue(236, Characters.Papyrus, DateStates.PapyrusComplete);
                        SetDateValue(236, Characters.Undyne, DateStates.UndyneComplete);
                        SAVEInstance.pacifistStage = 12;
                        SAVEInstance.Location = 236;
                        SAVEInstance.rodeCastleLift = true;
                        SAVEInstance.unlockedCastleChain = true;
                        SAVEInstance.beatTrueLab = true;
                        SAVEInstance.asrielDefeated = true;
                        SAVEInstance.undyneOnPhone = true;
                        SAVEInstance.randomCountersDisabled = true;
                        break;
                }

                SetPhoneItems(locationID, routeToSet);
                SetPlotValue(routeToSet, locationID);

                return SAVEInstance;
            }
        }
    }
}
