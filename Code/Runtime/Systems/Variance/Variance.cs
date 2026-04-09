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