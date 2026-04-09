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