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

using System;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// A data class for a clip to play in the audio player inspector.
    /// </summary>
    [Serializable]
    public class AudioPlayerData
    {
        /// <summary>
        /// Should the data be expanded?
        /// </summary>
        public bool show;
        
        
        /// <summary>
        /// The name of the clip.
        /// </summary>
        public string clipName;
        
        
        /// <summary>
        /// The volume ranges for the clip.
        /// </summary>
        public MinMaxFloat volume;
        
        
        /// <summary>
        /// The pitch ranges for the clip.
        /// </summary>
        public MinMaxFloat pitch;

        
        /// <summary>
        /// Should the optional options be shown?
        /// </summary>
        public bool showOptional;
        
        
        /// <summary>
        /// The time the clip should play from.
        /// </summary>
        public float fromTime;
        
        
        /// <summary>
        /// The delay the clip should have.
        /// </summary>
        public float clipDelay;
    }
}