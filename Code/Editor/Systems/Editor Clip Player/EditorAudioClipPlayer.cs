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