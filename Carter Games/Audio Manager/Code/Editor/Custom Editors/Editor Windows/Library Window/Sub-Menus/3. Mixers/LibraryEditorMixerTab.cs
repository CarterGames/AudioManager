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
using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the mixer tab logic for the audio library editor window.
    /// </summary>
    public sealed class LibraryEditorMixerTab : DividedDisplayBase, ILibraryTab
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private static AudioLibrary LibAsset => ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef;
        private static SerializedObject LibObj => ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef;
        
        private static SerializedProperty SelectedProperty { get; set; }

        private static Dictionary<string, UnityEditor.Editor> EditorsCache { get; set; }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   ILibraryTab
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Initializes the tab when called.
        /// </summary>
        public void Initialize()
        {
            if (LibAsset.MixerCount > 0 && PerUserSettings.LastLibMixerEntry >= 0)
            {
                SelectedProperty = LibObj.Fp("mixers").Fpr("list")
                    .GetIndex(PerUserSettings.LastLibMixerEntry);
            }

            EditorsCache = new Dictionary<string, UnityEditor.Editor>();

            for (var i = 0; i < LibObj.Fp("mixers").Fpr("list").arraySize; i++)
            {
                EditorsCache.Add(LibObj.Fp("mixers").Fpr("list").GetIndex(i).Fpr("key").stringValue,
                    UnityEditor.Editor.CreateEditor(LibObj.Fp("mixers").Fpr("list").GetIndex(i)
                        .Fpr("value").Fpr("mixerGroup").objectReferenceValue));
            }
        }

        
        /// <summary>
        /// Displays the tab when called.
        /// </summary>
        public void Display()
        {
            if (LibObj.Fp("mixers").Fpr("list").arraySize <= 0)
            {
                EditorGUILayout.HelpBox("No mixer groups found in the project so there is nothing to show.", MessageType.Info);
                return;
            }
            
            DisplaySections();
        }


        /// <summary>
        /// Changes the left control GUI to something else.
        /// </summary>
        protected override void LeftSectionControl()
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.MaxWidth(250));
            GUILayout.Space(5f);
            
            GUI.backgroundColor = EditorColors.PrimaryYellow;
            if (GUILayout.Button("Update Mixers Struct", GUILayout.MaxHeight(25)))
            {
                if (EditorUtility.DisplayDialog("Update Mixers Struct",
                        "Are you sure you want to update the mixers struct?", "Update Mixers Struct", "Cancel"))
                {
                    StructHandler.RefreshMixers();
                }
            }
            GUI.backgroundColor = Color.white;
            
            if (PerUserSettings.ShowHelpBoxes)
            {
                EditorGUILayout.HelpBox("Press ^^^ to update the Mixer.??? struct to have the latest changes",
                    MessageType.None);
            }
            
            GUILayout.Space(1.5f);
            UtilEditor.DrawHorizontalGUILine();
            GUILayout.Space(1.5f);
            
            PerUserSettings.MixerBtnScrollRectPos = EditorGUILayout.BeginScrollView(PerUserSettings.MixerBtnScrollRectPos);
            base.LeftSectionControl();
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }
        
        
        /// <summary>
        /// Changes the right GUI control to something else.
        /// </summary>
        protected override void RightSectionControl()
        {
            PerUserSettings.MixerScrollRectPos = EditorGUILayout.BeginScrollView(PerUserSettings.MixerScrollRectPos);
            EditorGUILayout.BeginVertical("Box");
            base.RightSectionControl();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }


        /// <summary>
        /// Adds the implemented for the left GUI.
        /// </summary>
        protected override void OnLeftGUI()
        {
            var mixers = LibObj.Fp("mixers").Fpr("list");

            if (mixers.arraySize.Equals(0)) return;
            
            for (var i = 0; i < mixers.arraySize; i++)
            {
                if (SelectedProperty != null)
                {
                    if (SelectedProperty.Fpr("key").stringValue.Equals(mixers.GetIndex(i).Fpr("value").Fpr("uuid").stringValue))
                    {
                        GUI.backgroundColor = UtilEditor.Grey;
                    }
                    else
                    {
                        GUI.backgroundColor = Color.white;
                    }
                }
                
                var toDisplay = new GUIContent(LibObj
                    .Fp("mixers").Fpr("list").GetIndex(i).Fpr("value").Fpr("key").stringValue);

                if (toDisplay.text.Length > 25)
                {
                    toDisplay.text = toDisplay.text.Substring(0, Mathf.Clamp(toDisplay.text.Length, 20, 35)) + "...";
                }
                
                if (GUILayout.Button(toDisplay))
                {
                    PerUserSettings.LastLibMixerEntry = i;
                    SelectedProperty = mixers.GetIndex(i);
                    GUI.FocusControl(null);
                }
                
                GUI.backgroundColor = Color.white;
            }
        }


        /// <summary>
        /// Adds the implemented for the right GUI.
        /// </summary>
        protected override void OnRightGUI()
        {
            if (SelectedProperty == null)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Select a mixer option to see its contents here.");
                EditorGUILayout.EndVertical();
                return;
            }
            
            if (SelectedProperty == null) return;

            DrawMixerGroup();
        }


        private void DrawMixerGroup()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.Space(2.5f);
            
            MixerGUIMixerInfo.Draw(SelectedProperty);
            
            EditorGUILayout.Space(5f);
            
            MixerGUIMixerInspector.Draw(SelectedProperty, EditorsCache);

            EditorGUILayout.Space(5f);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }
}