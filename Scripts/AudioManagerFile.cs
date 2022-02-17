using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/*
 * 
 *  Audio Manager
 *							  
 *	Audio Manager File (AMF) Scriptable Object
 *      Holds data for the audio manager script to then use to play audio.
 *
 *  Also Contains:
 *      DataStore
 *          Holds the values that are inputted into a library for use in the asset. 
 *			
 *  Written by:
 *      Jonathan Carter
 *
 *  Published By:
 *      Carter Games
 *      E: hello@carter.games
 *      W: https://www.carter.games
 *		
 *  Version: 2.5.6
*	Last Updated: 09/02/2022 (d/m/y)									
 * 
 */

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Scriptable Object | Holds the audio data that is used in the Audio Manager asset.
    /// </summary>
    [CreateAssetMenu(fileName = "Audio Manager File", menuName = "Carter Games/Audio Manager | CG/Audio Manager File")]
    public class AudioManagerFile : ScriptableObject
    {
        public List<AudioMixerGroup> audioMixer;  // Holds a list of the audio clips stored in the AMF.
        public GameObject soundPrefab;     // Holds the prefab spawned in to play sound from this AMF.
        public bool isPopulated;           // Holds the boolean value for whether or not this AMF has been used to store audio.
        public List<string> directory;     // Holds a list of directory strings for use in the AM.
        public List<AudioLibrary> library;     // Moved in 2.3.5 to be here instead of in the AM reference as it caused some issues with the automation.
        
        [SerializeField] private int tabPos;                  // Used in the audio manager file editor script to move the tab around...
    }
}