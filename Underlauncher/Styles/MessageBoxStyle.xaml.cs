using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Underlauncher
{
    partial class MessageBoxStyle : ResourceDictionary
    {
        public MessageBoxStyle()
        {
            
        }

        public void MessageBoxLoaded(object sender, RoutedEventArgs e)
        {
            UTMessageBoxWindow senderBox = (UTMessageBoxWindow)sender;

            System.Windows.Controls.Button leftButton = new System.Windows.Controls.Button();
            System.Windows.Controls.Button centreButton = new System.Windows.Controls.Button();
            System.Windows.Controls.Button rightButton = new System.Windows.Controls.Button();

            leftButton.Click += new RoutedEventHandler(ButtonClicked);
            centreButton.Click += new RoutedEventHandler(ButtonClicked);
            rightButton.Click += new RoutedEventHandler(ButtonClicked);

            StackPanel buttonPanel = (StackPanel)senderBox.Template.FindName("buttonPanel", senderBox);
            TextBlock messageBlock = (TextBlock)senderBox.Template.FindName("messageBlock", senderBox);

            leftButton.Margin = new Thickness(0, 0, 10, 0);
            rightButton.Margin = new Thickness(10, 0, 0, 0);

            if (senderBox.Character == Characters.None)
            {
                messageBlock.Margin = new Thickness(-65, 20, 20, 0);
            }

            else if (senderBox.Character == Characters.Papyrus)
            {
                messageBlock.FontFamily = (FontFamily)Application.Current.Resources["FontFamily.papyrusFont.Regular"];
            }

            else if (senderBox.Character == Characters.Sans)
            {
                messageBlock.FontFamily = (FontFamily)Application.Current.Resources["FontFamily.sansFont.Regular"];
            }

            if (senderBox.Buttons == MessageBoxButton.OK)
            {
                centreButton.Content = "OK";
                buttonPanel.Children.Add(centreButton);
            }

            else if(senderBox.Buttons == MessageBoxButton.OKCancel)
            {
                leftButton.Content = "OK";
                rightButton.Content = "CANCEL";
                buttonPanel.Children.Add(leftButton);
                buttonPanel.Children.Add(rightButton);
            }

            else if (senderBox.Buttons == MessageBoxButton.YesNo)
            {
                leftButton.Content = "YES";
                rightButton.Content = "NO";
                buttonPanel.Children.Add(leftButton);
                buttonPanel.Children.Add(rightButton);
            }

            else if (senderBox.Buttons == MessageBoxButton.YesNoCancel)
            {
                leftButton.Content = "YES";
                centreButton.Content = "NO";
                rightButton.Content = "CANCEL";
                buttonPanel.Children.Add(leftButton);
                buttonPanel.Children.Add(centreButton);
                buttonPanel.Children.Add(rightButton);
            }

            senderBox.BeginCharacterMessageOutput();
        }

        public void ButtonClicked(object sender, RoutedEventArgs e)
        {
            UTMessageBoxWindow messageWindow = (UTMessageBoxWindow)Window.GetWindow(((FrameworkElement)e.Source));
            System.Windows.Controls.Button clickedButton = (System.Windows.Controls.Button)sender;

            if (clickedButton.Content.ToString() == "OK")
            {
                messageWindow.Result = MessageBoxResult.OK;
            }

            else if (clickedButton.Content.ToString() == "YES")
            {
                messageWindow.Result = MessageBoxResult.Yes;
            }

            else if (clickedButton.Content.ToString() == "NO")
            {
                messageWindow.Result = MessageBoxResult.No;
            }

            else if (clickedButton.Content.ToString() == "CANCEL")
            {
                messageWindow.Result = MessageBoxResult.Cancel;
            }

            else
            {
                messageWindow.Result = MessageBoxResult.None;
            }

            messageWindow.DialogResult = true;
            messageWindow.Close();
        }

        private void KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Z || e.Key == System.Windows.Input.Key.Enter)
            {
                UTMessageBoxWindow messageWindow = (UTMessageBoxWindow)Window.GetWindow(((FrameworkElement)e.Source));
                messageWindow.SkipMessage();
            }
        }
    }
}
