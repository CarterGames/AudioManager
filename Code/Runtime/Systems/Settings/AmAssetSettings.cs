/*
 * Audio Manager (3.x)
 * Copyright (c) Carter Games
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the
 * GNU General Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version. 
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
 *
 * You should have received a copy of the GNU General Public License along with this program.
 * If not, see <https://www.gnu.org/licenses/>. 
 */

using CarterGames.Shared.AudioManager;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// The settings object for runtime usage.
    /// </summary>
    public sealed class AmAssetSettings : AmDataAsset
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        [SerializeField] private PlayState playAudioState = PlayState.Play;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject sourceInstancePrefab;
        [SerializeField] private AudioMixerGroup clipAudioMixer;

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
        public GameObject PlayerPrefab
        {
            get
            {
                if (playerPrefab != null) return playerPrefab;
                playerPrefab = (GameObject) Resources.Load(UtilRuntime.DefaultAudioPlayerPrefabResourcesPath);
                return playerPrefab;
            }
        }
        
        
        /// <summary>
        /// The prefab for playing standard clips.
        /// </summary>
        public GameObject SourceInstancePrefab
        {
            get
            {
                if (sourceInstancePrefab != null) return sourceInstancePrefab;
                sourceInstancePrefab = (GameObject) Resources.Load(UtilRuntime.DefaultAudioSourcePrefabResourcesPath);
                return sourceInstancePrefab;
            }
        }


        /// <summary>
        /// The mixer group for playing standard clips.
        /// </summary>
        public AudioMixerGroup ClipAudioMixer => clipAudioMixer;
        
        
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
        /// Gets whether or not normal audio can be played currently based on the users settings.
        /// </summary>
        public bool CanPlayAudio => playAudioState != PlayState.Disabled;
        

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
            if (playerPrefab == null)
            {
                playerPrefab = (GameObject)Resources.Load(UtilRuntime.DefaultAudioPlayerPrefabResourcesPath);
            }
            
            if (sourceInstancePrefab == null)
            {
                sourceInstancePrefab = (GameObject) Resources.Load(UtilRuntime.DefaultAudioSourcePrefabResourcesPath);
            }
        }
        
        
        /// <summary>
        /// Reset the settings asset to default values (except for the editor settings).
        /// </summary>
        public void ResetSettings()
        {
            playerPrefab = (GameObject) Resources.Load(UtilRuntime.DefaultAudioPlayerPrefabResourcesPath);
            sourceInstancePrefab = (GameObject) Resources.Load(UtilRuntime.DefaultAudioSourcePrefabResourcesPath);
            
            clipAudioMixer = default;
            playAudioState = PlayState.Play;
            
            useGlobalVariance = true;
            volumeVarianceOffset = .1f;
            pitchVarianceOffset = .1f;

            dynamicDetectionOffset = 50;
        }
    }
}