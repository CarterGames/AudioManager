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

using CarterGames.Shared.AudioManager;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles a random clip player with a random clip from the group playing when called to.
    /// </summary>
    public sealed class GroupSourcePlayerRandom : IPlayMethod
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private readonly GroupData groupData;
        private readonly AudioPlayer player;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Constructors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Create a random sequence with the entered data.
        /// </summary>
        /// <param name="player">The sequence to use.</param>
        /// <param name="groupData">The data to use.</param>
        private GroupSourcePlayerRandom(AudioPlayer player, GroupData groupData)
        {
            this.player = player;
            this.groupData = groupData;
        }


        public static GroupSourcePlayerRandom InitializePlayMethod(AudioPlayer player, GroupData groupData,
            AudioClipSettings clipSettings = null)
        {
            var playMethodHandler = new GroupSourcePlayerRandom(player, groupData);
            
            // Random - play 1 clip - when looping play another random.
            var clip = groupData.Clips[Random.Range(0, groupData.Clips.Count - 1)];

            if (!AmAssetAccessor.GetAsset<AudioLibrary>().TryGetClip(clip, out var data)) return null;
            
            player.Source.InitializePlayer(player, data, clipSettings);
            player.Source.Completed.Remove(player.PlayerComplete);
            player.Source.Completed.Add(player.PlayerComplete);
            
            return playMethodHandler;
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Plays the sequence when called.
        /// </summary>
        public void Play()
        {
            player.Source.PlaySourceInstance();
        }
        
        
        /// <summary>
        /// Pauses the sequence when called.
        /// </summary>
        public void Pause()
        {
            player.Source.PauseSourceInstance();
        }

        
        /// <summary>
        /// Resumes the sequence when called.
        /// </summary>
        public void Resume()
        {
            player.Source.ResumeSourceInstance();
        }


        /// <summary>
        /// Stops the sequence when called.
        /// </summary>
        public void Stop()
        {
            player.Source.StopSourceInstance();
        }

        
        /// <summary>
        /// Runs when the sequence loops.
        /// </summary>
        public void OnLoop()
        {
            var clip = groupData.Clips[Random.Range(0, groupData.Clips.Count - 1)];

            if (!AmAssetAccessor.GetAsset<AudioLibrary>().TryGetClip(clip, out var data)) return;
            player.Source.InitializePlayer(player, data);
        }
    }
}