using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// The class than handles all the log messages for the asset.
    /// </summary>
    public class AmLog
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private const string LogPrefix = "<color=#e07979><b>Audio Manager</b></color> | ";
        private const string WarningPrefix = "<color=#D6BA64><b>Warning</b></color> | ";
        private const string ErrorPrefix = "<color=#E77A7A><b>Error</b></color> | ";

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Displays a normal debug message for the build versions asset...
        /// </summary>
        /// <param name="message">The message to show...</param>
        public static void Normal(string message)
        {
            if (!AssetAccessor.GetAsset<AudioManagerSettings>().showDebugMessages) return;
            Debug.Log($"{LogPrefix}{message}");
        }
        
        
        /// <summary>
        /// Displays a warning debug message for the build versions asset...
        /// </summary>
        /// <param name="message">The message to show...</param>
        public static void Warning(string message) 
        {
            if (!AssetAccessor.GetAsset<AudioManagerSettings>().showDebugMessages) return;
            Debug.LogWarning($"{LogPrefix}{WarningPrefix}{message}");
        }
        
        
        /// <summary>
        /// Displays a error debug message for the build versions asset...
        /// </summary>
        /// <param name="message">The message to show...</param>
        public static void Error(string message)
        {
            if (!AssetAccessor.GetAsset<AudioManagerSettings>().showDebugMessages) return;
            Debug.LogError($"{LogPrefix}{ErrorPrefix}{message}");
        }
    }
}