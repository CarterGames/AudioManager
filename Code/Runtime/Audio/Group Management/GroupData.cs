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
using System.Linq;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Defines a group of clips that can be used with the manager...
    /// </summary>
    [Serializable]
    public sealed class GroupData
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        [SerializeField] private string groupName;
        [SerializeField] private GroupPlayMode groupPlayMode;
        [SerializeField] private List<string> clipNames;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets/Sets the name of the group...
        /// </summary>
        public string GroupName
        {
            get => groupName;
            set => groupName = value;
        }

        
        /// <summary>
        /// Gets the clips in the group...
        /// </summary>
        public List<string> Clips => clipNames;

        public GroupPlayMode PlayMode => groupPlayMode;


        /// <summary>
        /// Returns if the clip exists in the group...
        /// </summary>
        /// <param name="clipName">The clip name to search for...</param>
        /// <returns>The result...</returns>
        public bool HasClip(string clipName) => clipName.Contains(clipName);
        
        
        /// <summary>
        /// Sets the clips stored in this group to the entered data...
        /// </summary>
        /// <param name="clips"></param>
        public void SetClips(List<string> clips) => clipNames = clips;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Constructors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Creates a new group data with the name of the group...
        /// </summary>
        /// <param name="groupName">The name for the group...</param>
        public GroupData(string groupName)
        {
            this.groupName = groupName;
            clipNames = new List<string>();
        }
        
        
        /// <summary>
        /// Creates a new group data with the name & data entered...
        /// </summary>
        /// <param name="groupName">The name for the group...</param>
        /// <param name="clips">The clips to entered...</param>
        public GroupData(string groupName, IEnumerable<string> clips)
        {
            this.groupName = groupName;
            clipNames = clips.ToList();
        }
        
        
        /// <summary>
        /// Creates a new group data with the name & data entered...
        /// </summary>
        /// <param name="groupName">The name for the group...</param>
        /// <param name="clips">The clips to entered...</param>
        public GroupData(string groupName, IEnumerable<string> clips, GroupPlayMode playMode)
        {
            this.groupName = groupName;
            clipNames = clips.ToList();
            groupPlayMode = playMode;
        }
    }
}