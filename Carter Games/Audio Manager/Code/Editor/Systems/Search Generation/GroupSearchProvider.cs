/*
 * Copyright (c) 2024 Carter Games
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
 * FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections.Generic;
using System.Text;
using CarterGames.Common;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the search provider for the groups.
    /// </summary>
    public sealed class GroupSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private static readonly StringBuilder Builder = new StringBuilder();

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// The last element selected.
        /// </summary>
        public static SearchTreeEntry LastSelected { get; private set; }
        
        
        /// <summary>
        /// The elements to exclude from the search provider.
        /// </summary>
        public static List<string> ToExclude { get; } = new List<string>();

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Events
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Raises when an entry is selected.
        /// </summary>
        public static readonly Evt<SearchTreeEntry> OnSearchTreeSelectionMade = new Evt<SearchTreeEntry>();

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   ISearchWindowProvider Implementation
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Creates the search GUI when called.
        /// </summary>
        /// <param name="context">The window ctx.</param>
        /// <returns>A list of entries to show.</returns>
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var searchList = new List<SearchTreeEntry>();

            // The group that names the search window popup when searching...
            searchList.Add(new SearchTreeGroupEntry(new GUIContent("Search Groups"), 0));

            foreach (var group in UtilEditor.Library.Groups)
            {
                if (ToExclude.Contains(group.GroupName)) continue;

                Builder.Clear();
                Builder.Append(" ");
                Builder.Append(group.GroupName);

                searchList.Add(new SearchTreeEntry(GUIContent.none)
                {
                    level = 1,
                    content = new GUIContent(Builder.ToString()),
                    userData = group
                });
            }

            return searchList;
        }


        /// <summary>
        /// Runs when an entry is selected.
        /// </summary>
        /// <param name="searchTreeEntry">The selected tree entry.</param>
        /// <param name="context">The context window.</param>
        /// <returns></returns>
        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            // Select the searched clip in the library & just show that result in the window.
            LastSelected = searchTreeEntry;
            OnSearchTreeSelectionMade.Raise(searchTreeEntry);
            return true;
        }
    }
}