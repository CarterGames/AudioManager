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