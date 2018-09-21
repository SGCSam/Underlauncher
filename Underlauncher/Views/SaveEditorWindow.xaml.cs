using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using ManagedXAudio2SoundEngine;
using System.Text.RegularExpressions;

using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;
using CheckBox = System.Windows.Controls.CheckBox;
using RadioButton = System.Windows.Controls.RadioButton;

namespace Underlauncher
{
    /// <summary>
    /// Interaction logic for SaveEditor.xaml
    /// </summary>
    /// 

    public partial class SaveEditorWindow : SharpWindow
    {
        public SaveEditorWindow()
        {
            InitializeComponent();
            this.Owner = System.Windows.Application.Current.MainWindow;
        }

        public static int soundEngine;

        SAVE.SAVEFile thisSave;
        INI.INIFile thisINI;

        bool loading = false;

        //GetSelectedRoute checks the status of the Radio Buttons and the Neutral Combobox and returns the corresponding GameRoutes enum value
        Routes.GameRoutes GetSelectedRoute()
        {
            if ((bool)genocideRadio.IsChecked)
            {
                return Routes.GetEnumValue("Genocide");
            }

            else if ((bool)neutralRadio.IsChecked || (bool)pacifistRadio.IsChecked)
            {
                if (routeCombo.SelectedItem != null)
                {
                    return Routes.GetEnumValue(routeCombo.SelectedItem.ToString());   //routeCombo
                }

                else
                {
                    return Routes.GameRoutes.Family;
                }
            }

            else
            {
                return Routes.GameRoutes.Family;
            }
        }

        //updateCurrentRoute sets up the UI, SAVE and INI files, as well as the selectedRoute when the game route is changed via the UI
        void updateCurrentRoute()
        {
            makeSave();
            Routes.GameRoutes selectedRoute = GetSelectedRoute();

            thisSave = SAVE.SAVEFile.SetSaveForRoute(thisSave, IDs.Locations.GetRoomID(locationCombo.SelectedItem.ToString()), selectedRoute);
            thisINI = INI.SetINIForRoute(IDs.Locations.GetRoomID(locationCombo.SelectedItem.ToString()), selectedRoute);

            loadSAVE(thisSave);
            loadINI();
            LOVEChanged();
        }

        //LOVEChanged is called whenever the LOVE values is or needs to be changed. It acquires the new HP, AT, DF and EXP values for this LOVE value
        void LOVEChanged()
        {
            ComboBoxItem LOVEItem = (ComboBoxItem)LOVECombo.SelectedItem;
            int LOVEVal = Convert.ToInt16(LOVEItem.Content);

            HPBox.Text = Stats.getLOVEValue(LOVEVal, Stats.LOVEHPs);
            friskATBox.Text = Stats.getLOVEValue(LOVEVal, Stats.LOVEATs);
            friskDFBox.Text = Stats.getLOVEValue(LOVEVal, Stats.LOVEDFs);
            thisSave.EXP = Convert.ToInt32(Stats.getLOVEValue(LOVEVal, Stats.LOVEEXPs));
        }

        //loadSAVE sets up the UI for the values from the new saveFile
        void loadSAVE(SAVE.SAVEFile saveFile)
        {
            loading = true;

            nameBox.Text = saveFile.PlayerName;

            foreach (var thisItem in locationCombo.Items)
            {
                if ((string)thisItem == IDs.Locations.GetRoomString(saveFile.Location))
                {
                    locationCombo.SelectedIndex = locationCombo.Items.IndexOf(thisItem);
                }
            }

            LOVECombo.SelectedIndex = saveFile.LOVE - 1;
            killsBox.Text = saveFile.KillCount.ToString();
            GOLDBox.Text = saveFile.GOLD.ToString();

            setComboIndexFromLoad(weaponCombo, saveFile.Weapon);
            setComboIndexFromLoad(armorCombo, saveFile.Armor);

            HPBox.Text = saveFile.HP.ToString();
            friskATBox.Text = saveFile.friskAT.ToString();
            friskDFBox.Text = saveFile.friskDF.ToString();
            weaponATBox.Text = saveFile.weaponAT.ToString();
            armorDFBox.Text = saveFile.armorDF.ToString();

            for (int i = 0; i < saveFile.inventoryItems.Length; i++)    //Can this be improved like how we handle setting the save?
            {
                int item = saveFile.inventoryItems[i];
                ComboBox currentComboBox = new ComboBox();

                switch (i)
                {
                    case 0:
                        currentComboBox = item1Combo;
                        break;

                    case 1:
                        currentComboBox = item2Combo;
                        break;

                    case 2:
                        currentComboBox = item3Combo;
                        break;

                    case 3:
                        currentComboBox = item4Combo;
                        break;

                    case 4:
                        currentComboBox = item5Combo;
                        break;

                    case 5:
                        currentComboBox = item6Combo;
                        break;

                    case 6:
                        currentComboBox = item7Combo;
                        break;

                    case 7:
                        currentComboBox = item8Combo;
                        break;
                }

                foreach (var thisItem in currentComboBox.Items)
                {
                    if ((string)thisItem == IDs.GetStringValue(item))
                    {
                        currentComboBox.SelectedIndex = currentComboBox.Items.IndexOf(thisItem);
                    }
                }
            }

            torielCheck.IsChecked = false;
            papyrusCheck.IsChecked = false;
            boxACheck.IsChecked = false;
            boxBCheck.IsChecked = false;

            foreach (var phoneItem in saveFile.phoneItems)
            {
                switch (phoneItem)
                {
                    case 206:
                        torielCheck.IsChecked = true;
                        break;

                    case 210:
                        papyrusCheck.IsChecked = true;
                        break;

                    case 220:
                        boxACheck.IsChecked = true;
                        break;

                    case 221:
                        boxBCheck.IsChecked = true;
                        break;
                }
            }

            loading = false;

            saveNameBox.Text = saveFile.SaveName;
            thisSave = saveFile;
        }

        //loadINI is responsible for setting up the UI based on the variable values of thisINI
        private void loadINI()
        {
            thisINI = INI.GetINI();

            playedBox.Text = thisINI.timePlayed.ToString();
            deathsBox.Text = thisINI.deaths.ToString();
            FUNBox.Text = thisINI.FUN.ToString();
            asrielCheck.IsChecked = thisINI.skipAsrielStory;
            resetCheck.IsChecked = thisINI.trueResetted;
            doorCheck.IsChecked = thisINI.doorUnlocked;
            floweyMetBox.Text = thisINI.timesMetFlowey.ToString();
            floweyChatCombo.SelectedIndex = thisINI.floweyChatProgress;
            pieCombo.SelectedIndex = thisINI.piePreference - 1;
            sansMetBox.Text = thisINI.timesMetSans.ToString();
            sansPasswordCombo.SelectedIndex = thisINI.sansPasswordProgress;
            papyrusMetBox.Text = thisINI.timesMetPapyrus.ToString();
            skipTurnCheck.IsChecked = thisINI.mettatonSkip;
            fightCombo.SelectedIndex = thisINI.fightStage;
            skipPhotoshopCheck.IsChecked = thisINI.skipFight;
        }

        //setComboIndexFromLoad sets the correct Combobox item index for the comparisonValue item when a save is loaded
        void setComboIndexFromLoad(ComboBox combo, object comparisonValue)
        {
            foreach (var thisItem in combo.Items)
            {
                if (IDs.GetID((string)thisItem) == IDs.GetID(comparisonValue.ToString()))
                {
                    combo.SelectedIndex = combo.Items.IndexOf(thisItem);
                }
            }
        }

        //populateAllCombos populates all Comboboxes with their corresponding items
        void populateAllCombos()
        {
            string[] neutralRouteItems = new string[] {"Family", "Betrayed Undyne (No Date)", "Betrayed Undyne (Killed Mettaton)",
                                                       "Betrayed Undyne (Dated)", "Exiled Queen (10+ Monsters Killed)", "Exiled Queen (Killed Papyrus)",
                                                       "Exiled Queen (Killed Undyne)", "Exiled Queen (Killed Undyne and Papyrus)", "Queen Undyne",
                                                       "Queen Undyne (Killed Papyrus)", "King Mettaton", "King Mettaton (Killed Papyrus)",
                                                       "King Papyrus", "King Dog", "Leaderless", "Queen Alphys"};

            addComboItemsFromDict(IDs.Locations.locationsDict, locationCombo);
            addComboItemsFromDict(IDs.Weapons.weaponDict, weaponCombo, false);
            addComboItemsFromDict(IDs.Armors.armorsDict, armorCombo, false);

            for (int i = 1; i < 21; i++)
            {
                ComboBoxItem thisLOVE = new ComboBoxItem() { Content = i.ToString() };
                LOVECombo.Items.Add(thisLOVE);
            }

            foreach (var item in IDs.allItemsDict.Keys)
            {
                foreach (Grid grid in inventoryGrid.Children.OfType<Grid>())
                {
                    foreach (ComboBox comboBox in grid.Children.OfType<ComboBox>())
                    {
                        ComboBoxItem thisItem = new ComboBoxItem { Content = item };

                        if ((string)thisItem.Content != "Toriel's Phone Number" && (string)thisItem.Content != "Papyrus's Phone Number" && (string)thisItem.Content != "Dimensional Box A" && (string)thisItem.Content != "Dimensional Box B")
                        {
                            comboBox.Items.Add(thisItem.Content);
                        }
                    }
                }
            }

            floweyChatCombo.Items.Add("0 - Why...?");
            floweyChatCombo.Items.Add("1 - Pissing him off");
            floweyChatCombo.Items.Add("2 - Flowey Fan Club");
            floweyChatCombo.Items.Add("3 - Smiley Trashbag");
            floweyChatCombo.Items.Add("4 - Bored");
            floweyChatCombo.Items.Add("5 - Nothing Better To Do");

            pieCombo.Items.Add("Butterscotch");
            pieCombo.Items.Add("Cinnamon");

            sansPasswordCombo.Items.Add("0 - Not Restarted");
            sansPasswordCombo.Items.Add("1 - Secret");
            sansPasswordCombo.Items.Add("2 - Triple-Secret");

            fightCombo.Items.Add("Glitched Main-Menu");
            fightCombo.Items.Add("Light Blue");
            fightCombo.Items.Add("Orange");
            fightCombo.Items.Add("Dark Blue");
            fightCombo.Items.Add("Purple");
            fightCombo.Items.Add("Green");
            fightCombo.Items.Add("Yellow");
            fightCombo.Items.Add("Finale");
        }

        //addComboItemsFromDict adds a new item to the combo from the content of the item in the dict, including an item "Nothing" if set to true
        void addComboItemsFromDict(Dictionary<string, int> dict, ComboBox combo, bool includeNothing = true)
        {
            foreach (var item in dict.Keys)
            {
                ComboBoxItem thisItem = new ComboBoxItem() { Content = item };

                if (item != "Nothing")
                {
                    combo.Items.Add(thisItem.Content);
                }

                else if (item == "Nothing" && includeNothing)
                {
                    combo.Items.Add(thisItem.Content);
                }
            }
        }

        //areTextBoxesFull iterates through all textbox controls and ensures they have valid text within, highlighting them in red if not
        bool areTextBoxesFull()
        {
            bool full = true;

            foreach (Grid grid in windowGrid.Children.OfType<Grid>())
            {
                foreach (TextBox textBox in grid.Children.OfType<TextBox>())
                {
                    textBox.Background = Brushes.White;

                    if (String.IsNullOrWhiteSpace(textBox.Text))
                    {
                        full = false;
                        textBox.Background = Brushes.Red;
                    }
                }
            }

            return full;
        }

        //makeSave loops through all UI controls and converts and casts to thisSave variable types as required to create a new save
        void makeSave()
        {
            thisSave.PlayerName = nameBox.Text;
            thisSave.Location = IDs.Locations.GetRoomID(locationCombo.SelectedItem.ToString());
            thisSave.LOVE = Convert.ToInt16(LOVECombo.Text);
            thisSave.KillCount = Convert.ToInt16(killsBox.Text);
            thisSave.GOLD = Convert.ToInt16(GOLDBox.Text);
            thisSave.Weapon = IDs.GetID(weaponCombo.SelectedItem.ToString());
            thisSave.Armor = IDs.GetID(armorCombo.SelectedItem.ToString());
            thisSave.HP = Convert.ToInt16(HPBox.Text);
            thisSave.friskAT = Convert.ToInt16(friskATBox.Text) + 10;   //For some reason, the game sets Frisk's base AT to -10 of what is set in the
            thisSave.friskDF = Convert.ToInt16(friskDFBox.Text) + 10;   //save file. Am I missing something regarding the AT/DF system?
            thisSave.weaponAT = Convert.ToInt16(weaponATBox.Text);
            thisSave.armorDF = Convert.ToInt16(armorDFBox.Text);

            int i = 0;

            Array.Clear(thisSave.inventoryItems, 0, thisSave.inventoryItems.Length);
            foreach (Grid grid in inventoryGrid.Children.OfType<Grid>())
            {
                foreach (ComboBox comboBox in grid.Children.OfType<ComboBox>())
                {
                    thisSave.inventoryItems[i] = IDs.GetID(comboBox.SelectedItem.ToString());
                    i++;
                }
            }

            i = 0;

            Array.Clear(thisSave.phoneItems, 0, thisSave.phoneItems.Length);
            foreach (var checkBox in phoneItemsGrid.Children.OfType<CheckBox>())
            {
                if ((bool)checkBox.IsChecked)
                {
                    thisSave.phoneItems[i] = IDs.GetID(checkBox.Content.ToString());
                    i++;
                }
            }

            thisSave.SaveName = saveNameBox.Text;

            thisINI = INI.GetINI();
            thisINI.sansMetInJudgment = MiscFunctions.SetBooleanValueFromLocation(thisSave.Location, 232);

            if (thisSave.Location == 999)
            {
                thisINI.photoshopFight = true;
            }

            else
            {
                thisINI.skipFight = false;
            }

            if (thisSave.Location == 998)
            {
                thisSave = SAVE.SAVEFile.SetSaveForRoute(thisSave, thisSave.Location, Routes.GameRoutes.Genocide);
                genocideRadio.IsChecked = true;
            }

            if (GetSelectedRoute() >= Routes.GameRoutes.TruePacifistAsrielTalk)
            {
                thisINI.barrierDestroyed = true;

            }

            if (GetSelectedRoute() == Routes.GameRoutes.TruePacifistEpilogue)
            {
                thisINI.canTrueReset = true;
            }

            else if (GetSelectedRoute() == Routes.GameRoutes.Genocide)
            {
                thisINI.killedSans = MiscFunctions.SetBooleanValueFromLocation(thisSave.Location, 232);
            }
        }

        //makeINI takes all entered values from the UI and converts/casts as necessary to the thisINI variable values
        public void makeINI()
        {
            thisINI.defaultINI = false;
            thisINI.timePlayed = Convert.ToInt16(playedBox.Text);
            thisINI.deaths = Convert.ToInt16(deathsBox.Text);
            thisINI.FUN = Convert.ToInt16(FUNBox.Text);
            thisINI.skipAsrielStory = (bool)asrielCheck.IsChecked;
            thisINI.trueResetted = (bool)resetCheck.IsChecked;
            thisINI.doorUnlocked = (bool)doorCheck.IsChecked;
            thisINI.timesMetFlowey = Convert.ToInt16(floweyMetBox.Text);
            thisINI.floweyChatProgress = floweyChatCombo.SelectedIndex;
            thisINI.piePreference = pieCombo.SelectedIndex + 1;
            thisINI.timesMetSans = Convert.ToInt16(sansMetBox.Text);
            thisINI.sansPasswordProgress = sansPasswordCombo.SelectedIndex;
            thisINI.timesMetPapyrus = Convert.ToInt16(papyrusMetBox.Text);
            thisINI.mettatonSkip = (bool)skipTurnCheck.IsChecked;
            thisINI.fightStage = fightCombo.SelectedIndex;
            thisINI.skipFight = (bool)skipPhotoshopCheck.IsChecked;

            if (thisINI.floweyChatProgress == 0)
            {
                thisINI.beatNeutralRun = false;
            }

            else if (thisINI.floweyChatProgress > 0)
            {
                thisINI.beatNeutralRun = true;
            }

            INI.setINI(thisINI);
        }

        //setDefaultValues sets all UI controls to their default values when the window is loaded
        void setDefaultValues()
        {
            locationCombo.SelectedIndex = 0;
            LOVECombo.SelectedIndex = 0;
            nameBox.Text = "Frisk";
            killsBox.Text = "0";
            GOLDBox.Text = "0";
            weaponCombo.SelectedIndex = 0;
            armorCombo.SelectedIndex = 0;
            HPBox.Text = "20";
            friskATBox.Text = "10";
            friskDFBox.Text = "10";
            weaponATBox.Text = "0";
            armorDFBox.Text = "0";
            playedBox.Text = "0";
            deathsBox.Text = "0";
            FUNBox.Text = "1";
            floweyMetBox.Text = "0";
            floweyChatCombo.SelectedIndex = 0;
            pieCombo.SelectedIndex = 0;
            sansMetBox.Text = "0";
            sansPasswordCombo.SelectedIndex = 0;
            papyrusMetBox.Text = "0";
            fightCombo.SelectedIndex = 0;

            foreach (Grid grid in inventoryGrid.Children.OfType<Grid>())
            {
                foreach (ComboBox comboBox in grid.Children.OfType<ComboBox>())
                {
                    comboBox.SelectedIndex = 0;
                }
            }

            neutralRadio.IsChecked = true;

            routeCombo.Items.Add("Family");
            routeCombo.Items.Add("Betrayed Undyne (No Date)");
            routeCombo.Items.Add("Betrayed Undyne (Killed Mettaton)");
            routeCombo.Items.Add("Betrayed Undyne (Dated)");
            routeCombo.Items.Add("Exiled Queen (10+ Monsters Killed)");
            routeCombo.Items.Add("Exiled Queen (Killed Papyrus)");
            routeCombo.Items.Add("Exiled Queen (Killed Undyne)");
            routeCombo.Items.Add("Exiled Queen (Killed Undyne and Papyrus)");
            routeCombo.Items.Add("Queen Undyne");
            routeCombo.Items.Add("Queen Undyne (Killed Papyrus)");
            routeCombo.Items.Add("King Mettaton");
            routeCombo.Items.Add("King Mettaton (Killed Papyrus)");
            routeCombo.Items.Add("King Papyrus");
            routeCombo.Items.Add("King Dog");
            routeCombo.Items.Add("Leaderless");
            routeCombo.Items.Add("Queen Alphys");
            routeCombo.SelectedIndex = 0;

            saveNameBox.Text = "New Save";
            makeSave();
        }

        //We use the Window_Loaded event to create new instances of our SAVEFile and INI file classes, as well as populating our UI controls and item dicts
        private void SharpWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loading = true;
            thisSave = new SAVE.SAVEFile();
            thisINI = new INI.INIFile();

            IDs.Weapons.populateDictionary();
            IDs.Armors.populateDictionary();
            IDs.Items.populateDictionary();
            IDs.PhoneItems.populateDictionary();
            IDs.InventoryItems.populateDictionary();
            IDs.Locations.populateDictionary();
            Stats.populateDictionary();
            populateAllCombos();

            setDefaultValues();

            thisINI = INI.GetINI();
            loadINI();

            loading = false;
        }

        //We use the loadButton_Click event to open a new folder browser to allow the user to load a save and set up the UI for the loaded save
        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowser2 directoryFolderBrowser = new FolderBrowser2();
            directoryFolderBrowser.ShowDialog(null);

            if (File.Exists(directoryFolderBrowser.DirectoryPath + "//file0"))
            {
                SAVE.SAVEFile saveFile = SAVE.SAVEFile.LoadSaveFile(directoryFolderBrowser.DirectoryPath);
                loadSAVE(saveFile);
                thisINI = INI.ReadINI(saveFile);
                loadINI();

                Routes.GameRoutes closestRoute = SAVE.SAVEFile.GetClosestRoute(saveFile);

                if (closestRoute == Routes.GameRoutes.Genocide)
                {
                    genocideRadio.IsChecked = true;
                }

                else if (closestRoute >= Routes.GameRoutes.TruePacifistDate)
                {
                    pacifistRadio.IsChecked = true;
                    routeCombo.SelectedItem = Routes.GetEnumDescription(SAVE.SAVEFile.GetClosestRoute(thisSave));
                }

                else
                {
                    neutralRadio.IsChecked = true;
                    routeCombo.SelectedItem = Routes.GetEnumDescription(SAVE.SAVEFile.GetClosestRoute(thisSave));
                }

                MXA2SE.play_sound(soundEngine, "Assets//Sounds//fileLoaded.wav");
            }

            else
            {
                Dictionary<Characters, string> messageDict = new Dictionary<Characters, string>()
                {
                    {Characters.Alphys, "S-s-sorry, that folder doesn't contain a valid Undertale save file." },
                    {Characters.Asgore, "Human, I'm sorry, but folder doesn't contain a valid Undertale save file." },
                    {Characters.Asriel, "I'm really sorry, I tried, but couldn't find a valid Undertale save file in that folder." },
                    {Characters.Flowey, "YOU. IDIOT! That folder doesn't contain a valid Undertale save file!" },
                    {Characters.Papyrus, "NYOO HOO HOO. I, THE GREAT PAPYRUS, HAVE FAILED TO FIND A VALID SAVE FILE IN THAT FOLDER." },
                    {Characters.Sans, "heyo, pal. try again. i couldn't find an Undertale save file in that folder." },
                    {Characters.Toriel, "My child, I am sorry, but I was unable to find a valid Undertale save file in that folder." },
                    {Characters.Undyne, "HEY, PUNK! I CAN'T FIND AN UNDERTALE SAVE FILE IN THAT FOLDER!" },
                    {Characters.None, "The folder specified does not contain a valid Undertale save file!" }
                };

                UTMessageBox.Show(messageDict, Constants.CharacterReactions.Negative, MessageBoxButton.OK);
            }
        }

        //SelectionChanged is used with some UI controls to update our save variable values when we select a new armor, weapon, item or route
        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!loading)
            {
                if (((ComboBox)sender == weaponCombo || (ComboBox)sender == armorCombo))
                {
                    weaponATBox.Text = Stats.getItemAT(IDs.GetID(weaponCombo.SelectedItem.ToString()), IDs.GetID(armorCombo.SelectedItem.ToString()));
                    armorDFBox.Text = Stats.getItemDF(IDs.GetID(armorCombo.SelectedItem.ToString()));
                }

                else if ((ComboBox)sender == LOVECombo)
                {
                    LOVEChanged();
                }

                else if ((ComboBox)sender == routeCombo || (ComboBox)sender == locationCombo)
                {
                    updateCurrentRoute();
                }
            }
        }

        //We use the saveButton_Click event to make a save from the UI control values and to write a new file0
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (areTextBoxesFull())
            {
                makeSave();
                makeINI();

                if (SAVE.SAVEFile.AskToWrite(thisSave, thisINI))
                {
                    MXA2SE.play_sound(soundEngine, "Assets//Sounds//fileSaved.wav");
                    ((MainWindow)this.Owner).populateSavesCombo();

                    Dictionary<Characters, string> messageDict = new Dictionary<Characters, string>()
                    {
                        {Characters.Alphys, "N-n-no problem! I've made the \"" + thisSave.SaveName + "\" save file successfully!" },
                        {Characters.Asgore, "Not a problem at all, human. The \"" + thisSave.SaveName + "\" save file has been created successfully!" },
                        {Characters.Asriel, "Hey, howdy! I've made the \"" + thisSave.SaveName + "\" save successfully!" },
                        {Characters.Flowey, "URGH, don't you think I've got something better to do? I've made the \"" + thisSave.SaveName + "\" save for you." },
                        {Characters.Papyrus, "FEAR NOT, HUMAN! I'VE MASTERFULLY PREPARED THE \"" + thisSave.SaveName + "\" SAVE FOR YOU." },
                        {Characters.Sans, "sure thing buddy. i've created the \"" + thisSave.SaveName + "\" save for you." },
                        {Characters.Toriel, "I would be glad to assist you my child. The \"" + thisSave.SaveName + "\" save has been created successfully." },
                        {Characters.Undyne, "FUHUHUH! No problem, bestie! I've created the \"" + thisSave.SaveName + "\" save for you." },
                        {Characters.None, "Save \"" + thisSave.SaveName + "\" created successfully!" }
                    };

                    UTMessageBox.Show(messageDict, Constants.CharacterReactions.Positive, MessageBoxButton.OK);
                    Close();
                }
            }

            else
            {
                System.Windows.MessageBox.Show("Error! Some text boxes contain empty or invalid data! Please ensure the highlighted text boxes are corrected and then try again.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //We use PreviewTextInput to check that the user's input to a Textbox contains valid, allowed symbols before allowing the text entry
        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox senderBox = (TextBox)sender;

            if (senderBox == nameBox)
            {
                Regex regex = new Regex("[^a-zA-Z]");
                e.Handled = regex.IsMatch(e.Text);
            }

            else if (senderBox == saveNameBox)
            {
                foreach (char illegalCharacter in System.IO.Path.GetInvalidFileNameChars())
                {
                    if (e.Text.Contains(illegalCharacter))
                    {
                        e.Handled = true;
                        break;
                    }
                }
            }

            else
            {
                Regex regex = new Regex("[^0-9]");
                e.Handled = regex.IsMatch(e.Text);
            }
        }

        //Checked is used when a Radiobutton check is made. This is most commonly used when we select a new route to disable the other route UI controls
        private void Checked(object sender, RoutedEventArgs e)
        {
            if (!loading)
            {
                if (sender is RadioButton)
                {
                    RadioButton checkedButton = (RadioButton)sender;

                    routeCombo.SelectedIndex = 0;

                    routeCombo.IsEnabled = true;

                    if (checkedButton == neutralRadio)
                    {
                        routeCombo.Items.Clear();
                        routeCombo.Items.Add("Family");
                        routeCombo.Items.Add("Betrayed Undyne (No Date)");
                        routeCombo.Items.Add("Betrayed Undyne (Killed Mettaton)");
                        routeCombo.Items.Add("Betrayed Undyne (Dated)");
                        routeCombo.Items.Add("Exiled Queen (10+ Monsters Killed)");
                        routeCombo.Items.Add("Exiled Queen (Killed Papyrus)");
                        routeCombo.Items.Add("Exiled Queen (Killed Undyne)");
                        routeCombo.Items.Add("Exiled Queen (Killed Undyne and Papyrus)");
                        routeCombo.Items.Add("Queen Undyne");
                        routeCombo.Items.Add("Queen Undyne (Killed Papyrus)");
                        routeCombo.Items.Add("King Mettaton");
                        routeCombo.Items.Add("King Mettaton (Killed Papyrus)");
                        routeCombo.Items.Add("King Papyrus");
                        routeCombo.Items.Add("King Dog");
                        routeCombo.Items.Add("Leaderless");
                        routeCombo.Items.Add("Queen Alphys");
                    }

                    else if (checkedButton == pacifistRadio)
                    {
                        routeCombo.Items.Clear();
                        routeCombo.Items.Add("True Pacifist Alphys Date");
                        routeCombo.Items.Add("True Pacifist True Lab");
                        routeCombo.Items.Add("True Pacifist Asriel Battle");
                        routeCombo.Items.Add("True Pacifist Asriel Talk");
                        routeCombo.Items.Add("True Pacifist Epilogue");
                    }

                    else if (checkedButton == genocideRadio)
                    {
                        routeCombo.Items.Clear();
                        routeCombo.Items.Add("Genocide");
                        routeCombo.IsEnabled = false;
                    }

                    routeCombo.SelectedIndex = 1;
                    updateCurrentRoute();
                }
            }
        }
    }
}
