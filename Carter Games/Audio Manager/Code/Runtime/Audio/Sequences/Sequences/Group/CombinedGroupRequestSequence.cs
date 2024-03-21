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

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles a combined clip player with all the clips playing together.
    /// </summary>
    public sealed class CombinedGroupRequestSequence : ISequenceHandler
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
        /// Creates a combined sequence with the entered data.
        /// </summary>
        /// <param name="playerSequence">The sequence to use.</param>
        /// <param name="groupData">The data to use.</param>
        /// <param name="clipSettings">The settings to apply.</param>
        public CombinedGroupRequestSequence(AudioPlayerSequence playerSequence, GroupData groupData, AudioClipSettings clipSettings)
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
            if (playerSequence.Players.Count < groupData.Clips.Count)
            {
                var totalRequired = groupData.Clips.Count - playerSequence.Players.Count;
                
                for (var i = 0; i < totalRequired; i++)
                {
                    playerSequence.AssignNewPlayer();
                }
            }

            for (var i = 0; i < playerSequence.Players.Count; i++)
            {
                playerSequence.Players[i].PlayerSequence = playerSequence;
                playerSequence.Players[i].SetClip(AssetAccessor.GetAsset<AudioLibrary>().GetData(groupData.Clips[i]), clipSettings);
                
                playerSequence.Players[i].Completed.Remove(playerSequence.PlayerComplete);
                playerSequence.Players[i].Completed.Add(playerSequence.PlayerComplete);
            }
        }
        
        
        /// <summary>
        /// Plays the sequence when called.
        /// </summary>
        public void Play()
        {
            foreach (var player in playerSequence.Players)
            {
                player.PlayPlayer();
            }
        }
        
      
        /// <summary>
        /// Pauses the sequence when called.
        /// </summary>
        public void Pause()
        {
            foreach (var player in playerSequence.Players)
            {
                player.PausePlayer();
            }
        }

        
        /// <summary>
        /// Resumes the sequence when called.
        /// </summary>
        public void Resume()
        {
            foreach (var player in playerSequence.Players)
            {
                player.ResumePlayer();
            }
        }

        
        /// <summary>
        /// Stops the sequence when called.
        /// </summary>
        public void Stop()
        {
            foreach (var player in playerSequence.Players)
            {
                player.StopPlayer();
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