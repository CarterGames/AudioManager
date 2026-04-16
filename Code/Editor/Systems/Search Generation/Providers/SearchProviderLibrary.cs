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
using UnityEditor;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the search provider for the clips.
    /// </summary>
    public sealed class SearchProviderLibrary : SearchProvider<AudioData>
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private static SerializedObject LibObj => ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef;
        private static AudioLibrary LibAsset => ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   SearchProvider Implementation
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        public override string ProviderTitle => "Search Audio Library";
        
        
        public override List<SearchGroup<AudioData>> GetEntriesToDisplay()
        {
            var searchList = new List<SearchGroup<AudioData>>();
            var groupsLookup = new Dictionary<string, SearchGroup<AudioData>>();

            for (var i = 0; i < LibObj.Fp("tags").arraySize; i++)
            {
                var key = LibObj.Fp("tags").GetIndex(i).stringValue;
                groupsLookup.Add(key, new SearchGroup<AudioData>(key, new List<SearchItem<AudioData>>()));
            }
            
            groupsLookup.Add(string.Empty, new SearchGroup<AudioData>(string.Empty, new List<SearchItem<AudioData>>()));
            
            for (var i = 0; i < LibObj.Fp("library").Fpr("list").arraySize; i++)
            {
                var entry = LibObj.Fp("library").Fpr("list").GetIndex(i);
                
                if (ToExclude.Contains(entry.Fpr("key").stringValue)) continue;
                if (!LibAsset.TryGetClip(entry.Fpr("key").stringValue, out var foundData)) continue;

                var value = SearchItem<AudioData>.Set(entry.Fpr("value").Fpr("key").stringValue, foundData);
                
                if (entry.Fpr("value").Fpr("metaData").Fpr("tags").arraySize > 0)
                {
                    for (var j = 0; j < entry.Fpr("value").Fpr("metaData").Fpr("tags").arraySize; j++)
                    {
                        var key = entry.Fpr("value").Fpr("metaData").Fpr("tags").GetIndex(j).stringValue;
                        groupsLookup[key].Values.Add(value);
                    }
                }
                else
                {
                    groupsLookup[string.Empty].Values.Add(value);
                }
            }
            
            foreach (var entry in groupsLookup)
            {
                searchList.Add(entry.Value);
            }

            return searchList;
        }
    }
}