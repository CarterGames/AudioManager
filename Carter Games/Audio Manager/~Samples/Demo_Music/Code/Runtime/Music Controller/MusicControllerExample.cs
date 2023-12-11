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
 * FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;
using UnityEngine.UI;

namespace CarterGames.Assets.AudioManager.Demo
{
    /// <summary>
    /// A controller for the music demo scene.
    /// </summary>
    public sealed class MusicControllerExample : MonoBehaviour
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        [SerializeField] private string musicTrackListName;

        [Space]
        [SerializeField] private Text statusLabel;
        [SerializeField] private Text songNameLabel;
        [SerializeField] private Text playButtonLabel;
        [SerializeField] private Text pauseButtonLabel;
        [SerializeField] private Text transitionButtonLabel;

        [Space]
        [SerializeField] private Button playStopButton;
        [SerializeField] private Button pauseResumeButton;
        [SerializeField] private Button transitionButton;
        
        private IMusicPlayer player;
        private IMusicTransition musicTransition;
        private bool transitionOut;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private IMusicPlayer MusicPlayer
        {
            get
            {
                if (player != null) return player;
                player = MusicManager.Prepare(musicTrackListName);
                return player;
            }
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private void OnEnable()
        {
            // Assigns the methods to the buttons in the scene via code.
            playStopButton.onClick.AddListener(PlayOrStop);
            pauseResumeButton.onClick.AddListener(PauseOrResume);
            transitionButton.onClick.AddListener(Transition);

            // Setting initial button & display states.
            statusLabel.text = "Inactive";
            songNameLabel.text = string.Empty;
            transitionOut = true;
            
            playButtonLabel.text = "Play";

            pauseResumeButton.interactable = false;
            pauseButtonLabel.text = "Pause";

            transitionButton.interactable = false;
            transitionButtonLabel.text = "Transition Out";
        }


        private void OnDestroy()
        {
            // Removes the methods to the buttons in the scene via code.
            playStopButton.onClick.RemoveListener(PlayOrStop);
            pauseResumeButton.onClick.RemoveListener(PauseOrResume);
            transitionButton.onClick.RemoveListener(Transition);
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Handles the toggling of play/stop for the music track.
        /// </summary>
        private void PlayOrStop()
        {
            if (MusicPlayer.IsPlaying)
            {
                Stop();
            }
            else
            {
                Play();
            }
        }


        /// <summary>
        /// Handles the toggling of pause/resume for the music track.
        /// </summary>
        private void PauseOrResume()
        {
            if (MusicPlayer.IsPlaying)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }

        
        /// <summary>
        /// Handles the transition in/out for the music track.
        /// </summary>
        private void Transition()
        {
            if (transitionOut)
            {
                TransitionOut();
            }
            else
            {
                TransitionIn();
            }
        }

        
        /// <summary>
        /// The logic to play the track & update the demo scene visually.
        /// </summary>
        private void Play()
        {
            MusicPlayer.Play();
            playButtonLabel.text = "Stop";
            statusLabel.text = "Now Playing";
            songNameLabel.text = player.TrackList.GetTracks()[0].name;

            pauseResumeButton.interactable = true;
            transitionButton.interactable = true;
        }


        /// <summary>
        /// The logic to stop the track & update the demo scene visually.
        /// </summary>
        private void Stop()
        {
            MusicPlayer.Stop();
            playButtonLabel.text = "Play";
            statusLabel.text = "Inactive";
            songNameLabel.text = string.Empty;
            
            pauseResumeButton.interactable = false;
            transitionButton.interactable = false;
        }


        /// <summary>
        /// The logic to pause the track & update the demo scene visually.
        /// </summary>
        private void Pause()
        {
            MusicPlayer.Pause();
            pauseButtonLabel.text = "Resume";
            statusLabel.text = "Paused";
        }


        /// <summary>
        /// The logic to resume the track & update the demo scene visually.
        /// </summary>
        private void Resume()
        {
            MusicPlayer.Resume();
            pauseButtonLabel.text = "Pause";
            statusLabel.text = "Now Playing";
        }


        /// <summary>
        /// The logic to transition in the track & update the demo scene visually.
        /// </summary>
        private void TransitionIn()
        {
            if (musicTransition != null)
            {
                if (MusicPlayer.Transition != musicTransition)
                {
                    MusicPlayer.SetTransition(musicTransition);
                }
            }
            
            MusicPlayer.Transition.Completed.Add(OnTransitionCompleted);
            
            MusicPlayer.TransitionIn();
            transitionButtonLabel.text = "Transition Out";
            statusLabel.text = "Transitioning In";
            transitionOut = true;
        }
        
        
        /// <summary>
        /// The logic to transition out the track & update the demo scene visually.
        /// </summary>
        private void TransitionOut()
        {
            if (musicTransition != null)
            {
                if (MusicPlayer.Transition != musicTransition)
                {
                    MusicPlayer.SetTransition(musicTransition);
                }
            }
            
            MusicPlayer.Transition.Completed.Add(OnTransitionCompleted);
            
            MusicPlayer.TransitionOut();
            transitionButtonLabel.text = "Transition In";
            statusLabel.text = "Transitioning Out";
            transitionOut = false;
        }


        /// <summary>
        /// Runs when the in/out transition has completed.
        /// </summary>
        private void OnTransitionCompleted()
        {
            MusicPlayer.Transition.Completed.Remove(OnTransitionCompleted);
            statusLabel.text = MusicPlayer.IsPlaying ? "Now Playing" : string.Empty;
        }
    }
}