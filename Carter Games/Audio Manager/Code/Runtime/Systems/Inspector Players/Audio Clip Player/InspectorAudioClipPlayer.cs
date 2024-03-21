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
using CarterGames.Common.Serializiation;
using UnityEngine;
using UnityEngine.Events;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// The inspector player for audio clips with the audio manager setup...
    /// </summary>
    [AddComponentMenu("Carter Games/Audio Manager/Audio Clip Player")]
    public class InspectorAudioClipPlayer : MonoBehaviour
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        [SerializeField] private bool playInstantly;
        
        [SerializeField] private bool isGroup;
        [SerializeField] private string request;
        [SerializeField] private string groupRequest;
        
        [SerializeField] private SerializableDictionary<string, SerializableDictionary<string, string>> editModuleSettings =
            new SerializableDictionary<string, SerializableDictionary<string, string>>();

        [SerializeField] private bool showEvents;
        
        public UnityEvent onStarted;
        public UnityEvent onLooped;
        public UnityEvent onCompleted;
        
        private AudioPlayerSequence sequence;
        private bool isPlaying;
        
        private readonly Dictionary<string, IInspectorPlayerParse> inspectorParsers =
            new Dictionary<string, IInspectorPlayerParse>()
            {
                { "CarterGames.Assets.AudioManager.VolumeEdit", new VolumeEditParse() },
                { "CarterGames.Assets.AudioManager.PitchEdit", new PitchEditParse() },
                { "CarterGames.Assets.AudioManager.MixerEdit", new MixerEditParse() },
                { "CarterGames.Assets.AudioManager.DelayEdit", new DelayEditParse() },
                { "CarterGames.Assets.AudioManager.GlobalVarianceEdit", new GlobalVarianceEditParse() },
                { "CarterGames.Assets.AudioManager.DynamicStartTimeEdit", new DynamicStartTimeEditParse() },
                { "CarterGames.Assets.AudioManager.LoopEdit", new LoopEditParse() },
            };

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private AudioPlayerSequence Sequence
        {
            get
            {
                if (sequence != null) return sequence;
                sequence = SetupSequence();
                return sequence;
            }
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private void OnEnable()
        {
            if (!playInstantly) return;
            Play();
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Sets up the sequence to be used.
        /// </summary>
        /// <returns>The setup sequence.</returns>
        private AudioPlayerSequence SetupSequence()
        {
            AudioPlayerSequence sequence;
            var modules = new List<IEditModule>();

            foreach (var settings in editModuleSettings)
            {
                if (!bool.Parse(settings.Value["enabled"])) continue;
                modules.Add(inspectorParsers[settings.Key].Parse(settings.Value));
            }
            
            if (isGroup)
            { 
                sequence = AudioManager.PrepareGroup(groupRequest, modules.ToArray());
            }
            else
            {
                sequence = AudioManager.Prepare(request, modules.ToArray());
            }

            return sequence;
        }


        /// <summary>
        /// Plays the player when called.
        /// </summary>
        public void Play()
        {
            if (isPlaying) return;
            
            Sequence.Play();
            
            Sequence.Started.Add(OnSequenceStarted);
            Sequence.Looped.Add(OnSequenceLooped);
            Sequence.Completed.Add(OnSequenceCompleted);

            isPlaying = true;
        }


        /// <summary>
        /// Pauses the player when called.
        /// </summary>
        public void Pause()
        {
            if (!isPlaying) return;
            Sequence.Pause();
            isPlaying = false;
        }
        
        
        /// <summary>
        /// Resumes the player when called.
        /// </summary>
        public void Resume()
        {
            if (isPlaying) return;
            Sequence.Resume();
            isPlaying = true;
        }
        
        
        /// <summary>
        /// Stops the player when called.
        /// </summary>
        public void Stop()
        {
            if (!isPlaying) return;
            Sequence.Stop();
            isPlaying = false;
        }
        

        /// <summary>
        /// Runs when the sequence has started.
        /// </summary>
        private void OnSequenceStarted()
        {
            onStarted?.Invoke();
        }
        
        
        /// <summary>
        /// Runs when the sequence has looped.
        /// </summary>
        private void OnSequenceLooped()
        {
            onLooped?.Invoke();
        }
        

        /// <summary>
        /// Runs when the sequence has completed.
        /// </summary>
        private void OnSequenceCompleted()
        {
            onCompleted?.Invoke();
            Sequence.Completed.Remove(OnSequenceCompleted);
            isPlaying = false;
        }
    }
}