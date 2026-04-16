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