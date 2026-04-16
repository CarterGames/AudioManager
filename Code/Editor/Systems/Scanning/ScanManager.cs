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