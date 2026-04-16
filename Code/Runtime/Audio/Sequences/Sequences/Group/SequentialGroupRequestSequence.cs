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
    /// Handles a sequential clip player with all the clips playing one after the other.
    /// </summary>
    public sealed class SequentialGroupRequestSequence : IPlayMethod
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private readonly GroupData groupData;
        private readonly AudioPlayer player;
        private int currentIndex;
        private bool hasDelayed;
        private DelayEdit delayEdit;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Constructors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Creates a sequential sequence with the entered data.
        /// </summary>
        /// <param name="player">The sequence to use.</param>
        /// <param name="groupData">The data to use.</param>
        /// <param name="clipSettings">The settings to apply.</param>
        public SequentialGroupRequestSequence(AudioPlayer player, GroupData groupData, AudioClipSettings clipSettings)
        {
            this.player = player;
            this.groupData = groupData;
            
            currentIndex = 0;
            
            // player.Players[0].PlayMethod = player;
            if (!AmAssetAccessor.GetAsset<AudioLibrary>().TryGetClip(groupData.Clips[currentIndex], out var data)) return;
            
            player.Source.InitializePlayer(player, data, clipSettings);
            
            player.Source.Completed.Remove(OnClipCompleted);
            player.Source.Completed.Add(OnClipCompleted);
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
            if (delayEdit != null)
            {
                player.Source.EditParams.SetValue("delay", delayEdit);
                hasDelayed = false;
            }
            
            currentIndex = 0;

            if (!AmAssetAccessor.GetAsset<AudioLibrary>().TryGetClip(groupData.Clips[currentIndex], out var data)) return;
            player.Source.InitializePlayer(player, data);
        }
        
        
        /// <summary>
        /// Runs when a clip has completed playing.
        /// </summary>
        private void OnClipCompleted()
        {
            currentIndex++;

            if (!hasDelayed && player.Source.EditParams.HasKey("delay"))
            {
                hasDelayed = true;
                delayEdit = player.Source.EditParams.GetValue<DelayEdit>("delay");
                player.Source.EditParams.RemoveValue("delay");
            }
            
            if (groupData.Clips.Count.Equals(currentIndex))
            {
                // Complete Sequence....
                if (delayEdit != null)
                {
                    player.Source.EditParams.SetValue("delay", delayEdit);
                    hasDelayed = false;
                    // Debug.Log("Reset Edit Removal");
                }
                
                player.PlayerComplete();
                return;
            }
            
            if (!AmAssetAccessor.GetAsset<AudioLibrary>().TryGetClip(groupData.Clips[currentIndex], out var data)) return;
            player.Source.InitializePlayer(player, data);
            
            Play();
        }
    }
}