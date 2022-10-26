using System;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Provides some events you can listen to for a clip that is playing or due to play.
    /// </summary>
    public class AudioEvents : MonoBehaviour
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Events
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Called when a clip starts playing...
        /// </summary>
        public static event Action OnClipStart;
        
        /// <summary>
        /// Called when a clip finishes playing...
        /// </summary>
        public static event Action OnClipEnd;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private void OnEnable() => OnClipStart?.Invoke();

        private void OnDisable() => OnClipEnd?.Invoke();
    }
}