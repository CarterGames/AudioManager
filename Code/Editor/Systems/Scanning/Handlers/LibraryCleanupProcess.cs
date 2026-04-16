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
using CarterGames.Assets.AudioManager.Logging;
using CarterGames.Shared.AudioManager.Editor;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles clearing null entries from the library, where the clip the entry was for has been removed from the project.
    /// </summary>
    public sealed class LibraryCleanupProcess : IScanProcess
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Stores all the null keys to clear from the library.
        /// </summary>
        private List<string> NullEntryKeys { get; set; } = new List<string>();
        
        
        /// <summary>
        /// Defines the oder this scan processor runs in.
        /// </summary>
        public int Priority => 20;
        
        
        /// <summary>
        /// Defines if this processor did something or not.
        /// </summary>
        public bool DidSomething { get; set; }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Works out if the library needs updates based on what this processor is used for.
        /// </summary>
        /// <returns>Gets if the library should be updated by this processor or not.</returns>
        public bool ShouldUpdateLibrary()
        {
            if (ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.LibraryLookup.Count <= 0) return false;
            
            NullEntryKeys.Clear();

            foreach (var entry in ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.LibraryLookup)
            {
                if (entry.Value.value == null || (((object)entry.Value.value) != null && !entry.Value.value))
                {
                    NullEntryKeys.Add(entry.Key);
                }
            }

            return NullEntryKeys.Count > 0;
        }

        
        /// <summary>
        /// Updates the library with the required changes when called.
        /// </summary>
        public void UpdateLibrary()
        {
            DidSomething = false;
            
            if (NullEntryKeys.Count <= 0) return;

            var lookup = ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.Fp("library").Fpr("list");
            
            foreach (var nullKey in NullEntryKeys)
            {
                var data = ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.LibraryLookup[nullKey];

                if (PerUserSettings.DeveloperDebugMode)
                {
                    AmDebugLogger.Normal($"[Library cleanup]: removing: {nullKey}");
                }
                
                for (var j = 0; j < lookup.arraySize; j++)
                {
                    if (lookup.GetIndex(j).Fpr("key").stringValue != data.id) continue;
                    lookup.DeleteIndex(j);
                }
                
                ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.ApplyModifiedProperties();
                ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.Update();
            }

            DidSomething = true;
        }
    }
}