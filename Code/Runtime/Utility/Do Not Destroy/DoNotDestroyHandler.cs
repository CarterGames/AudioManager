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

using CarterGames.Shared.AudioManager;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles accessing stuff in the do not destroy scene for the asset.
    /// </summary>
    public static class DoNotDestroyHandler
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets if the handler is initialized.
        /// </summary>
        public static bool IsInitialized { get; private set; }
        
        
        /// <summary>
        /// The base parent of the do not destroy setup.
        /// </summary>
        public static Transform BaseParent { get; private set; }
        
        
        /// <summary>
        /// The audio parent for pooling objects.
        /// </summary>
        public static Transform AudioParent { get; private set; }
        
        
        /// <summary>
        /// The audio parent for pooling objects.
        /// </summary>
        public static Transform AudioPlayersParent { get; private set; }
        
        
        /// <summary>
        /// The audio parent for pooling objects.
        /// </summary>
        public static Transform AudioInstancesParent { get; private set; }
        
        
        public static Transform PoolParentPlayers { get; private set; }
        public static Transform PoolParentSourceInstances { get; private set; }
        public static Transform PoolParentActive { get; private set; }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Initializes the class when called.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            if (IsInitialized) return;
            SetupParent();
        }


        /// <summary>
        /// Sets up the parents when called.
        /// </summary>
        private static void SetupParent()
        {
            if (IsInitialized) return;
            
            var obj = new GameObject("[Carter Games] Audio Manager");
            BaseParent = obj.transform;
            BaseParent.gameObject.AddComponent<Ref>();
            
            var audioPoolParent = new GameObject("-- Audio");
            var poolParent = new GameObject("-- Pool");
            PoolParentActive = new GameObject("-- Active").transform;
            PoolParentPlayers = new GameObject("- Players").transform;
            PoolParentSourceInstances = new GameObject("- Instances").transform;
                
            audioPoolParent.transform.SetParent(BaseParent);
            poolParent.transform.SetParent(audioPoolParent.transform);
            PoolParentActive.SetParent(audioPoolParent.transform);
            PoolParentPlayers.SetParent(poolParent.transform);
            PoolParentSourceInstances.SetParent(poolParent.transform);
            
            AudioParent = audioPoolParent.transform;
            AudioPlayersParent = PoolParentPlayers;
            AudioInstancesParent = PoolParentSourceInstances;
                
            Object.DontDestroyOnLoad(BaseParent.gameObject);
            IsInitialized = true;
        }
    }
}