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
    [Serializable]
    public class SearchGroup<TValue>
    {
        public string Key { get; private set; }
        public List<SearchItem<TValue>> Values { get; private set; }


        public bool IsValidGroup => !string.IsNullOrEmpty(Key);

        
        
        public SearchGroup(string groupName, List<SearchItem<TValue>> values)
        {
            Key = groupName;
            Values = values;
        }
        

        public SearchGroup(List<SearchItem<TValue>> values)
        {
            Key = string.Empty;
            Values = values;
        }
    }
}