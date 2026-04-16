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
// using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles the data for a mixer entry in the library.
    /// </summary>
    [Serializable]
    public class MixerData
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        [SerializeField] private string uuid;
        [SerializeField] private string key;
        [SerializeField] private string path;
        [SerializeField] private AudioMixerGroup mixerGroup;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The uuid for the mixer group.
        /// </summary>
        public string Uuid => uuid;
        
        
        /// <summary>
        /// The key for the mixer group.
        /// </summary>
        public string Key => key;
        
        
        /// <summary>
        /// The actual mixer reference.
        /// </summary>
        public AudioMixerGroup MixerGroup => mixerGroup;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Constructors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Makes a new mixer data when called.
        /// </summary>
        /// <param name="uuid">The uuid to set.</param>
        /// <param name="key">The key to set.</param>
        /// <param name="mixerGroup">The mixer group to set.</param>
        public MixerData(string uuid, string key, AudioMixerGroup mixerGroup)
        {
            this.uuid = uuid;
            this.key = key;
            this.mixerGroup = mixerGroup;
        }
    }
}