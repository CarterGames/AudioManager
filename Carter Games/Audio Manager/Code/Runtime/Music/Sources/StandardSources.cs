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

using System.Collections.Generic;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Defines the standard music source setup with 1 main & 1 transition source.
    /// </summary>
    [DefaultExecutionOrder(-10000)]
    public sealed class StandardSources : MonoBehaviour
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        [SerializeField] private AudioSource mainSource;
        [SerializeField] private AudioSource transitionSource;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The main audio source to play from.
        /// </summary>
        public AudioSource MainSource => mainSource;
        
        
        /// <summary>
        /// The transition audio source to use when transitioning if needed.
        /// </summary>
        public AudioSource TransitionSource => transitionSource;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private void OnEnable()
        {
            AssetAccessor.GetAsset<SettingsAssetRuntime>().MusicStateChanged.Add(OnMusicStateChanged);
        }


        private void OnDestroy()
        {
            AssetAccessor.GetAsset<SettingsAssetRuntime>().MusicStateChanged.Remove(OnMusicStateChanged);
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Initializes the class when called.
        /// </summary>
        public void Initialize()
        {
            if (MainSource != null && TransitionSource != null) return;

            var sources = new List<AudioSource>();

            if (MainSource != null)
            {
                sources.Add(MainSource);
            }

            if (TransitionSource != null)
            {
                sources.Add(TransitionSource);
            }

            if (!sources.Count.Equals(2))
            {
                while (sources.Count < 2)
                {
                    GameObject obj;
                    
                    if (sources.Count.Equals(0))
                    {
                        obj = new GameObject("Main Source");
                        obj.transform.SetParent(transform);
                        obj.AddComponent<AudioSource>();
                        mainSource = obj.GetComponent<AudioSource>();
                        sources.Add(mainSource);
                    }

                    if (sources.Count.Equals(1))
                    {
                        obj = new GameObject("Transition Source");
                        obj.transform.SetParent(transform);
                        obj.AddComponent<AudioSource>();
                        transitionSource = obj.GetComponent<AudioSource>();
                        sources.Add(transitionSource);
                    }
                }
            }

            foreach (var s in sources)
            {
                s.playOnAwake = false;
                
                if (AssetAccessor.GetAsset<SettingsAssetRuntime>().MusicAudioMixer != null)
                {
                    s.outputAudioMixerGroup = AssetAccessor.GetAsset<SettingsAssetRuntime>().MusicAudioMixer;
                }
            }
            
            AssetAccessor.GetAsset<SettingsAssetRuntime>().MusicStateChanged.Add(OnMusicStateChanged);
        }
        
        
        /// <summary>
        /// Switches the source references around so the main & transition sources are the other way around.
        /// </summary>
        public void SwitchSourceReferences()
        {
            var oldMain = MainSource;
            var oldTransition = TransitionSource;

            mainSource = oldTransition;
            transitionSource = oldMain;
        }


        private void OnMusicStateChanged()
        {
            MainSource.mute = AssetAccessor.GetAsset<SettingsAssetRuntime>().PlayMusicState == PlayState.PlayMuted;
            TransitionSource.mute = AssetAccessor.GetAsset<SettingsAssetRuntime>().PlayMusicState == PlayState.PlayMuted;
        }
    }
}