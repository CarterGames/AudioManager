using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// The audio player, designed to play audio from an AMF from a UI object.
    /// </summary>
    [AddComponentMenu("Carter Games/Audio Manager/Audio Player")]
    public class AudioPlayer : MonoBehaviour
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        // The file to read and use in the player...
        [SerializeField] private AudioManagerFile audioManagerFile = default;
        [SerializeField] private AudioMixerGroup mixer = default;
        
        // Used to define what the script will play xD
        [SerializeField] private List<AudioPlayerData> clipsToPlay = default;
        [SerializeField, HideInInspector] private Vector2 scrollPos;

        // An instance of the library to use in the 
        private Dictionary<string, AudioClip> lib;
        private AudioManager am;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private void Start()
        {
#if Use_CGAudioManager_Static || USE_CG_AM_STATIC
            am = AudioManager.instance;
#else
            am = FindObjectOfType<AudioManager>();
#endif
            
            lib = new Dictionary<string, AudioClip>();

            foreach (var _t in audioManagerFile.library)
                lib.Add(_t.key, _t.value);
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Plays the clip(s) selected in the inspector as they are with the volume/pitch/mixer from the inspector.
        /// </summary>
        public void Play()
        {
            if (audioManagerFile.library == null) return;
            {
                for (var i = 0; i < clipsToPlay.Count; i++)
                {
                    if (lib.ContainsKey(clipsToPlay[i].clipName))
                    {
                        if (!am)
                        {
#if Use_CGAudioManager_Static || USE_CG_AM_STATIC
                            am = AudioManager.instance;
#else
                            am = FindObjectOfType<AudioManager>();
#endif
                        }

                        var clip = AudioPool.Assign();
                        clip.gameObject.SetActive(true);
                        
                        if (!clip) return;

                        var source = clip.GetComponent<AudioSource>();
                        var audioRemoval = source.GetComponent<AudioClipPlayer>();

                        source.clip = lib[clipsToPlay[i].clipName];
                        source.volume = clipsToPlay[i].volume.Random();
                        source.pitch = clipsToPlay[i].pitch.Random();

                        if (clipsToPlay[i].fromTime > 0)
                            source.time = clipsToPlay[i].fromTime;

                        source.outputAudioMixerGroup = mixer;
                        
                        if (clipsToPlay[i].clipDelay > 0)
                            source.PlayDelayed(clipsToPlay[i].clipDelay);
                        else
                            source.Play();
                        
                        audioRemoval.Cleanup(source.clip.length);
                    }
                    else
                    {
                        AmLog.Warning("Could not find clip. Please ensure the clip is scanned and the string you entered is correct (Note the input is CaSe SeNsItIvE).");
                    }
                }
            }
        }
    }
}