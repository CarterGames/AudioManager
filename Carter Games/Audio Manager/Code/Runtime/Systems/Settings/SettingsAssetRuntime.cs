/*
 * Copyright (c) 2024 Carter Games
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
 * FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using CarterGames.Common;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// The settings object for runtime usage.
    /// </summary>
    public sealed class SettingsAssetRuntime : AudioManagerAsset
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        [SerializeField] private PlayState playAudioState = PlayState.Play;
        [SerializeField] private GameObject audioPrefab;
        [SerializeField] private AudioMixerGroup clipAudioMixer;
        
        [SerializeField] private PlayState playMusicState = PlayState.Play;
        [SerializeField] private GameObject sequencePrefab;
        [SerializeField] private AudioMixerGroup musicAudioMixer;

        [SerializeField] private int audioPoolInitSize = 5;
        
        [SerializeField] private bool useGlobalVariance = true;
        [SerializeField] private float volumeVarianceOffset = .1f;
        [SerializeField] private float pitchVarianceOffset = .1f;
        
        [SerializeField] private bool useDynamicStartTime = true;
        [SerializeField] private int dynamicDetectionOffset = 50;
        [SerializeField] private int dynamicMaxSampleDivision = 4;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The prefab for playing standard clips.
        /// </summary>
        public GameObject SequencePrefab
        {
            get
            {
                if (sequencePrefab != null) return sequencePrefab;
                sequencePrefab = (GameObject) Resources.Load(UtilRuntime.DefaultAudioSequencePrefabResourcesPath);
                return sequencePrefab;
            }
        }
        
        
        /// <summary>
        /// The prefab for playing standard clips.
        /// </summary>
        public GameObject Prefab
        {
            get
            {
                if (audioPrefab != null) return audioPrefab;
                audioPrefab = (GameObject) Resources.Load(UtilRuntime.DefaultAudioPrefabResourcesPath);
                return audioPrefab;
            }
        }


        /// <summary>
        /// The mixer group for playing standard clips.
        /// </summary>
        public AudioMixerGroup ClipAudioMixer => clipAudioMixer;
        
        
        /// <summary>
        /// The mixer group for playing standard clips.
        /// </summary>
        public AudioMixerGroup MusicAudioMixer => musicAudioMixer;
        
        
        /// <summary>
        /// Should the user see the debug messages from the asset?
        /// </summary>
        public bool ShowDebugMessages => PerUserSettingsRuntime.ShowDebugLogs;
        
        
        /// <summary>
        /// The play state for standard clips.
        /// </summary>
        public PlayState PlayAudioState
        {
            get => playAudioState;
            set 
            {
                playAudioState = value;
                AudioStateChanged.Raise();
            }
        }
        
        
        /// <summary>
        /// The play state for standard clips.
        /// </summary>
        public PlayState PlayMusicState
        {
            get => playMusicState;
            set 
            {
                playMusicState = value;
                AudioStateChanged.Raise();
            }
        }
        

        /// <summary>
        /// Gets whether or not normal audio can be played currently based on the users settings.
        /// </summary>
        public bool CanPlayAudio => playAudioState != PlayState.Disabled;
        
        
        /// <summary>
        /// Gets whether or not normal audio can be played currently based on the users settings.
        /// </summary>
        public bool CanPlayMusic => playMusicState != PlayState.Disabled;


        /// <summary>
        /// Gets whether or not the user wants global variance to take affect.
        /// </summary>
        public bool UseGlobalVariance => useGlobalVariance;
        
        
        /// <summary>
        /// The variance that should apply to the volume of clips if the global variance is enabled.
        /// </summary>
        public float VariantVolume => Random.Range(-volumeVarianceOffset, volumeVarianceOffset);
        
        
        /// <summary>
        /// The variance that should apply to the pitch of clips if the global variance is enabled.
        /// </summary>
        public float VariantPitch => Random.Range(-pitchVarianceOffset, pitchVarianceOffset);


        /// <summary>
        /// Gets the initial size of the audio pool.
        /// </summary>
        public int AudioPoolInitialSize => audioPoolInitSize;
        
        
        /// <summary>
        /// Gets if the dynamic start time should be used for clips globally (can be locally toggled on/off as well).
        /// </summary>
        public bool UseDynamicStartTime => useDynamicStartTime;
        
        
        /// <summary>
        /// Gets the offset applied to the estimated time when setting a dynamic start/end time.
        /// </summary>
        public float DynamicDetectionOffset => dynamicDetectionOffset;
        
        
        /// <summary>
        /// Gets the default sample division used when detecting the start/end of clips.
        /// </summary>
        public float DynamicMaxSampleDivision => dynamicMaxSampleDivision;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Events
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Raises when the audio play state is changed.
        /// </summary>
        public readonly Evt AudioStateChanged = new Evt();
        
        
        /// <summary>
        /// Raises when the music play state is changed.
        /// </summary>
        public readonly Evt MusicStateChanged = new Evt();

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        public void Initialize()
        {
            if (sequencePrefab == null)
            {
                sequencePrefab = (GameObject)Resources.Load(UtilRuntime.DefaultAudioSequencePrefabResourcesPath);
            }
            
            if (audioPrefab == null)
            {
                audioPrefab = (GameObject)Resources.Load(UtilRuntime.DefaultAudioPrefabResourcesPath);
            }
        }
        
        
        /// <summary>
        /// Reset the settings asset to default values (except for the editor settings).
        /// </summary>
        public void ResetSettings()
        {
            sequencePrefab = (GameObject) Resources.Load(UtilRuntime.DefaultAudioSequencePrefabResourcesPath);
            audioPrefab = (GameObject) Resources.Load(UtilRuntime.DefaultAudioPrefabResourcesPath);
            
            clipAudioMixer = default;
            musicAudioMixer = default;
            playAudioState = PlayState.Play;
            playMusicState = PlayState.Play;
            
            useGlobalVariance = true;
            volumeVarianceOffset = .1f;
            pitchVarianceOffset = .1f;

            dynamicDetectionOffset = 50;
        }
    }
}