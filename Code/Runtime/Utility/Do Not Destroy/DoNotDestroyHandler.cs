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