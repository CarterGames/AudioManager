/*
 * Copyright (c) 2018-Present Carter Games
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System.Linq;
using Random = UnityEngine.Random;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// The main Audio Manager script used to play audio in your game...
    /// Static Instanced version of the class available if enabled in the inspector...
    /// </summary>
    [Serializable, AddComponentMenu("Carter Games/Audio Manager/Audio Manager")]
    public class AudioManager : MonoBehaviour
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        // Editor boolean values, not used in this script but needed for the custom inspectors to correctly function....
        [SerializeField, HideInInspector] private bool hasScannedOnce;
        [SerializeField, HideInInspector] private bool shouldShowMixers;
        [SerializeField, HideInInspector] private bool shouldShowDir;
        [SerializeField, HideInInspector] private bool shouldShowClips;

        [SerializeField] private AudioManagerFile audioManagerFile;

        private Dictionary<string, AudioClip> lib;
        private bool canPlayAudio = true;
        
#if Use_CGAudioManager_Static || USE_CG_AM_STATIC
        public static AudioManager instance;
#endif
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Toggles whether or not the manager can play audio....
        /// This doesn't affect the Music Player....
        /// </summary>
        public bool CanPlayAudio
        {
            get => canPlayAudio;
            set => canPlayAudio = value;
        }
        
        
        /// <summary>
        /// Runs the common code for getting a prefab to play the audio on...
        /// </summary>
        private GameObject ClipSetup
        {
            get
            {
                AudioSource _go;

                if (!AudioPool.IsInitialised)
                    AudioPool.Initialise(audioManagerFile.soundPrefab, 10);

                _go = AudioPool.Assign();
                _go.gameObject.SetActive(true);
                
                if (!_go.GetComponent<AudioSource>())
                {
                    AmLog.Warning("No AudioSource Component found on the Sound Prefab. Please ensure a AudioSource Component is attached to your prefab.");
                    return null;
                }

                return _go.gameObject;
            }
        }
        
        
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
        
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
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
                AmLog.Warning("Prefab has not been assigned! Please assign a prefab in the inspector before using the manager.");
            
            // For the audio source on the script, only used for previewing clips xD
            GetComponent<AudioSource>().hideFlags = HideFlags.HideInInspector;

            // Sets up the library from the audio manager file in use...
            lib = new Dictionary<string, AudioClip>();
            
            foreach (var t in audioManagerFile.library)
                lib.Add(t.key, t.value);

            canPlayAudio = true;

            if (AudioPool.ExistsInScene) return;
            var obj = new GameObject("Audio Pool");
            obj.AddComponent<AudioPool>();
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Play Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Play a sound that is scanned into the audio manager.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="volume">Optional | Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Optional | Float | The pitch that the sound is played at | Default = 1.</param>
        public void Play(string request, float? volume = 1f, float? pitch = 1f, int? priority = 128, bool? loop = false)
        {
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request], volume, pitch,  priority: priority, loop: loop);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
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
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request], volume, pitch);

            source.outputAudioMixerGroup = mixer;
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
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
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request], volume, pitch);

            source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            source.Play();

            AddToAudioRemoval(audioRemoval, source);
        }


        /// <summary>
        /// Play a sound that is scanned into the audio manager.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public void Play(string request, Hashtable args)
        {
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source.clip = lib[request];
            source = UpdateSourceWithArgs(source, args);
            source.time = 0;
            source.Play();

            AddToAudioRemoval(audioRemoval, source);
        }
        
        
        /// <summary>
        /// Play a sound that is scanned into the audio manager and returns the audio source for you to check / use as needed.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1.</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1.</param>
        public AudioSource PlayAndGetSource(string request, float volume = 1f, float pitch = 1f)
        {
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            
            return source;
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
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch);
            source.outputAudioMixerGroup = mixer;
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            
            return source;
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
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch);
            source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            
            return source;
        }
        
        
        /// <summary>
        /// Play a sound that is scanned into the audio manager and returns the audio source for you to check / use as needed.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public AudioSource PlayAndGetSource(string request, Hashtable args)
        {
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source.clip = lib[request];
            source = UpdateSourceWithArgs(source, args);
            source.time = 0;
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            
            return source;
        }
        
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String Array | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="volume">Optional | Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Optional | Float | The pitch that the sound is played at | Default = 1.</param>
        public void PlayRange(string[] request, float volume = 1f, float pitch = 1f)
        {
            if (!CanPlayAudio) return;

            for (var i = 0; i < request.Length; i++)
                if (!HasClip(request[i])) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request[Random.Range(0, request.Length)]], volume, pitch);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String List | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="volume">Optional | Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Optional | Float | The pitch that the sound is played at | Default = 1.</param>
        public void PlayRange(List<string> request, float volume = 1f, float pitch = 1f)
        {
            if (!CanPlayAudio) return;

            for (var i = 0; i < request.Count; i++)
                if (!HasClip(request[i])) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request[Random.Range(0, request.Count)]], volume, pitch);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
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
            if (!CanPlayAudio) return;

            for (var i = 0; i < request.Length; i++)
                if (!HasClip(request[i])) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request[Random.Range(0, request.Length)]], volume, pitch);
            source.outputAudioMixerGroup = mixer;
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
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
            if (!CanPlayAudio) return;

            for (var i = 0; i < request.Count; i++)
                if (!HasClip(request[i])) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request[Random.Range(0, request.Count)]], volume, pitch);
            source.outputAudioMixerGroup = mixer;
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
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
            if (!CanPlayAudio) return;

            for (var i = 0; i < request.Length; i++)
                if (!HasClip(request[i])) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request[Random.Range(0, request.Length)]], volume, pitch);
            source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
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
            if (!CanPlayAudio) return;

            for (var i = 0; i < request.Count; i++)
                if (!HasClip(request[i])) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request[Random.Range(0, request.Count)]],volume, pitch);
            source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String Array | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public void PlayRange(string[] request, Hashtable args)
        {
            if (!CanPlayAudio) return;

            for (var i = 0; i < request.Length; i++)
                if (!HasClip(request[i])) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request[Random.Range(0, request.Length)]]);
            source = UpdateSourceWithArgs(source, args);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String List | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public void PlayRange(List<string> request, Hashtable args)
        {
            if (!CanPlayAudio) return;

            for (var i = 0; i < request.Count; i++)
                if (!HasClip(request[i])) return;
            
            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request[Random.Range(0, request.Count)]]);
            source = UpdateSourceWithArgs(source, args);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String Array | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="volume">Optional | Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Optional | Float | The pitch that the sound is played at | Default = 1.</param>
        public AudioSource PlayRangeAndGetSource(string[] request, float volume = 1f, float pitch = 1f)
        {
            if (!CanPlayAudio) return null;

            for (var i = 0; i < request.Length; i++)
                if (!HasClip(request[i])) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request[Random.Range(0, request.Length)]], volume, pitch);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            return source;
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String List | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="volume">Optional | Float | The volume that the clip will be played at | Default = 1.</param>
        /// <param name="pitch">Optional | Float | The pitch that the sound is played at | Default = 1.</param>
        public AudioSource PlayRangeAndGetSource(List<string> request, float volume = 1f, float pitch = 1f)
        {
            if (!CanPlayAudio) return null;

            for (var i = 0; i < request.Count; i++)
                if (!HasClip(request[i])) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request[Random.Range(0, request.Count)]],volume, pitch);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            return source;
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
            if (!CanPlayAudio) return null;

            for (var i = 0; i < request.Length; i++)
                if (!HasClip(request[i])) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request[Random.Range(0, request.Length)]], volume, pitch);
            source.outputAudioMixerGroup = mixer;
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            return source;
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
            if (!CanPlayAudio) return null;

            for (var i = 0; i < request.Count; i++)
                if (!HasClip(request[i])) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request[Random.Range(0, request.Count)]], volume, pitch);
            source.outputAudioMixerGroup = mixer;
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            return source;
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
            if (!CanPlayAudio) return null;

            for (var i = 0; i < request.Length; i++)
                if (!HasClip(request[i])) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request[Random.Range(0, request.Length)]], volume, pitch);
            source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            return source;
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
            if (!CanPlayAudio) return null;

            for (var i = 0; i < request.Count; i++)
                if (!HasClip(request[i])) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request[Random.Range(0, request.Count)]], volume, pitch);
            source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            return source;
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String Array | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public AudioSource PlayRangeAndGetSource(string[] request, Hashtable args)
        {
            if (!CanPlayAudio) return null;

            for (var i = 0; i < request.Length; i++)
                if (!HasClip(request[i])) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request[Random.Range(0, request.Length)]]);
            source = UpdateSourceWithArgs(source, args);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            return source;
        }
        
        
        /// <summary>
        /// Plays a sound from a collection of clips that you have entered...
        /// </summary>
        /// <param name="request">String List | The name of the audio clip you want to play (note it is case sensitive).</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public AudioSource PlayRangeAndGetSource(List<string> request, Hashtable args)
        {
            if (!CanPlayAudio) return null;

            for (var i = 0; i < request.Count; i++)
                if (!HasClip(request[i])) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request[Random.Range(0, request.Count)]]);
            source = UpdateSourceWithArgs(source, args);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            return source;
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
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch, time);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
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
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch, time);
            source.outputAudioMixerGroup = mixer;
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
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
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch, time);
            source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
        }
        
        
        /// <summary>
        /// Play a sound from a particular time code on the audio clip audioManagerFile.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="time">Float | The time you want to clip the be played from (float value for seconds)</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public void PlayFromTime(string request, float time, Hashtable args)
        {
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source.clip = lib[request];
            source.time = time;
            source = UpdateSourceWithArgs(source, args);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
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
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch, time);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            return source;
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
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch, time);
            source.outputAudioMixerGroup = mixer;
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            return source;
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
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch, time);
            source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            return source;
        }
        
        
        /// <summary>
        /// Play a sound from a particular time code on the audio clip audioManagerFile and returns the audio source for you to check / use as needed.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="time">Float | The time you want to clip the be played from (float value for seconds)</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public AudioSource PlayFromTimeAndGetSource(string request, float time, Hashtable args)
        {
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source.clip = lib[request];
            source.time = time;
            source = UpdateSourceWithArgs(source, args);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);

            return source;
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
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch);
            
            source.PlayDelayed(delay);
            
            AddToAudioRemoval(audioRemoval, source, delay);
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
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch);

            source.outputAudioMixerGroup = mixer;
            source.PlayDelayed(delay);
            
            AddToAudioRemoval(audioRemoval, source, delay);
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
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch);

            source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            source.PlayDelayed(delay);
            
            AddToAudioRemoval(audioRemoval, source, delay);
        }
        
        
        /// <summary>
        /// Play a sound after a defined amount of time
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="delay">Float | The amount of time you want the clip to wait before playing</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public void PlayWithDelay(string request, float delay, Hashtable args)
        {
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source.clip = lib[request];
            source = UpdateSourceWithArgs(source, args);
            source.time = 0;
            source.PlayDelayed(delay);
            
            AddToAudioRemoval(audioRemoval, source, delay);
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
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request], volume, pitch);
            source.PlayDelayed(delay);
            
            AddToAudioRemoval(audioRemoval, source, delay);
            
            return source;
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
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request], volume, pitch);
            source.outputAudioMixerGroup = mixer;
            source.PlayDelayed(delay);
            
            AddToAudioRemoval(audioRemoval, source, delay);
            
            return source;
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
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request], volume, pitch);
            source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            source.PlayDelayed(delay);
            
            AddToAudioRemoval(audioRemoval, source, delay);
            
            return source;
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
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source.clip = lib[request];
            source = UpdateSourceWithArgs(source, args);
            source.time = 0;
            source.PlayDelayed(delay);
            
            AddToAudioRemoval(audioRemoval, source, delay);
            
            return source;
        }
        

        /// <summary>
        /// Play a sound at a particular position in your game.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="position">Vector3 | The position to play at.</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayAtLocation(string request, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch);
            source.transform.position = position;
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
        }


        /// <summary>
        /// Play a sound at a particular position in your game.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="position">Vector3 | The position to play at.</param>
        /// <param name="mixer">AudioMixerGroup | The mixer group to use...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayAtLocation(string request, Vector3 position, AudioMixerGroup mixer, float volume = 1f, float pitch = 1f)
        {
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request], volume, pitch);
            source.transform.position = position;
            source.outputAudioMixerGroup = mixer;
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
        }
        
        
        /// <summary>
        /// Play a sound at a particular position in your game.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="position">Vector3 | The position to play at.</param>
        /// <param name="mixerID">Int | The mixer ID to use... Set in the Audio Manager Inspector...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayAtLocation(string request, Vector3 position, int mixerID, float volume = 1f, float pitch = 1f)
        {
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch);
            source.transform.position = position;
            source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
        }
        
        
        /// <summary>
        /// Play a sound at a particular position in your game.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="position">Vector3 | The position to play at.</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public void PlayAtLocation(string request, Vector3 position, Hashtable args)
        {
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source.clip = lib[request];
            source.transform.position = position;
            source = UpdateSourceWithArgs(source, args);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
        }
        
        
        /// <summary>
        /// Play a sound at a particular position in your game and returns the audio source for you to check / use as needed.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="position">Vector3 | The position to play at.</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public AudioSource PlayAtLocationAndGetSource(string request, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch);
            source.transform.position = position;
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            return source;
        }
        
        
        /// <summary>
        /// Play a sound at a particular position in your game and returns the audio source for you to check / use as needed.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="position">Vector3 | The position to play at.</param>
        /// <param name="mixer">AudioMixerGroup | The mixer group to use...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public AudioSource PlayAtLocationAndGetSource(string request, Vector3 position, AudioMixerGroup mixer,  float volume = 1f, float pitch = 1f)
        {
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch);
            source.transform.position = position;
            source.outputAudioMixerGroup = mixer;
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            return source;
        }
        
        
        /// <summary>
        /// Play a sound at a particular position in your game and returns the audio source for you to check / use as needed.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="position">Vector3 | The position to play at.</param>
        /// <param name="mixerID">Int | The mixer ID to use... Set in the Audio Manager Inspector...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public AudioSource PlayAtLocationAndGetSource(string request, Vector3 position, int mixerID,  float volume = 1f, float pitch = 1f)
        {
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch);
            source.transform.position = position;
            source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            return source;
        }
        
        
        /// <summary>
        /// Play a sound at a particular position in your game and returns the audio source for you to check / use as needed.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="position">Vector3 | The position to play at.</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public AudioSource PlayAtLocationAndGetSource(string request, Vector3 position, Hashtable args)
        {
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source.clip = lib[request];
            source.transform.position = position;
            source = UpdateSourceWithArgs(source, args);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);

            return source;
        }
        
        
        /// <summary>
        /// Play a sound at a particular position in your game.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="obj">Transform | The parent to play on.</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayOnObject(string request, Transform obj, float volume = 1f, float pitch = 1f)
        {
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch);
            source.transform.SetParent(obj);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
        }


        /// <summary>
        /// Play a sound at a particular position in your game.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="obj">Transform | The parent to play on.</param>
        /// <param name="mixer">AudioMixerGroup | The mixer group to use...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayOnObject(string request, Transform obj, AudioMixerGroup mixer, float volume = 1f, float pitch = 1f)
        {
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[request], volume, pitch);
            source.transform.SetParent(obj);
            source.outputAudioMixerGroup = mixer;
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
        }
        
        
        /// <summary>
        /// Play a sound at a particular position in your game.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="obj">Transform | The parent to play on.</param>
        /// <param name="mixerID">Int | The mixer ID to use... Set in the Audio Manager Inspector...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayOnObject(string request, Transform obj, int mixerID, float volume = 1f, float pitch = 1f)
        {
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch);
            source.transform.SetParent(obj);
            source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
        }
        
        
        /// <summary>
        /// Play a sound at a particular position in your game.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="obj">Transform | The parent to play on.</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public void PlayOnObject(string request, Transform obj, Hashtable args)
        {
            if (!CanPlayAudio) return;
            if (!HasClip(request)) return;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source.clip = lib[request];
            source.transform.SetParent(obj);
            source = UpdateSourceWithArgs(source, args);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
        }
        
        
        /// <summary>
        /// Play a sound at a particular position in your game and returns the audio source for you to check / use as needed.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="obj">Transform | The parent to play on.</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public AudioSource PlayOnObjectAndGetSource(string request, Transform obj, float volume = 1f, float pitch = 1f)
        {
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch);
            source.transform.SetParent(obj);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            return source;
        }
        
        
        /// <summary>
        /// Play a sound at a particular position in your game and returns the audio source for you to check / use as needed.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="obj">Transform | The parent to play on.</param>
        /// <param name="mixer">AudioMixerGroup | The mixer group to use...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public AudioSource PlayOnObjectAndGetSource(string request, Transform obj, AudioMixerGroup mixer,  float volume = 1f, float pitch = 1f)
        {
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch);
            source.transform.SetParent(obj);
            source.outputAudioMixerGroup = mixer;
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            return source;
        }
        
        
        /// <summary>
        /// Play a sound at a particular position in your game and returns the audio source for you to check / use as needed.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="obj">Transform | The parent to play on.</param>
        /// <param name="mixerID">Int | The mixer ID to use... Set in the Audio Manager Inspector...</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public AudioSource PlayOnObjectAndGetSource(string request, Transform obj, int mixerID,  float volume = 1f, float pitch = 1f)
        {
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[request], volume, pitch);
            source.transform.SetParent(obj);
            source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
            return source;
        }
        
        
        /// <summary>
        /// Play a sound at a particular position in your game and returns the audio source for you to check / use as needed.
        /// </summary>
        /// <param name="request">String | The name of the audio clip you want to play (note it is case sensitive)</param>
        /// <param name="obj">Transform | The parent to play on.</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public AudioSource PlayOnObjectAndGetSource(string request, Transform obj, Hashtable args)
        {
            if (!CanPlayAudio) return null;
            if (!HasClip(request)) return null;

            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source.clip = lib[request];
            source.transform.SetParent(obj);
            source = UpdateSourceWithArgs(source, args);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);

            return source;
        }
        
        
        /// <summary>
        /// Play a random sound that has been scanned by this manager...
        /// </summary>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayRandom(float volume = 1, float pitch = 1)
        {
            if (!CanPlayAudio) return;
            
            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[GetRandomSound.name], volume, pitch);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
        }
        
        
        /// <summary>
        /// Play a random sound that has been scanned by this manager
        /// </summary>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public void PlayRandom(Hashtable args)
        {
            if (!CanPlayAudio) return;
            
            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source.clip = lib[GetRandomSound.name];
            source = UpdateSourceWithArgs(source, args);
            source.time = 0;
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
        }

        
        /// <summary>
        /// Play a random sound that has been scanned by this manager, from a particular time
        /// </summary>
        /// <param name="time">Float | The time you want to clip the be played from (float value for seconds)</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayRandomFromTime(float time, float volume = 1f, float pitch = 1f)
        {
            if (!CanPlayAudio) return;
            
            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[GetRandomSound.name], volume, pitch, time);
            
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
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
            if (!CanPlayAudio) return;
            
            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[GetRandomSound.name], volume, pitch, time);

            source.outputAudioMixerGroup = mixer;
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
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
            if (!CanPlayAudio) return;
            
            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[GetRandomSound.name], volume, pitch, time);

            source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            source.Play();

            AddToAudioRemoval(audioRemoval, source);
        }

        
        /// <summary>
        /// Play a random sound that has been scanned by this manager, from a particular time
        /// </summary>
        /// <param name="time">Float | The time you want to clip the be played from (float value for seconds)</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public void PlayRandomFromTime(float time, Hashtable args)
        {
            if (!CanPlayAudio) return;
            
            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source.clip = lib[GetRandomSound.name];
            source.time = time;
            source = UpdateSourceWithArgs(source, args);
            source.Play();
            
            AddToAudioRemoval(audioRemoval, source);
        }
        
        
        /// <summary>
        /// Play a random sound that has been scanned by this manager...
        /// </summary>
        /// <param name="delay">Float | The amount of time you want the clip to wait before playing</param>
        /// <param name="volume">Float | The volume that the clip will be played at, default = 1</param>
        /// <param name="pitch">Float | The pitch that the sound is played at, default = 1</param>
        public void PlayRandomWithDelay(float delay, float volume = 1f, float pitch = 1f)
        {
            if (!CanPlayAudio) return;
            
            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();
            
            source = SourceSetup(source, lib[GetRandomSound.name], volume, pitch);
            
            source.PlayDelayed(delay);
            
            AddToAudioRemoval(audioRemoval, source, delay);
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
            if (!CanPlayAudio) return;
            
            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[GetRandomSound.name], volume, pitch);

            source.outputAudioMixerGroup = mixer;
            source.PlayDelayed(delay);
            
            AddToAudioRemoval(audioRemoval, source, delay);
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
            if (!CanPlayAudio) return;
            
            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source = SourceSetup(source, lib[GetRandomSound.name], volume, pitch);

            source.outputAudioMixerGroup = audioManagerFile.audioMixer[mixerID];
            source.PlayDelayed(delay);
            
            AddToAudioRemoval(audioRemoval, source, delay);
        }

        
        /// <summary>
        /// Play a random sound that has been scanned by this manager...
        /// </summary>
        /// <param name="delay">Float | The amount of time you want the clip to wait before playing</param>
        /// <param name="args">Hashtable | Full custom options, e.g. of use: AudioManager.instance.AudioArgs("Volume", 1f, "Pitch", 1f);</param>
        public void PlayRandomWithDelay(float delay, Hashtable args)
        {
            if (!CanPlayAudio) return;
            
            var clip = ClipSetup;
            var source = clip.GetComponent<AudioSource>();
            var audioRemoval = source.GetComponent<AudioClipPlayer>();

            source.clip = lib[GetRandomSound.name];
            source = UpdateSourceWithArgs(source, args);
            source.time = 0;
            source.PlayDelayed(delay);

            AddToAudioRemoval(audioRemoval, source, delay);
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Utility Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Sets up some of the standard audio source values that are consistently used in methods...
        /// </summary>
        /// <param name="source">AudioSource | Sauce to edit...</param>
        /// <param name="clip">AudioClip | Clip to play...</param>
        /// <param name="time">Float | The time to play from, default is 0...</param>
        /// <param name="vol">Float | The volume for the clip, default is 1...</param>
        /// <param name="pit">Float | The pitch for the clip, default is 1...</param>
        /// <returns>AudioSource | The edited AudioSource...</returns>
        private AudioSource SourceSetup(AudioSource source, AudioClip clip, float? vol = 1f, float? pit = 1f, float? time = 0f, int? priority = 128, bool? loop = false)
        {
            source.clip = clip;
            source.volume = vol ?? 1f;
            source.pitch = pit ?? 1f;
            source.priority = priority ?? 128;
            source.loop = loop ?? false;
            source.time = time ?? 0;
            return source;
        }


        /// <summary>
        /// Adds the clip to audio removal for clean up once the clip has finished playing...
        /// </summary>
        /// <param name="clipPlayer">AudioRemoval | The removal script...</param>
        /// <param name="source">AudioSource | The source to check...</param>
        /// <param name="extraTime">Float | Any extra time to wait for before removing...</param>
        private void AddToAudioRemoval(AudioClipPlayer clipPlayer, AudioSource source, float extraTime = 0f)
        {
            if (source.loop) return;
            clipPlayer.Cleanup(source.clip.length + extraTime);
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
            
            AmLog.Warning($"Could not find clip: <b><i>{request}</i></b>. Please ensure the clip is scanned and the string you entered is correct (Note the input is CaSe SeNsItIvE).");
            return false;
        }


        /// <summary>
        /// Picks a random sound from the current AMF and returns it.
        /// </summary>
        public AudioClip GetRandomSound => lib.Values.ElementAt(Random.Range(0, audioManagerFile.library.Count - 1));
        
        
        /// <summary>
        /// Checks to see if the clip in question is playing
        /// </summary>
        /// <param name="clip">String | Clip to check</param>
        /// <returns>Bool</returns>
        public bool IsClipPlaying(string clip)
        {
            return AudioPool.AnyActiveWithClip(clip);
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