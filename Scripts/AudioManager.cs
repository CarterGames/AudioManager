using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System.Linq;
using UnityEngine.SceneManagement;

/*
 * 
 *  Audio Manager
 *							  
 *	Audio Manager Script
 *      The main script of the Audio Manager asset, controls the playing of audio clips in a scene. 
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
    /// The main Audio Manager script used to play audio in your game...
    /// Instanced version of the class available if enabled in the inspector...
    /// </summary>
    [System.Serializable]
    public class AudioManager : MonoBehaviour
    {
        // Editor boolean values, not used in this script but needed for the custom inspector possibly....
        [SerializeField] private bool hasScannedOnce; // Tells the script whether or not the AMF has being scanned before.
        [SerializeField] private bool shouldShowDir; // Tells the script whether it should be displaying the directories in the inspector.
        [SerializeField] private bool shouldShowClips; // Tells the script whether it should be displaying the clips in the inspector.

        [SerializeField] private AudioManagerFile audioManagerFile; // The AMF currently in use by this instance of the Audio Manager.

        private Dictionary<string, AudioClip> lib;
        internal Stack<GameObject> pool;
        internal List<AudioSource> active;

        private bool canPlayAudio = true;
        
        
#if Use_CGAudioManager_Static || USE_CG_AM_STATIC
        public static AudioManager instance;
#endif
        
        private void OnEnable()
        {
#if Use_CGAudioManager_Static || USE_CG_AM_STATIC
            // Checks and removed instances if extra are present.
            DontDestroyOnLoad(this);

            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
#endif
        }
        
        private void Awake()
        {
            // Normal AM setup stuff xD
            if (audioManagerFile.soundPrefab == null)
                Debug.LogWarning("<color=#E77A7A><b>Audio Manager</b></color> | <color=#D6BA64>Warning Code 1</color> | Prefab has not been assigned! Please assign a prefab in the inspector before using the manager.");
            

            // For the audio source on the script, only used for previewing clips xD
            GetComponent<AudioSource>().hideFlags = HideFlags.HideInInspector;

            // Sets up the library from the audio manager file in use...
            lib = new Dictionary<string, AudioClip>();
            
            foreach (var _t in audioManagerFile.library)
            {
                lib.Add(_t.key, _t.value);
            }
            
            // Sets up the object pool & active list for use...
            pool = new Stack<GameObject>();
            active = new List<AudioSource>();

            canPlayAudio = true;
            
            // Runs the method to clear the active list and object pool on a scene change...
            SceneManager.sceneLoaded += ResetOnSceneChange;
        }



        /// <summary>
        /// Resets the manager on a scene change, as some references wil be lost on the scene change....
        /// </summary>
        /// <param name="scene">Scene | The scene to change to.</param>
        /// <param name="mode">LoadSceneMode | The method of scene loading to use.</param>
        private void ResetOnSceneChange(Scene scene, LoadSceneMode mode)
        {
            if (pool != null)
                pool.Clear();

            if (active != null)
                active.Clear();
        }
        
        
        
        /// <summary>
        /// Play a sound that is scanned into the audio manager.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="volume">Optional | Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Optional | Float | The pitch that the sound is played at | Default = 1.</param>
        public void Play(string request, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;
            if (!HasClip(request)) return;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request], 0, volume, pitch);
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Play a sound that is scanned into the audio manager.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="mixer">AudioMixerGroup | The mixer group to use...</param>
        /// <param name="volume">Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Float | The pitch that the sound is played at | Default = 1.</param>
        public void Play(string request, AudioMixerGroup mixer, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;
            if (!HasClip(request)) return;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request], 0, volume, pitch);

            _source.outputAudioMixerGroup = mixer;
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }


        /// <summary>
        /// Play a sound that is scanned into the audio manager.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="mixerID">Int | The mixer ID to use... Set in the Audio Manager Inspector...</param>
        /// <param name="volume">Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Float | The pitch that the sound is played at | Default = 1.</param>
        public void Play(string request, int mixerID, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;
            if (!HasClip(request)) return;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request], 0, volume, pitch);

            _source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }


        /// <summary>
        /// Play a sound that is scanned into the audio manager.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public void Play(string request, Hashtable args)
        {
            if (!canPlayAudio) return;
            if (!HasClip(request)) return;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source.clip = lib[request];
            _source = UpdateSourceWithArgs(_source, args);
            _source.time = 0;
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Play a sound that is scanned into the audio manager and returns the audio source for you to check / use as needed.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1.</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1.</param>
        public AudioSource PlayAndGetSource(string request, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return null;
            if (!HasClip(request)) return null;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source = SourceSetup(_source, lib[request], 0, volume, pitch);
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
            
            return _source;
        }
        
        
        /// <summary>
        /// Play a sound that is scanned into the audio manager and returns the audio source for you to check / use as needed.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="mixer">AudioMixerGroup | The mixer group to use...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1.</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1.</param>
        public AudioSource PlayAndGetSource(string request, AudioMixerGroup mixer, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return null;
            if (!HasClip(request)) return null;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source = SourceSetup(_source, lib[request], 0, volume, pitch);
            _source.outputAudioMixerGroup = mixer;
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
            
            return _source;
        }
        
        
        /// <summary>
        /// Play a sound that is scanned into the audio manager and returns the audio source for you to check / use as needed.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="mixerID">Int | The mixer ID to use... Set in the Audio Manager Inspector...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1.</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1.</param>
        public AudioSource PlayAndGetSource(string request, int mixerID, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return null;
            if (!HasClip(request)) return null;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source = SourceSetup(_source, lib[request], 0, volume, pitch);
            _source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
            
            return _source;
        }
        
        
        /// <summary>
        /// Play a sound that is scanned into the audio manager and returns the audio source for you to check / use as needed.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public AudioSource PlayAndGetSource(string request, Hashtable args)
        {
            if (!canPlayAudio) return null;
            if (!HasClip(request)) return null;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source.clip = lib[request];
            _source = UpdateSourceWithArgs(_source, args);
            _source.time = 0;
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
            
            return _source;
        }
        
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String Array | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="volume">Optional | Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Optional | Float | The pitch that the sound is played at | Default = 1.</param>
        public void PlayRange(string[] request, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;

            for (int i = 0; i < request.Length; i++)
            {
                if (!HasClip(request[i])) return;
            }

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request[Random.Range(0, request.Length)]],0, volume, pitch);
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String List | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="volume">Optional | Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Optional | Float | The pitch that the sound is played at | Default = 1.</param>
        public void PlayRange(List<string> request, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;

            for (int i = 0; i < request.Count; i++)
            {
                if (!HasClip(request[i])) return;
            }

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request[Random.Range(0, request.Count)]],0, volume, pitch);
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String Array | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="mixer">AudioMixerGroup | The mixer group to use...</param>
        /// <param name="volume">Optional | Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Optional | Float | The pitch that the sound is played at | Default = 1.</param>
        public void PlayRange(string[] request, AudioMixerGroup mixer, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;

            for (int i = 0; i < request.Length; i++)
            {
                if (!HasClip(request[i])) return;
            }

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request[Random.Range(0, request.Length)]],0, volume, pitch);
            _source.outputAudioMixerGroup = mixer;
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String List | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="mixer">AudioMixerGroup | The mixer group to use...</param>
        /// <param name="volume">Optional | Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Optional | Float | The pitch that the sound is played at | Default = 1.</param>
        public void PlayRange(List<string> request, AudioMixerGroup mixer, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;

            for (int i = 0; i < request.Count; i++)
            {
                if (!HasClip(request[i])) return;
            }

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request[Random.Range(0, request.Count)]],0, volume, pitch);
            _source.outputAudioMixerGroup = mixer;
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String Array | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="mixerID">Int | The mixer ID to use... Set in the Audio Manager Inspector...</param>
        /// <param name="volume">Optional | Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Optional | Float | The pitch that the sound is played at | Default = 1.</param>
        public void PlayRange(string[] request, int mixerID, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;

            for (int i = 0; i < request.Length; i++)
            {
                if (!HasClip(request[i])) return;
            }

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request[Random.Range(0, request.Length)]],0, volume, pitch);
            _source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String List | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="mixerID">Int | The mixer ID to use... Set in the Audio Manager Inspector...</param>
        /// <param name="volume">Optional | Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Optional | Float | The pitch that the sound is played at | Default = 1.</param>
        public void PlayRange(List<string> request, int mixerID, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;

            for (int i = 0; i < request.Count; i++)
            {
                if (!HasClip(request[i])) return;
            }

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request[Random.Range(0, request.Count)]],0, volume, pitch);
            _source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String Array | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public void PlayRange(string[] request, Hashtable args)
        {
            if (!canPlayAudio) return;

            for (int i = 0; i < request.Length; i++)
            {
                if (!HasClip(request[i])) return;
            }

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request[Random.Range(0, request.Length)]]);
            _source = UpdateSourceWithArgs(_source, args);
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String List | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public void PlayRange(List<string> request, Hashtable args)
        {
            if (!canPlayAudio) return;

            for (int i = 0; i < request.Count; i++)
            {
                if (!HasClip(request[i])) return;
            }

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request[Random.Range(0, request.Count)]]);
            _source = UpdateSourceWithArgs(_source, args);
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String Array | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="volume">Optional | Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Optional | Float | The pitch that the sound is played at | Default = 1.</param>
        public AudioSource PlayRangeAndGetSource(string[] request, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return null;

            for (int i = 0; i < request.Length; i++)
            {
                if (!HasClip(request[i])) return null;
            }

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request[Random.Range(0, request.Length)]],0, volume, pitch);
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
            return _source;
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String List | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="volume">Optional | Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Optional | Float | The pitch that the sound is played at | Default = 1.</param>
        public AudioSource PlayRangeAndGetSource(List<string> request, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return null;

            for (int i = 0; i < request.Count; i++)
            {
                if (!HasClip(request[i])) return null;
            }

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request[Random.Range(0, request.Count)]],0, volume, pitch);
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
            return _source;
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String Array | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="mixer">AudioMixerGroup | The mixer group to use...</param>
        /// <param name="volume">Optional | Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Optional | Float | The pitch that the sound is played at | Default = 1.</param>
        public AudioSource PlayRangeAndGetSource(string[] request, AudioMixerGroup mixer, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return null;

            for (int i = 0; i < request.Length; i++)
            {
                if (!HasClip(request[i])) return null;
            }

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request[Random.Range(0, request.Length)]],0, volume, pitch);
            _source.outputAudioMixerGroup = mixer;
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
            return _source;
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String List | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="mixer">AudioMixerGroup | The mixer group to use...</param>
        /// <param name="volume">Optional | Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Optional | Float | The pitch that the sound is played at | Default = 1.</param>
        public AudioSource PlayRangeAndGetSource(List<string> request, AudioMixerGroup mixer, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return null;

            for (int i = 0; i < request.Count; i++)
            {
                if (!HasClip(request[i])) return null;
            }

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request[Random.Range(0, request.Count)]],0, volume, pitch);
            _source.outputAudioMixerGroup = mixer;
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
            return _source;
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String Array | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="mixerID">Int | The mixer ID to use... Set in the Audio Manager Inspector...</param>
        /// <param name="volume">Optional | Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Optional | Float | The pitch that the sound is played at | Default = 1.</param>
        public AudioSource PlayRangeAndGetSource(string[] request, int mixerID, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return null;

            for (int i = 0; i < request.Length; i++)
            {
                if (!HasClip(request[i])) return null;
            }

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request[Random.Range(0, request.Length)]],0, volume, pitch);
            _source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
            return _source;
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String List | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="mixerID">Int | The mixer ID to use... Set in the Audio Manager Inspector...</param>
        /// <param name="volume">Optional | Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Optional | Float | The pitch that the sound is played at | Default = 1.</param>
        public AudioSource PlayRangeAndGetSource(List<string> request, int mixerID, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return null;

            for (int i = 0; i < request.Count; i++)
            {
                if (!HasClip(request[i])) return null;
            }

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request[Random.Range(0, request.Count)]],0, volume, pitch);
            _source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
            return _source;
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String Array | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public AudioSource PlayRangeAndGetSource(string[] request, Hashtable args)
        {
            if (!canPlayAudio) return null;

            for (int i = 0; i < request.Length; i++)
            {
                if (!HasClip(request[i])) return null;
            }

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request[Random.Range(0, request.Length)]]);
            _source = UpdateSourceWithArgs(_source, args);
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
            return _source;
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String List | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public AudioSource PlayRangeAndGetSource(List<string> request, Hashtable args)
        {
            if (!canPlayAudio) return null;

            for (int i = 0; i < request.Count; i++)
            {
                if (!HasClip(request[i])) return null;
            }

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request[Random.Range(0, request.Count)]]);
            _source = UpdateSourceWithArgs(_source, args);
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
            return _source;
        }
        

        /// <summary>
        /// Play a sound from a particular time code on the audio clip audioManagerFile.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="time">Float | The time you want to clip the be played from (float value for seconds)</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayFromTime(string request, float time, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;
            if (!HasClip(request)) return;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source = SourceSetup(_source, lib[request], time, volume, pitch);
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }


        /// <summary>
        /// Play a sound from a particular time code on the audio clip audioManagerFile.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="time">Float | The time you want to clip the be played from (float value for seconds)</param>
        /// <param name="mixer">AudioMixerGroup | The mixer group to use...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayFromTime(string request, float time, AudioMixerGroup mixer, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;
            if (!HasClip(request)) return;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source = SourceSetup(_source, lib[request], time, volume, pitch);
            _source.outputAudioMixerGroup = mixer;
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Play a sound from a particular time code on the audio clip audioManagerFile.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="time">Float | The time you want to clip the be played from (float value for seconds)</param>
        /// <param name="mixerID">Int | The mixer ID to use... Set in the Audio Manager Inspector...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayFromTime(string request, float time, int mixerID, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;
            if (!HasClip(request)) return;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source = SourceSetup(_source, lib[request], time, volume, pitch);
            _source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Play a sound from a particular time code on the audio clip audioManagerFile.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="time">Float | The time you want to clip the be played from (float value for seconds)</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public void PlayFromTime(string request, float time, Hashtable args)
        {
            if (!canPlayAudio) return;
            if (!HasClip(request)) return;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source.clip = lib[request];
            _source.time = time;
            _source = UpdateSourceWithArgs(_source, args);
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Play a sound from a particular time code on the audio clip audioManagerFile and returns the audio source for you to check / use as needed.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="time">Float | The time you want to clip the be played from (float value for seconds)</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public AudioSource PlayFromTimeAndGetSource(string request, float time, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return null;
            if (!HasClip(request)) return null;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source = SourceSetup(_source, lib[request], time, volume, pitch);
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
            return _source;
        }
        
        
        /// <summary>
        /// Play a sound from a particular time code on the audio clip audioManagerFile and returns the audio source for you to check / use as needed.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="time">Float | The time you want to clip the be played from (float value for seconds)</param>
        /// <param name="mixer">AudioMixerGroup | The mixer group to use...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public AudioSource PlayFromTimeAndGetSource(string request, float time, AudioMixerGroup mixer,  float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return null;
            if (!HasClip(request)) return null;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source = SourceSetup(_source, lib[request], time, volume, pitch);
            _source.outputAudioMixerGroup = mixer;
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
            return _source;
        }
        
        
        /// <summary>
        /// Play a sound from a particular time code on the audio clip audioManagerFile and returns the audio source for you to check / use as needed.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="time">Float | The time you want to clip the be played from (float value for seconds)</param>
        /// <param name="mixerID">Int | The mixer ID to use... Set in the Audio Manager Inspector...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public AudioSource PlayFromTimeAndGetSource(string request, float time, int mixerID,  float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return null;
            if (!HasClip(request)) return null;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source = SourceSetup(_source, lib[request], time, volume, pitch);
            _source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
            return _source;
        }
        
        
        /// <summary>
        /// Play a sound from a particular time code on the audio clip audioManagerFile and returns the audio source for you to check / use as needed.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="time">Float | The time you want to clip the be played from (float value for seconds)</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public AudioSource PlayFromTimeAndGetSource(string request, float time, Hashtable args)
        {
            if (!canPlayAudio) return null;
            if (!HasClip(request)) return null;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source.clip = lib[request];
            _source.time = time;
            _source = UpdateSourceWithArgs(_source, args);
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);

            return _source;
        }

        
        /// <summary>
        /// Play a sound after a defined amount of time
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="delay">Float | The amount of time you want the clip to wait before playing</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayWithDelay(string request, float delay, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;
            if (!HasClip(request)) return;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source = SourceSetup(_source, lib[request], 0, volume, pitch);
            
            _source.PlayDelayed(delay);
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Play a sound after a defined amount of time
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="delay">Float | The amount of time you want the clip to wait before playing</param>
        /// <param name="mixer">AudioMixerGroup | The mixer group to use...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayWithDelay(string request, float delay, AudioMixerGroup mixer, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;
            if (!HasClip(request)) return;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source = SourceSetup(_source, lib[request], 0, volume, pitch);

            _source.outputAudioMixerGroup = mixer;
            _source.PlayDelayed(delay);
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Play a sound after a defined amount of time
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="delay">Float | The amount of time you want the clip to wait before playing</param>
        /// <param name="mixerID">Int | The mixer ID to use... Set in the Audio Manager Inspector...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayWithDelay(string request, float delay, int mixerID, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;
            if (!HasClip(request)) return;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source = SourceSetup(_source, lib[request], 0, volume, pitch);

            _source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            _source.PlayDelayed(delay);
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Play a sound after a defined amount of time
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="delay">Float | The amount of time you want the clip to wait before playing</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public void PlayWithDelay(string request, float delay, Hashtable args)
        {
            if (!canPlayAudio) return;
            if (!HasClip(request)) return;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source.clip = lib[request];
            _source = UpdateSourceWithArgs(_source, args);
            _source.time = 0;
            _source.PlayDelayed(delay);
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Play a sound after a defined amount of time and returns the audio source for you to check / use as needed...
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="delay">Float | The amount of time you want the clip to wait before playing</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        /// <returns>AudioSource | The AudioSource that has been set on this method call.</returns>
        public AudioSource PlayWithDelayAndGetSource(string request, float delay, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return null;
            if (!HasClip(request)) return null;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request], 0, volume, pitch);
            _source.PlayDelayed(delay);
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
            
            return _source;
        }
        
        
        /// <summary>
        /// Play a sound after a defined amount of time and returns the audio source for you to check / use as needed...
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="delay">Float | The amount of time you want the clip to wait before playing</param>
        /// <param name="mixer">AudioMixerGroup | The mixer group to use...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        /// <returns>AudioSource | The AudioSource that has been set on this method call.</returns>
        public AudioSource PlayWithDelayAndGetSource(string request, float delay, AudioMixerGroup mixer, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return null;
            if (!HasClip(request)) return null;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request], 0, volume, pitch);
            _source.outputAudioMixerGroup = mixer;
            _source.PlayDelayed(delay);
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
            
            return _source;
        }
        
        
        /// <summary>
        /// Play a sound after a defined amount of time and returns the audio source for you to check / use as needed...
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="delay">Float | The amount of time you want the clip to wait before playing</param>
        /// <param name="mixerID">Int | The mixer ID to use... Set in the Audio Manager Inspector...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        /// <returns>AudioSource | The AudioSource that has been set on this method call.</returns>
        public AudioSource PlayWithDelayAndGetSource(string request, float delay, int mixerID, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return null;
            if (!HasClip(request)) return null;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[request], 0, volume, pitch);
            _source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            _source.PlayDelayed(delay);
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
            
            return _source;
        }
        
        
        /// <summary>
        /// Play a sound after a defined amount of time and returns the audio source for you to check / use as needed...
        /// </summary>
        /// <param name="request">The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="delay">The amount of time you want the clip to wait before playing</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        /// <returns>AudioSource | The AudioSource that has been set on this method call.</returns>
        public AudioSource PlayWithDelayAndGetSource(string request, float delay, Hashtable args)
        {
            if (!canPlayAudio) return null;
            if (!HasClip(request)) return null;

            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source.clip = lib[request];
            _source = UpdateSourceWithArgs(_source, args);
            _source.time = 0;
            _source.PlayDelayed(delay);
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
            
            return _source;
        }
        

        /// <summary>
        /// Play a random sound that has been scanned by this manager...
        /// </summary>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayRandom(float volume = 1, float pitch = 1)
        {
            if (!canPlayAudio) return;
            
            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source = SourceSetup(_source, lib[GetRandomSound.name], 0, volume, pitch);
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Play a random sound that has been scanned by this manager
        /// </summary>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public void PlayRandom(Hashtable args)
        {
            if (!canPlayAudio) return;
            
            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source.clip = lib[GetRandomSound.name];
            _source = UpdateSourceWithArgs(_source, args);
            _source.time = 0;
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }

        
        /// <summary>
        /// Play a random sound that has been scanned by this manager, from a particular time
        /// </summary>
        /// <param name="time">Float | The time you want to clip the be played from (float value for seconds)</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayRandomFromTime(float time, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;
            
            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source = SourceSetup(_source, lib[GetRandomSound.name], time, volume, pitch);
            
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Play a random sound that has been scanned by this manager, from a particular time
        /// </summary>
        /// <param name="time">Float | The time you want to clip the be played from (float value for seconds)</param>
        /// <param name="mixer">AudioMixerGroup | The mixer group to use...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayRandomFromTime(float time, AudioMixerGroup mixer, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;
            
            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source = SourceSetup(_source, lib[GetRandomSound.name], time, volume, pitch);

            _source.outputAudioMixerGroup = mixer;
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Play a random sound that has been scanned by this manager, from a particular time
        /// </summary>
        /// <param name="time">Float | The time you want to clip the be played from (float value for seconds)</param>
        /// <param name="mixerID">Int | The mixer ID to use... Set in the Audio Manager Inspector...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayRandomFromTime(float time, int mixerID, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;
            
            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source = SourceSetup(_source, lib[GetRandomSound.name], time, volume, pitch);

            _source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }

        
        /// <summary>
        /// Play a random sound that has been scanned by this manager, from a particular time
        /// </summary>
        /// <param name="time">Float | The time you want to clip the be played from (float value for seconds)</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public void PlayRandomFromTime(float time, Hashtable args)
        {
            if (!canPlayAudio) return;
            
            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source.clip = lib[GetRandomSound.name];
            _source.time = time;
            _source = UpdateSourceWithArgs(_source, args);
            _source.Play();
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Play a random sound that has been scanned by this manager...
        /// </summary>
        /// <param name="delay">Float | The amount of time you want the clip to wait before playing</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayRandomWithDelay(float delay, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;
            
            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();
            
            _source = SourceSetup(_source, lib[GetRandomSound.name], 0, volume, pitch);
            
            _source.PlayDelayed(delay);
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Play a random sound that has been scanned by this manager...
        /// </summary>
        /// <param name="delay">Float | The amount of time you want the clip to wait before playing</param>
        /// <param name="mixer">AudioMixerGroup | The mixer group to use...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayRandomWithDelay(float delay, AudioMixerGroup mixer, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;
            
            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source = SourceSetup(_source, lib[GetRandomSound.name], 0, volume, pitch);

            _source.outputAudioMixerGroup = mixer;
            _source.PlayDelayed(delay);
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Play a random sound that has been scanned by this manager...
        /// </summary>
        /// <param name="delay">Float | The amount of time you want the clip to wait before playing</param>
        /// <param name="mixerID">Int | The mixer ID to use... Set in the Audio Manager Inspector...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayRandomWithDelay(float delay, int mixerID, float volume = 1f, float pitch = 1f)
        {
            if (!canPlayAudio) return;
            
            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source = SourceSetup(_source, lib[GetRandomSound.name], 0, volume, pitch);

            _source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            _source.PlayDelayed(delay);
            
            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }

        
        /// <summary>
        /// Play a random sound that has been scanned by this manager...
        /// </summary>
        /// <param name="delay">Float | The amount of time you want the clip to wait before playing</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public void PlayRandomWithDelay(float delay, Hashtable args)
        {
            if (!canPlayAudio) return;
            
            var _clip = ClipSetup;
            var _source = _clip.GetComponent<AudioSource>();
            var _audioRemoval = _source.GetComponent<AudioRemoval>();

            _source.clip = lib[GetRandomSound.name];
            _source = UpdateSourceWithArgs(_source, args);
            _source.time = 0;
            _source.PlayDelayed(delay);

            AddToAudioRemoval(_audioRemoval, _clip, _source);
        }
        
        
        /// <summary>
        /// Runs the common code for getting a prefab to play the audio on...
        /// </summary>
        private GameObject ClipSetup
        {
            get
            {
                GameObject _go;

                if (pool.Count > 0)
                {
                    _go = pool.Pop();
                    _go.SetActive(true);
                }
                else
                    _go = Instantiate(audioManagerFile.soundPrefab);

                if (!_go.GetComponent<AudioSource>())
                {
                    Debug.LogWarning(
                        "<color=#E77A7A><b>Audio Manager</b></color> | <color=#D6BA64>Warning Code 4</color> | No AudioSource Component found on the Sound Prefab. Please ensure a AudioSource Component is attached to your prefab.");
                    return null;
                }

                return _go;
            }
        }

        
        /// <summary>
        /// Sets up some of the standard audio source values that are consistently used in methods...
        /// </summary>
        /// <param name="source">AudioSource | Sauce to edit...</param>
        /// <param name="clip">AudioClip | Clip to play...</param>
        /// <param name="time">Float | The time to play from, default is 0...</param>
        /// <param name="vol">Float | The volume for the clip, default is 1...</param>
        /// <param name="pit">Float | The pitch for the clip, default is 1...</param>
        /// <returns>AudioSource | The edited AudioSource...</returns>
        private AudioSource SourceSetup(AudioSource source, AudioClip clip, float time = 0f, float vol = 1f, float pit = 1f)
        {
            source.clip = clip;
            source.volume = vol;
            source.pitch = pit;
            source.time = time;
            return source;
        }
        
        
        /// <summary>
        /// Adds the clip to audio removal for clean up once the clip has finished playing...
        /// </summary>
        /// <param name="removal">AudioRemoval | The removal script...</param>
        /// <param name="clip">GameObject | The clip gameObject...</param>
        /// <param name="source">AudioSource | The source to check...</param>
        private void AddToAudioRemoval(AudioRemoval removal, GameObject clip, AudioSource source)
        {
            active.Add(source);
            if (source.loop) return;
            removal.Cleanup(clip, source.clip.length);
        }
        

        /// <summary>
        /// Toggles whether or not the manager can play audio....
        /// This doesn't affect the Audio Player or Music Player....
        /// </summary>
        public bool CanPlayAudio
        {
            get => canPlayAudio;
            set => canPlayAudio = value;
        }
        
        
        /// <summary>
        /// Checks to see if a clip exists...
        /// </summary>
        /// <param name="request">String | The clip to find...</param>
        /// <returns>Bool</returns>
        public bool HasClip(string request)
        {
            if (lib.ContainsKey(request))
                return true;
            
            Debug.LogWarning($"<color=#E77A7A><b>Audio Manager</b></color> | <color=#D6BA64>Warning Code 2</color> | Could not find clip: <b><i>{request}</i></b>. Please ensure the clip is scanned and the string you entered is correct (Note the input is CaSe SeNsItIvE).");
            return false;
        }
        
        
        /// <summary>
        /// Gets the number of clips currently in this instance of the Audio Manager.
        /// </summary>
        public int GetNumberOfClips
        {
            get
            {
                if (audioManagerFile.library != null)
                    return audioManagerFile.library.Count;
                else
                    return 0;
            }
        }
        
        
        /// <summary>
        /// Picks a random sound from the current AMF and returns it.
        /// </summary>
        public AudioClip GetRandomSound => lib.Values.ElementAt(UnityEngine.Random.Range(0, audioManagerFile.library.Count - 1));
        
        
        /// <summary>
        /// Changes the Audio Manager File to what is inputted.
        /// </summary>
        public AudioManagerFile AudioManagerFile
        {
            get => audioManagerFile;
            set => audioManagerFile = value;
        }
        
        
        /// <summary>
        /// Gets the current AMF in use.
        /// </summary>
        /// <returns>AMF | The file currently in use by this instance of the Audio Manager</returns>
        public AudioManagerFile GetAudioManagerFile => audioManagerFile;
        
        
        /// <summary>
        /// Checks to see if the clip in question is playing
        /// </summary>
        /// <param name="clip">String | Clip to check</param>
        /// <returns>Bool</returns>
        public bool IsClipPlaying(string clip)
        {
            if (active == null) return false;
            if (active.Count == 0) return false;
            
            for (int i = 0; i < active.Count; i++)
            {
                if (active[i].clip.name.Equals(clip))
                    return true;
            }

            return false;
        }
        
        
        /// <summary>
        /// Updates the arguments to the audio source for use...
        /// </summary>
        /// <param name="source">AudioSource | Audio Source to edit...</param>
        /// <param name="args">Hashtable/AudioArgs | Arguments to edit...</param>
        /// <returns>AudioSource | The audio source editing with the arguments entered...</returns>
        private AudioSource UpdateSourceWithArgs(AudioSource source, Hashtable args)
        {
            // Replaces the old AtPosition methods...
            if (args.ContainsKey("position"))
                source.gameObject.transform.position = (Vector3) args["position"];
            
            // Replaces the old OnObject methods...
            if (args.ContainsKey("gameobject"))
                source.gameObject.transform.position = (Vector3) args["gameobject"];

            // Audio Source Args
            if (args.ContainsKey("volume"))
                source.volume = (float) args["volume"];

            if (args.ContainsKey("pitch"))
                source.pitch = (float) args["pitch"];
            
            if (args.ContainsKey("loop"))
                source.loop = (bool) args["loop"];
            
            if (args.ContainsKey("priority"))
                source.priority = (int) args["priority"];
            
            if (args.ContainsKey("mixergroup"))
                source.outputAudioMixerGroup = (AudioMixerGroup) args["mixergroup"]; ;
            
            if (args.ContainsKey("mixerid"))
                source.outputAudioMixerGroup = audioManagerFile.audioMixer[(int) args["mixerid"]];
            
            if (args.ContainsKey("stereopan"))
                source.panStereo = (float) args["stereopan"];

            if (args.ContainsKey("spatialblend"))
                source.spatialBlend = (float) args["spatialblend"];

            if (args.ContainsKey("reverbzonemix"))
                source.reverbZoneMix = (float) args["reverbzonemix"];

            if (args.ContainsKey("rolloffmode"))
                source.rolloffMode = (AudioRolloffMode) args["rolloffmode"];

            if (args.ContainsKey("spread"))
                source.spread = (float) args["spread"];

            if (args.ContainsKey("dopplerlevel"))
                source.dopplerLevel = (float) args["dopplerlevel"];

            if (args.ContainsKey("velocityupdatemode"))
                source.velocityUpdateMode = (AudioVelocityUpdateMode) args["velocityupdatemode"];

            if (args.ContainsKey("spatializeposteffects"))
                source.spatializePostEffects = (bool) args["spatializeposteffects"];

            if (args.ContainsKey("mindistance"))
                source.minDistance = (float) args["mindistance"];

            if (args.ContainsKey("maxdistance"))
                source.maxDistance = (float) args["maxdistance"];

            return source;
        }
    }
}