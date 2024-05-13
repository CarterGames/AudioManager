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
        private static MusicPlaylist ActivePlaylist { get; set; } = null;
        
        
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


        private static bool CanUse => ActivePlaylist != null;
        
        
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
        /// <param name="playlistId">The track list id to get.</param>
        /// <returns>The player prepared for use.</returns>
        public static IMusicPlayer Prepare(string playlistId)
        {
            var list = AssetAccessor.GetAsset<AudioLibrary>().GetTrackList(playlistId);
            
            if (list != null)
            {
                ActivePlaylist = list;
            }
            else
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.TrackListCannotBeFound));
                return null;
            }
            
            var player = ActivePlaylist.GetMusicPlayer();

            player.Playlist = ActivePlaylist;
            
            return player;
        }
        
        
        /// <summary>
        /// Prepares a player for use, but doesn't play it.
        /// </summary>
        /// <param name="playlistId">The track list id to get.</param>
        /// <param name="firstTrack">The track clip name to first play.</param>
        /// <returns>The player prepared for use.</returns>
        public static IMusicPlayer Prepare(string playlistId, string firstTrack)
        {
            var trackList = AssetAccessor.GetAsset<AudioLibrary>().GetTrackList(playlistId);
            
            if (trackList != null)
            {
                ActivePlaylist = trackList;
            }
            else
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.TrackListCannotBeFound));
                return null;
            }
            
            var player = ActivePlaylist.GetMusicPlayer();
            
            player.Playlist = ActivePlaylist;
            player.SetFirstTrack(ActivePlaylist.GetTrack(firstTrack));
            ActiveId = firstTrack;
                
            return player;
        }
        
        
        /// <summary>
        /// Prepares a player for use, but doesn't play it.
        /// </summary>
        /// <param name="playlistId">The track list id to get.</param>
        /// <param name="firstTrackIndex">The index of the first track to play in the list.</param>
        /// <returns>The player prepared for use.</returns>
        public static IMusicPlayer Prepare(string playlistId, int firstTrackIndex)
        {
            var trackList = AssetAccessor.GetAsset<AudioLibrary>().GetTrackList(playlistId);
            
            if (trackList != null)
            {
                ActivePlaylist = trackList;
            }
            else
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.TrackListCannotBeFound));
                return null;
            }
            
            var player = ActivePlaylist.GetMusicPlayer();

            player.Playlist = ActivePlaylist;
            player.SetFirstTrack(ActivePlaylist.GetTracks()[firstTrackIndex]);
            ActiveId = ActivePlaylist.GetTracksRaw()[firstTrackIndex].ClipId;
                
            return player;
        }
        
        
        /// <summary>
        /// Plays the music track list of the entered id.
        /// </summary>
        /// <param name="playlistId">The id to play.</param>
        /// <param name="volume">The volume for the audio source to play at.</param>
        public static void Play(string playlistId, float volume = 1f)
        {
            if (!AssetAccessor.GetAsset<SettingsAssetRuntime>().CanPlayMusic)
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.MusicDisabled));
                return;
            }

            var trackList = AssetAccessor.GetAsset<AudioLibrary>().GetTrackList(playlistId);
            
            if (trackList != null)
            {
                var player = Prepare(playlistId);
                player.Volume = volume;
                
                Play(trackList);
            }
            else
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.TrackListCannotBeFound));
            }
        }


        /// <summary>
        /// Plays the music track list of the entered id.
        /// </summary>
        /// <param name="playlist">The list to play.</param>
        /// <param name="volume">The volume for the audio source to play at.</param>
        private static void Play(MusicPlaylist playlist, float volume = 1f)
        {
            if (!AssetAccessor.GetAsset<SettingsAssetRuntime>().CanPlayMusic)
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.MusicDisabled));
                return;
            }

            if (playlist == null)
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.TrackListCannotBeFound));
                return;
            }

            ActivePlaylist = playlist;
            
            Player = ActivePlaylist.GetMusicPlayer();
            Player.Volume = volume;

            Player.Playlist = ActivePlaylist;
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
        public static void TransitionPlayerIn()
        {
            Player.TransitionIn();
        }
        
        
        /// <summary>
        /// Transitions out the active player when called.
        /// </summary>
        public static void TransitionPlayerOut()
        {
            Player.TransitionOut();
        }
        

        /// <summary>
        /// Transitions the active player when called to the new clip.
        /// </summary>
        /// <param name="trackId">The clip to transition to.</param>
        public static void TransitionToTrack(string trackId)
        {
            if (!CanUse)
            {
                AmDebugLogger.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.MusicPlaylistNotSet));
                return;
            }
            
            if (!ActivePlaylist.LibraryHasTrackClip(trackId) && !ActivePlaylist.TrackIsInList(trackId))
            {   
                AmDebugLogger.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.TrackClipNotInListOrLibrary));
                return;
            }
            
            ActiveId = trackId;
            
            Player.Transition.Data.AddParam("musicClip", ActivePlaylist.GetTrack(trackId));
            Player.Transition.Data.AddParam("musicClipStartTime", ActivePlaylist.GetStartTime(trackId));
            
            Player.DefaultVolumeTransition.Data.AddParam("musicClip", ActivePlaylist.GetTrack(trackId));
            Player.DefaultVolumeTransition.Data.AddParam("musicClipStartTime", ActivePlaylist.GetStartTime(trackId));
            
            Player.Transition.Transition(TransitionDirection.InAndOut);
        }
        
        
        /// <summary>
        /// Transitions the active player when called to the new clip.
        /// </summary>
        /// <param name="trackId">The clip to transition to.</param>
        /// <param name="musicTransition">The transition to use.</param>
        /// <param name="duration">The duration for the transition. Def: 1f</param>
        public static void TransitionToTrack(string trackId, MusicTransition musicTransition, float duration = 1f)
        {
            TransitionToTrack(trackId, GetTransitionFromEnum(musicTransition, duration), duration);
        }
        
        
        /// <summary>
        /// Transitions the active player when called to the new clip.
        /// </summary>
        /// <param name="trackId">The clip to transition to.</param>
        /// <param name="musicTransition">The transition to use.</param>
        /// <param name="duration">The duration for the transition. Def: 1f</param>
        public static void TransitionToTrack(string trackId, IMusicTransition musicTransition, float duration = 1f)
        {
            if (!CanUse)
            {
                AmDebugLogger.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.MusicPlaylistNotSet));
                return;
            }
            
            if (!ActivePlaylist.LibraryHasTrackClip(trackId) && !ActivePlaylist.TrackIsInList(trackId))
            {   
                AmDebugLogger.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.TrackClipNotInListOrLibrary));
                return;
            }

            ActiveId = trackId;
            musicTransition.Data.AddParam("musicClip", ActivePlaylist.GetTrack(trackId));
            musicTransition.Data.AddParam("musicClipStartTime", ActivePlaylist.GetStartTime(trackId));
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
        /// <param name="playlistId">The track list to use.</param>
        public static void SetPlaylist(string playlistId)
        {
            SetPlaylist(AssetAccessor.GetAsset<AudioLibrary>().GetTrackList(playlistId));
        }
        
        
        /// <summary>
        /// Sets the active track list and player, but nothing else.
        /// </summary>
        /// <param name="playlist">The track list to use.</param>
        public static void SetPlaylist(MusicPlaylist playlist)
        {
            if (playlist == null)
            {
                AmDebugLogger.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.TrackListCannotBeFound));
                return;
            }
            
            if (ActivePlaylist != playlist)
            {
                ActivePlaylist = playlist;
            }

            if (Player != null) return;
            Player = ActivePlaylist.GetMusicPlayer();
        }
    }
}