using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Accesses the scriptable objects for the asset at runtime.
    /// </summary>
    public static class AssetAccessor
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static AudioManagerAsset[] _assets;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets all the assets from the build versions asset...
        /// </summary>
        private static IEnumerable<AudioManagerAsset> Assets
        {
            get
            {
                if (_assets != null) return _assets;
                _assets = Resources.LoadAll("Carter Games/Audio Manager", typeof(AudioManagerAsset)).Cast<AudioManagerAsset>().ToArray();
                return _assets;
            }
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets the Build Versions Asset requested...
        /// </summary>
        /// <typeparam name="T">The build versions asset to get.</typeparam>
        /// <returns>The asset if it exists.</returns>
        public static T GetAsset<T>() where T : AudioManagerAsset
        {
            return (T)Assets.FirstOrDefault(t => t.GetType() == typeof(T));
        }
    }
}