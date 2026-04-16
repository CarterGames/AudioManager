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
using System.Collections.Generic;
using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Audio;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// The editor GUI logic for the mixer group edit module.
    /// </summary>
    public sealed class MixerGroupEditModuleInspector : EditModuleInspectorBase
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// The properties the edit module has to edit.
        /// </summary>
        protected override Dictionary<string, string> EditPropertiesDefaults { get; set; } = new Dictionary<string, string>()
        {
            { "showModule", "False" },
            { "enabled", "True" },
            { "mixerName", string.Empty },
        };
        
        public override Type EditModule => typeof(MixerEdit);
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Draws the inspector GUI for the module.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="index">The index of the module in the object.</param>
        public override void DrawInspector(SerializedObject targetObject, int index)
        {
            InitializeValues(targetObject, index);
            
            EditorGUILayout.BeginVertical("HelpBox");

            DrawDropDown("Mixer Group Edit");
            
            GUILayout.Space(2.5f);
            
            if (ShouldReturn)
            {
                ShouldReturn = false;
                return;
            }

            if (bool.Parse(GetValue( "showModule")))
            {
                UtilEditor.DrawHorizontalGUILine();
                
                if (string.IsNullOrEmpty(GetValue("mixerName")))
                {
                    if (GUILayout.Button("Select Mixer"))
                    {
                        SearchProviderInstancing.SearchProviderMixers.ToExclude.Clear();
                
                        SearchProviderInstancing.SearchProviderMixers.SelectionMade.AddAnonymous("mixerSearch", (s) => SelectMixer(targetObject, index, s));
                        SearchProviderInstancing.SearchProviderMixers.Open();
                    }
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUI.BeginDisabledGroup(true);
                    
                    EditorGUILayout.ObjectField(
                        ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.GetMixer(GetValue("mixerName")),
                        typeof(AudioMixerGroup), false);
                    
                    EditorGUI.EndDisabledGroup();
                    
                    if (GUILayout.Button("Change Mixer", GUILayout.Width(100)))
                    {
                        SearchProviderInstancing.SearchProviderMixers.SelectionMade.AddAnonymous("mixerSearch", (s) => SelectMixer(targetObject, index, s));
                        SearchProviderInstancing.SearchProviderMixers.Open(GetValue("mixerName"));
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
            }
            
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }
        
        
        /// <summary>
        /// Runs when a mixer is selected by the search provider.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="index">The index of the module in the object.</param>
        /// <param name="treeEntry">The entry selected from the search provider.</param>
        private void SelectMixer(SerializedObject targetObject, int index, SearchTreeEntry treeEntry)
        {
            SearchProviderInstancing.SearchProviderMixers.SelectionMade.RemoveAnonymous("mixerSearch");
            SetValue("mixerName", treeEntry.userData.ToString());
            targetObject.ApplyModifiedProperties();
            targetObject.Update();
        }
    }
}