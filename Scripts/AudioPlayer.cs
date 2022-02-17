using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

/*
 * 
 *  Audio Manager
 *							  
 *	Audio Player Script
 *      A script to play allow sounds to be played on any event using the Audio Manager asset. 		
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
    /// MonoBehaviour Class | The audio player, designed to play audio from an AMF from a UI object.
    /// </summary>
    public class AudioPlayer : MonoBehaviour
    {
        // The file to read and use in the player...
        [SerializeField] private AudioManagerFile audioManagerFile = default;
        [SerializeField] private AudioMixerGroup mixer = default;
        
        // Used to define what the script will play xD
        [SerializeField] private List<string> clipsToPlay = default;
        [SerializeField] private List<float> clipsVolume = default;
        [SerializeField] private List<float> clipsPitch = default;
        [SerializeField] private List<float> clipsFromTime = default;
        [SerializeField] private List<float> clipsWithDelay = default;
        
        // Used in the editor code to make the custom inspector work xD
        [SerializeField] private List<bool> dropDowns;
        [SerializeField] private List<bool> dropDownsOptional;
        
        // An instance of the library to use in the 
        private Dictionary<string, AudioClip> lib;
        private AudioManager am;
        
        

        private void Awake()
        {
#if Use_CGAudioManager_Static || USE_CG_AM_STATIC
            am = AudioManager.instance;
#else
            am = FindObjectOfType<AudioManager>();
#endif
            
            lib = new Dictionary<string, AudioClip>();

            foreach (var _t in audioManagerFile.library)
            {
                lib.Add(_t.key, _t.value);
            }
        }


        // Legit only here so you can disable the script in the inspector xD
        private void Start()
        {
        }


        /// <summary>
        /// Plays the clip(s) selected in the inspector as they are with the volume/pitch/mixer from the inspector.
        /// </summary>
        public void Play()
        {
            if (audioManagerFile.library == null) return;
            {
                for (int i = 0; i < clipsToPlay.Count; i++)
                {
                    if (lib.ContainsKey(clipsToPlay[i]))
                    {
                        GameObject _clip;

                        if (!am)
                        {
#if Use_CGAudioManager_Static || USE_CG_AM_STATIC
                            am = AudioManager.instance;
#else
                            am = FindObjectOfType<AudioManager>();
#endif
                        }

                        if (am.pool.Count > 0)
                        {
                            _clip = am.pool.Pop();
                            _clip.SetActive(true);
                        }
                        else
                            _clip = Instantiate(audioManagerFile.soundPrefab);


                        if (!_clip.GetComponent<AudioSource>()) return;

                        var _source = _clip.GetComponent<AudioSource>();
                        var _audioRemoval = _source.GetComponent<AudioRemoval>();

                        _source.clip = lib[clipsToPlay[i]];
                        _source.volume = clipsVolume[i];
                        _source.pitch = clipsPitch[i];

                        if (clipsFromTime[i] > 0)
                            _source.time = clipsFromTime[i];

                        _source.outputAudioMixerGroup = mixer;
                        
                        if (clipsWithDelay[i] > 0)
                            _source.PlayDelayed(clipsWithDelay[i]);
                        else
                            _source.Play();
                        
                        _audioRemoval.Cleanup(_clip, _source.clip.length);
                    }
                    else
                        Debug.LogWarning(
                            "<color=#E77A7A><b>Audio Manager</b></color> | Audio Player | <color=#D6BA64>Warning Code 1</color> | Could not find clip. Please ensure the clip is scanned and the string you entered is correct (Note the input is CaSe SeNsItIvE).");
                }
            }
        }
    }
}