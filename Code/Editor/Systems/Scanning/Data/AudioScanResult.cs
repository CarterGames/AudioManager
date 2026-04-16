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
using System.Collections.Generic;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Contains the result of a scan, which can be any type.
    /// </summary>
    /// <typeparam name="T">The type to store.</typeparam>
    [Serializable]
    public sealed class AudioScanResult<T>
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        [SerializeField] private List<T> foundData;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets if there is any data stored.
        /// </summary>
        public bool HasData => foundData is { } && foundData.Count > 0;
        
        
        /// <summary>
        /// Gets the data stored.
        /// </summary>
        public List<T> Data => foundData;
        
        
        /// <summary>
        /// Gets the total number of entries in the data.
        /// </summary>
        public int DataCount => foundData.Count;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Creates a new result with the entered data.
        /// </summary>
        /// <param name="data">The data to store.</param>
        public AudioScanResult(List<T> data)
        {
            foundData = data;
        }
    }
}