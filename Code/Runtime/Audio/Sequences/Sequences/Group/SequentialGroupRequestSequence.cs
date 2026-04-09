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