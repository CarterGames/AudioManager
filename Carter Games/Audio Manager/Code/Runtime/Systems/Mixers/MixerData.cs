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