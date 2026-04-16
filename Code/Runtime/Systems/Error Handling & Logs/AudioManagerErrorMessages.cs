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

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles all the error messages in the asset.
    /// </summary>
    public static class AudioManagerErrorMessages
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static readonly Dictionary<AudioManagerErrorCode, string> MessagesLookup 
            = new Dictionary<AudioManagerErrorCode, string>()
        {
            {
                AudioManagerErrorCode.AudioDisabled,
                $"{AudioManagerErrorCode.AudioDisabled}\nAudio is disabled via the play state. so the call was cancelled."
            },
            {
                AudioManagerErrorCode.PrefabNotValid,
                $"{AudioManagerErrorCode.PrefabNotValid}\nThe prefab you tried to assign was not a valid type/setup for the field."
            },
            {
                AudioManagerErrorCode.EditorOnlyMethod,
                $"{AudioManagerErrorCode.EditorOnlyMethod}\nMethod can only be called by the editor logic, it is not intended for runtime use."
            },
            {
                AudioManagerErrorCode.StructGeneratorNoData,
                $"{AudioManagerErrorCode.StructGeneratorNoData}\nThere was no data detected for the struct to be generated with."
            },
            {
                AudioManagerErrorCode.StructElementNameAlreadyExists,
                $"{AudioManagerErrorCode.StructElementNameAlreadyExists}\nThere was another entry in the struct with the same name."
            },
        };

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets if a error code has a default error message assigned to it.
        /// </summary>
        /// <param name="errorCode">The code to check.</param>
        /// <returns>If a default message exists for the code.</returns>
        public static bool HasMessage(AudioManagerErrorCode errorCode)
        {
            return MessagesLookup.ContainsKey(errorCode);
        }


        /// <summary>
        /// Gets the message for an error code.
        /// </summary>
        /// <param name="errorCode">The code to check.</param>
        /// <returns>The default message for the code.</returns>
        public static string GetMessage(AudioManagerErrorCode errorCode)
        {
            if (!HasMessage(errorCode)) return $"{errorCode}";
            return MessagesLookup[errorCode];
        }


        public static string CodeToMessage(this AudioManagerErrorCode errorCode)
        {
            if (HasMessage(errorCode))
            {
                return GetMessage(errorCode);
            }

            return errorCode.ToString();
        }
    }
}