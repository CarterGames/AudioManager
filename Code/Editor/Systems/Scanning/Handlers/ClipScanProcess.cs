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