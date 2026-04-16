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