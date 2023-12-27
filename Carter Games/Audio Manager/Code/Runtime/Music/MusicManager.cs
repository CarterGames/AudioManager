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

using CarterGames.Assets.AudioManager.Logging;
using CarterGames.Common;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles the music setup in the audio manager.
    /// </summary>
    public static class MusicManager
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The track list that is currently playing.
        /// </summary>
        private static MusicTrackList ActiveTrackList { get; set; }
        
        
        /// <summary>
        /// The player that is currently in use.
        /// </summary>
        private static IMusicPlayer Player { get; set; }


        /// <summary>
        /// Gets the active track id.
        /// </summary>
        public static string ActiveId { get; private set; }
        
        
        /// <summary>
        /// Gets the active clip being played.
        /// </summary>
        public static AudioClip ActiveClip => MusicSource.Standard.MainSource.clip;

        
        /// <summary>
        /// Gets if the player is playing.
        /// </summary>
        public static bool IsPlaying
        {
            get
            {
                if (Player == null) return false;
                return Player.IsPlaying;
            }
        }

        
        /// <summary>
        /// Gets if the player is transitioning.
        /// </summary>
        public static bool IsTransitioning
        {
            get
            {
                if (Player == null) return false;
                return Player.IsTransitioning;
            }
        }


        /// <summary>
        /// Updates the volume of the sources when called.
        /// </summary>
        public static float PlayerVolume
        {
            get => Player.Volume;
            set => Player.Volume = value;
        }
        

        /// <summary>
        /// The music source handler reference for use.
        /// </summary>
        public static MusicSourceHandler MusicSource => DoNotDestroyHandler.MusicSourceHandler;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Events
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Raises when the player starts playing initially.
        /// </summary>
        public static readonly Evt Started = new Evt();
        
        
        /// <summary>
        /// Raises when the player paused playing.
        /// </summary>
        public static readonly Evt Paused = new Evt();
                
        
        /// <summary>
        /// Raises when the player resumed playing.
        /// </summary>
        public static readonly Evt Resumed = new Evt();
        
        
        /// <summary>
        /// Raises when the player stopped playing.
        /// </summary>
        public static readonly Evt Stopped = new Evt();
        
        
        /// <summary>
        /// Raises when the player has looped playing.
        /// </summary>
        public static readonly Evt Looped = new Evt();
        
        
        /// <summary>
        /// Raises when the player has completed playing.
        /// </summary>
        public static readonly Evt Completed = new Evt();
        
        
        /// <summary>
        /// Raises when the player changes the track.
        /// </summary>
        public static readonly Evt TrackChanged = new Evt();
        
        
        /// <summary>
        /// Raises when a transition is started.
        /// </summary>
        public static readonly Evt TransitionStarted = new Evt();
        
        
        /// <summary>
        /// Raises when a transition is started with ctx.
        /// </summary>
        public static readonly Evt<string, IMusicTransition> TransitionStartedCtx = new Evt<string, IMusicTransition>();
        
        
        /// <summary>
        /// Raises when a transition is completed.
        /// </summary>
        public static readonly Evt TransitionCompleted = new Evt();
        
        
        /// <summary>
        /// Raises when a transition is completed with ctx.
        /// </summary>
        public static readonly Evt<string, IMusicTransition> TransitionCompletedCtx = new Evt<string, IMusicTransition>();

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Prepares a player for use, but doesn't play it.
        /// </summary>
        /// <param name="id">The track list id to get.</param>
        /// <returns>The player prepared for use.</returns>
        public static IMusicPlayer Prepare(string id)
        {
            var list = AssetAccessor.GetAsset<AudioLibrary>().GetTrackList(id);
            
            if (list != null)
            {
                ActiveTrackList = list;
            }
            else
            {
                AmLog.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.TrackListCannotBeFound));
                return null;
            }
            
            var player = ActiveTrackList.GetMusicPlayer();

            player.TrackList = ActiveTrackList;

            return player;
        }
        
        
        /// <summary>
        /// Prepares a player for use, but doesn't play it.
        /// </summary>
        /// <param name="id">The track list id to get.</param>
        /// <param name="firstTrack">The track clip name to first play.</param>
        /// <returns>The player prepared for use.</returns>
        public static IMusicPlayer Prepare(string id, string firstTrack)
        {
            var trackList = AssetAccessor.GetAsset<AudioLibrary>().GetTrackList(id);
            
            if (trackList != null)
            {
                ActiveTrackList = trackList;
            }
            else
            {
                AmLog.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.TrackListCannotBeFound));
                return null;
            }
            
            var player = ActiveTrackList.GetMusicPlayer();
            
            player.TrackList = ActiveTrackList;
            player.SetFirstTrack(ActiveTrackList.GetTrack(firstTrack));
            ActiveId = firstTrack;
                
            return player;
        }
        
        
        /// <summary>
        /// Prepares a player for use, but doesn't play it.
        /// </summary>
        /// <param name="id">The track list id to get.</param>
        /// <param name="firstTrackIndex">The index of the first track to play in the list.</param>
        /// <returns>The player prepared for use.</returns>
        public static IMusicPlayer Prepare(string id, int firstTrackIndex)
        {
            var trackList = AssetAccessor.GetAsset<AudioLibrary>().GetTrackList(id);
            
            if (trackList != null)
            {
                ActiveTrackList = trackList;
            }
            else
            {
                AmLog.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.TrackListCannotBeFound));
                return null;
            }
            
            var player = ActiveTrackList.GetMusicPlayer();

            player.TrackList = ActiveTrackList;
            player.SetFirstTrack(ActiveTrackList.GetTracks()[firstTrackIndex]);
            ActiveId = ActiveTrackList.GetTracksRaw()[firstTrackIndex].ClipId;
                
            return player;
        }
        
        
        /// <summary>
        /// Plays the music track list of the entered id.
        /// </summary>
        /// <param name="id">The id to play.</param>
        /// <param name="volume">The volume for the audio source to play at.</param>
        public static void Play(string id, float volume = 1f)
        {
            if (!AssetAccessor.GetAsset<SettingsAssetRuntime>().CanPlayMusic)
            {
                AmLog.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.MusicDisabled));
                return;
            }

            var trackList = AssetAccessor.GetAsset<AudioLibrary>().GetTrackList(id);
            
            if (trackList != null)
            {
                var player = Prepare(id);
                player.Volume = volume;
                
                Play(trackList);
            }
            else
            {
                AmLog.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.TrackListCannotBeFound));
            }
        }


        /// <summary>
        /// Plays the music track list of the entered id.
        /// </summary>
        /// <param name="trackList">The list to play.</param>
        /// <param name="volume">The volume for the audio source to play at.</param>
        private static void Play(MusicTrackList trackList, float volume = 1f)
        {
            if (!AssetAccessor.GetAsset<SettingsAssetRuntime>().CanPlayMusic)
            {
                AmLog.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.MusicDisabled));
                return;
            }

            if (trackList == null)
            {
                AmLog.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.TrackListCannotBeFound));
                return;
            }

            ActiveTrackList = trackList;
            
            Player = ActiveTrackList.GetMusicPlayer();
            Player.Volume = volume;

            Player.TrackList = ActiveTrackList;
            Player.TransitionIn();
            Player.Play();
        }


        /// <summary>
        /// Skips the current track and moves to the next if possible.
        /// </summary>
        public static void SkipForward()
        {
            Player.SkipForwards();
        }


        /// <summary>
        /// Skips backwards a track, replaying the current is set.
        /// </summary>
        /// <param name="replayCurrentFirst">Should the current track restart first? Def: true</param>
        /// <param name="transitionIn">Should the switching transition run when going backwards. Def: true</param>
        public static void SkipBackwards(bool replayCurrentFirst, bool transitionIn)
        {
            Player.SkipBackwards(replayCurrentFirst, transitionIn);
        }
        

        /// <summary>
        /// Pauses the active player when called.
        /// </summary>
        public static void Pause()
        {
            Player.Pause();
            Paused.Raise();
        }
        
        
        /// <summary>
        /// Resumes the active player when called.
        /// </summary>
        public static void Resume()
        {
            Player.Resume();
            Resumed.Raise();
        }


        /// <summary>
        /// Stops the active player when called.
        /// </summary>
        public static void Stop()
        {
            if (!IsPlaying) return;
            Player.Stop();
            Stopped.Raise();
        }


        /// <summary>
        /// Transitions in the active player when called.
        /// </summary>
        public static void TransitionIn()
        {
            Player.TransitionIn();
        }
        
        
        /// <summary>
        /// Transitions out the active player when called.
        /// </summary>
        public static void TransitionOut()
        {
            Player.TransitionOut();
        }
        

        /// <summary>
        /// Transitions the active player when called to the new clip.
        /// </summary>
        /// <param name="clip">The clip to transition to.</param>
        public static void TransitionTo(string clip)
        {
            if (!ActiveTrackList.HasTrack(clip))
            {   
                AmLog.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.TrackNotInList));
                return;
            }
            
            ActiveId = clip;
            
            Player.Transition.Data.AddParam("musicClip", ActiveTrackList.GetTrack(clip));
            Player.Transition.Data.AddParam("musicClipStartTime", ActiveTrackList.GetStartTime(clip));
            
            Player.DefaultVolumeTransition.Data.AddParam("musicClip", ActiveTrackList.GetTrack(clip));
            Player.DefaultVolumeTransition.Data.AddParam("musicClipStartTime", ActiveTrackList.GetStartTime(clip));
            
            Player.Transition.Transition(TransitionDirection.InAndOut);
        }
        
        
        /// <summary>
        /// Transitions the active player when called to the new clip.
        /// </summary>
        /// <param name="clip">The clip to transition to.</param>
        /// <param name="musicTransition">The transition to use.</param>
        /// <param name="duration">The duration for the transition. Def: 1f</param>
        public static void TransitionTo(string clip, MusicTransition musicTransition, float duration = 1f)
        {
            TransitionTo(clip, GetTransitionFromEnum(musicTransition, duration));
        }
        
        
        /// <summary>
        /// Transitions the active player when called to the new clip.
        /// </summary>
        /// <param name="clip">The clip to transition to.</param>
        /// <param name="musicTransition">The transition to use.</param>
        /// <param name="duration">The duration for the transition. Def: 1f</param>
        public static void TransitionTo(string clip, IMusicTransition musicTransition, float duration = 1f)
        {
            // Debug.Log(Player);
            
            if (!ActiveTrackList.HasTrack(clip))
            {   
                AmLog.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.TrackNotInList));
                return;
            }

            ActiveId = clip;
            musicTransition.Data.AddParam("musicClip", ActiveTrackList.GetTrack(clip));
            musicTransition.Data.AddParam("musicClipStartTime", ActiveTrackList.GetStartTime(clip));
            musicTransition.Data.AddParam("duration", duration);
            musicTransition.Transition(TransitionDirection.InAndOut);
        }


        /// <summary>
        /// Gets the music transition from the enum for the transitions.
        /// </summary>
        /// <param name="musicTransition">The transition to get.</param>
        /// <param name="duration">The duration of the transition.</param>
        /// <returns>The transition received.</returns>
        public static IMusicTransition GetTransitionFromEnum(MusicTransition musicTransition, float duration)
        {
            switch (musicTransition)
            {
                case MusicTransition.Cut:
                default:
                    return new Cut();
                case MusicTransition.Fade:
                    return new Fade(duration);
                case MusicTransition.CrossFade:
                    return new CrossFade(duration);
            }
        }

        
        /// <summary>
        /// Sets the active track list and player, but nothing else.
        /// </summary>
        /// <param name="trackList">The track list to use.</param>
        public static void SetTrackList(MusicTrackList trackList)
        {
            if (ActiveTrackList != trackList)
            {
                ActiveTrackList = trackList;
            }

            if (Player != null) return;
            Player = ActiveTrackList.GetMusicPlayer();
        }
    }
}