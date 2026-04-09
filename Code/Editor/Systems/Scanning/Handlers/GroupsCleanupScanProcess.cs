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
using CarterGames.Shared.AudioManager.Editor;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the cleaning up of groups so any removed library entries are not used in any of the groups.
    /// </summary>
    public sealed class GroupsCleanupScanProcess : IScanProcess
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Stores the group keys and the keys to clear in said groups for use.
        /// </summary>
        private Dictionary<string, List<string>> ToClear { get; set; } = new Dictionary<string, List<string>>();
        
        
        /// <summary>
        /// Defines the oder this scan processor runs in.
        /// </summary>
        public int Priority => 21;
        
        
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
            ToClear.Clear();
            
            foreach (var group in ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.Groups)
            {
                foreach (var clipId in group.Clips)
                {
                    if (ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.LibraryLookup.ContainsKey(clipId)) continue;
                    
                    if (ToClear.ContainsKey(group.GroupName))
                    {
                        ToClear[group.GroupName].Add(clipId);
                    }
                    else
                    {
                        ToClear.Add(group.GroupName, new List<string>() { clipId } );
                    }
                }
            }
            
            return ToClear.Count > 0;
        }
        

        /// <summary>
        /// Updates the library with the required changes when called.
        /// </summary>
        public void UpdateLibrary()
        {
            DidSomething = false;
            
            if (ToClear.Count <= 0) return;

            var groupsLookup = ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.Fp("groups").Fpr("list");
            
            foreach (var groupDataToClear in ToClear)
            {
                foreach (var clipIdToRemove in groupDataToClear.Value)
                {
                    for (var i = 0; i < groupsLookup.arraySize; i++)
                    {
                        for (var j = 0; j < groupsLookup.GetIndex(i).Fpr("value").Fpr("clipNames").arraySize; j++)
                        {
                            if (groupsLookup.GetIndex(i).Fpr("value").Fpr("clipNames").GetIndex(j).stringValue != clipIdToRemove) continue;
                            groupsLookup.GetIndex(i).Fpr("value").Fpr("clipNames").DeleteIndex(j);
                            
                            ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.ApplyModifiedProperties();
                            ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.Update();
                            
                            goto NextToClear;
                        }
                    }
                    
                    NextToClear: ;
                }
            }
            
            DidSomething = true;
        }
    }
}