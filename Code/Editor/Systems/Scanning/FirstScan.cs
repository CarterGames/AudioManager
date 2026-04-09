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

using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the initial scan when the asset is imported.
    /// </summary>
    public class FirstScan : IAssetEditorReload
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   IAssetEditorReload Implementation
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Runs when the editor is reloaded.
        /// </summary>
        public void OnEditorReloaded()
        {
            if (PerUserSettings.ScannerInitialized || ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.LibraryLookup.Count > 0 && AudioScanner.AnyAudioInProject)
            {
                ShowFirstScan();
            }
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Shows the dialogue to scan for audio.
        /// </summary>
        private static void ShowFirstScan()
        {
            EditorApplication.update -= ShowFirstScan;

            if (PerUserSettings.ScannerInitialized) return;
            
            if (ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.LibraryLookup.Count > 0)
            {
                PerUserSettings.ScannerInitialized = true;
                return;
            }
            
            if (AudioScanner.AnyAudioInProject)
            {
                if (EditorUtility.DisplayDialog("Audio Library Scan",
                        "Your library has no entries, do you want to scan for audio and mixer groups now?", "Scan",
                        "Cancel"))
                {
                    ScanManager.ProcessHandlers();
                }
            }
            
            PerUserSettings.ScannerInitialized = true;
        }
    }
}