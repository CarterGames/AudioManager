﻿/*
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
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private MixerSearchProvider mixerSearchProvider;
        
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

            DrawDropDown(targetObject, index, "Mixer Group Edit");
            
            GUILayout.Space(2.5f);
            
            if (ShouldReturn)
            {
                ShouldReturn = false;
                return;
            }

            if (bool.Parse(EditModuleInspectorHelper.GetValue(targetObject, index, "showModule")))
            {
                UtilEditor.DrawHorizontalGUILine();
                
                if (string.IsNullOrEmpty(EditModuleInspectorHelper.GetValue(targetObject, index, "mixerName")))
                {
                    if (GUILayout.Button("Select Mixer"))
                    {
                        mixerSearchProvider ??= ScriptableObject.CreateInstance<MixerSearchProvider>();
                        
                        MixerSearchProvider.ToExclude.Clear();
                
                        MixerSearchProvider.OnSearchTreeSelectionMade.AddAnonymous("mixerSearch", (s) => SelectMixer(targetObject, index, s));
                        SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), mixerSearchProvider);
                    }
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUI.BeginDisabledGroup(true);
                    
                    EditorGUILayout.ObjectField(
                        UtilEditor.Library.GetMixer(EditModuleInspectorHelper.GetValue(targetObject, index, "mixerName")),
                        typeof(AudioMixerGroup), false);
                    
                    EditorGUI.EndDisabledGroup();
                    
                    if (GUILayout.Button("Change Mixer", GUILayout.Width(100)))
                    {
                        mixerSearchProvider ??= ScriptableObject.CreateInstance<MixerSearchProvider>();
                        
                        MixerSearchProvider.ToExclude.Clear();
                        MixerSearchProvider.ToExclude.Add(EditModuleInspectorHelper.GetValue(targetObject, index, "mixerName"));
                
                        MixerSearchProvider.OnSearchTreeSelectionMade.AddAnonymous("mixerSearch", (s) => SelectMixer(targetObject, index, s));
                        SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), mixerSearchProvider);
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
        private static void SelectMixer(SerializedObject targetObject, int index, SearchTreeEntry treeEntry)
        {
            MixerSearchProvider.OnSearchTreeSelectionMade.RemoveAnonymous("mixerSearch");
            EditModuleInspectorHelper.SetValue(targetObject, index, "mixerName", treeEntry.userData.ToString());
            targetObject.ApplyModifiedProperties();
            targetObject.Update();
        }
    }
}