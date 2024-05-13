/*
 * Copyright (c) 2024 Carter Games
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

using CarterGames.Assets.AudioManager.Logging;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// The inspector player for music tracks with the music manager setup...
    /// </summary>
    [AddComponentMenu("Carter Games/Audio Manager/Music Track Player")]
    public class InspectorMusicTrackPlayer : MonoBehaviour
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        [SerializeField] private string trackListId;
        [SerializeField] private string firstTrackId;
        [SerializeField] private bool playInstantly;
        [SerializeField] private float volume = 1f;
        [SerializeField] private string mixerGroupId;
        [SerializeField] private AudioMixerGroup mixerGroupRef;
        [SerializeField] private MusicTransition inMusicTransition;
        [SerializeField] private float inTransitionDuration;
        
        [SerializeField] private bool showEvents;

        public UnityEvent onStarted;
        public UnityEvent onChanged;
        public UnityEvent onLooped;
        public UnityEvent onCompleted;
        
        
        private IMusicPlayer musicPlayer;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private void Start()
        {
            if (!playInstantly) return;
            Play();
        }


        private void OnDestroy()
        {
            MusicManager.Started.Remove(onStarted.Invoke);
            MusicManager.TrackChanged.Remove(onChanged.Invoke);
            MusicManager.Looped.Remove(onLooped.Invoke);
            MusicManager.Completed.Remove(onCompleted.Invoke);
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Prepares the track player for use.
        /// </summary>
        private void Prepare()
        {
            if (string.IsNullOrEmpty(firstTrackId))
            {
                musicPlayer = MusicManager.Prepare(trackListId);
            }
            else
            {
                musicPlayer = MusicManager.Prepare(trackListId, firstTrackId);
            }
            
            MusicManager.Started.Add(onStarted.Invoke);
            MusicManager.TrackChanged.Add(onChanged.Invoke);
            MusicManager.Looped.Add(onLooped.Invoke);
            MusicManager.Completed.Add(onCompleted.Invoke);

            MusicManager.PlayerVolume = volume;

            MusicManager.MusicSource.Standard.MainSource.outputAudioMixerGroup = mixerGroupRef;
            MusicManager.MusicSource.Standard.TransitionSource.outputAudioMixerGroup = mixerGroupRef;
            
            musicPlayer.DefaultVolumeTransition =
                MusicManager.GetTransitionFromEnum(inMusicTransition, inTransitionDuration);
        }
        

        /// <summary>
        /// Plays the track when called.
        /// </summary>
        public void Play()
        {
            Prepare();
            musicPlayer.Play();
        }


        /// <summary>
        /// Stops the track player when called.
        /// </summary>
        public void Stop()
        {
            if (musicPlayer == null)
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.MusicPlayerNotInitialized));
                return;
            }
            
            musicPlayer.Stop();
        }


        /// <summary>
        /// Pauses the track player when called.
        /// </summary>
        public void Pause()
        {
            if (musicPlayer == null)
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.MusicPlayerNotInitialized));
                return;
            }
            
            musicPlayer.Pause();
        }

        
        /// <summary>
        /// Resumes the track player when called.
        /// </summary>
        public void Resume()
        {
            if (musicPlayer == null)
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.MusicPlayerNotInitialized));
                return;
            }
            
            musicPlayer.Resume();
        }


        /// <summary>
        /// Changes the track to the requested track when called.
        /// </summary>
        /// <param name="key">The track to change to.</param>
        public void ChangeTrack(string key)
        {
            if (musicPlayer == null)
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.MusicPlayerNotInitialized));
                return;
            }
            
            MusicManager.MusicSource.Standard.MainSource.outputAudioMixerGroup = mixerGroupRef;
            MusicManager.MusicSource.Standard.TransitionSource.outputAudioMixerGroup = mixerGroupRef;
            
            musicPlayer.DefaultVolumeTransition = MusicManager.GetTransitionFromEnum(inMusicTransition, inTransitionDuration);
            musicPlayer.SetTrack(AssetAccessor.GetAsset<AudioLibrary>().MusicTrackLookup[trackListId].GetTrack(key));
        }
    }
}