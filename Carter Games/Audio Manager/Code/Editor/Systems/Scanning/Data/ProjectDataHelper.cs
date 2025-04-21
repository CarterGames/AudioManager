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
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// A helper class for finding assets in the project.
    /// </summary>
    public static class ProjectDataHelper
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private const string AudioClipFilter = "t:audioclip";        
        private const string AudioMixerGroupFilter = "t:audiomixergroup";
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Tries to get all audio clips in the project.
        /// </summary>
        /// <param name="clips">The clips found.</param>
        /// <returns>If it was successful or not.</returns>
        public static bool TryGetAllClipsInProject(out List<AudioClip> clips)
        {
            clips = null;
            
            var assets = AssetDatabase.FindAssets(AudioClipFilter, null);

            if (assets.Length <= 0) return false;

            clips = new List<AudioClip>();

            for (var i = 0; i < assets.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(assets[i]);
                clips.Add((AudioClip)AssetDatabase.LoadAssetAtPath(path, typeof(AudioClip)));
            }

            return clips.Count > 0;
        }
        
        
        /// <summary>
        /// Tries to get all audio mixer groups in the project.
        /// </summary>
        /// <param name="mixers">The mixer groups found.</param>
        /// <returns>If it was successful or not.</returns>
        public static bool TryGetAllMixersInProject(out List<AudioMixerGroup> mixers)
        {
            mixers = null;
            
            var assets = AssetDatabase.FindAssets(AudioMixerGroupFilter, null);

            if (assets.Length <= 0) return false;

            mixers = new List<AudioMixerGroup>();

            for (var i = 0; i < assets.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(assets[i]);
                mixers.Add((AudioMixerGroup)AssetDatabase.LoadAssetAtPath(path, typeof(AudioMixerGroup)));
            }

            return mixers.Count > 0;
        }
    }
}