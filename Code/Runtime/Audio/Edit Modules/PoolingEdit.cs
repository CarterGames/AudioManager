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
    /// An audio sequence edit that modifies the pooling logic for the sequence the player is on.
    /// </summary>
    public sealed class PoolingEdit : IEditModule
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private bool RecycleOnComplete { get; set; }

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
            // source.PlayMethod.RecycleOnComplete = RecycleOnComplete;
        }

        
        /// <summary>
        /// Revers the edit to default when called.
        /// </summary>
        /// <param name="source">The AudioSource to edit.</param>
        public void Revert(AudioSourceInstance source)
        {
            // source.PlayMethod.RecycleOnComplete = AssetAccessor.GetAsset<AmAssetSettings>().UseObjectPooling;
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Makes a new pooling edit with the setting entered.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public PoolingEdit(bool value)
        {
            RecycleOnComplete = value;
        }
    }
}