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
using System.Collections.Generic;
using CarterGames.Assets.AudioManager.Logging;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles the music player for playlist tracks.
    /// </summary>
    public sealed class PlaylistMusicTrackPlayer : IMusicPlayer
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        // Cached Id's for spelling stuff.
        private const string RoutineId = "Music-PlaylistTrackPlayerLifetime";
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
        public IMusicTransition DefaultVolumeTransition { get; set; } = new CrossFade(1f);

        
        /// <summary>
        /// Gets the starting track index.
        /// </summary>
        private int StartingTrackIndex { get; set; }
        
        
        /// <summary>
        /// Gets the current track index.
        /// </summary>
        private int CurrentIndex { get; set; }
        
        
        /// <summary>
        /// Gets the played track indexes.
        /// </summary>
        private List<int> PlayedIndexes { get; set; } = new List<int>();
        
        
        /// <summary>
        /// Gets the number of tracks played.
        /// </summary>
        private int TracksPlayed { get; set; }
        
        
        /// <summary>
        /// Gets if the playlist player is at the end of the playlist or not.
        /// </summary>
        private bool IsAtEnd => Playlist.GetTracks().Count.Equals(TracksPlayed);


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
                volume = value;

                if (volume > MusicManager.MusicSource.Standard.MainSource.volume)
                {
                    MusicManager.MusicSource.Standard.MainSource.volume = value;
                }
            }
        }
        
        
        /// <summary>
        /// Gets if the player can transition.
        /// </summary>
        private bool CanTransition { get; set; } = true;

        
        /// <summary>
        /// Gets the current transition to use based on the current context.
        /// </summary>
        public IMusicTransition Transition
        {
            get
            {
                if (TracksPlayed.Equals(0))
                {
                    if (customTransition != null) return customTransition;
                    if (Playlist.StartingTransition != null) return Playlist.StartingTransition;
                    return DefaultVolumeTransition;
                }
                else
                {
                    if (customTransition != null) return customTransition;
                    if (Playlist.SwitchingTransition != null) return Playlist.SwitchingTransition;
                    return DefaultVolumeTransition;
                }
            }
        }
        
        
        /// <summary>
        /// The timeout applied between end of playlist checks.
        /// </summary>
        private float EndCheckTimeout { get; set; }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Sets the track to the first track.
        /// </summary>
        private void SetToFirstTrack()
        {
            MusicManager.MusicSource.Standard.MainSource.playOnAwake = false;
            MusicManager.MusicSource.Standard.MainSource.loop = Playlist.TrackListLoops;
            MusicManager.MusicSource.Standard.MainSource.time = Playlist.GetStartTime(StartingTrackIndex);
            MusicManager.MusicSource.Standard.MainSource.clip = Playlist.GetTrack(StartingTrackIndex);
            CurrentIndex = StartingTrackIndex;
        }


        /// <summary>
        /// Plays the playlist when called.
        /// </summary>
        public void Play()
        {
            if (Playlist.PlayListShuffled)
            {
                StartingTrackIndex = Random.Range(0, Playlist.GetTracks().Count);
            }
            else
            {
                StartingTrackIndex = 0;
            }
            
            SetToFirstTrack();
            TransitionIn();
            
            if (!AssetAccessor.GetAsset<SettingsAssetRuntime>().CanPlayAudio)
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.AudioDisabled));
                return;
            }

            MusicManager.MusicSource.Standard.MainSource.volume = Volume;
            MusicManager.MusicSource.Standard.MainSource.Play();
            MusicRoutineHandler.RunRoutine(RoutineId, Co_LifetimeRoutine());
            IsPlaying = true;
            
            PlayedIndexes.Add(Playlist.GetTracks().IndexOf(MusicManager.MusicSource.Standard.MainSource.clip));
            
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
        }

        
        /// <summary>
        /// Transitions out the playlist when called.
        /// </summary>
        public void TransitionOut()
        {
            Transition.Completed.Add(OnTransitionCompleted);
            Transition.Transition(TransitionDirection.Out);
        }


        /// <summary>
        /// Is called when the transition is completed.
        /// </summary>
        private void OnTransitionCompleted()
        {
            Transition.Completed.Remove(OnTransitionCompleted);
            IsTransitioning = false;
        }
        

        /// <summary>
        /// Skips forwards a track when called.
        /// </summary>
        public void SkipForwards(bool smoothTransition = true)
        {
            PlayedIndexes.Add(CurrentIndex);

            
            if (Playlist.PlayListShuffled)
            {
                CurrentIndex = GetRandomUnPlayedTrack();
            }
            else
            {
                CurrentIndex++;
            }
            
            
            if (smoothTransition)
            {
                Transition.Data.AddParam("musicClip", Playlist.GetTrack(CurrentIndex));
                Transition.Data.AddParam("musicClipStartTime", Playlist.GetStartTime(CurrentIndex));

                Transition.Transition(TransitionDirection.InAndOut);
            }
            else
            {
                MusicManager.MusicSource.Standard.MainSource.clip = Playlist.GetTrack(CurrentIndex);
                MusicManager.MusicSource.Standard.MainSource.time = Playlist.GetStartTime(CurrentIndex);
                
                TransitionIn();
            }
            
            
            MusicManager.TrackChanged.Raise();
        }


        /// <summary>
        /// Skips backwards a track when called.
        /// </summary>
        /// <param name="replayCurrentFirst">Should the current track restart first? Def: true</param>
        /// <param name="smoothTransition">Should the switching transition run when going backwards. Def: true</param>
        public void SkipBackwards(bool replayCurrentFirst = true, bool smoothTransition = true)
        {
            if (replayCurrentFirst)
            {
                if (MusicManager.MusicSource.Standard.MainSource.time > 3f)
                {
                    // Restart clip...
                    MusicManager.MusicSource.Standard.MainSource.time = Playlist.GetStartTime(CurrentIndex);
                    
                    if (smoothTransition)
                    {
                        TransitionIn();
                    }
                    
                    if (!MusicManager.MusicSource.Standard.MainSource.isPlaying)
                    {
                        MusicManager.MusicSource.Standard.MainSource.Play();
                    }
                    
                    return;
                }
            }

            var toPlay = PlayedIndexes[PlayedIndexes.Count - 1];
            PlayedIndexes.Remove(toPlay);
            CurrentIndex = toPlay;
            
            
            if (smoothTransition)
            {
                Transition.Data.AddParam("musicClip", Playlist.GetTrack(CurrentIndex));
                Transition.Data.AddParam("musicClipStartTime", Playlist.GetStartTime(CurrentIndex));

                Transition.Transition(TransitionDirection.InAndOut);
            }
            else
            {
                MusicManager.MusicSource.Standard.MainSource.clip = Playlist.GetTrack(CurrentIndex);
                MusicManager.MusicSource.Standard.MainSource.time = Playlist.GetStartTime(CurrentIndex);

                TransitionIn();
            }
            
            if (!MusicManager.MusicSource.Standard.MainSource.isPlaying)
            {
                MusicManager.MusicSource.Standard.MainSource.Play();
            }
            
            MusicManager.TrackChanged.Raise();
        }
        
        
        /// <summary>
        /// Gets an un-played track to play (only used when shuffled).
        /// </summary>
        /// <returns></returns>
        private int GetRandomUnPlayedTrack()
        {
            var options = new List<int>();

            for (var i = 0; i < Playlist.GetTracks().Count; i++)
            {
                if (PlayedIndexes.Contains(i)) continue;
                options.Add(i);
            }

            if (options.Count.Equals(0))
            {
                return -1;
            }
            
            return options[Random.Range(0, options.Count - 1)];
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
            
            MusicManager.SetPlaylist(Playlist);
            
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

            MusicManager.SetPlaylist(Playlist);
            
            var indexOfTrack = Playlist.GetTracks().IndexOf(audioClip);
            
            MusicManager.MusicSource.Standard.MainSource.playOnAwake = false;
            MusicManager.MusicSource.Standard.MainSource.loop = Playlist.TrackListLoops;
            MusicManager.MusicSource.Standard.MainSource.time = Playlist.GetStartTime(indexOfTrack);
            MusicManager.MusicSource.Standard.MainSource.clip = audioClip;
            
            MusicManager.MusicSource.Standard.MainSource.mute =
                AssetAccessor.GetAsset<SettingsAssetRuntime>().PlayMusicState == PlayState.PlayMuted;
            
            Transition.Transition(TransitionDirection.InAndOut);
        }

        
        public void SetTransition(IMusicTransition musicTransition)
        {
            customTransition = musicTransition;
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Coroutines
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Runs checks based on the current state when the player is active.
        /// </summary>
        private IEnumerator Co_LifetimeRoutine()
        {
            while (true)
            {
                if (!Mathf.Approximately(EndCheckTimeout, 0f))
                {
                    EndCheckTimeout -= Time.unscaledDeltaTime;
                    EndCheckTimeout = Mathf.Clamp(EndCheckTimeout, 0f, float.MaxValue);
                }

                yield return null;

                if (MusicManager.MusicSource.Standard.MainSource.time >= Playlist.GetEndTime(CurrentIndex))
                {
                    if (Mathf.Approximately(EndCheckTimeout, 0f))
                    {
                        TracksPlayed++;
                    }
                    
                    
                    if (IsAtEnd)
                    {
                        if (Playlist.TrackListLoops && !Transition.InProgress)
                        {
                            CurrentIndex = StartingTrackIndex;
                            
                            PlayedIndexes.Clear();
                            PlayedIndexes.Add(CurrentIndex);
                            
                            Transition.Data.AddParam("musicClip", Playlist.GetTrack(CurrentIndex));
                            Transition.Data.AddParam("musicClipStartTime", Playlist.GetStartTime(CurrentIndex));
                            Transition.Completed.Add(OnTransitionCompleted);
                            Transition.Transition(TransitionDirection.InAndOut);

                            MusicManager.TrackChanged.Raise();
                            
                            EndCheckTimeout = Playlist.GetTrack(CurrentIndex).length / 20f;
                            CanTransition = true;
                            TracksPlayed = 0;
                        }
                        else
                        {
                            if (MusicManager.MusicSource.Standard.MainSource.time >= Playlist.GetEndTime(CurrentIndex) - Transition.Data.GetParam<float>("Duration"))
                            {
                                TransitionOut();
                                CanTransition = false;
                            }
                            
                            yield break;
                        }
                    }
                    else if (Mathf.Approximately(EndCheckTimeout, 0f))
                    {
                        if (!Transition.InProgress)
                        {
                            PlayedIndexes.Add(CurrentIndex);

                            if (Playlist.PlayListShuffled)
                            {
                                CurrentIndex = GetRandomUnPlayedTrack();
                            }
                            else
                            {
                                CurrentIndex++;
                            }

                            EndCheckTimeout = Playlist.GetTrack(CurrentIndex).length / 20f;
                            
                            Transition.Data.AddParam("musicClip", Playlist.GetTrack(CurrentIndex));
                            Transition.Data.AddParam("musicClipStartTime", Playlist.GetStartTime(CurrentIndex));
                            Transition.Completed.Add(OnTransitionCompleted);
                            Transition.Transition(TransitionDirection.InAndOut);
                            
                            CanTransition = true;
                        }
                    }
                }
            }
        }
    }
}