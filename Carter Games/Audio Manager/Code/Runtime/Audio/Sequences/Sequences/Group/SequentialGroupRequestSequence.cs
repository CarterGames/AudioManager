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

using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles a sequential clip player with all the clips playing one after the other.
    /// </summary>
    public sealed class SequentialGroupRequestSequence : ISequenceHandler
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private readonly GroupData groupData;
        private readonly AudioClipSettings clipSettings;
        private readonly AudioPlayerSequence playerSequence;
        private int currentIndex;
        private bool hasDelayed;
        private DelayEdit delayEdit;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Constructors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Creates a sequential sequence with the entered data.
        /// </summary>
        /// <param name="playerSequence">The sequence to use.</param>
        /// <param name="groupData">The data to use.</param>
        /// <param name="clipSettings">The settings to apply.</param>
        public SequentialGroupRequestSequence(AudioPlayerSequence playerSequence, GroupData groupData, AudioClipSettings clipSettings)
        {
            this.playerSequence = playerSequence;
            this.groupData = groupData;
            this.clipSettings = clipSettings;
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Sets up the sequence for use.
        /// </summary>
        public void Setup()
        {
            currentIndex = 0;
            
            playerSequence.Players[0].PlayerSequence = playerSequence;
            playerSequence.Players[0].SetClip(AssetAccessor.GetAsset<AudioLibrary>().GetData(groupData.Clips[currentIndex]), clipSettings);
            
            playerSequence.Players[0].Completed.Remove(OnClipCompleted);
            playerSequence.Players[0].Completed.Add(OnClipCompleted);
        }
        
        
        /// <summary>
        /// Plays the sequence when called.
        /// </summary>
        public void Play()
        {
            playerSequence.Players[0].PlayPlayer();
        }
        
        
        /// <summary>
        /// Pauses the sequence when called.
        /// </summary>
        public void Pause()
        {
            playerSequence.Players[0].PausePlayer();
        }

        
        /// <summary>
        /// Resumes the sequence when called.
        /// </summary>
        public void Resume()
        {
            playerSequence.Players[0].ResumePlayer();
        }

        
        /// <summary>
        /// Stops the sequence when called.
        /// </summary>
        public void Stop()
        {
            playerSequence.Players[0].StopPlayer();
        }

        
        /// <summary>
        /// Runs when the sequence loops.
        /// </summary>
        public void OnLoop()
        {
            if (delayEdit != null)
            {
                playerSequence.Players[0].EditParams.SetValue("delay", delayEdit);
                hasDelayed = false;
            }
            
            currentIndex = 0;
            playerSequence.Players[0].SetClip(AssetAccessor.GetAsset<AudioLibrary>().GetData(groupData.Clips[currentIndex]));
        }
        
        
        /// <summary>
        /// Runs when a clip has completed playing.
        /// </summary>
        private void OnClipCompleted()
        {
            currentIndex++;

            if (!hasDelayed && playerSequence.Players[0].EditParams.HasKey("delay"))
            {
                hasDelayed = true;
                delayEdit = playerSequence.Players[0].EditParams.GetValue<DelayEdit>("delay");
                playerSequence.Players[0].EditParams.RemoveValue("delay");
            }
            
            if (groupData.Clips.Count.Equals(currentIndex))
            {
                // Complete Sequence....
                if (delayEdit != null)
                {
                    playerSequence.Players[0].EditParams.SetValue("delay", delayEdit);
                    hasDelayed = false;
                    // Debug.Log("Reset Edit Removal");
                }
                
                playerSequence.PlayerComplete();
                return;
            }
            
            playerSequence.Players[0].SetClip(AssetAccessor.GetAsset<AudioLibrary>().GetData(groupData.Clips[currentIndex]));
            
            Play();
        }
    }
}