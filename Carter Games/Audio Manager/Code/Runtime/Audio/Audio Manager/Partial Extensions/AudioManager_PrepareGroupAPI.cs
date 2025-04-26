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
using System.Linq;
using CarterGames.Assets.AudioManager.Logging;

namespace CarterGames.Assets.AudioManager
{
    public static partial class AudioManager
    {
        private static AudioPlayer PrepareGroupBase(string groupRef)
        {
            return InternalPrepareGroup(groupRef);
        }
        
        private static AudioPlayer PrepareGroupBase(string groupRef, IEditModule[] edits)
        {
            return InternalPrepareGroup(groupRef, edits);
        }
        
        
        private static AudioPlayer PrepareGroupBase(string request, IEditModule methodEdit, IEditModule[] edits)
        {
            return PrepareGroupBase(request, edits.Concat(new IEditModule[] { methodEdit }).ToArray());
        }
        
        
        private static AudioPlayer PrepareGroupBase(string request, IEnumerable<IEditModule> methodEdit, params IEditModule[] edits)
        {
            return InternalPrepareGroup(request, edits.Concat(methodEdit).ToArray());
        }
        
        
        private static AudioPlayer InternalPrepareGroup(string request, params IEditModule[] edits)
        {
            // Checks to see if the audio manager can actually play audio.
            // if not, do nothing as it won't be heard...
            if (!Settings.CanPlayAudio)
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.AudioDisabled));
                return null;
            }

            var clipSettings = new AudioClipSettings(edits);
            var sequence = AudioPool.AssignPlayer();

            sequence.InitializeGroup(request, clipSettings);
            return sequence;
        }
        
        
        private static AudioPlayer PrepareGroupBase(string[] request, GroupPlayMode playMode, IEditModule methodEdit, IEditModule[] edits)
        {
            return PrepareGroupBase(request, playMode, edits.Concat(new IEditModule[] { methodEdit }).ToArray());
        }
        
        
        private static AudioPlayer PrepareGroupBase(string[] request, GroupPlayMode playMode, IEditModule[] methodEdit, IEditModule[] edits)
        {
            return PrepareGroupBase(request, playMode, edits.Concat(methodEdit).ToArray());
        }
        
        
        private static AudioPlayer PrepareGroupBase(string[] request, GroupPlayMode playMode, IEditModule[] edits)
        {
            // Checks to see if the audio manager can actually play audio.
            // if not, do nothing as it won't be heard...
            if (!Settings.CanPlayAudio)
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.AudioDisabled));
                return null;
            }

            var clipSettings = new AudioClipSettings(edits);
            var sequence = AudioPool.AssignPlayer();

            sequence.InitializeGroup(request, playMode, clipSettings);
            return sequence;
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Prepare Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Prepares the audio clip requested to be played, but doesn't play it automatically.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The play mode for the group.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PrepareGroup(string[] request, GroupPlayMode playMode, params IEditModule[] edits)
        {
            return PrepareGroupBase(request, playMode, edits);
        }
        
        
        /// <summary>
        /// Prepares the audio clip requested to be played, but doesn't play it automatically.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PrepareGroup(string request, params IEditModule[] edits)
        {
            return PrepareGroupBase(request, edits);
        }
        
    }
}