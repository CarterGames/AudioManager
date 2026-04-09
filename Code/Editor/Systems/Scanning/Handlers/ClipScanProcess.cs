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
using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles scanning for new clips in the project that are not in the library and adds them.
    /// </summary>
    public sealed class ClipScanProcess : IScanProcess
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Holds the result of the scan that need processing.
        /// </summary>
        private AudioScanResult<AudioClip> ScanResult { get; set; }

        
        /// <summary>
        /// Defines the oder this scan processor runs in.
        /// </summary>
        public int Priority => -10;
        
        
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
            if (!ProjectDataHelper.TryGetAllClipsInProject(out var clipsFound)) return false;
            
            var clipsToAdd = new List<AudioClip>();
                
            foreach (var clipToCheck in clipsFound)
            {
                if (ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.LibraryLookup.Any(t => t.Value.value.Equals(clipToCheck))) continue;
                clipsToAdd.Add(clipToCheck);
            }

            ScanResult = new AudioScanResult<AudioClip>(clipsToAdd);
            return clipsToAdd.Count > 0;
        }
        

        /// <summary>
        /// Updates the library with the required changes when called.
        /// </summary>
        public void UpdateLibrary()
        {
            DidSomething = false;
            
            if (!ScanResult.HasData) return;
            
            var lookup = ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.Fp("library").Fpr("list");
            
            foreach (var clip in ScanResult.Data)
            {
                var data = new AudioData(clip.name, clip, AssetDatabase.GetAssetPath(clip));
                
                lookup.InsertIndex(lookup.arraySize);

                var index = lookup.arraySize - 1;

                lookup.GetIndex(index).Fpr("key").stringValue = data.id;
                
                lookup.GetIndex(index).Fpr("value").Fpr("key").stringValue = data.key;
                lookup.GetIndex(index).Fpr("value").Fpr("id").stringValue = data.id;
                lookup.GetIndex(index).Fpr("value").Fpr("defaultKey").stringValue = data.defaultKey;
                lookup.GetIndex(index).Fpr("value").Fpr("value").objectReferenceValue = data.value;
                lookup.GetIndex(index).Fpr("value").Fpr("path").stringValue = data.path;
                lookup.GetIndex(index).Fpr("value").Fpr("showDynamicTime").boolValue = false;
                
                lookup.GetIndex(index).Fpr("value").Fpr("dynamicStartTime").Fpr("time").floatValue = data.dynamicStartTime.time;
                lookup.GetIndex(index).Fpr("value").Fpr("dynamicStartTime").Fpr("threshold").floatValue = data.dynamicStartTime.threshold;
                lookup.GetIndex(index).Fpr("value").Fpr("dynamicStartTime").Fpr("option").intValue = 1;
                lookup.GetIndex(index).Fpr("value").Fpr("dynamicStartTime").Fpr("tabPos").intValue = 1;

                lookup.GetIndex(index).Fpr("value").Fpr("defaultSettings").Fpr("volume").floatValue = 1f;
                lookup.GetIndex(index).Fpr("value").Fpr("defaultSettings").Fpr("pitch").floatValue = 1f;

                lookup.GetIndex(index).Fpr("value").Fpr("metaData").Fpr("category").stringValue = string.Empty;
                lookup.GetIndex(index).Fpr("value").Fpr("metaData").Fpr("tags").ClearArray();
                
                ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.ApplyModifiedProperties();
                ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.Update();
            }
            
            DidSomething = true;
        }
    }
}