namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// The settings asset for the audio manager.
    /// </summary>
    public class AudioManagerSettings : AudioManagerAsset
    {
        /// <summary>
        /// The base scan directory for the asset.
        /// </summary>
        public string baseAudioScanPath = "Assets";
        
        
        /// <summary>
        /// Defines if the asset is using the static instance or not.
        /// </summary>
        public bool isUsingStatic;
        
        
        /// <summary>
        /// Defines if the asset shows any log messages.
        /// </summary>
        public bool showDebugMessages = true;
    }
}