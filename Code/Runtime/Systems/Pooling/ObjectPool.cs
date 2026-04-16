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
using CarterGames.Assets.AudioManager.Logging;
using UnityEngine;
         
namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// A class for making object pools.
    /// </summary>
    /// <typeparam name="T">The type to pool as.</typeparam>
    public class ObjectPool<T>
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private readonly List<T> memberObjects;
        private readonly HashSet<T> unavailableObjects;
        private readonly GameObject prefab;
        private readonly Transform parent;
        private bool startActive;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Gets if the pool is initialized.
        /// </summary>
        public bool IsInitialized => memberObjects != null;


        /// <summary>
        /// Gets/Sets if the pool should auto-expand. Def = true.
        /// </summary>
        public bool ShouldExpand { get; set; } = true;
        
        
        /// <summary>
        /// Gets all the members of the pool.
        /// </summary>
        public List<T> AllMembers => memberObjects;
        
        
        /// <summary>
        /// Gets all the in use members of the pool.
        /// </summary>
        public HashSet<T> AllInUse => unavailableObjects;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Constructors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Creates a new pool.
        /// </summary>
        /// <param name="prefab">The prefab to pool.</param>
        /// <param name="parent">The parent of the pool objects.</param>
        /// <param name="initialCount">The initial count of objects in the pool.</param>
        /// <param name="startActive">Should the elements be active when spawned?</param>
        public ObjectPool(GameObject prefab, Transform parent, int initialCount = 3, bool startActive = false)
        {
            this.prefab = prefab;
            this.parent = parent;

            this.startActive = startActive;

            memberObjects = new List<T>();
            unavailableObjects = new HashSet<T>();

            Initialize(initialCount, startActive);
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Initializes the pool when called.
        /// </summary>
        /// <param name="initialCount">The initial count of objects in the pool.</param>
        /// <param name="startActive">Should the elements be active when spawned?</param>
        private void Initialize(int initialCount, bool startActive)
        {
            for (var i = 0; i < initialCount; i++)
            {
                var obj = Object.Instantiate(prefab, parent);
                var comp = obj.GetComponentInChildren<T>(true);
                
                obj.SetActive(startActive);
                memberObjects.Add(comp);
            }
        }


        /// <summary>
        /// Creates a new member of the pool when called.
        /// </summary>
        /// <returns>The newly created pool member.</returns>
        private T Create()
        {
            var newObj = Object.Instantiate(prefab, parent);
            var newMember = newObj.GetComponentInChildren<T>(true);
            memberObjects.Add(newMember);
            newObj.SetActive(startActive);
            return newMember;
        }


        /// <summary>
        /// Assigns a new member of the pool to be used. Creates a new member if needed and is allowed to expand.
        /// </summary>
        /// <returns>The assigned member.</returns>
        public T Assign()
        {
            foreach (var t in memberObjects)
            {
                if (unavailableObjects.Contains(t)) continue;
                unavailableObjects.Add(t);
                return t;
            }

            if (!ShouldExpand)
            {
                AmDebugLogger.Normal("No free member objects to return.");
                return default;
            }
            
            var newMember = Create();
            unavailableObjects.Add(newMember);
            return newMember;
        }


        /// <summary>
        /// Returns a member back to the pool for re-use.
        /// </summary>
        /// <param name="member">The member to return.</param>
        public void Return(T member)
        {
            unavailableObjects?.Remove(member);
        }
        
        
        /// <summary>
        /// Resets all pool members to be inactive by returning any active.
        /// </summary>
        public void Reset()
        {
            foreach (var inUseMember in unavailableObjects)
            {
                if (inUseMember == null) continue;
                Return(inUseMember);
            }
            
            unavailableObjects?.Clear();
        }
    }
}