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
    /// The data for an entry in the library...
    /// </summary>
    [Serializable]
    public class AudioData
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public string key;
        public string id;
        public string defaultKey;
        public AudioClip value;
        public string path;
        public DynamicTime dynamicStartTime;
        public DefaultClipSettings defaultSettings;
        [SerializeField] private bool showDynamicTime;
        [SerializeField] private LibraryMetaData metaData;



        public string[] Tags => metaData.Tags;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Constructors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Creates a new audio data with the key & clip entered.
        /// </summary>
        /// <param name="key">The key to use.</param>
        /// <param name="value">The clip data to use.</param>
        /// <param name="path">The path of the clip.</param>
        public AudioData(string key, AudioClip value, string path)
        {
            id = $"{key}-{Guid.NewGuid().ToString()}";
            this.key = key;
            defaultKey = id;
            this.value = value;
            this.path = path;
            dynamicStartTime = new DynamicTime();
            defaultSettings = new DefaultClipSettings();
        }
    }
}