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

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles a single clip player.
    /// </summary>
    public sealed class SingleSourcePlayMethod : IPlayMethod
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private readonly AudioPlayer player;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Constructors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private SingleSourcePlayMethod(AudioPlayer player)
        {
            this.player = player;
        }
        
        
        public static SingleSourcePlayMethod InitializePlayMethod(AudioPlayer player, AudioData requestData, AudioClipSettings clipSettings)
        {
            var playMethodHandler = new SingleSourcePlayMethod(player);
            
            player.Source.InitializePlayer(player, requestData, clipSettings);
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
            // No logic for this sequence...
        }
    }
}