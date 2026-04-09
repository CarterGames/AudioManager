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