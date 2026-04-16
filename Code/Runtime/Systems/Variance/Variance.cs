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

using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// A struct to define a variance from a set value.
    /// </summary>
    [Serializable]
    public struct Variance
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The starting value to base off.
        /// </summary>
        public float StartingValue { get; set; }
        
        
        /// <summary>
        /// The amount of variance to add to either side of the starting value.
        /// </summary>
        public float Offset { get; set; }
        
        
        /// <summary>
        /// The min value the resulting value can be.
        /// </summary>
        public float MinValue { get; set; }
        
        
        /// <summary>
        /// The max value the resulting value can be.
        /// </summary>
        public float MaxValue { get; set; }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Constructors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Defines a new variance struct with the starting value & offset defined with no clamping.
        /// </summary>
        /// <param name="startingValue">The starting value to set.</param>
        /// <param name="offset">The offset to set.</param>
        public Variance(float startingValue, float offset)
        {
            StartingValue = startingValue;
            Offset = offset;
            MinValue = float.MinValue;
            MaxValue = float.MaxValue;
        }
        
        
        /// <summary>
        /// Defines a new variance struct with the starting value & offset defined with the max clamp set.
        /// </summary>
        /// <param name="startingValue">The starting value to set.</param>
        /// <param name="offset">The offset to set.</param>
        /// <param name="maxValue">The max value to clamp to.</param>
        /// <remarks>Min value is set to 0 with this setup.</remarks>
        public Variance(float startingValue, float offset, float maxValue)
        {
            StartingValue = startingValue;
            Offset = offset;
            MinValue = 0f;
            MaxValue = maxValue;
        }
        
        
        /// <summary>
        /// Defines a new variance struct with the starting value & offset defined with the min/max clamps set.
        /// </summary>
        /// <param name="startingValue">The starting value to set.</param>
        /// <param name="offset">The offset to set.</param>
        /// <param name="minValue">The min value to clamp to.</param>
        /// <param name="maxValue">The max value to clamp to.</param>
        public Variance(float startingValue, float offset, float minValue, float maxValue)
        {
            StartingValue = startingValue;
            Offset = offset;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets a random variance within the ranges defined.
        /// </summary>
        /// <returns>The variance selected.</returns>
        public float GetVariance()
        {
            return Mathf.Clamp(Random.Range(StartingValue - Offset, StartingValue + Offset), MinValue, MaxValue);
        }
    }
}