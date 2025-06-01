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
    /// Handles the object pooling for audio players....
    /// </summary>
    [DefaultExecutionOrder(short.MinValue)]
    public static class AudioPool
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static GameObject doNotDestroyParent;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// The pool of audio sequences.
        /// </summary>
        private static ObjectPool<AudioPlayer> PlayerObjectPool { get; set; }
        
        
        /// <summary>
        /// The pool of audio players.
        /// </summary>
        private static ObjectPool<AudioSourceInstance> SourceInstanceObjectPool { get; set; }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Runs before any game logic runs and initializes the pools for use with some objects in them by default...
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (PlayerObjectPool != null) return;

            if (!DoNotDestroyHandler.IsInitialized)
            {
                DoNotDestroyHandler.Initialize();
            }
            
            // Initializes the pool collections...
            var playerPrefab = AmAssetAccessor.GetAsset<AmAssetSettings>().PlayerPrefab;
            var instancePrefab = AmAssetAccessor.GetAsset<AmAssetSettings>().SourceInstancePrefab;
            
            var initSize = AmAssetAccessor.GetAsset<AmAssetSettings>().AudioPoolInitialSize;

            PlayerObjectPool = new ObjectPool<AudioPlayer>(playerPrefab, DoNotDestroyHandler.PoolParentPlayers, initSize, false)
            {
                ShouldExpand = true
            };
            
            SourceInstanceObjectPool = new ObjectPool<AudioSourceInstance>(instancePrefab, DoNotDestroyHandler.PoolParentSourceInstances, initSize, false)
            {
                ShouldExpand = true
            };
        }


        /// <summary>
        /// Assigns a new audio sequence to use from the pool when called.
        /// </summary>
        /// <returns>An AudioPlayerSequence instance.</returns>
        public static AudioPlayer AssignPlayer()
        {
            var activePlayer = PlayerObjectPool.Assign();
            activePlayer.transform.SetParent(DoNotDestroyHandler.PoolParentActive);
            return activePlayer;
        }
        
        
        /// <summary>
        /// Assigns a new audio player to use from the pool when called.
        /// </summary>
        /// <returns>An AudioPlayer instance.</returns>
        public static AudioSourceInstance AssignSource()
        {
            return SourceInstanceObjectPool.Assign();
        }


        /// <summary>
        /// Returns a audio sequence to the pool for re-use.
        /// </summary>
        /// <param name="player">The element to return.</param>
        public static void Return(AudioPlayer player)
        {
            player.gameObject.SetActive(false);
            PlayerObjectPool.Return(player);
            player.transform.SetParent(DoNotDestroyHandler.PoolParentPlayers);
        }
        
        
        
        /// <summary>
        /// Returns a audio player to the pool for re-use.
        /// </summary>
        /// <param name="instance">The element to return.</param>
        public static void Return(AudioSourceInstance instance)
        {
            instance.ResetSourceInstance();
            instance.transform.SetParent(DoNotDestroyHandler.PoolParentSourceInstances);
            instance.gameObject.SetActive(false);
            
            SourceInstanceObjectPool.Return(instance);
        }


        /// <summary>
        /// Resets all the pools when called.
        /// </summary>
        public static void Reset()
        {
            PlayerObjectPool.Reset();
            SourceInstanceObjectPool.Reset();
        }
    }
}