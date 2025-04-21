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

using System.Collections.Generic;
using CarterGames.Assets.AudioManager.Logging;

namespace CarterGames.Assets.AudioManager
{
    public static partial class AudioManager
    {
        /// <summary>
        /// Gets a list of clips by a tag assigned to them in the audio library.
        /// </summary>
        /// <param name="tag">The tag to look for.</param>
        /// <returns>A list of keys to lookup that have the tag assigned.</returns>
        public static List<string> GetClipIdsWithTag(string tag)
        {
            if (!Library.ClipByTagLookup.ContainsKey(tag))
            {
                // No tag in library...
                // return error...
                AmDebugLogger.Warning(AudioManagerErrorCode.TagCannotBeFound.CodeToMessage());
                return null;
            }

            return Library.ClipByTagLookup[tag];
        }
        
        
        /// <summary>
        /// Gets a list of clips by a tag assigned to them in the audio library.
        /// </summary>
        /// <param name="tag">The tag to look for.</param>
        /// <returns>A list of audio data that have the tag assigned.</returns>
        public static List<AudioData> GetAudioDataWithTag(string tag)
        {
            if (!Library.ClipByTagLookup.ContainsKey(tag))
            {
                // No tag in library...
                // return error...
                AmDebugLogger.Warning(AudioManagerErrorCode.TagCannotBeFound.CodeToMessage());
                return null;
            }

            var data = new List<AudioData>();

            foreach (var clipKey in Library.ClipByTagLookup[tag])
            {
                data.Add(Library.LibraryLookup[clipKey]);
            }

            return data;
        }
    }
}