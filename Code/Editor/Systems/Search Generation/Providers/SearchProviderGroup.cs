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
    /// Handles the search provider for the groups.
    /// </summary>
    public sealed class SearchProviderGroup : SearchProvider<GroupData>
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   SearchProvider Implementation
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        public override string ProviderTitle => "Search Groups";
        
        
        public override List<SearchGroup<GroupData>> GetEntriesToDisplay()
        {
            var searchList = new List<SearchGroup<GroupData>>();
            var options = new List<SearchItem<GroupData>>();
            
            foreach (var group in ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.Groups)
            {
                if (ToExclude.Contains(group.GroupName)) continue;

                var entry = SearchItem<GroupData>.Set(group.GroupName, group);

                options.Add(entry);
            }
            
            searchList.Add(new SearchGroup<GroupData>(string.Empty, options));

            return searchList;
        }
    }
}