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