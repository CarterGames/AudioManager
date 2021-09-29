using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CarterGames.Assets.AudioManager.Demo
{
    public class DemoSceneScript : MonoBehaviour
    {
        private float startTime;
        private float endTime;
        private float transitionLength = 1;
        public List<AudioClip> tracks;
        public TransitionType TransType { get; set; }
        
        public void OpenDocs()
        {
            Application.OpenURL("https://carter.games/audiomanager");
        }

        public void PlayStopTrack(Button b)
        {
            if (!MusicPlayer.instance.GetAudioSource.isPlaying)
            {
                b.transform.GetChild(0).GetComponent<Text>().text = "Stop";
                MusicPlayer.instance.PlayTrack();
            }
            else
            {
                b.transform.GetChild(0).GetComponent<Text>().text = "Play";
                MusicPlayer.instance.GetAudioSource.Stop();
            }
        }

        public void SwitchTrack()
        {
            var _currentTrack = MusicPlayer.instance.GetActiveTrack;
            var _newTrack = tracks.FirstOrDefault(t => !t.Equals(_currentTrack));
            
            if (startTime > 0 && endTime > 0)
                MusicPlayer.instance.PlayTrack(_newTrack, startTime, endTime, TransType, transitionLength);
            else if (startTime > 0 && endTime.Equals(0))
                MusicPlayer.instance.PlayTrack(_newTrack, startTime, TransType, transitionLength);
            else
                MusicPlayer.instance.PlayTrack(_newTrack, TransType, transitionLength);
        }

        public void SetTransitionType(int value) => TransType = (TransitionType) value;

        public void SetStartTime(string value)
        {
            int.TryParse(value.Trim(), out var _string);
            startTime = _string;
        }
        
        public void SetEndTime(string value)
        {
            int.TryParse(value.Trim(), out var _string);
            endTime = _string;
        }
        
        public void SetTransitionLength(string value)
        {
            int.TryParse(value.Trim(), out var _string);
            transitionLength = _string;
        }
    }
}