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
    /// An audio source edit that modifies the looping of the audio source.
    /// </summary>
    public sealed class LoopEdit : IEditModule
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Defines how many times a clip should loop.
        /// </summary>
        public int LoopCount { get; private set; }


        /// <summary>
        /// Defines if the loop should use delay on each loop or not.
        /// </summary>
        private bool IgnoreDelayAfterFirstCall { get; set; }


        /// <summary>
        /// Gets if this loop is infinite or not.
        /// </summary>
        public bool IsInfiniteLoop => LoopCount.Equals(-1);


        /// <summary>
        /// Gets if the loop should loop with a delay on each loop.
        /// </summary>
        public bool ShouldLoopWithDelays => !IgnoreDelayAfterFirstCall;
        
        
        /// <summary>
        /// Gets the current loop total.
        /// </summary>
        public int CurrentLoopCount { get; set; }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   IAudioEditModule
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets if the edits should process when looping
        /// </summary>
        public bool ProcessOnLoop => false;
        
        
        /// <summary>
        /// Processes the edit when called.
        /// </summary>
        /// <param name="source">The AudioSource to edit.</param>
        public void Process(AudioSourceInstance source)
        {
            source.TargetPlayer.LoopInfo = this;
            CurrentLoopCount = 1;
        }

        
        /// <summary>
        /// Revers the edit to default when called.
        /// </summary>
        /// <param name="source">The AudioSource to edit.</param>
        public void Revert(AudioSourceInstance source)
        {
            source.TargetPlayer.LoopInfo = null;
            CurrentLoopCount = 1;
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Makes a new loop edit with the setting entered.
        /// </summary>
        /// <param name="value">The number of times it should loop, set to -1 for infinite loops.</param>
        /// <param name="ignoreDelayAfterFirst">Should any delay be ignored after the first play, so no delay on repeats? DEF: True</param>
        public LoopEdit(int value = -1, bool ignoreDelayAfterFirst = true)
        {
            LoopCount = value;
            IgnoreDelayAfterFirstCall = ignoreDelayAfterFirst;
            CurrentLoopCount = 1;
        }
    }
}