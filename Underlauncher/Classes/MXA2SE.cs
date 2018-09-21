using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ManagedXAudio2SoundEngine
{
    class MXA2SE
    {
        [System.Runtime.InteropServices.DllImport("ManagedXAudio2SoundEngine.dll")]
        public static extern bool startup();

        [System.Runtime.InteropServices.DllImport("ManagedXAudio2SoundEngine.dll")]
        public static extern int create_sound_engine();

        [System.Runtime.InteropServices.DllImport("ManagedXAudio2SoundEngine.dll")]
        public static extern int play_sound(int engine, string sound, bool loop = false);

        [System.Runtime.InteropServices.DllImport("ManagedXAudio2SoundEngine.dll")]
        public static extern bool set_engine_pause(int engine, bool pause);

        [System.Runtime.InteropServices.DllImport("ManagedXAudio2SoundEngine.dll")]
        public static extern bool stop_sound(int engine, int sound);

        [System.Runtime.InteropServices.DllImport("ManagedXAudio2SoundEngine.dll")]
        public static extern bool is_sound_playing(int engine, int sound);

        [System.Runtime.InteropServices.DllImport("ManagedXAudio2SoundEngine.dll")]
        public static extern void destroy_sound_engine(int engine_index);

        [System.Runtime.InteropServices.DllImport("ManagedXAudio2SoundEngine.dll")]
        public static extern void cleanup();
    }
}
