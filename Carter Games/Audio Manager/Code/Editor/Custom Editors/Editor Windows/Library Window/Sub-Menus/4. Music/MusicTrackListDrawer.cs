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

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the editor GUI logic for a music track list.
    /// </summary>
    public static class MusicTrackListDrawer
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static LibrarySearchProvider librarySearchProvider;
        private static SerializedProperty TargetProperty;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Draws the clips in the track list when called.
        /// </summary>
        /// <param name="prop">The base property to call.</param>
        public static void DrawTracks(SerializedProperty prop)
        {
            var list = prop.Fpr("value").Fpr("tracks");
            // var playType = prop.Fpr("value").Fpr("playlistType");
            
            EditorGUILayout.BeginVertical("HelpBox");
            
            GUI.color = UtilEditor.Yellow;
            EditorGUILayout.LabelField("Tracks", EditorStyles.boldLabel);
            GUI.color = Color.white;
            
            UtilEditor.DrawHorizontalGUILine();

            EditorGUILayout.BeginVertical("Box");
            GUILayout.Space(1.5f);

            if (list.arraySize.Equals(0))
            {
                list.InsertIndex(0);
                
                OpenClipSelect(list, list.GetIndex(0));
                
                EditorGUILayout.EndVertical();
            
                GUILayout.Space(1.5f);
                EditorGUILayout.EndVertical();
                return;
            }

            
            for (var i = 0; i < list.arraySize; i++)
            {
                EditorGUILayout.BeginVertical();

                if (list.arraySize > 0 && !string.IsNullOrEmpty(list.GetIndex(i).Fpr("clipId").stringValue))
                {
                    if (i.Equals(0))
                    {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField("Clip");
                        EditorGUILayout.LabelField("", GUILayout.Width(50f));
                        EditorGUILayout.LabelField("Start Time", GUILayout.Width(175f));
                        EditorGUILayout.LabelField("End Time", GUILayout.Width(175f));
                        EditorGUILayout.LabelField("", GUILayout.Width(50));

                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(1.5f);
                    }
                }


                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(1.5f);
                
                EditorGUI.BeginDisabledGroup(list.arraySize == 1 && i > 0);
                // EditorGUI.BeginDisabledGroup(playType.intValue == 0 && i > 0);
                
                if (string.IsNullOrEmpty(list.GetIndex(i).Fpr("clipId").stringValue))
                {
                    OpenClipSelect(list, list.GetIndex(i));
                }
                else
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.TextField(GUIContent.none, UtilEditor.Library
                        .LibraryLookup[list.GetIndex(i).Fpr("clipId").stringValue].key);
                    EditorGUI.EndDisabledGroup();
                    // EditorGUILayout.PropertyField(list.GetIndex(i).Fpr("clipId"), GUIContent.none);

                    if (GUILayout.Button("Edit", GUILayout.Width(50f)))
                    {
                        librarySearchProvider ??= ScriptableObject.CreateInstance<LibrarySearchProvider>();
                        
                        LibrarySearchProvider.ToExclude.Clear();
                        
                        for (var j = 0; j < list.arraySize; j++)
                        {
                            if (string.IsNullOrEmpty(list.GetIndex(j).Fpr("clipId").stringValue)) continue;
                            LibrarySearchProvider.ToExclude.Add(list.GetIndex(j).Fpr("clipId").stringValue);
                        }

                        TargetProperty = list.GetIndex(i);
                        
                        LibrarySearchProvider.OnSearchTreeSelectionMade.Add(SelectClip);
                        SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), librarySearchProvider);
                    }
                    
                    EditorGUILayout.PropertyField(list.GetIndex(i).Fpr("startTime"), GUIContent.none, GUILayout.Width(175f));
                    EditorGUILayout.PropertyField(list.GetIndex(i).Fpr("endTime"), GUIContent.none, GUILayout.Width(175f));
                }
                
                GUILayout.Space(1.5f);
                
                EditorGUI.EndDisabledGroup();
                // EditorGUI.BeginDisabledGroup(list.arraySize == 1);
                // EditorGUI.BeginDisabledGroup(playType.intValue == 0);
                
                GUI.backgroundColor = UtilEditor.Green;
                if (GUILayout.Button(" + ", GUILayout.MaxWidth(22.5f)))
                {
                    list.InsertIndex(i + 1);
                    list.GetIndex(i + 1).Fpr("clipId").stringValue = string.Empty;
                    list.GetIndex(i + 1).Fpr("startTime").floatValue = 0f;
                    list.GetIndex(i + 1).Fpr("endTime").floatValue = 0f;
                    list.serializedObject.ApplyModifiedProperties();
                    list.serializedObject.Update();
                    return;
                }
                
                // EditorGUI.EndDisabledGroup();
                
                GUI.backgroundColor = UtilEditor.Red;
                if (GUILayout.Button(" - ", GUILayout.MaxWidth(22.5f)))
                {
                    list.DeleteIndex(i);
                    list.serializedObject.ApplyModifiedProperties();
                    list.serializedObject.Update();
                    return;
                }
                GUI.backgroundColor = Color.white;
                
                GUILayout.Space(1.5f);
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
            }
            
            
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Runs when the clip select is opened.
        /// </summary>
        /// <param name="property">The property to use.</param>
        /// <param name="target">The target object.</param>
        private static void OpenClipSelect(SerializedProperty property, SerializedProperty target)
        {
            librarySearchProvider ??= ScriptableObject.CreateInstance<LibrarySearchProvider>();

            GUI.backgroundColor = UtilEditor.Green;

            TargetProperty = target;
            
            if (GUILayout.Button("+ Add Clip"))
            {
                LibrarySearchProvider.ToExclude.Clear();
                        
                for (var j = 0; j < property.arraySize; j++)
                {
                    if (string.IsNullOrEmpty(property.GetIndex(j).Fpr("clipId").stringValue)) continue;
                    LibrarySearchProvider.ToExclude.Add(property.GetIndex(j).Fpr("clipId").stringValue);
                }
                
                LibrarySearchProvider.OnSearchTreeSelectionMade.Add(SelectClip);
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), librarySearchProvider);
            }
            
            GUI.backgroundColor = Color.white;
        }

        
        /// <summary>
        /// Runs when the clip selected has selected something.
        /// </summary>
        /// <param name="property">The property to edit.</param>
        /// <param name="treeEntry">The entry selected.</param>
        private static void SelectClip(SearchTreeEntry treeEntry)
        {
            LibrarySearchProvider.OnSearchTreeSelectionMade.Remove(SelectClip);
            
            TargetProperty.Fpr("clipId").stringValue = ((AudioData)treeEntry.userData).id;
            TargetProperty.Fpr("startTime").floatValue = ((AudioData)treeEntry.userData).dynamicStartTime.time;
            TargetProperty.Fpr("endTime").floatValue = ((AudioData)treeEntry.userData).value.length;
            
            TargetProperty.serializedObject.ApplyModifiedProperties();
            TargetProperty.serializedObject.Update();
            
            GUI.FocusControl(null);
        }
    }
}