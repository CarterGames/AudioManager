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