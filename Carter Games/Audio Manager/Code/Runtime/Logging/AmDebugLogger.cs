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
 * FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using UnityEngine;

namespace CarterGames.Assets.AudioManager.Logging
{
    /// <summary>
    /// Used for sending logs formatted with the asset...
    /// </summary>
    public static class AmDebugLogger
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private const string ShowLogsPrefKey = "AudioManager_Settings_ShowDebugLogs";
        
        private const string LogPrefix = "<color=#E77A7A><b>Audio Manager</b></color> | ";
        private const string WarningPrefix = "<color=#D6BA64><b>Warning</b></color> | ";
        private const string ErrorPrefix = "<color=#E77A7A><b>Error</b></color> | ";
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Defines if the logs should appear unless overriden by the method that is calling for a log.
        /// </summary>
        private static bool CanShowMessage
        {
            get
            {
                if (PlayerPrefs.HasKey(ShowLogsPrefKey))
                {
                    return PlayerPrefs.GetInt(ShowLogsPrefKey) == 1;
                }

                return false;
            }
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Sends a standard message when called.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <param name="overrideUserShowMessage">Should the message override the user's preference?</param>
        public static void Normal(string message, bool overrideUserShowMessage = false) 
        {
            if (!overrideUserShowMessage)
            {
                if (!CanShowMessage) return;
            }
            
            Debug.Log($"{LogPrefix}{message}");
        }
        
        
        /// <summary>
        /// Sends an warning message when called.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <param name="overrideUserShowMessage">Should the message override the user's preference?</param>
        public static void Warning(string message, bool overrideUserShowMessage = false)  
        {
            if (!overrideUserShowMessage)
            {
                if (!CanShowMessage) return;
            }
            
            Debug.LogWarning($"{LogPrefix}{WarningPrefix}{message}");
        }


        /// <summary>
        /// Sends an error message when called.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <param name="overrideUserShowMessage">Should the message override the user's preference?</param>
        public static void Error(string message, bool overrideUserShowMessage = false) 
        {
            if (!overrideUserShowMessage)
            {
                if (!CanShowMessage) return;
            }
            
            Debug.LogError($"{LogPrefix}{ErrorPrefix}{message}");
        }
    }
}