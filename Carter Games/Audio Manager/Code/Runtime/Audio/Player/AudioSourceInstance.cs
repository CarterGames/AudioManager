/*
 * Copyright (c) 2025 Carter Games
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

using System.Collections;
using System.Linq;
using CarterGames.Shared.AudioManager;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles playing audio from the audio library.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioSourceInstance : MonoBehaviour
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        [SerializeField] private AudioSourceState state;

        private AudioPlayer targetPlayer;
        private AudioSource source;
        private AudioClipSettings settings;
        private AudioData data;
        private Coroutine playerRoutine;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public bool IsInitialized { get; private set; }
        
        
        /// <summary>
        /// The audio source for the player.
        /// </summary>
        public AudioSource Source => source ??= GetComponent<AudioSource>();
        

        /// <summary>
        /// Gets if the player has been prepared or not. 
        /// </summary>
        public bool IsPrepared { get; private set; }


        /// <summary>
        /// Gets if the player is playing a clip currently.
        /// </summary>
        public bool IsPlaying => Source.isPlaying;
        
        
        /// <summary>
        /// Gets the parameters for the player.
        /// </summary>
        [field: SerializeField] public EditParameters EditParams { get; private set; } = new EditParameters();


        /// <summary>
        /// Gets the time remaining on the player.
        /// </summary>
        private float ClipTimeRemaining => this.GetTotalDuration(data);


        public AudioSourceState State
        {
            get => state;
            set => state = value;
        }

        public AudioPlayer TargetPlayer => targetPlayer;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Events
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Raises when the clip has started playing.
        /// </summary>
        public readonly Evt Started = new Evt();
        
        
        /// <summary>
        /// Raises when the clip has started paused.
        /// </summary>
        public readonly Evt Paused = new Evt();
        
        
        /// <summary>
        /// Raises when the clip has started resumed.
        /// </summary>
        public readonly Evt Resumed = new Evt();
        
        
        /// <summary>
        /// Raises when the clip has started stopped.
        /// </summary>
        public readonly Evt Stopped = new Evt();
        
        
        /// <summary>
        /// Raises when the clip has completed playing.
        /// </summary>
        public readonly Evt Completed = new Evt();

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private void OnEnable()
        {
            AmAssetAccessor.GetAsset<AmAssetSettings>().AudioStateChanged.Add(OnClipStateChanged);
        }


        private void OnDestroy()
        {
            AmAssetAccessor.GetAsset<AmAssetSettings>().AudioStateChanged.Remove(OnClipStateChanged);
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        public void InitializePlayer(AudioPlayer player, AudioData audioData, AudioClipSettings clipSettings = null)
        {
            targetPlayer = player;
            data = audioData;
            Source.clip = audioData.value;
            Source.mute = AmAssetAccessor.GetAsset<AmAssetSettings>().PlayAudioState == PlayState.PlayMuted;

            Source.volume = audioData.defaultSettings.Volume;
            Source.pitch = audioData.defaultSettings.Pitch;

            if (clipSettings != null)
            {
                settings = clipSettings;
                settings.ProcessEdits(this);
            }
            
            if (Source.outputAudioMixerGroup == null)
            {
                Source.outputAudioMixerGroup = AmAssetAccessor.GetAsset<AmAssetSettings>().ClipAudioMixer;
            }
            
            IsInitialized = true;
        }


        public void ResetSourceInstance()
        {
            Source.clip = null;
            Source.time = 0;
            Source.outputAudioMixerGroup = null;

            if (settings != null)
            {
                settings.RevertEdits(this);
            }

            settings = null;
            
            IsInitialized = false;
        }


        /// <summary>
        /// Prepares the player for use, but doesn't play the audio source just yet.
        /// </summary>
        private void PrepareSourceInstance()
        {
            if (!IsInitialized) return;
            if (AmAssetAccessor.GetAsset<AmAssetSettings>().PlayAudioState == PlayState.Disabled) return;
            
            if (IsPrepared) return;
            
            if (EditParams.TryGetValue("dynamicTime", out bool useDynamicTime))
            {
                if (useDynamicTime)
                {
                    Source.time = data.dynamicStartTime.time;
                }
            }
            
            if (Source.outputAudioMixerGroup == null)
            {
                Source.outputAudioMixerGroup = AmAssetAccessor.GetAsset<AmAssetSettings>().ClipAudioMixer;
            }
            
            AmAssetAccessor.GetAsset<AmAssetSettings>().AudioStateChanged.Add(OnClipStateChanged);
            RefreshGlobalVariance();
            IsPrepared = true;
            State = AudioSourceState.Ready;
        }


        /// <summary>
        /// Plays the player when called.
        /// </summary>
        public void PlaySourceInstance()
        {
            PrepareSourceInstance();
            
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            
            StartCoroutine(Co_PlayHandler());
            State = AudioSourceState.Playing;
            Started.Raise();
            playerRoutine = StartCoroutine(Co_ClipRuntimeRoutine());
        }


        /// <summary>
        /// Pauses the player when called.
        /// </summary>
        public void PauseSourceInstance()
        {
            if (playerRoutine != null)
            {
                StopCoroutine(playerRoutine);
            }
            
            State = AudioSourceState.Paused;
            Paused.Raise();
        }


        /// <summary>
        /// Resumes the player when called.
        /// </summary>
        public void ResumeSourceInstance()
        {
            playerRoutine = StartCoroutine(Co_ClipRuntimeRoutine());
            State = AudioSourceState.Playing;
            Resumed.Raise();
        }
        

        /// <summary>
        /// Stops the player when called.
        /// </summary>
        public void StopSourceInstance()
        {
            if (!IsPlaying) return;
            Source.Stop();
            State = AudioSourceState.Stopped;
            Stopped.Raise();
        }


        /// <summary>
        /// Resets the player when called.
        /// </summary>
        /// <param name="revertEdits"></param>
        public void ResetSourceInstance(bool revertEdits = true)
        {
            if (revertEdits)
            {
                settings.RevertEdits(this);
            }
            
            IsPrepared = false;
            EditParams.ClearAllParams();
            State = AudioSourceState.Unassigned;
            gameObject.SetActive(false);
        }
        
        
        /// <summary>
        /// Updates the variance of the clip for a fresh play. 
        /// </summary>
        private void RefreshGlobalVariance()
        {
            if (EditParams.TryGetValue<bool>("globalVariance", out var useVariance))
            {
                if (!useVariance) return;
            }

            if (!AmAssetAccessor.GetAsset<AmAssetSettings>().UseGlobalVariance) return;

            var volVariance = new Variance(Source.volume,
                AmAssetAccessor.GetAsset<AmAssetSettings>().VariantVolume);
            
            var pitchVariance = new Variance(Source.pitch,
                AmAssetAccessor.GetAsset<AmAssetSettings>().VariantPitch);

            Source.volume = volVariance.GetVariance();
            Source.pitch = pitchVariance.GetVariance();
        }


        /// <summary>
        /// Processes the edits when the clip loops to have it play differently to before.
        /// </summary>
        public void ProcessLoopEdits()
        {
            foreach (var edit in settings.Edits.Where(t => t.Value.ProcessOnLoop))
            {
                edit.Value.Process(this);
            }
        }


        /// <summary>
        /// Runs when the clip has completed playing.
        /// </summary>
        private void OnClipCompleted()
        {
            State = AudioSourceState.Completed;
            Completed.Raise();
        }
        

        /// <summary>
        /// Runs when the play state is changed for audio clips in the asset. 
        /// </summary>
        private void OnClipStateChanged()
        {
            Source.mute = AmAssetAccessor.GetAsset<AmAssetSettings>().PlayAudioState == PlayState.PlayMuted;
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Coroutines
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Runs a loop while the clip is playing only.
        /// </summary>
        private IEnumerator Co_ClipRuntimeRoutine()
        {
            yield return new WaitForSecondsRealtime(ClipTimeRemaining);
            OnClipCompleted();
        }


        private IEnumerator Co_PlayHandler()
        {
            if (EditParams.TryGetValue("delay", out DelayEdit edit))
            {
                yield return new WaitForSeconds(edit.Delay);
                Source.Play();
                yield break;
            }
            
            Source.Play();
        }
    }
}