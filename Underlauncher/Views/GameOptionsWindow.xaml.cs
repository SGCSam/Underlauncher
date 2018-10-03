using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using MessageBox = System.Windows.MessageBox;

namespace Underlauncher
{
    /// <summary>
    /// Interaction logic for GameOptions.xaml
    /// </summary>
    public partial class GameOptionsWindow : SharpWindow
    {
        public GameOptionsWindow()
        {
            InitializeComponent();
            Window wnd = System.Windows.Application.Current.MainWindow;
            this.Owner = wnd;
        }

        private void clearGenocideButton_Click(object sender, RoutedEventArgs e)
        {
            FileOperations.setGenocide(GenocideStates.None);

            Dictionary<Characters, string> messageDict = new Dictionary<Characters, string>()
            {
                {Characters.Alphys, "O-O-Okay, the effects are cleared!\nTrue Pacifist ending will proceed as normal." },
                {Characters.Asgore, "Well human, those effects have been removed!\nTrue Pacifist ending will proceed as normal." },
                {Characters.Asriel, "Hey, those effects are gone, ok?\nTrue Pacifist ending will proceed as normal." },
                {Characters.Flowey, "YOU. CHEATING. IDIOT.\nTrue Pacifist ending will proceed as normal." },
                {Characters.Papyrus, "HUMAN, THOSE BAD EFFECTS ARE GONE. I NEVER STOPPED BELIEVING IN YOU!\nTrue Pacifist ending will proceed as normal." },
                {Characters.Sans, "you dirty hacker.\nTrue Pacifist ending will proceed as normal." },
                {Characters.Toriel, "Those effects are gone now, my child.\nTrue Pacifist ending will proceed as normal." },
                {Characters.Undyne, "YOU PUNK! THOSE EFFECTS ARE GONE!\nTrue Pacifist ending will proceed as normal." },
                {Characters.None, "Genocide effects have been cleared. True Pacifist ending will proceed as normal." }
            };

            UTMessageBox.Show(messageDict, Constants.CharacterReactions.Positive, MessageBoxButton.OK);
        }

        private void setSoullessButton_Click(object sender, RoutedEventArgs e)
        {
            FileOperations.setGenocide(GenocideStates.Soulless);

            Dictionary<Characters, string> messageDict = new Dictionary<Characters, string>()
            {
                {Characters.Alphys, "W-w-why would you do such a horrible thing? True Pacifist ending will be altered." },
                {Characters.Asgore, "Uh, what kind of monster are you? True Pacifist ending will be altered." },
                {Characters.Asriel, "I can help, I can-- please don't kill me. True Pacifist ending will be altered." },
                {Characters.Flowey, "You're soulless. Just like me. True Pacifist ending will be altered." },
                {Characters.Papyrus, "YOU CAN DO BETTER HUMAN, I STILL BELIEVE IN YOU! True Pacifist ending will be altered." },
                {Characters.Sans, "you dirty brother killer. True Pacifist ending will be altered." },
                {Characters.Toriel, "Be good, alright? My child... True Pacifist ending will be altered." },
                {Characters.Undyne, "You've made your choice. You're going to have to get past ME. True Pacifist ending will be altered." },
                {Characters.None, "Genocide effects activated. True Pacifist ending will be altered." }
            };

            UTMessageBox.Show(messageDict, Constants.CharacterReactions.Negative, MessageBoxButton.OK);
        }

        private void backupButton_Click(object sender, RoutedEventArgs e)
        {
            var currentTime = DateTime.Now;
            string backupSaveName = "Appdata Save " + currentTime.ToString("dd" + "-" + "MM" + "-" + "yy" + " hhmmss");
            string backupDirPath = Constants.SavesPath + backupSaveName;
            Directory.CreateDirectory(backupDirPath);

            File.Copy(Constants.AppdataPath + "\\file0", backupDirPath + "\\file0", true);
            File.Copy(Constants.AppdataPath + "\\undertale.ini", backupDirPath + "\\undertale.ini", true);
            ((MainWindow)this.Owner).populateSavesCombo();

            Dictionary<Characters, string> messageDict = new Dictionary<Characters, string>()
            {
                {Characters.Alphys, "S-s-sure, no problem! I've backed up your Local Appdata save to \"" + backupSaveName + "\" successfully!" },
                {Characters.Asgore, "Human, I've backed up your Local Appdata save to \"" + backupSaveName + "\" successfully!" },
                {Characters.Asriel, "Hey, I've tried my best and I think I've backed up your Local Appdata save to \"" + backupSaveName + "\" successfully!" },
                {Characters.Flowey, "You're not the ONLY one with this power. I've backed up your Local Appdata save to \"" + backupSaveName + "\". I'll be watching you." },
                {Characters.Papyrus, "NYEH, HUMAN! I, THE GREAT PAPYRUS, HAVE BACKED UP YOUR LOCAL APPDATA SAVE TO \"" + backupSaveName + "\" SUCCESSFULLY... SANS, WHAT'S AN APPDATA?" },
                {Characters.Sans, "hey buddy, lazy like me huh? I've backed up your Local Appdata save to \"" + backupSaveName + "\"" },
                {Characters.Toriel, "Of course my child! I've backed up your Local Appdata save to \"" + backupSaveName + "\" successfully! Be good, alright?" },
                {Characters.Undyne, "OI, PUNK, I'M BUSY! URGH. Fine. I've backed up your Local Appdata save to \"" + backupSaveName + "\"" },
                {Characters.None, "Local Appdata save backed up to \"" + backupSaveName + "\" successfully!" }
            };

            UTMessageBox.Show(messageDict, Constants.CharacterReactions.Positive, MessageBoxButton.OK);
        }
    }
}
