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