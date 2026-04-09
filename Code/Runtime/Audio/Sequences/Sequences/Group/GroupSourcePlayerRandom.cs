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