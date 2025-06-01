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
using System.Linq;
using CarterGames.Assets.AudioManager.Logging;
using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles going through all the scan processes when required.
    /// </summary>
    public static class ScanManager
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// A list of all the processes in the project, mainly to help performance wise.
        /// Could be done dynamically if needed in the future.
        /// </summary>
        private static readonly List<IScanProcess> ProcessedCache = new List<IScanProcess>()
        {
            new LibraryCleanupProcess(),
            new ClipScanProcess(),
            new GroupsCleanupScanProcess(),
            new MixerScanProcess(),
            new MixerCleanupScanProcess(),
        };
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets through each process and runs it.
        /// </summary>
        public static void ProcessHandlers()
        {
            var processesRun = 0;
            
            foreach (var scanProcess in ProcessedCache.OrderBy(t => t.Priority).ToArray())
            {
                if (!scanProcess.ShouldUpdateLibrary())
                {
                    if (PerUserSettings.DeveloperDebugMode)
                    {
                        AmDebugLogger.Normal($"[Scan]: ({scanProcess.GetType().Name}) skipped.");
                    }
                    
                    continue;
                }
                
                scanProcess.UpdateLibrary();
                
                if (PerUserSettings.DeveloperDebugMode)
                {
                    AmDebugLogger.Normal($"[Scan]: ({scanProcess.GetType().Name}) ran.");
                }

                if (!scanProcess.DidSomething) continue;
                processesRun++;
            }

            if (processesRun <= 0) return;
            
            EditorUtility.SetDirty(ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef);
            AssetDatabase.SaveAssets();
            
            LibraryEditorWindow.ForceUpdate();
        }
    }
}