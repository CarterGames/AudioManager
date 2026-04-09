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