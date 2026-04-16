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

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Defines a dynamic time point.
    /// </summary>
    [Serializable]
    public sealed class DynamicTime
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public float time;
        [Range(0f, .5f)] public float threshold;
        [SerializeField] private DynamicTimeOption option;
        [SerializeField] private int tabPos;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Constructors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Creates a new dynamic time with default settings.
        /// </summary>
        public DynamicTime() { }

        
        /// <summary>
        /// Creates a new dynamic time with the params entered.
        /// </summary>
        /// <param name="time">The time to start at.</param>
        /// <param name="sample">The sample to start from.</param>
        /// <param name="threshold">The threshold to detect audio starting at.</param>
        public DynamicTime(float time, int sample, float threshold)
        {
            this.time = time;
            this.threshold = threshold;
        }
    }
}