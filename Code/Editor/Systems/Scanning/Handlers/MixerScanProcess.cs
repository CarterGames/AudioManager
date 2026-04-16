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

using System;
using System.Collections.Generic;
using System.Linq;
using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;
using UnityEngine.Audio;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the scanning of mixers into the library.
    /// </summary>
    public sealed class MixerScanProcess : IScanProcess
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Holds the result of the scan for mixers to process.
        /// </summary>
        private AudioScanResult<AudioMixerGroup> ScanResult { get; set; }
        
        
        /// <summary>
        /// Defines the oder this scan processor runs in.
        /// </summary>
        public int Priority => 10;
        
        
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
            if (!ProjectDataHelper.TryGetAllMixersInProject(out var mixersFound)) return false;
            
            var mixerToAdd = new List<AudioMixerGroup>();
                
            foreach (var mixerToCheck in mixersFound)
            {
                if (ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.MixerLookup.Any(t => t.Value.MixerGroup.Equals(mixerToCheck))) continue;
                mixerToAdd.Add(mixerToCheck);
            }
            
            ScanResult = new AudioScanResult<AudioMixerGroup>(mixerToAdd);
            return mixerToAdd.Count > 0;
        }
        

        /// <summary>
        /// Updates the library with the required changes when called.
        /// </summary>
        public void UpdateLibrary()
        {
            DidSomething = false;
            
            if (!ScanResult.HasData) return;

            var mixerLookup = ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.Fp("mixers").Fpr("list");
            
            foreach (var mixer in ScanResult.Data)
            {
                var uuid = Guid.NewGuid().ToString();
                var data = new MixerData(uuid, $"{mixer.name}{uuid.Split('-')[0]}", mixer);
             
                mixerLookup.InsertIndex(mixerLookup.arraySize);

                var index = mixerLookup.arraySize - 1;
                
                mixerLookup.GetIndex(index).Fpr("key").stringValue = data.Uuid;
                
                mixerLookup.GetIndex(index).Fpr("value").Fpr("uuid").stringValue = data.Uuid;
                mixerLookup.GetIndex(index).Fpr("value").Fpr("key").stringValue = data.Key;
                mixerLookup.GetIndex(index).Fpr("value").Fpr("path").stringValue = AssetDatabase.GetAssetPath(data.MixerGroup);
                mixerLookup.GetIndex(index).Fpr("value").Fpr("mixerGroup").objectReferenceValue = data.MixerGroup;
                
                ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.ApplyModifiedProperties();
                ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.Update();
            }
            
            DidSomething = true;
        }
    }
}