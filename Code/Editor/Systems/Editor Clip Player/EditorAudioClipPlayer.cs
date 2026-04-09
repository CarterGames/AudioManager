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

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// A helper class to handle playing audio in the editor without an audio source
    /// </summary>
    /// <remarks>
    /// Its a real pain, but possible via reflection to the UnityEditor.AudioUtil class.
    /// Ref: https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/Audio/Bindings/AudioUtil.bindings.cs
    /// </remarks>
    public static class EditorAudioClipPlayer
    {
        /* —————————————————————————————————————————————————————————————————————————————————————————————————————————————
        |   Fields
        ————————————————————————————————————————————————————————————————————————————————————————————————————————————— */
        
        private static readonly Assembly UnityEditorAssembly = typeof(AudioImporter).Assembly;
        private static readonly Type AudioUtilClass = UnityEditorAssembly.GetType("UnityEditor.AudioUtil");
        
        /* —————————————————————————————————————————————————————————————————————————————————————————————————————————————
        |   Properties
        ————————————————————————————————————————————————————————————————————————————————————————————————————————————— */
        
        /// <summary>
        /// Gets the current clip in use.
        /// </summary>
        public static AudioClip CurrentClip { get; private set; }

        /* —————————————————————————————————————————————————————————————————————————————————————————————————————————————
        |   Methods
        ————————————————————————————————————————————————————————————————————————————————————————————————————————————— */

        /// <summary>
        /// Gets if a track is currently being played.
        /// </summary>
        /// <returns>If a track is playing.</returns>
        public static bool IsClipPlaying()
        {
            var method = AudioUtilClass.GetMethod
            (
                "IsPreviewClipPlaying",
                BindingFlags.Static | BindingFlags.Public
            );
			
            var playing = (bool)method.Invoke
            (
                null,
                new object[] { }
            )!;
			
            return playing;
        }
        
        
        /// <summary>
        /// Plays the entered clip.
        /// </summary>
        /// <param name="data">The clip to play.</param>
        public static void Play(AudioData data)
        {
            StopAll();
            
            CurrentClip = data.value;
            
            var method = AudioUtilClass.GetMethod
            (
                "PlayPreviewClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
                null
            );
            
            var samples = new float[data.value.samples * data.value.channels];
            data.value.GetData(samples, 0);

            var amountThrough = data.dynamicStartTime.time / data.value.length;
            var startTime = Mathf.FloorToInt((samples.Length / 2f) * amountThrough);
            
            method?.Invoke(null, new object[] { data.value, startTime, false });
        }
        
            
        /// <summary>
        /// Stops any clip currently in play.
        /// </summary>
        public static void StopAll()
        {
            var method = AudioUtilClass.GetMethod
            (
                "StopAllPreviewClips",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { },
                null
            );

            method?.Invoke(null, new object[] { });

            CurrentClip = null;
        }
    }
}