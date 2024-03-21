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
            if (UtilEditor.Library.MixerCount > 0 && PerUserSettings.LastLibMixerEntry >= 0)
            {
                SelectedProperty = UtilEditor.LibraryObject.Fp("mixers").Fpr("list")
                    .GetIndex(PerUserSettings.LastLibMixerEntry);
            }

            EditorsCache = new Dictionary<string, UnityEditor.Editor>();

            for (var i = 0; i < UtilEditor.LibraryObject.Fp("mixers").Fpr("list").arraySize; i++)
            {
                EditorsCache.Add(UtilEditor.LibraryObject.Fp("mixers").Fpr("list").GetIndex(i).Fpr("key").stringValue,
                    UnityEditor.Editor.CreateEditor(UtilEditor.LibraryObject.Fp("mixers").Fpr("list").GetIndex(i)
                        .Fpr("value").Fpr("mixerGroup").objectReferenceValue));
            }
        }

        
        /// <summary>
        /// Displays the tab when called.
        /// </summary>
        public void Display()
        {
            if (UtilEditor.LibraryObject.Fp("mixers").Fpr("list").arraySize <= 0)
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
            
            GUI.backgroundColor = UtilEditor.Yellow;
            if (GUILayout.Button("Update Mixers Struct", GUILayout.MaxHeight(25)))
            {
                StructHandler.RefreshMixers();
            }
            GUI.backgroundColor = Color.white;
            
            if (PerUserSettings.ShowHelpBoxes)
            {
                EditorGUILayout.HelpBox("Press ^^^ to update the Mixer.??? struct to have the latest changes",
                    MessageType.None);
            }
            
            GUILayout.Space(7.5f);
            UtilEditor.DrawHorizontalGUILine();
            GUILayout.Space(7.5f);
            
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
            var mixers = UtilEditor.LibraryObject.Fp("mixers").Fpr("list");

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
                
                
                
                if (GUILayout.Button($"{mixers.GetIndex(i).Fpr("value").Fpr("key").stringValue}"))
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
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Select a mixer option to see its contents here.");
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
                return;
            }
            
            if (SelectedProperty == null) return;

            DrawMixerGroup();
        }


        private void DrawMixerGroup()
        {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.BeginVertical();
            
            GUILayout.Space(2.5f);
            
            EditorGUILayout.BeginVertical("HelpBox");

            GUI.contentColor = UtilEditor.Yellow;
            EditorGUILayout.LabelField("Meta Data", EditorStyles.boldLabel);
            GUI.contentColor = Color.white;
            
            UtilEditor.DrawHorizontalGUILine();
            GUILayout.Space(5f);
            
            EditorGUILayout.BeginHorizontal();
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(SelectedProperty.Fpr("value").Fpr("key"));
            if (EditorGUI.EndChangeCheck())
            {
                var oldIndexReverse = -1;
                
                for (var i = 0; i < UtilEditor.LibraryObject.Fp("mixersReverseLookup").Fpr("list").arraySize; i++)
                {
                    if (UtilEditor.LibraryObject.Fp("mixersReverseLookup").Fpr("list").GetIndex(i).Fpr("value").stringValue == SelectedProperty.Fpr("key").stringValue)
                    {
                        oldIndexReverse = i;
                    }
                }

                if (oldIndexReverse > -1)
                {
                    UtilEditor.LibraryObject.Fp("mixersReverseLookup").Fpr("list").DeleteIndex(oldIndexReverse);
                }
                
                UtilEditor.LibraryObject.Fp("mixersReverseLookup").Fpr("list")
                    .InsertIndex(UtilEditor.LibraryObject.Fp("mixersReverseLookup").Fpr("list").arraySize);
                    
                UtilEditor.LibraryObject.Fp("mixersReverseLookup").Fpr("list")
                    .GetIndex(UtilEditor.LibraryObject.Fp("mixersReverseLookup").Fpr("list").arraySize - 1)
                    .Fpr("key").stringValue = SelectedProperty.Fpr("value").Fpr("key").stringValue;
                    
                UtilEditor.LibraryObject.Fp("mixersReverseLookup").Fpr("list")
                    .GetIndex(UtilEditor.LibraryObject.Fp("mixersReverseLookup").Fpr("list").arraySize - 1)
                    .Fpr("value").stringValue = SelectedProperty.Fpr("key").stringValue;

                UtilEditor.LibraryObject.ApplyModifiedProperties();
                UtilEditor.LibraryObject.Update();
            }
            
            
            GUI.backgroundColor = UtilEditor.Yellow;

            if (GUILayout.Button("Copy Key", GUILayout.Width(80)))
            {
                Clipboard.Copy(SelectedProperty.Fpr("value").Fpr("key").stringValue);
                EditorUtility.DisplayDialog("Copy Mixer Key", "Key copied to clipboard", "Continue");
            }

            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(SelectedProperty.Fpr("value").Fpr("mixerGroup"), new GUIContent("Mixer Reference", "The mixer this entry is for."));
            EditorGUI.EndDisabledGroup();
            
            if (GUILayout.Button("Select Mixer", GUILayout.Width("Select Mixer    ".Width())))
            {
                Selection.activeObject = SelectedProperty.Fpr("value").Fpr("mixerGroup").objectReferenceValue;
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(5f);
            
            EditorGUILayout.BeginVertical("HelpBox");
            
            GUI.contentColor = UtilEditor.Yellow;
            EditorGUILayout.LabelField("Mixer Inspector", EditorStyles.boldLabel);
            GUI.contentColor = Color.white;
            
            UtilEditor.DrawHorizontalGUILine();
            GUILayout.Space(17.5f);
            
            if (EditorsCache.ContainsKey(SelectedProperty.Fpr("key").stringValue))
            {
                EditorsCache[SelectedProperty.Fpr("key").stringValue].OnInspectorGUI();
            }
            else
            {
                EditorsCache.Add(SelectedProperty.Fpr("key").stringValue, UnityEditor.Editor.CreateEditor(SelectedProperty.Fpr("value").Fpr("mixerGroup").objectReferenceValue));
                EditorsCache[SelectedProperty.Fpr("key").stringValue].OnInspectorGUI();
            }

            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }
    }
}