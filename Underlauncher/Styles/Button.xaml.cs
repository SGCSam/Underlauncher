using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ManagedXAudio2SoundEngine;

namespace Underlauncher.Styles
{
    public partial class Button : ResourceDictionary
    {
        bool seOK = MXA2SE.startup();
        int soundEngine = MXA2SE.create_sound_engine();

        private void Button_MouseEnter(object sender, RoutedEventArgs e)
        {
            MXA2SE.play_sound(soundEngine, "Assets/Sounds/buttonHover.wav");
        }

        private void Button_MouseDown(object sender, RoutedEventArgs e)
        {
            MXA2SE.play_sound(soundEngine, "Assets/Sounds/buttonClick.wav");
        }
    }
}
