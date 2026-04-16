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