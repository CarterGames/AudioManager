/*
 * Copyright (c) 2018-Present Carter Games
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CarterGames.Assets.AudioManager.Demo
{
    /// <summary>
    /// The demo script for the asset.
    /// </summary>
    public class DemoSceneScript : MonoBehaviour
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */   
        
        [SerializeField] private List<AudioClip> tracks;
        
        private float startTime;
        private float endTime;
        private float transitionLength = 1;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private TransitionType Transition { get; set; }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */   
        
        /// <summary>
        /// Opens the documentation for the asset (Online)
        /// </summary>
        public void OpenDocs()
        {
            Application.OpenURL("https://carter.games/audiomanager");
        }
        

        /// <summary>
        /// Players or stops a track based on if a track is playing.
        /// </summary>
        /// <param name="text">The button to read the text for.</param>
        public void PlayStopTrack(Text text)
        {
            if (!MusicPlayer.instance.GetAudioSource.isPlaying)
            {
                text.text = "Stop";
                MusicPlayer.instance.PlayTrack();
            }
            else
            {
                text.text = "Play";
                MusicPlayer.instance.GetAudioSource.Stop();
            }
        }

        
        /// <summary>
        /// Switches the track to the other track not in use in the demo.
        /// </summary>
        public void SwitchTrack()
        {
            var currentTrack = MusicPlayer.instance.GetActiveTrack;
            var newTrack = tracks.FirstOrDefault(t => !t.Equals(currentTrack));
            
            if (startTime > 0 && endTime > 0)
                MusicPlayer.instance.PlayTrack(newTrack, startTime, endTime, Transition, transitionLength);
            else if (startTime > 0 && endTime.Equals(0))
                MusicPlayer.instance.PlayTrack(newTrack, startTime, Transition, transitionLength);
            else
                MusicPlayer.instance.PlayTrack(newTrack, Transition, transitionLength);
        }
        

        /// <summary>
        /// Sets the transition type to the entered value.
        /// </summary>
        /// <param name="value">The transition selected as a int to convert back to the enum with a cast.</param>
        public void SetTransitionType(int value)
        {
            Transition = (TransitionType)value;
        }
        

        /// <summary>
        /// Sets the start time of the music track.
        /// </summary>
        /// <param name="value">The start time to set to.</param>
        public void SetStartTime(string value)
        {
            int.TryParse(value.Trim(), out var newValue);
            startTime = newValue;
        }
        
        
        /// <summary>
        /// Sets the start time of the music track.
        /// </summary>
        /// <param name="value">The end time to set to.</param>
        public void SetEndTime(string value)
        {
            int.TryParse(value.Trim(), out var newValue);
            endTime = newValue;
        }
        
        
        /// <summary>
        /// Sets the transition length.
        /// </summary>
        /// <param name="value">The length of the transition.</param>
        public void SetTransitionLength(string value)
        {
            int.TryParse(value.Trim(), out var newValue);
            transitionLength = newValue;
        }
    }
}