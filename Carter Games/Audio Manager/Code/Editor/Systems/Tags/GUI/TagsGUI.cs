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