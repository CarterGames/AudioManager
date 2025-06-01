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

using System.Collections.Generic;
using CarterGames.Shared.AudioManager.Serializiation;
using UnityEngine;
using UnityEngine.Events;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// The inspector player for audio clips with the audio manager setup...
    /// </summary>
    [AddComponentMenu("Carter Games/Audio Manager/Inspector Audio Clip Player")]
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
        
        protected AudioPlayer player;
        private bool isPlaying;
        
        private readonly Dictionary<string, IInspectorPlayerParse> inspectorParsers =
            new Dictionary<string, IInspectorPlayerParse>()
            {
                { typeof(VolumeEdit).AssemblyQualifiedName, new VolumeEditParse() },
                { typeof(PitchEdit).AssemblyQualifiedName, new PitchEditParse() },
                { typeof(MixerEdit).AssemblyQualifiedName, new MixerEditParse() },
                { typeof(DelayEdit).AssemblyQualifiedName, new DelayEditParse() },
                { typeof(GlobalVarianceEdit).AssemblyQualifiedName, new GlobalVarianceEditParse() },
                { typeof(DynamicStartTimeEdit).AssemblyQualifiedName, new DynamicStartTimeEditParse() },
                { typeof(LoopEdit).AssemblyQualifiedName, new LoopEditParse() },
            };
        
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
        private AudioPlayer SetupSequence()
        {
            var modules = new List<IEditModule>();

            foreach (var settings in editModuleSettings)
            {
                if (!bool.Parse(settings.Value["enabled"])) continue;
                modules.Add(inspectorParsers[settings.Key].Parse(settings.Value));
            }
            
            if (isGroup)
            { 
                player = AudioManager.PrepareGroup(groupRequest, modules.ToArray());
            }
            else
            {
                player = AudioManager.Prepare(request, modules.ToArray());
            }

            return player;
        }


        /// <summary>
        /// Plays the player when called.
        /// </summary>
        public void Play()
        {
            player = SetupSequence();
            player.Play();
            
            player.Started.Add(OnSequenceStarted);
            player.Looped.Add(OnSequenceLooped);
            player.Completed.Add(OnSequenceCompleted);

            isPlaying = true;
        }


        /// <summary>
        /// Pauses the player when called.
        /// </summary>
        public void Pause()
        {
            if (!isPlaying) return;
            player.Pause();
            isPlaying = false;
        }
        
        
        /// <summary>
        /// Resumes the player when called.
        /// </summary>
        public void Resume()
        {
            if (isPlaying) return;
            player.Resume();
            isPlaying = true;
        }
        
        
        /// <summary>
        /// Stops the player when called.
        /// </summary>
        public void Stop()
        {
            if (!isPlaying) return;
            player.Stop();
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
            player = null;
            isPlaying = false;
        }
    }
}