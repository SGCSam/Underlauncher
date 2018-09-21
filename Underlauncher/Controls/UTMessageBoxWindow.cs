using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using ManagedXAudio2SoundEngine;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Linq;

namespace Underlauncher
{
    public class UTMessageBoxWindow : SharpWindow
    {
        private Timer updateTimer = new Timer();
        private CharacterSpeech charaSpeech;
        public bool IsClosing { get; set; }

        public UTMessageBoxWindow(Characters chara, Constants.CharacterReactions react, string message, MessageBoxButton buttons)
        {
            DefaultStyleKey = typeof(UTMessageBoxWindow);
            Style = (Style)FindResource("UTMessageBoxStyle");
            Owner = System.Windows.Application.Current.Windows.OfType<SharpWindow>().SingleOrDefault(x => x.IsActive);
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            Character = chara;
            CharacterReaction = react;
            MessageToOutput = message;
            Buttons = buttons;

            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Interval = 1;
        }

        static UTMessageBoxWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UTMessageBoxWindow), new FrameworkPropertyMetadata(typeof(UTMessageBoxWindow)));
        }

        public MessageBoxResult Result { get; set; }

        public string MessageText
        {
            get { return (string)GetValue(MessageTextProperty); }
            set
            {
                SetValue(MessageTextProperty, value);
            }
        }

        public ImageSource CharacterImageSource
        {
            get { return (ImageSource)GetValue(CharacterImageSourceProperty); }
            set
            {
                SetValue(CharacterImageSourceProperty, value);
            }
        }

        public MessageBoxButton Buttons
        {
            get { return (MessageBoxButton)GetValue(ButtonsProperty); }
            set
            {
                SetValue(ButtonsProperty, value);
            }
        }

        public FontFamily MessageFam
        {
            get { return (FontFamily)GetValue(MessageFamProperty); }
            set
            {
                SetValue(MessageFamProperty, value);
            }
        }

        public Characters Character { get; private set; }
        public Constants.CharacterReactions CharacterReaction { get; private set; }
        public string MessageToOutput { get; set; }

        public void BeginCharacterMessageOutput()
        {
            if (XML.characterMessagesSetting)
            {
                charaSpeech = new CharacterSpeech(Character, CharacterReaction, MessageToOutput);
                updateTimer.Start();
            }

            else
            {
                MessageText = MessageToOutput;
            }
        }

        public void SkipMessage()
        {
            MessageText = MessageToOutput;
            updateTimer.Stop();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (!charaSpeech.Finished && !IsClosing)
            {
                charaSpeech.Update();
                MessageText = charaSpeech.SentenceToDisplay;
                CharacterImageSource = new BitmapImage(new Uri(charaSpeech.FaceToDisplay, UriKind.Relative));
            }

            else
            {
                updateTimer.Stop();
            }
        }

        public static readonly DependencyProperty MessageTextProperty = DependencyProperty.Register(("MessageText"), typeof(string), typeof(UTMessageBoxWindow));
        public static readonly DependencyProperty CharacterImageSourceProperty = DependencyProperty.Register(("CharacterImageSource"), typeof(ImageSource), typeof(UTMessageBoxWindow));
        public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register(("Buttons"), typeof(MessageBoxButton), typeof(UTMessageBoxWindow));
        public static readonly DependencyProperty MessageFamProperty = DependencyProperty.Register(("MessageFam"), typeof(FontFamily), typeof(UTMessageBox));
    }
}
