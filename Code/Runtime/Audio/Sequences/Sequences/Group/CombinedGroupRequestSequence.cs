/*
 * Copyright (c) 2025 Carter Games
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
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
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