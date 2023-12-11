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

using UnityEditor;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the initial scan when the asset is imported.
    /// </summary>
    public static class FirstScan
    {
        /// <summary>
        /// Runs when the editor is opened...
        /// </summary>
        [InitializeOnLoadMethod]
        private static void Init()
        {
            if (PerUserSettings.ScannerInitialized && UtilEditor.Library.LibraryLookup.Count > 0) return;
            
            EditorApplication.update -= ShowFirstScan;
            EditorApplication.update += ShowFirstScan;
        }


        /// <summary>
        /// Shows the dialogue to scan for audio.
        /// </summary>
        private static void ShowFirstScan()
        {
            EditorApplication.update -= ShowFirstScan;
            
            if (UtilEditor.Library.LibraryLookup.Count > 0)
            {
                PerUserSettings.ScannerInitialized = true;
                return;
            }
            
            if (EditorUtility.DisplayDialog("Audio Library Scan", "Your library has no entries, do you want to scan for audio and mixer groups now?", "Scan", "Cancel"))
            {
                AudioScanner.ScanForAudio(true);
            }
            
            PerUserSettings.ScannerInitialized = true;
        }
    }
}