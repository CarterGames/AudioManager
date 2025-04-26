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
    /// An audio source edit that modifies the volume of the audio source.
    /// </summary>
    public sealed class VolumeEdit : IEditModule
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private bool useRange;
        private bool useVariance;
        private float normalEditValue;
        private float[] rangeEditValue;
        private Variance varianceEditValue;
        private float statingValue;

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
            statingValue = source.Source.volume;
            var value = source.Source.volume;

            if (useRange)
            {
                value = Random.Range(rangeEditValue[0], rangeEditValue[1]);
            }
            else if (useVariance)
            {
                value = Mathf.Clamp(varianceEditValue.GetVariance(), 0f, 1f);
            }
            else
            {
                value = normalEditValue;
            }
            
            source.Source.volume = value;
        }
        
        
        /// <summary>
        /// Revers the edit to default when called.
        /// </summary>
        /// <param name="source">The AudioSource to edit.</param>
        public void Revert(AudioSourceInstance source)
        {
            source.Source.volume = statingValue;
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Makes a new volume edit with the setting entered.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public VolumeEdit(float value)
        {
            normalEditValue = Mathf.Clamp(value, 0f, 1f);
            useVariance = false;
            useRange = false;
        }
          
        
        /// <summary>
        /// Makes a new volume edit with the setting entered.
        /// </summary>
        /// <param name="minValue">The min value the volume can be.</param>
        /// <param name="maxValue">The max value the volume can be.</param>
        public VolumeEdit(float minValue, float maxValue)
        {
            rangeEditValue = new float[2]
            {
                Mathf.Clamp(minValue, 0f, 1f), 
                Mathf.Clamp(maxValue, 0f, 1f)
            };

            useRange = true;
            useVariance = false;
        }
        
            
        /// <summary>
        /// Makes a new volume edit with the setting entered.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public VolumeEdit(Variance value)
        {
            varianceEditValue = value;
            useRange = false;
            useVariance = true;
        }
    }
}