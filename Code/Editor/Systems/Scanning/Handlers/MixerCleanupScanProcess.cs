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
    /// Handles the cleaning up of any null mixer entries where the mixer group has been removed from the project but is still in the library.
    /// </summary>
    public sealed class MixerCleanupScanProcess : IScanProcess
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Stores all the null entry keys to clear.
        /// </summary>
        private List<string> NullEntryKeys { get; set; } = new List<string>();
        
        
        /// <summary>
        /// Defines the oder this scan processor runs in.
        /// </summary>
        public int Priority => 23;
        
        
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
            if (ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.MixerLookup.Count <= 0) return false;
            
            NullEntryKeys.Clear();

            foreach (var entry in ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.MixerLookup)
            {
                if (entry.Value.MixerGroup == null || (((object)entry.Value.MixerGroup) != null && !entry.Value.MixerGroup))
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

            var lookup = ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.Fp("mixers").Fpr("list");
            
            foreach (var nullKey in NullEntryKeys)
            {
                var data = ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.MixerLookup[nullKey];

                if (PerUserSettings.DeveloperDebugMode)
                {
                    AmDebugLogger.Normal($"[Mixer cleanup]: removing: {nullKey}");
                }
                
                
                for (var j = 0; j < lookup.arraySize; j++)
                {
                    if (lookup.GetIndex(j).Fpr("key").stringValue != data.Uuid) continue;
                    lookup.DeleteIndex(j);
                }
                
                ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.ApplyModifiedProperties();
                ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.Update();
            }

            DidSomething = true;
        }
    }
}