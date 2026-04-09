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