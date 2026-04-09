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