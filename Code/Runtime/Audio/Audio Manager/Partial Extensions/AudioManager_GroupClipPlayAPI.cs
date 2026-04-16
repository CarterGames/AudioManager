/*
 * Audio Manager (3.x)
 * Copyright (c) Carter Games
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the
 * GNU General Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version. 
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
 *
 * You should have received a copy of the GNU General Public License along with this program.
 * If not, see <https://www.gnu.org/licenses/>. 
 */

using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    public static partial class AudioManager
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Play Methods (Group)
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroup(string request, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, edits);
            player.Play();
            return player;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="delay">The start delay for the clip.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroupWithDelay(string request, float delay, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, new DelayEdit(delay), edits);
            player.Play();
            return player;
        }

        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroupAtLocation(string request, Vector2 position, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, new PositionEdit(position), edits);
            player.Play();
            return player;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroupAtLocation(string request, Vector3 position, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, new PositionEdit(position), edits);
            player.Play();
            return player;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="useLocalPosition">Should use the local position on the inputted transform?.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroupAtLocation(string request, Transform position, bool useLocalPosition, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, new PositionEdit(position, useLocalPosition), edits);
            player.Play();
            return player;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroup(string request, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, new IEditModule[] { new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f) }, edits);
            player.Play();
            return player;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="delay">The start delay for the clip.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroupWithDelay(string request, float delay, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, new IEditModule[]
            { 
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new DelayEdit(delay)
            }, edits);
            
            player.Play();
            return player;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroupAtLocation(string request, Vector2 position, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, new IEditModule[]
            {
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new PositionEdit(position)
            }, edits);
            
            player.Play();
            return player;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroupAtLocation(string request, Vector3 position, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, new IEditModule[]
            {
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new PositionEdit(position)
            }, edits);
            
            player.Play();
            return player;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="useLocalPosition">Should use the local position on the inputted transform?.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroupAtLocation(string request, Transform position, bool useLocalPosition, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, new IEditModule[]
            {
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new PositionEdit(position, useLocalPosition)
            }, edits);
            
            player.Play();
            return player;
        }
        
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroup(string[] request, GroupPlayMode playMode, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, playMode, edits);
            player.Play();
            return player;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="delay">The start delay for the clip.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroupWithDelay(string[] request, GroupPlayMode playMode, float delay, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, playMode, new DelayEdit(delay), edits);
            player.Play();
            return player;
        }
        

        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroupAtLocation(string[] request, GroupPlayMode playMode, Vector2 position, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, playMode, new PositionEdit(position), edits);
            player.Play();
            return player;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroupAtLocation(string[] request, GroupPlayMode playMode, Vector3 position, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, playMode, new PositionEdit(position), edits);
            player.Play();
            return player;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="useLocalPosition">Should use the local position on the inputted transform?.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroupAtLocation(string[] request, GroupPlayMode playMode, Transform position, bool useLocalPosition, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, playMode, new PositionEdit(position, useLocalPosition), edits);
            player.Play();
            return player;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroup(string[] request, GroupPlayMode playMode, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, playMode, new IEditModule[] { new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f) }, edits);
            player.Play();
            return player;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="delay">The start delay for the clip.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroupWithDelay(string[] request, GroupPlayMode playMode, float delay, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, playMode, new IEditModule[]
            { 
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new DelayEdit(delay)
            }, edits);
            
            player.Play();
            return player;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroupAtLocation(string[] request, GroupPlayMode playMode, Vector2 position, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, playMode, new IEditModule[]
            {
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new PositionEdit(position)
            }, edits);
            
            player.Play();
            return player;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroupAtLocation(string[] request, GroupPlayMode playMode, Vector3 position, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, playMode, new IEditModule[]
            {
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new PositionEdit(position)
            }, edits);
            
            player.Play();
            return player;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="useLocalPosition">Should use the local position on the inputted transform?.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayGroupAtLocation(string[] request, GroupPlayMode playMode, Transform position, bool useLocalPosition, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var player = PrepareGroupBase(request, playMode, new IEditModule[]
            {
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new PositionEdit(position, useLocalPosition)
            }, edits);
            
            player.Play();
            return player;
        }
    }
}