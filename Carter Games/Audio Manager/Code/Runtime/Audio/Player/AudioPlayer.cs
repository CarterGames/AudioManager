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

using System.Collections;
using System.Linq;
using CarterGames.Common;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles playing audio from the audio library.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioPlayer : MonoBehaviour
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private AudioSource source;
        private AudioClipSettings settings;
        private AudioData data;
        private Coroutine playerRoutine;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The audio source for the player.
        /// </summary>
        public AudioSource PlayerSource => source ??= GetComponent<AudioSource>();


        /// <summary>
        /// Gets if the player has been prepared or not. 
        /// </summary>
        public bool IsPrepared { get; private set; }


        /// <summary>
        /// Gets if the player is playing a clip currently.
        /// </summary>
        public bool IsPlaying => PlayerSource.isPlaying;
        
        
        /// <summary>
        /// Gets the parameters for the player.
        /// </summary>
        public EditParameters EditParams { get; private set; } = new EditParameters();


        /// <summary>
        /// Gets the time remaining on the player.
        /// </summary>
        private float ClipTimeRemaining
        {
            get
            {
                var time = 0f;
            
                if (EditParams.TryGetValue<bool>("dynamicTime", out var useDynamicTime))
                {
                    time += data.value.length - (useDynamicTime ? data.dynamicStartTime.time : 0);
                }
                else
                {
                    time += data.value.length;
                }

                if (EditParams.TryGetValue<DelayEdit>("delay", out var delayModule))
                {
                    time += delayModule.Delay;
                }
                
                if (EditParams.TryGetValue<LoopEdit>("loop", out var loop))
                {
                    if (loop.ShouldLoopWithDelays)
                    {
                        time += delayModule.Delay;
                    }
                    else
                    {
                        time -= delayModule.Delay;
                    }
                }

                return time;
            }
        }
        
        
        /// <summary>
        /// Gets/Sets the sequence the player is attached to.
        /// </summary>
        public AudioPlayerSequence PlayerSequence { get; set; }
        
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
            AssetAccessor.GetAsset<SettingsAssetRuntime>().AudioStateChanged.Add(OnClipStateChanged);
        }


        private void OnDestroy()
        {
            AssetAccessor.GetAsset<SettingsAssetRuntime>().AudioStateChanged.Remove(OnClipStateChanged);
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Sets the clip in the player to the data entered.
        /// </summary>
        /// <param name="clip">The data to play from.</param>
        public void SetClip(AudioData clip)
        {
            data = clip;
            PlayerSource.clip = clip.value;
            
            PreparePlayer();
        }
        

        /// <summary>
        /// Sets the clip in the player to the data entered.
        /// </summary>
        /// <param name="clip">The data to play from.</param>
        /// <param name="clipSettings">The settings to apply to the clip.</param>
        public void SetClip(AudioData clip, AudioClipSettings clipSettings)
        {
            data = clip;
            PlayerSource.clip = clip.value;
            settings = clipSettings;
            
            settings.ProcessEdits(this);

            if (PlayerSource.outputAudioMixerGroup == null)
            {
                PlayerSource.outputAudioMixerGroup = AssetAccessor.GetAsset<SettingsAssetRuntime>().ClipAudioMixer;
            }

            PreparePlayer();
        }


        /// <summary>
        /// Prepares the player for use, but doesn't play the audio source just yet.
        /// </summary>
        private void PreparePlayer()
        {
            if (AssetAccessor.GetAsset<SettingsAssetRuntime>().PlayAudioState == PlayState.Disabled) return;
            
            if (IsPrepared) return;
            IsPrepared = true;
                
            if (EditParams.TryGetValue("dynamicTime", out bool useDynamicTime))
            {
                if (useDynamicTime)
                {
                    PlayerSource.time = data.dynamicStartTime.time;
                }
            }
            
            if (PlayerSource.outputAudioMixerGroup == null)
            {
                PlayerSource.outputAudioMixerGroup = AssetAccessor.GetAsset<SettingsAssetRuntime>().ClipAudioMixer;
            }
            
            PlayerSequence.Completed.Remove(OnSequenceComplete);
            PlayerSequence.Completed.Add(OnSequenceComplete);
            
            AssetAccessor.GetAsset<SettingsAssetRuntime>().AudioStateChanged.Add(OnClipStateChanged);
            
            UpdateVariance();
        }


        /// <summary>
        /// Plays the player when called.
        /// </summary>
        public void PlayPlayer()
        {
            PreparePlayer();
            
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        
            Play();
            Started.Raise();
            playerRoutine = StartCoroutine(Co_ClipRuntimeRoutine());
        }


        private void Play()
        {
            StartCoroutine(Co_PlayHandler());
        }


        /// <summary>
        /// Pauses the player when called.
        /// </summary>
        public void PausePlayer()
        {
            if (playerRoutine != null)
            {
                StopCoroutine(playerRoutine);
            }
            
            Paused.Raise();
        }


        /// <summary>
        /// Resumes the player when called.
        /// </summary>
        public void ResumePlayer()
        {
            playerRoutine = StartCoroutine(Co_ClipRuntimeRoutine());
            Resumed.Raise();
        }
        

        /// <summary>
        /// Stops the player when called.
        /// </summary>
        public void StopPlayer()
        {
            if (!IsPlaying) return;
            PlayerSource.Stop();
            Stopped.Raise();
        }


        /// <summary>
        /// Resets the player when called.
        /// </summary>
        /// <param name="revertEdits"></param>
        public void ResetPlayer(bool revertEdits = true)
        {
            if (revertEdits)
            {
                settings.RevertEdits(this);
            }
        }
        
        
        /// <summary>
        /// Updates the variance of the clip for a fresh play. 
        /// </summary>
        private void UpdateVariance()
        {
            if (!EditParams.TryGetValue<bool>("globalVariance", out var useVariance)) return;
            if (!useVariance) return;
            if (!AssetAccessor.GetAsset<SettingsAssetRuntime>().UseGlobalVariance) return;

            var volVariance = new Variance(PlayerSource.volume,
                AssetAccessor.GetAsset<SettingsAssetRuntime>().VariantVolume);
            
            var pitchVariance = new Variance(PlayerSource.pitch,
                AssetAccessor.GetAsset<SettingsAssetRuntime>().VariantPitch);

            PlayerSource.volume = volVariance.GetVariance();
            PlayerSource.pitch = pitchVariance.GetVariance();
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
            // Debug.LogError("Completed");
            Completed.Raise();
        }


        /// <summary>
        /// Runs when the sequence this player is attached to is completed.
        /// </summary>
        private void OnSequenceComplete()
        {
            IsPrepared = false;
            EditParams.ClearAllParams();
            gameObject.SetActive(false);
        }


        /// <summary>
        /// Runs when the play state is changed for audio clips in the asset. 
        /// </summary>
        private void OnClipStateChanged()
        {
            PlayerSource.mute = AssetAccessor.GetAsset<SettingsAssetRuntime>().PlayAudioState == PlayState.PlayMuted;
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
            // Debug.LogError(ClipTimeRemaining);
            OnClipCompleted();
        }


        private IEnumerator Co_PlayHandler()
        {
            if (EditParams.TryGetValue("delay", out DelayEdit edit))
            {
                // Debug.LogError("Delayed");
                yield return new WaitForSeconds(edit.Delay);
                // Debug.LogError("Delay Completed");
                PlayerSource.Play();
                yield break;
            }

            // Debug.LogError("Played Normal");
            PlayerSource.Play();
        }
    }
}