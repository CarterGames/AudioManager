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
using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    public static class TagsGUI
    {
        private static GUIContent TagAddButtonContent => new GUIContent("+", EditorGUIUtility.IconContent("d_FilterByLabel@2x").image);
        public static GUIContent TagStdButtonContent => EditorGUIUtility.IconContent("d_FilterByLabel@2x");
        
        
        public static void DrawClipSection(SerializedProperty prop)
        {
            EditorGUILayout.BeginHorizontal();
            
            if (prop.Fpr("value").Fpr("metaData").Fpr("tags").arraySize > 0)
            {
                EditorGUILayout.BeginHorizontal();
                
                for (var i = 0; i < prop.Fpr("value").Fpr("metaData").Fpr("tags").arraySize; i++)
                {
                    DrawTag(prop.Fpr("value").Fpr("metaData").Fpr("tags").GetIndex(i),
                        prop.Fpr("value").Fpr("metaData").Fpr("tags"), i);
                }
                
                EditorGUILayout.EndHorizontal();
            }

            GUI.backgroundColor = Color.green;
            
            if (GUILayout.Button(TagAddButtonContent, GUILayout.Width(45), GUILayout.Height(22.5f)))
            {
                SearchProviderInstancing.SearchProviderTags.SelectionMade.Clear();
                SearchProviderInstancing.SearchProviderTags.SelectionMade.Add(OnTagSearchSelectionMade);

                var exclude = new List<string>();

                for (var i = 0; i < prop.Fpr("value").Fpr("metaData").Fpr("tags").arraySize; i++)
                {
                    exclude.Add(prop.Fpr("value").Fpr("metaData").Fpr("tags").GetIndex(i).stringValue);
                }
                
                SearchProviderInstancing.SearchProviderTags.Open(exclude);

                EditorGUILayout.EndHorizontal();
                return;
                void OnTagSearchSelectionMade(SearchTreeEntry entry)
                {
                    prop.Fpr("value").Fpr("metaData").Fpr("tags").InsertIndex(prop.Fpr("value").Fpr("metaData").Fpr("tags").arraySize);
                    prop.Fpr("value").Fpr("metaData").Fpr("tags").GetIndex(prop.Fpr("value").Fpr("metaData").Fpr("tags").arraySize - 1).stringValue = (string) entry.userData;
                    prop.serializedObject.ApplyModifiedProperties();
                    prop.serializedObject.Update();
                }
            }
            
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.EndHorizontal();
        }
        
        
        private static void DrawTag(SerializedProperty property, SerializedProperty arrayProp, int index)
        {
            var copy = property.stringValue;
            
            EditorGUILayout.BeginHorizontal("HelpBox", GUILayout.Width(copy.Width() + 17.5f));
            EditorGUILayout.LabelField(new GUIContent(copy, TagStdButtonContent.image), new GUIStyle("ToolbarTextField"), GUILayout.Width(copy.Width() + 17.5f));

            GUI.backgroundColor = Color.red;
            
            if (GUILayout.Button("(X)", new GUIStyle("ToolbarTextField")))
            {
                arrayProp.DeleteIndex(index);
                arrayProp.serializedObject.ApplyModifiedProperties();
                arrayProp.serializedObject.Update();
            }
            
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.EndHorizontal();
        }
    }
}