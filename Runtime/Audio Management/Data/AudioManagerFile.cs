/*
 * Copyright (c) 2018-Present Carter Games
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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Holds the audio data for the audio manager class to use.
    /// </summary>
    [CreateAssetMenu(fileName = "Audio Manager File", menuName = "Carter Games/Audio Manager/Audio Manager File")]
    public class AudioManagerFile : AudioManagerAsset
    {
        /// <summary>
        /// Used in the audio manager file editor script to move the tab around...
        /// </summary>
        [SerializeField, HideInInspector] private int tabPos = 0;     
        
        
        /// <summary>
        /// Holds a list of the audio clips stored in the AMF.
        /// </summary>
        public List<AudioMixerGroup> audioMixer;
        
        
        /// <summary>
        /// Holds the prefab spawned in to play sound from this AMF.
        /// </summary>
        public GameObject soundPrefab;
        
        
        /// <summary>
        /// Holds the boolean value for whether or not this AMF has been used to store audio.
        /// </summary>
        public bool isPopulated;
        
        
        /// <summary>
        /// Holds a list of directory strings for use in the AM.
        /// </summary>
        public List<string> directory; 
        
        
        /// <summary>
        /// Moved in 2.3.5 to be here instead of in the AM reference as it caused some issues with the automation.
        /// </summary>
        public List<AudioLibraryData> library;
    }
}