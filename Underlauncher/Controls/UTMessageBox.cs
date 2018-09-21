using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Underlauncher
{
    public static class UTMessageBox
    {
        public static MessageBoxResult Show(string message, MessageBoxButton buttons)
        {
            UTMessageBoxWindow messageBox = new UTMessageBoxWindow(Characters.None, Constants.CharacterReactions.None, message, buttons);

            if(messageBox.ShowDialog() == true)
            {
                messageBox.IsClosing = true;
                return messageBox.Result;
            }

            else
            {
                messageBox.IsClosing = true;
                return MessageBoxResult.None;
            }
        }

        public static MessageBoxResult Show(Characters chara, Constants.CharacterReactions react, string message, MessageBoxButton buttons)
        {
            if (XML.characterMessagesSetting == false)
            {
                chara = Characters.None;
            }

            UTMessageBoxWindow messageBox = new UTMessageBoxWindow(chara, react, message, buttons);

            if (messageBox.ShowDialog() == true)
            {
                messageBox.IsClosing = true;
                return messageBox.Result;
            }

            else
            {
                messageBox.IsClosing = true;
                return MessageBoxResult.None;
            }
        }

        public static MessageBoxResult Show(Dictionary<Characters, string> messages, Constants.CharacterReactions react, MessageBoxButton buttons)
        {
            Characters randChara = MiscFunctions.GetRandomMessageBoxCharacter(new List<Characters>(messages.Keys));

            if (XML.characterMessagesSetting == false)
            {
                randChara = Characters.None;
            }

            UTMessageBoxWindow messageBox = new UTMessageBoxWindow(randChara, react, GetMessageForRandCharacter(randChara, messages), buttons);

            if (messageBox.ShowDialog() == true)
            {
                messageBox.IsClosing = true;
                return messageBox.Result;
            }

            else
            {
                messageBox.IsClosing = true;
                return MessageBoxResult.None;
            }
        }

        private static string GetMessageForRandCharacter(Characters chara, Dictionary<Characters, string> messages)
        {
            foreach(var message in messages)
            {
                if (message.Key == chara)
                {
                    return message.Value;
                }
            }

            return "Error! No message for this character!";
        }
    }
}
