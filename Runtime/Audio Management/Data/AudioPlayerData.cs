using System;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// A data class for a clip to play in the audio player inspector.
    /// </summary>
    [Serializable]
    public class AudioPlayerData
    {
        /// <summary>
        /// Should the data be expanded?
        /// </summary>
        public bool show;
        
        
        /// <summary>
        /// The name of the clip.
        /// </summary>
        public string clipName;
        
        
        /// <summary>
        /// The volume ranges for the clip.
        /// </summary>
        public MinMaxFloat volume;
        
        
        /// <summary>
        /// The pitch ranges for the clip.
        /// </summary>
        public MinMaxFloat pitch;

        
        /// <summary>
        /// Should the optional options be shown?
        /// </summary>
        public bool showOptional;
        
        
        /// <summary>
        /// The time the clip should play from.
        /// </summary>
        public float fromTime;
        
        
        /// <summary>
        /// The delay the clip should have.
        /// </summary>
        public float clipDelay;
    }
}