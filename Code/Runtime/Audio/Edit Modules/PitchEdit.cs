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