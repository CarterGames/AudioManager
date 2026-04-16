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

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles a combined clip player with all the clips playing together.
    /// </summary>
    public sealed class CombinedGroupRequestSequence : IPlayMethod
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private readonly GroupData groupData;
        private readonly AudioClipSettings clipSettings;
        private readonly AudioPlayer player;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Constructors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Creates a combined sequence with the entered data.
        /// </summary>
        /// <param name="player">The sequence to use.</param>
        /// <param name="groupData">The data to use.</param>
        /// <param name="clipSettings">The settings to apply.</param>
        public CombinedGroupRequestSequence(AudioPlayer player, GroupData groupData, AudioClipSettings clipSettings)
        {
            this.player = player;
            this.groupData = groupData;
            this.clipSettings = clipSettings;
            
            Setup();
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Sets up the sequence for use.
        /// </summary>
        private void Setup()
        {
            if (player.AllSources.Count < groupData.Clips.Count)
            {
                var totalRequired = groupData.Clips.Count - player.AllSources.Count;
                
                for (var i = 0; i < totalRequired; i++)
                {
                    player.AssignNewInstance();
                }
            }

            for (var i = 0; i < player.AllSources.Count; i++)
            {
                if (!AmAssetAccessor.GetAsset<AudioLibrary>().TryGetClip(groupData.Clips[i], out var data)) continue;
                player.AllSources[i].InitializePlayer(player, data, clipSettings);
                
                player.AllSources[i].Completed.Remove(player.PlayerComplete);
                player.AllSources[i].Completed.Add(player.PlayerComplete);
            }
        }
        
        
        /// <summary>
        /// Plays the sequence when called.
        /// </summary>
        public void Play()
        {
            foreach (var sourceInstance in player.AllSources)
            {
                sourceInstance.PlaySourceInstance();
            }
        }
        
      
        /// <summary>
        /// Pauses the sequence when called.
        /// </summary>
        public void Pause()
        {
            foreach (var sourceInstance in player.AllSources)
            {
                sourceInstance.PauseSourceInstance();
            }
        }

        
        /// <summary>
        /// Resumes the sequence when called.
        /// </summary>
        public void Resume()
        {
            foreach (var sourceInstance in player.AllSources)
            {
                sourceInstance.ResumeSourceInstance();
            }
        }

        
        /// <summary>
        /// Stops the sequence when called.
        /// </summary>
        public void Stop()
        {
            foreach (var sourceInstance in player.AllSources)
            {
                sourceInstance.StopSourceInstance();
            }
        }
        

        /// <summary>
        /// Runs when the sequence loops.
        /// </summary>
        public void OnLoop()
        {
            // No logic for this sequence...
        }
    }
}