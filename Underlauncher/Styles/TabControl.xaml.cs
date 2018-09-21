using ManagedXAudio2SoundEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Underlauncher
{
    partial class TabControl : ResourceDictionary
    {
        int soundEngine;

        public TabControl()
        {
            bool isOK = MXA2SE.startup();

            if (!isOK)
            {
                System.Windows.MessageBox.Show("There was a problem starting up the XAudio2 Sound Engine. Tab Controls will not play audio.");
            }

            else
            {
                soundEngine = MXA2SE.create_sound_engine();
            }
        }

        public void TabItemHover(object sender, RoutedEventArgs e)
        {
            MXA2SE.play_sound(soundEngine, "Assets/Sounds/buttonHover.wav");
        }

        public void TabItemClick(object sender, RoutedEventArgs e)
        {
            //MXA2SE.play_sound(soundEngine, "Assets/Sounds/buttonClick.wav");
        }
    }
}
