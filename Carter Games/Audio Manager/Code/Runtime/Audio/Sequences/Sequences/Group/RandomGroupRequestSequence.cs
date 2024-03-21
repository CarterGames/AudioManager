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
    /// Handles a random clip player with a random clip from the group playing when called to.
    /// </summary>
    public sealed class RandomGroupRequestSequence : ISequenceHandler
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private readonly GroupData groupData;
        private readonly AudioClipSettings clipSettings;
        private readonly AudioPlayerSequence playerSequence;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Constructors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Create a random sequence with the entered data.
        /// </summary>
        /// <param name="playerSequence">The sequence to use.</param>
        /// <param name="groupData">The data to use.</param>
        /// <param name="clipSettings">The settings to apply.</param>
        public RandomGroupRequestSequence(AudioPlayerSequence playerSequence, GroupData groupData, AudioClipSettings clipSettings)
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
            // Random - play 1 clip - when looping play another random.
            var clip = groupData.Clips[Random.Range(0, groupData.Clips.Count - 1)];
            
            playerSequence.Players[0].PlayerSequence = playerSequence;
            playerSequence.Players[0].SetClip(AssetAccessor.GetAsset<AudioLibrary>().GetData(clip), clipSettings);
            
            playerSequence.Players[0].Completed.Remove(playerSequence.PlayerComplete);
            playerSequence.Players[0].Completed.Add(playerSequence.PlayerComplete);
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
            var clip = groupData.Clips[Random.Range(0, groupData.Clips.Count - 1)];
            playerSequence.Players[0].SetClip(AssetAccessor.GetAsset<AudioLibrary>().GetData(clip));
        }
    }
}