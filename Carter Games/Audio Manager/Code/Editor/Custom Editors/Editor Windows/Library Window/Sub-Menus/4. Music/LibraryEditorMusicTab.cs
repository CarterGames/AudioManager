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

using System;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the music tab logic for the audio library editor window.
    /// </summary>
    public sealed class LibraryEditorMusicTab : DividedDisplayBase, ILibraryTab
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private Rect rect;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private static SerializedProperty SelectedProperty { get; set; }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   ILibraryTab
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Initializes the tab when called.
        /// </summary>
        public void Initialize()
        {
            if (PerUserSettings.LastLibMusicEntry < 0) return;

            if (UtilEditor.LibraryObject.Fp("tracks").Fpr("list").arraySize <
                PerUserSettings.LastLibMusicEntry)
            {
                PerUserSettings.LastLibMusicEntry = UtilEditor.LibraryObject.Fp("tracks")
                    .Fpr("list").arraySize;
            }

            if (UtilEditor.LibraryObject.Fp("tracks").Fpr("list").arraySize
                .Equals(0)) return;
            
            SelectedProperty = UtilEditor.LibraryObject.Fp("tracks").Fpr("list")
                .GetIndex(PerUserSettings.LastLibMusicEntry);
        }


        /// <summary>
        /// Displays the tab when called.
        /// </summary>
        public void Display()
        {
            if (UtilEditor.LibraryObject.Fp("tracks").Fpr("list").arraySize <= 0)
            {
                EditorGUILayout.HelpBox("No track lists in the library so there is nothing to show.", MessageType.Info);
                
                EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);
                GUI.backgroundColor = UtilEditor.Green;
                
                if (GUILayout.Button("Make Track List", GUILayout.Height(25)))
                {
                    AddNewBlankTrack();
                }

                GUI.backgroundColor = Color.white;
                EditorGUI.EndDisabledGroup();
                
                return;
            }
            
            DisplaySections();
            UtilEditor.CreateDeselectZone(ref rect);
        }


        /// <summary>
        /// Changes the left control GUI to something else.
        /// </summary>
        protected override void LeftSectionControl()
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.MaxWidth(250));

            GUI.backgroundColor = UtilEditor.Yellow;
            if (GUILayout.Button("Update Playlist Struct", GUILayout.MaxHeight(25)))
            {
                StructHandler.RefreshTracks();
            }

            GUI.backgroundColor = Color.white;

            if (PerUserSettings.ShowHelpBoxes)
            {
                EditorGUILayout.HelpBox("Press ^^^ to update the Playlist.??? struct to have the latest changes",
                    MessageType.None);
            }

            GUILayout.Space(7.5f);
            UtilEditor.DrawHorizontalGUILine();
            GUILayout.Space(7.5f);

            GUI.backgroundColor = UtilEditor.Green;
            if (GUILayout.Button("+ Add New Playlist"))
            {
                AddNewBlankTrack();
                GUI.FocusControl(null);
            }
            GUI.backgroundColor = Color.white;
            
            GUILayout.Space(7.5f);
            UtilEditor.DrawHorizontalGUILine();
            GUILayout.Space(7.5f);

            if (UtilEditor.LibraryObject.Fp("tracks").Fpr("list").arraySize > 0)
            {
                for (var i = 0; i < UtilEditor.LibraryObject.Fp("tracks").Fpr("list").arraySize; i++)
                {
                    if (i.Equals(PerUserSettings.LastLibMusicEntry))
                    {
                        GUI.backgroundColor = UtilEditor.Grey;
                    }
                    else
                    {
                        GUI.backgroundColor = Color.white;
                    }

                    var label = UtilEditor.LibraryObject.Fp("tracks").Fpr("list")
                        .GetIndex(i).Fpr("value").Fpr("listKey")
                        .stringValue;

                    if (GUILayout.Button(label))
                    {
                        SelectedProperty = UtilEditor.LibraryObject.Fp("tracks").Fpr("list")
                            .GetIndex(i);

                        PerUserSettings.LastLibMusicEntry = i;
                        GUI.FocusControl(null);
                    }


                    GUI.backgroundColor = Color.white;
                }
            }
            
            PerUserSettings.MusicBtnScrollRectPos = EditorGUILayout.BeginScrollView(PerUserSettings.MusicBtnScrollRectPos);
            base.LeftSectionControl();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }
        
        
        /// <summary>
        /// Changes the right GUI control to something else.
        /// </summary>
        protected override void RightSectionControl()
        {
            PerUserSettings.MusicScrollRectPos = EditorGUILayout.BeginScrollView(PerUserSettings.MusicScrollRectPos);
            EditorGUILayout.BeginVertical("Box");
            base.RightSectionControl();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }


        /// <summary>
        /// Adds the implemented for the left GUI.
        /// </summary>
        protected override void OnLeftGUI()
        { }


        /// <summary>
        /// Adds the implemented for the right GUI.
        /// </summary>
        protected override void OnRightGUI()
        {
            if (SelectedProperty == null)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Select a playlist to see its contents here.");
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
                return;
            }
            
            if (SelectedProperty == null) return;

            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.BeginVertical("HelpBox");
            EditorGUILayout.Space();
            
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical();
            
            GUI.color = UtilEditor.Yellow;
            EditorGUILayout.LabelField("Meta Data", EditorStyles.boldLabel);
            GUI.color = Color.white;
            
            UtilEditor.DrawHorizontalGUILine();
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(SelectedProperty.Fpr("value").Fpr("listKey"), new GUIContent("Key"));
            if (EditorGUI.EndChangeCheck())
            {
                var oldIndexReverse = -1;
                
                for (var i = 0; i < UtilEditor.LibraryObject.Fp("tracksReverseLookup").Fpr("list").arraySize; i++)
                {
                    if (UtilEditor.LibraryObject.Fp("tracksReverseLookup").Fpr("list").GetIndex(i).Fpr("value").stringValue == SelectedProperty.Fpr("key").stringValue)
                    {
                        oldIndexReverse = i;
                    }
                }
                
                if (oldIndexReverse > -1)
                {
                    UtilEditor.LibraryObject.Fp("tracksReverseLookup").Fpr("list").DeleteIndex(oldIndexReverse);
                }
                
                UtilEditor.LibraryObject.Fp("tracksReverseLookup").Fpr("list").InsertIndex(UtilEditor.LibraryObject.Fp("tracksReverseLookup").Fpr("list").arraySize);
                UtilEditor.LibraryObject.Fp("tracksReverseLookup").Fpr("list").GetIndex(UtilEditor.LibraryObject.Fp("tracksReverseLookup").Fpr("list").arraySize - 1).Fpr("key").stringValue = SelectedProperty.Fpr("value").Fpr("listKey").stringValue;
                UtilEditor.LibraryObject.Fp("tracksReverseLookup").Fpr("list").GetIndex(UtilEditor.LibraryObject.Fp("tracksReverseLookup").Fpr("list").arraySize - 1).Fpr("value").stringValue = SelectedProperty.Fpr("key").stringValue;
            }
            
            
            // EditorGUILayout.PropertyField(SelectedProperty.Fpr("value").Fpr("playlistType"));
            EditorGUILayout.PropertyField(SelectedProperty.Fpr("value").Fpr("loop"));

            if (SelectedProperty.Fpr("value").Fpr("tracks").arraySize > 1)
            {
                EditorGUILayout.PropertyField(SelectedProperty.Fpr("value").Fpr("shuffle"));
            }
            
            EditorGUILayout.PropertyField(SelectedProperty.Fpr("value").Fpr("useCustomTransitions"));
            
            if (SelectedProperty.Fpr("value").Fpr("useCustomTransitions").boolValue)
            {
                EditorGUILayout.PropertyField(SelectedProperty.Fpr("value").Fpr("startingTransition"));
                EditorGUILayout.PropertyField(SelectedProperty.Fpr("value").Fpr("startingTransitionDuration"));
                EditorGUILayout.PropertyField(SelectedProperty.Fpr("value").Fpr("switchTrackTransition"));
                EditorGUILayout.PropertyField(SelectedProperty.Fpr("value").Fpr("switchingTransitionDuration"));
            }
     
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(15f);
            
            MusicTrackListDrawer.DrawTracks(SelectedProperty);
            
            
            if (EditorGUI.EndChangeCheck())
            {
                SelectedProperty.serializedObject.ApplyModifiedProperties();
                SelectedProperty.serializedObject.Update();
            }

            GUILayout.Space(15f);

            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(2.5f);
            
            GUI.color = UtilEditor.Yellow;
            EditorGUILayout.LabelField("Danger Zone", EditorStyles.boldLabel);
            GUI.color = Color.white;
            
            GUILayout.Space(1.5f);
            GUI.backgroundColor = UtilEditor.Red;
            
            if (GUILayout.Button("Delete Playlist"))
            {
                if (!EditorUtility.DisplayDialog("Delete Playlist",
                        "Are you sure you want to delete this playlist?", "Delete", "Cancel")) return;

                var oldKey = UtilEditor.LibraryObject.Fp("tracks").Fpr("list").GetIndex(PerUserSettings.LastLibMusicEntry).Fpr("key").stringValue;
                UtilEditor.LibraryObject.Fp("tracks").Fpr("list").DeleteIndex(PerUserSettings.LastLibMusicEntry);

                for (int i = 0; i < UtilEditor.LibraryObject.Fp("tracksReverseLookup").Fpr("list").arraySize; i++)
                {
                    if (UtilEditor.LibraryObject.Fp("tracksReverseLookup").Fpr("list").GetIndex(i).Fpr("value").stringValue != oldKey) continue;
                    UtilEditor.LibraryObject.Fp("tracksReverseLookup").Fpr("list").DeleteIndex(i);
                }
                
                PerUserSettings.LastLibMusicEntry = UtilEditor.LibraryObject.Fp("tracks").Fpr("list").arraySize - 1;
                
                if (UtilEditor.LibraryObject.Fp("tracks").Fpr("list").arraySize > 0)
                {
                    SelectedProperty = UtilEditor.LibraryObject.Fp("tracks").Fpr("list")
                        .GetIndex(PerUserSettings.LastLibMusicEntry);
                }
                else
                {
                    SelectedProperty = null;
                }
                
                UtilEditor.LibraryObject.ApplyModifiedProperties();
                UtilEditor.LibraryObject.Update();
            }

            GUI.backgroundColor = Color.white;
            
            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Adds a blank track list when called.
        /// </summary>
        private static void AddNewBlankTrack()
        {
            var tracksProp = UtilEditor.LibraryObject.Fp("tracks").Fpr("list");
            var reverseTracksProp = UtilEditor.LibraryObject.Fp("tracksReverseLookup").Fpr("list");
            
            tracksProp.InsertIndex(tracksProp.arraySize);

            var newTrackProp = tracksProp.GetIndex(tracksProp.arraySize - 1);
            
            newTrackProp.Fpr("key").stringValue = Guid.NewGuid().ToString();
            newTrackProp.Fpr("value").Fpr("listKey").stringValue = "New Playlist " + newTrackProp.Fpr("key").stringValue.Substring(0, 7);
            newTrackProp.Fpr("value").Fpr("playlistType").intValue = 0;
            newTrackProp.Fpr("value").Fpr("loop").boolValue = false;
            newTrackProp.Fpr("value").Fpr("shuffle").boolValue = false;
            newTrackProp.Fpr("value").Fpr("tracks").ClearArray();
            newTrackProp.Fpr("value").Fpr("useCustomTransitions").boolValue = false;
            newTrackProp.Fpr("value").Fpr("startingTransition").intValue = 0;
            newTrackProp.Fpr("value").Fpr("startingTransitionDuration").floatValue = 0;
            newTrackProp.Fpr("value").Fpr("switchTrackTransition").intValue = 0;
            newTrackProp.Fpr("value").Fpr("switchingTransitionDuration").floatValue = 0;

            reverseTracksProp.InsertIndex(reverseTracksProp.arraySize);
            
            var newReverseTrackProp = reverseTracksProp.GetIndex(reverseTracksProp.arraySize - 1);
            
            newReverseTrackProp.Fpr("key").stringValue = newTrackProp.Fpr("value").Fpr("listKey").stringValue;
            newReverseTrackProp.Fpr("value").stringValue = newTrackProp.Fpr("key").stringValue;

            if (tracksProp.arraySize.Equals(1))
            {
                PerUserSettings.LastLibMusicEntry = 0;
                SelectedProperty = newTrackProp;
            }
            
            UtilEditor.LibraryObject.ApplyModifiedProperties();
            UtilEditor.LibraryObject.Update();
        }
    }
}