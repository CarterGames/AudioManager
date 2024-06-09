﻿/*
 * Copyright (c) 2024 Carter Games
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

using System.Collections.Generic;
using CarterGames.Assets.AudioManager.Logging;

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
            if (UtilEditor.Library.MixerLookup.Count <= 0) return false;
            
            NullEntryKeys.Clear();

            foreach (var entry in UtilEditor.Library.MixerLookup)
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

            var lookup = UtilEditor.LibraryObject.Fp("mixers").Fpr("list");
            var reverseLookup = UtilEditor.LibraryObject.Fp("mixersReverseLookup").Fpr("list");
            
            foreach (var nullKey in NullEntryKeys)
            {
                var data = UtilEditor.Library.MixerLookup[nullKey];

                if (PerUserSettings.DeveloperDebugMode)
                {
                    AmDebugLogger.Normal($"[Mixer cleanup]: removing: {nullKey}");
                }

                for (var j = 0; j < reverseLookup.arraySize; j++)
                {
                    if (reverseLookup.GetIndex(j).Fpr("key").stringValue != data.Key) continue;
                    reverseLookup.DeleteIndex(j);
                }
                
                
                for (var j = 0; j < lookup.arraySize; j++)
                {
                    if (lookup.GetIndex(j).Fpr("key").stringValue != data.Uuid) continue;
                    lookup.DeleteIndex(j);
                }
                
                UtilEditor.LibraryObject.ApplyModifiedProperties();
                UtilEditor.LibraryObject.Update();
            }

            DidSomething = true;
        }
    }
}