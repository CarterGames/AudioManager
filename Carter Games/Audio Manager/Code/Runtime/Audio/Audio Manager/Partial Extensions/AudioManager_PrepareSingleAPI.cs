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
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Prepare Methods (Single)
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Prepares the audio clip requested to be played, but doesn't play it automatically.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer Prepare(string request, params IEditModule[] edits)
        {
            return InternalPrepare(request, edits);
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Internal Prepare Methods (Single)
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Handles actually playing the request.
        /// </summary>
        /// <param name="request">The request edit specifically.</param>
        /// <param name="methodEdit">The edit specific for the method call.</param>
        /// <param name="edits">Any extra edits the user wants to add.</param>
        /// <returns>The AudioClipPlayer setup with the params requested.</returns>
        private static AudioPlayer InternalPrepare(string request, IEditModule methodEdit, params IEditModule[] edits)
        {
            return InternalPrepare(request, new [] { methodEdit }, edits);
        }


        /// <summary>
        /// Handles actually playing the request.
        /// </summary>
        /// <param name="request">The request edit specifically.</param>
        /// <param name="methodEdits">The edits specific for the method call.</param>
        /// <param name="edits">Any extra edits the user wants to add.</param>
        /// <returns>The AudioClipPlayer setup with the params requested.</returns>
        private static AudioPlayer InternalPrepare(string request, IEnumerable<IEditModule> methodEdits, params IEditModule[] edits)
        {
            return InternalPrepare(request, edits.Concat(methodEdits).ToArray());
        }


        /// <summary>
        /// Handles actually playing/preparing the request.
        /// </summary>
        /// <param name="request">The request edit specifically.</param>
        /// <param name="edits">The edits to apply.</param>
        /// <returns>The AudioClipPlayer setup with the params requested.</returns>
        private static AudioPlayer InternalPrepare(string request, params IEditModule[] edits)
        {
            // Checks to see if the audio manager can actually play audio.
            // if not, do nothing as it won't be heard...
            if (!UtilRuntime.SettingCanPlayAudio)
            {
                AmDebugLogger.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.AudioDisabled));
                return null;
            }

            var clipSettings = new AudioClipSettings(edits);
            var sequence = AudioPool.AssignPlayer();
            
            sequence.Initialize(request, clipSettings);
            return sequence;
        }

        
    }
}