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