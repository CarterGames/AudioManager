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