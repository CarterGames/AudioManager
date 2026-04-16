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