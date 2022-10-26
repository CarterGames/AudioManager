using System;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Used the store the key/pair values for each clip found in the audio manager scan.
    /// </summary>
    [Serializable]
    public class AudioLibraryData
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The key for the library entry.
        /// </summary>
        public string key;
        
        
        /// <summary>
        /// The value for the library entry.
        /// </summary>
        public AudioClip value;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Constructors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Creates a new library entry with the entered data.
        /// </summary>
        /// <param name="key">The name of the clip.</param>
        /// <param name="value">The clip itself.</param>
        public AudioLibraryData(string key, AudioClip value)
        {
            this.key = key;
            this.value = value;
        }
    }
}