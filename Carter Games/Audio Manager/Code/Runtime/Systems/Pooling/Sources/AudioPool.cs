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
        private static ObjectPool<AudioPlayerSequence> SequencePool { get; set; }
        
        
        /// <summary>
        /// The pool of audio players.
        /// </summary>
        private static ObjectPool<AudioPlayer> PlayerPool { get; set; }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Runs before any game logic runs and initializes the pools for use with some objects in them by default...
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (SequencePool != null) return;
            
            // Initializes the pool collections...
            var seqPrefab = AssetAccessor.GetAsset<SettingsAssetRuntime>().SequencePrefab;
            var playerPrefab = AssetAccessor.GetAsset<SettingsAssetRuntime>().Prefab;
            
            var initSize = AssetAccessor.GetAsset<SettingsAssetRuntime>().AudioPoolInitialSize;

            SequencePool = new ObjectPool<AudioPlayerSequence>(seqPrefab, DoNotDestroyHandler.AudioParent, initSize, false)
            {
                ShouldExpand = true
            };
            
            PlayerPool = new ObjectPool<AudioPlayer>(playerPrefab, DoNotDestroyHandler.AudioParent, initSize, false)
            {
                ShouldExpand = true
            };
        }


        /// <summary>
        /// Assigns a new audio sequence to use from the pool when called.
        /// </summary>
        /// <returns>An AudioPlayerSequence instance.</returns>
        public static AudioPlayerSequence AssignSequence()
        {
            return SequencePool.Assign();
        }
        
        
        /// <summary>
        /// Assigns a new audio player to use from the pool when called.
        /// </summary>
        /// <returns>An AudioPlayer instance.</returns>
        public static AudioPlayer AssignPlayer()
        {
            return PlayerPool.Assign();
        }


        /// <summary>
        /// Returns a audio sequence to the pool for re-use.
        /// </summary>
        /// <param name="playerSequence">The element to return.</param>
        public static void Return(AudioPlayerSequence playerSequence)
        {
            SequencePool.Return(playerSequence);
        }
        
        
        
        /// <summary>
        /// Returns a audio player to the pool for re-use.
        /// </summary>
        /// <param name="player">The element to return.</param>
        public static void Return(AudioPlayer player)
        {
            PlayerPool.Return(player);
        }


        /// <summary>
        /// Resets all the pools when called.
        /// </summary>
        public static void Reset()
        {
            SequencePool.Reset();
            PlayerPool.Reset();
        }
    }
}