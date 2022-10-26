using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Holds the audio data for the audio manager class to use.
    /// </summary>
    [CreateAssetMenu(fileName = "Audio Manager File", menuName = "Carter Games/Audio Manager/Audio Manager File")]
    public class AudioManagerFile : AudioManagerAsset
    {
        /// <summary>
        /// Used in the audio manager file editor script to move the tab around...
        /// </summary>
        [SerializeField, HideInInspector] private int tabPos = 0;     
        
        
        /// <summary>
        /// Holds a list of the audio clips stored in the AMF.
        /// </summary>
        public List<AudioMixerGroup> audioMixer;
        
        
        /// <summary>
        /// Holds the prefab spawned in to play sound from this AMF.
        /// </summary>
        public GameObject soundPrefab;
        
        
        /// <summary>
        /// Holds the boolean value for whether or not this AMF has been used to store audio.
        /// </summary>
        public bool isPopulated;
        
        
        /// <summary>
        /// Holds a list of directory strings for use in the AM.
        /// </summary>
        public List<string> directory; 
        
        
        /// <summary>
        /// Moved in 2.3.5 to be here instead of in the AM reference as it caused some issues with the automation.
        /// </summary>
        public List<AudioLibraryData> library;
    }
}