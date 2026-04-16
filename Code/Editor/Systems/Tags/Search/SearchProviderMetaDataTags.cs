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
    public class SearchProviderMetaDataTags : SearchProvider<string>
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private static SerializedObject LibObj => ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   SearchProvider Implementation
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        public override string ProviderTitle => "Search Tags";
        
        
        public override List<SearchGroup<string>> GetEntriesToDisplay()
        {
            var searchList = new List<SearchGroup<string>>();
            var options = new List<SearchItem<string>>();
            
            for (var i = 0; i < LibObj.Fp("tags").arraySize; i++)
            {
                if (string.IsNullOrEmpty(LibObj.Fp("tags").GetIndex(i).stringValue)) continue;
                if (ToExclude.Contains(LibObj.Fp("tags").GetIndex(i).stringValue)) continue;
                var entry = SearchItem<string>.Set(LibObj.Fp("tags").GetIndex(i).stringValue, LibObj.Fp("tags").GetIndex(i).stringValue);
                options.Add(entry);
            }
            
            searchList.Add(new SearchGroup<string>(string.Empty, options));

            return searchList;
        }
    }
}