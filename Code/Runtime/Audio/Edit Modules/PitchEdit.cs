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

using UnityEngine;
using Random = UnityEngine.Random;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// An audio source edit that modifies the pitch of the audio source.
    /// </summary>
    public sealed class PitchEdit : IEditModule
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private const float MinRange = -3f;
        private const float MaxRange = 3f;
        
        private bool useRange;
        private bool useVariance;
        private float normalEditValue;
        private float[] rangeEditValue;
        private Variance varianceEditValue;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   IAudioEditModule
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets if the edits should process when looping
        /// </summary>
        public bool ProcessOnLoop => true;
        
        
        /// <summary>
        /// Processes the edit when called.
        /// </summary>
        /// <param name="source">The AudioSource to edit.</param>
        public void Process(AudioSourceInstance source)
        {
            var value = 1f;

            if (useRange)
            {
                value = Random.Range(rangeEditValue[0], rangeEditValue[1]);
            }
            else if (useVariance)
            {
                value = Mathf.Clamp(varianceEditValue.GetVariance(), MinRange, MaxRange);
            }
            else
            {
                value = normalEditValue;
            }

            source.Source.pitch = value;
        }

        
        /// <summary>
        /// Revers the edit to default when called.
        /// </summary>
        /// <param name="source">The AudioSource to edit.</param>
        public void Revert(AudioSourceInstance source)
        {
            source.Source.pitch = 1f;
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Makes a new pitch edit with the setting entered.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public PitchEdit(float value)
        {
            normalEditValue = Mathf.Clamp(value, MinRange, MaxRange);
        }
        
        
        /// <summary>
        /// Makes a new volume edit with the setting entered.
        /// </summary>
        /// <param name="minValue">The min value the volume can be.</param>
        /// <param name="maxValue">The max value the volume can be.</param>
        public PitchEdit(float minValue, float maxValue)
        {
            rangeEditValue = new float[2]
            {
                Mathf.Clamp(minValue, MinRange, MaxRange), 
                Mathf.Clamp(maxValue, MinRange, MaxRange)
            };
            
            useVariance = false;
            useRange = true;
        }
        
            
        /// <summary>
        /// Makes a new volume edit with the setting entered.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public PitchEdit(Variance value)
        {
            varianceEditValue = value;
            useVariance = true;
            useRange = false;
        }
    }
}