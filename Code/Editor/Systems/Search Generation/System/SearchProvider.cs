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
using CarterGames.Shared.AudioManager;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    // Implement to make a search provider for something....
    // You still have to have a way to open it, but it will show the values entered.
    public abstract class SearchProvider<TValue> : ScriptableObject, ISearchWindowProvider
    {
        public readonly Evt<SearchTreeEntry> SelectionMade = new Evt<SearchTreeEntry>();
        
        
        public abstract string ProviderTitle { get; }
        public abstract List<SearchGroup<TValue>> GetEntriesToDisplay();
        public List<string> ToExclude { get; set; } = new List<string>();
        

        public void Open()
        {
            ToExclude.Clear();
            
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), this);
        }
        
        
        public void Open(string toIgnore)
        {
            ToExclude.Clear();
            ToExclude.Add(toIgnore);
            
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), this);
        }
        
        
        public void Open(IEnumerable<string> toIgnore)
        {
            ToExclude.Clear();
            ToExclude.AddRange(toIgnore);
            
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), this);
        }

        
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var searchList = new List<SearchTreeEntry>();
            
            searchList.Add(new SearchTreeGroupEntry(new GUIContent(ProviderTitle), 0));
            searchList.AddRange(AdditionalEntries);

            return searchList;
        }
        
        
        private List<SearchTreeEntry> AdditionalEntries
        {
            get
            {
                var list = new List<SearchTreeEntry>();

                foreach (var entries in GetEntriesToDisplay())
                {
                    if (!entries.IsValidGroup)
                    {
                        foreach (var value in entries.Values)
                        {
                            if (ToExclude.Contains(value.Key)) continue;
                            list.Add(SearchHelper.CreateEntry(value.Key, 1, value.Value));
                        }
                    }
                    else
                    {
                        list.Add(SearchHelper.CreateGroup(entries.Key, 1));
                        
                        foreach (var value in entries.Values)
                        {
                            if (ToExclude.Contains(value.Key)) continue;
                            list.Add(SearchHelper.CreateEntry(value.Key, 2, value.Value));
                        }
                    }
                }

                return list;
            }
        }
        

        
        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            if (searchTreeEntry == null) return false;
            SelectionMade.Raise(searchTreeEntry);
            return true;
        }
    }
}