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

using System.Collections;
using CarterGames.Assets.AudioManager.Logging;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles the music player for a single track.
    /// </summary>
    public sealed class SingleMusicTrackPlayer : IMusicPlayer
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        // Cached Id's for spelling stuff.
        private const string RoutineId = "Music-SingleTrackPlayerLifetime";
        private IMusicTransition customTransition;
        private float volume = 1f;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// The track list to play.
        /// </summary>
        public MusicPlaylist Playlist { get; set; }
        
        
        /// <summary>
        /// The default transition to use if none is set.
        /// </summary>
        public IMusicTransition DefaultVolumeTransition { get; set; } = new Fade(1f);


        /// <summary>
        /// The transition to use for the track.
        /// </summary>
        public IMusicTransition Transition
        {
            get
            {
                if (customTransition != null) return customTransition;
                if (Playlist.StartingTransition != null) return Playlist.StartingTransition;
                return DefaultVolumeTransition;
            }
        }
        
        
        /// <summary>
        /// Gets if the player is playing.
        /// </summary>
        public bool IsPlaying { get; set; }
        
        
        /// <summary>
        /// Gets if the player is transitioning.
        /// </summary>
        public bool IsTransitioning { get; set; }


        /// <summary>
        /// The base volume of the source for this player.
        /// </summary>
        public float Volume
        {
            get => volume;
            set
            {
                volume = Mathf.Clamp01(value);

                if (volume > MusicManager.MusicSource.Standard.MainSource.volume)
                {
                    MusicManager.MusicSource.Standard.MainSource.volume = volume;
                }
            }
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Sets the track to the first track.
        /// </summary>
        private void SetToFirstTrack()
        {
            MusicManager.SetPlaylist(Playlist);
            
            MusicManager.MusicSource.Standard.MainSource.playOnAwake = false;
            MusicManager.MusicSource.Standard.MainSource.loop = Playlist.TrackListLoops;
            MusicManager.MusicSource.Standard.MainSource.time = Playlist.GetStartTime(0);
            MusicManager.MusicSource.Standard.MainSource.clip = Playlist.GetTrack(0);
            MusicManager.MusicSource.Standard.MainSource.volume = Volume;
            
            MusicManager.MusicSource.Standard.MainSource.mute =
                AssetAccessor.GetAsset<SettingsAssetRuntime>().PlayMusicState == PlayState.PlayMuted;
        }
        

        /// <summary>
        /// Plays the playlist when called.
        /// </summary>
        public void Play()
        {
            Transition.Data.AddParam("musicClip", Playlist.GetTrack(0));
            Transition.Data.AddParam("musicClipStartTime", Playlist.GetStartTime(0));
            
            SetToFirstTrack();
            TransitionIn();

            if (!AssetAccessor.GetAsset<SettingsAssetRuntime>().CanPlayAudio)
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.AudioDisabled));
                return;
            }
            
            MusicManager.MusicSource.Standard.MainSource.Play();
            MusicRoutineHandler.RunRoutine(RoutineId, Co_LifetimeRoutine());
            IsPlaying = true;
            
            MusicManager.Started.Raise();
        }

        
        /// <summary>
        /// Pauses the playlist when called.
        /// </summary>
        public void Pause()
        {
            MusicManager.MusicSource.Standard.MainSource.Pause();
            IsPlaying = false;
        }
        

        /// <summary>
        /// Resumes the playlist when called.
        /// </summary>
        public void Resume()
        {
            if (!AssetAccessor.GetAsset<SettingsAssetRuntime>().CanPlayAudio)
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.AudioDisabled));
                return;
            }
            
            MusicManager.MusicSource.Standard.MainSource.Play();
            IsPlaying = true;
        }

        
        /// <summary>
        /// Stops the playlist when called.
        /// </summary>
        public void Stop()
        {
            MusicManager.MusicSource.Standard.MainSource.Stop();
            MusicRoutineHandler.StopRoutine(RoutineId);
            IsPlaying = false;
        }

        
        /// <summary>
        /// Transitions in the playlist when called.
        /// </summary>
        public void TransitionIn()
        {
            Transition.Completed.Add(OnTransitionCompleted);
            Transition.Transition(TransitionDirection.In);
            IsTransitioning = true;
        }

        
        /// <summary>
        /// Transitions out the playlist when called.
        /// </summary>
        public void TransitionOut()
        {
            Transition.Completed.Add(OnTransitionCompleted);
            Transition.Transition(TransitionDirection.Out);
            IsTransitioning = true;
        }


        /// <summary>
        /// Skips forwards a track when called.
        /// </summary>
        public void SkipForwards(bool smoothTransition = true)
        {
            if (Playlist.TrackListLoops)
            {
                MusicManager.MusicSource.Standard.MainSource.time = Playlist.GetStartTime();
                
                Transition.Data.AddParam("musicClip", Playlist.GetTrack(0));
                Transition.Data.AddParam("musicClipStartTime", Playlist.GetStartTime(0));
                
                Transition.Transition(TransitionDirection.InAndOut);
            }
            else
            {
                Stop();
            }
        }
        

        /// <summary>
        /// Skips backwards a track when called.
        /// </summary>
        /// <param name="replayCurrentFirst">Should the current track restart first? Def: true</param>
        /// <param name="smoothTransition">Should the switching transition run when going backwards. Def: true</param>
        public void SkipBackwards(bool replayCurrentFirst = true, bool smoothTransition = true)
        {
            if (MusicManager.MusicSource.Standard.MainSource.time > 3f)
            {
                // Restart clip...
                MusicManager.MusicSource.Standard.MainSource.time = Playlist.GetStartTime();
            }
            else
            {
                MusicManager.MusicSource.Standard.MainSource.time = 0f;
                
                if (!AssetAccessor.GetAsset<SettingsAssetRuntime>().CanPlayAudio)
                {
                    AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.AudioDisabled));
                    return;
                }
                
                if (!MusicManager.MusicSource.Standard.MainSource.isPlaying)
                {
                    MusicManager.MusicSource.Standard.MainSource.Play();
                }
            }

            if (!smoothTransition) return;
            
            Transition.Data.AddParam("musicClip", Playlist.GetTrack(0));
            Transition.Data.AddParam("musicClipStartTime", Playlist.GetStartTime(0));
            
            TransitionIn();
        }

        
        /// <summary>
        /// Sets the first track to be played to the entered clip if its in the track list.
        /// </summary>
        /// <param name="audioClip">The clip to try and play.</param>
        public void SetFirstTrack(AudioClip audioClip)
        {
            if (!Playlist.GetTracks().Contains(audioClip))
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.TrackClipNotInListOrLibrary));
                return;
            }
            
            MusicManager.MusicSource.Standard.MainSource.playOnAwake = false;
            MusicManager.MusicSource.Standard.MainSource.loop = Playlist.TrackListLoops;
            MusicManager.MusicSource.Standard.MainSource.time = Playlist.GetStartTime(0);
            MusicManager.MusicSource.Standard.MainSource.clip = audioClip;
            
            MusicManager.MusicSource.Standard.MainSource.mute =
                AssetAccessor.GetAsset<SettingsAssetRuntime>().PlayMusicState == PlayState.PlayMuted;
        }

        
        public void SetTrack(AudioClip audioClip)
        {
            if (!Playlist.GetTracks().Contains(audioClip))
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.TrackClipNotInListOrLibrary));
                return;
            }
            
            MusicManager.MusicSource.Standard.MainSource.playOnAwake = false;
            MusicManager.MusicSource.Standard.MainSource.loop = Playlist.TrackListLoops;
            MusicManager.MusicSource.Standard.MainSource.time = Playlist.GetStartTime(0);
            MusicManager.MusicSource.Standard.MainSource.clip = audioClip;
            
            MusicManager.MusicSource.Standard.MainSource.mute =
                AssetAccessor.GetAsset<SettingsAssetRuntime>().PlayMusicState == PlayState.PlayMuted;
            
            Transition.Transition(TransitionDirection.InAndOut);
        }

        
        public void SetTransition(IMusicTransition musicTransition)
        {
            customTransition = musicTransition;
        }


        /// <summary>
        /// Is called when the transition is completed.
        /// </summary>
        private void OnTransitionCompleted()
        {
            Transition.Completed.Remove(OnTransitionCompleted);
            
            if (Transition is CrossFade)
            {
                MusicManager.MusicSource.Standard.SwitchSourceReferences();
            }
            
            IsTransitioning = false;
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Coroutines
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Runs checks based on the current state when the player is active.
        /// </summary>
        private IEnumerator Co_LifetimeRoutine()
        {
            if (!MusicManager.MusicSource.Standard.MainSource.isPlaying)
            {
                yield break;
            }
            
            yield return null;
            
            if (MusicManager.MusicSource.Standard.MainSource.time >= Playlist.GetEndTime(0))
            {
                if (Playlist.TrackListLoops)
                {
                    MusicManager.MusicSource.Standard.MainSource.time = Playlist.GetStartTime(0);
                    MusicManager.Looped.Raise();
                }
                else
                {
                    Stop();
                    yield break;
                }
            }

            MusicManager.Completed.Raise();
            MusicRoutineHandler.RunRoutine(RoutineId, Co_LifetimeRoutine());
        }
    }
}