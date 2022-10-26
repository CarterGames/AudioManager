using System;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// A copy of the Scarlet Library MinMaxFloat setup for use in the asset.
    /// </summary>
    [Serializable]
    public class MinMaxFloat
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public float min;
        public float max;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets a random value within the ranges defined.
        /// </summary>
        /// <returns>A value within the ranges defined.</returns>
        public float Random()
        {
            return UnityEngine.Random.Range(min, max);
        }
    }
}