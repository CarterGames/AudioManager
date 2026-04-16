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