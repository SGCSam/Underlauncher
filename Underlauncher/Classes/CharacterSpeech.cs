using ManagedXAudio2SoundEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Underlauncher
{
    public class CharacterSpeech
    {
        private string _MessageToSpeak;
        private Characters _Character;
        private Constants.CharacterReactions _Reaction;
        private DeltaTimer _LetterTimer = new DeltaTimer();
        private DeltaTimer _VoiceTimer = new DeltaTimer();
        private DeltaTimer _FaceTimer = new DeltaTimer();
        private int _CurrentAnimImage;
        private int _TimeUntilNextLetter;
        private List<int> _PauseIndexes = new List<int>();
        private int _CurrentPauseIndex;
        public string SentenceToDisplay { get; private set; }
        public string FaceToDisplay { get; private set; }
        public bool Finished { get; private set; }
        private bool _VoicePaused;
        private int _VoicePauseDuration;

        private int soundEngine;

        public CharacterSpeech(Characters chara, Constants.CharacterReactions react, string message)
        {
            _Character = chara;
            _Reaction = react;
            _MessageToSpeak = message;

            SentenceToDisplay = "";
            _TimeUntilNextLetter = 25;
            Finished = false;
            _VoicePaused = false;
            FaceToDisplay = "/Underlauncher;component/Assets/Images/Characters/" + _Character.ToString() + "/" + _Reaction.ToString() + "/" + "1.png";
            _CurrentAnimImage = 1;

            bool seOK = MXA2SE.startup();
            soundEngine = MXA2SE.create_sound_engine();

            AnalyzeSpeech();
        }

        private void AnalyzeSpeech()
        {
            List<char> pauseChars = new List<char>();
            pauseChars.AddRange(new List<char> { '!', '.', '?', ',' });
            List<string> words = _MessageToSpeak.Split(' ').ToList();

            int currentChar = 1;
            foreach (var word in words)
            {
                foreach (var letter in word)
                {
                    if (pauseChars.Contains(char.ToLower(letter)))
                    {
                        _PauseIndexes.Add(currentChar);
                    }

                    currentChar++;
                }

                currentChar++;
            }
        }

        private void AddNextLetter()
        {
            if (SentenceToDisplay.Length < _MessageToSpeak.Length)
            {
                if (_CurrentPauseIndex < _PauseIndexes.Count)
                {
                    if (SentenceToDisplay.Length >= _PauseIndexes[_CurrentPauseIndex])
                    {
                        if (_MessageToSpeak[SentenceToDisplay.Length - 1] == ',' || _MessageToSpeak[SentenceToDisplay.Length - 1] == '.' || _MessageToSpeak[SentenceToDisplay.Length - 1] == '!' || _MessageToSpeak[SentenceToDisplay.Length - 1] == '?')
                        {
                            PauseVoice(175);
                            _TimeUntilNextLetter = 220;
                        }

                        _CurrentPauseIndex++;
                    }

                    else
                    {
                        _TimeUntilNextLetter = 25;
                    }
                }

                else
                {
                    _TimeUntilNextLetter = 25;
                }

                SentenceToDisplay += _MessageToSpeak[SentenceToDisplay.Length];
            }

            else
            {
                Finished = true;
                FaceToDisplay = "/Underlauncher;component/Assets/Images/Characters/" + _Character.ToString() + "/" + _Reaction.ToString() + "/" + "1.png";
            }
        }

        private void PauseVoice(int duration)
        {
            _VoicePauseDuration = duration;
            _VoicePaused = true;
        }

        private void PlayCharacterVoice()
        {
            switch (_Character)
            {
                case Characters.Alphys:
                    MXA2SE.play_sound(soundEngine, "Assets//Sounds//Characters//Alphys.wav");
                    break;

                case Characters.Asgore:
                    MXA2SE.play_sound(soundEngine, "Assets//Sounds//Characters//Asgore.wav");
                    break;

                case Characters.Asriel:
                    MXA2SE.play_sound(soundEngine, "Assets//Sounds//Characters//Asriel.wav");
                    break;

                case Characters.Flowey:
                    if (_Reaction == Constants.CharacterReactions.Negative)
                    {
                        MXA2SE.play_sound(soundEngine, "Assets//Sounds//Characters//FloweyNegative.wav");
                    }

                    else
                    {
                        MXA2SE.play_sound(soundEngine, "Assets//Sounds//Characters//Flowey.wav");
                    }
                    break;

                case Characters.Papyrus:
                    MXA2SE.play_sound(soundEngine, "Assets//Sounds//Characters//Papyrus.wav");
                    break;

                case Characters.Sans:
                    MXA2SE.play_sound(soundEngine, "Assets//Sounds//Characters//Sans.wav");
                    break;

                case Characters.Toriel:
                    MXA2SE.play_sound(soundEngine, "Assets//Sounds//Characters//Toriel.wav");
                    break;

                case Characters.Undyne:
                    MXA2SE.play_sound(soundEngine, "Assets//Sounds//Characters//Undyne.wav");
                    break;
            }
        }

        private void FlipFace()
        {
            switch (_CurrentAnimImage)
            {
                case 2:
                    _CurrentAnimImage = 1;
                    FaceToDisplay = "/Underlauncher;component/Assets/Images/Characters/" + _Character.ToString() + "/" + _Reaction.ToString() + "/" + "1.png";
                    break;

                case 1:
                    _CurrentAnimImage = 2;
                    FaceToDisplay = "/Underlauncher;component/Assets/Images/Characters/" + _Character.ToString() + "/" + _Reaction.ToString() + "/" + "2.png";
                    break;

                default:
                    FaceToDisplay = "/Underlauncher;component/Assets/Images/Characters/" + _Character.ToString() + "/" + _Reaction.ToString() + "/" + "1.png";
                    break;

            }
        }

        public void Update()
        {
            if (_LetterTimer.Wait(_TimeUntilNextLetter))
            {
                AddNextLetter();
            }

            if (_FaceTimer.Wait(107))
            {
                FlipFace();
            }

            if (_VoicePaused)
            {
                if (_VoiceTimer.Wait(_VoicePauseDuration))
                {
                    PlayCharacterVoice();
                    _VoicePauseDuration = 0;
                    _VoicePaused = false;
                }
            }

            else if (_VoiceTimer.Wait(45) && !_VoicePaused)
            {
                PlayCharacterVoice();
            }
        }
    }
}
