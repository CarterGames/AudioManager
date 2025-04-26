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