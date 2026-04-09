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