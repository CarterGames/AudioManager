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
    /// Handles the search provider for the mixers.
    /// </summary>
    public sealed class SearchProviderMixer : SearchProvider<string>
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   SearchProvider Implementation
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        public override string ProviderTitle => "Search mixers in library";
        
        
        public override List<SearchGroup<string>> GetEntriesToDisplay()
        {
            var searchList = new List<SearchGroup<string>>();
            var options = new List<SearchItem<string>>();
            
            foreach (var mixer in ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.MixerLookup)
            {
                if (ToExclude.Contains(mixer.Value.Key)) continue;

                var entry = SearchItem<string>.Set(mixer.Value.Key, mixer.Value.Uuid);

                options.Add(entry);
            }
            
            searchList.Add(new SearchGroup<string>(string.Empty, options));

            return searchList;
        }
    }
}