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
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Scans for audio clips when a new audio clip is added to the project...
    /// </summary>
    [DefaultExecutionOrder(1000)]
    public sealed class AudioScanner : AssetPostprocessor, IAssetEditorFileChanges
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Returns if any audio is found in the project or not.
        /// </summary>
        public static bool AnyAudioInProject
        {
            get
            {
                var assets = AssetDatabase.FindAssets("t:AudioClip", null);
                if (assets == null) return false;
                return assets.Length > 0;
            }
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   IAssetEditorFileChanges Implementation
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Runs when the editor detects any file changes.
        /// </summary>
        public void OnEditorFileChanges()
        {
            ScanManager.ProcessHandlers();
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Menu Items
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Performs a scan on the user's command if needed.
        /// </summary>
        [MenuItem("Tools/Carter Games/Audio Manager/Perform Manual Scan", priority = 22)]
        public static void ManualScan()
        {
            if (!PerUserSettings.ScannerInitialized) return;
            
            var option = EditorUtility.DisplayDialogComplex("Manual Audio Scan",
                "Do you want to do a clean scan of all files or just find new ones not in the library?",
                "Clean Scan",
                "New Only Scan", "Cancel");
            
            if (option.Equals(2)) return;

            if (option.Equals(1))
            {
                ScanManager.ProcessHandlers();
            }
            else
            {
                var libObj = ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef;
                
                libObj.Fp("library").Fpr("list").ClearArray();
                libObj.Fp("mixers").Fpr("list").ClearArray();
                    
                StructHandler.ResetLibraryStructs();

                libObj.ApplyModifiedProperties();
                libObj.Update();
                
                ScanManager.ProcessHandlers();
            }
        }
    }
}