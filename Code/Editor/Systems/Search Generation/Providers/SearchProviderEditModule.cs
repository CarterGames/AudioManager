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

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the search provider for the edit modules.
    /// </summary>
    public sealed class SearchProviderEditModule : SearchProvider<Type>
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   SearchProvider Implementation
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        public override string ProviderTitle => "Select Edit Module";
        
        
        public override List<SearchGroup<Type>> GetEntriesToDisplay()
        {
            var searchList = new List<SearchGroup<Type>>();
            var options = new List<SearchItem<Type>>();
            
            foreach (var implementation in EditModuleInspectors.Inspectors)
            {
                if (ToExclude.Contains(implementation.Key.AssemblyQualifiedName)) continue;
                var entry = SearchItem<Type>.Set(implementation.Key.FullName.Replace("CarterGames.Assets.AudioManager.", string.Empty).Replace("Edit", string.Empty), implementation.Key);
                options.Add(entry);
            }
            
            searchList.Add(new SearchGroup<Type>(string.Empty, options));

            return searchList;
        }
    }
}