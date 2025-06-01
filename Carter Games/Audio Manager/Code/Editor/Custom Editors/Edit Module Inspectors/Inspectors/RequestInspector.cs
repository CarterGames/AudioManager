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

using System.Linq;
using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// The editor GUI logic for the users request.
    /// </summary>
    public static class RequestInspector
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private static readonly string[] Options = new string[2] { "Single", "Group" };
        private static SearchProviderLibrary searchProviderLibrary;
        private static SearchProviderGroup searchProviderGroup;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static SerializedObject TargetObject { get; set; }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Draws the inspector GUI for the module.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        public static void DrawInspector(SerializedObject targetObject)
        {
            if (searchProviderLibrary == null)
            {
                searchProviderLibrary = ScriptableObject.CreateInstance<SearchProviderLibrary>();
            }

            if (searchProviderGroup == null)
            {
                searchProviderGroup = ScriptableObject.CreateInstance<SearchProviderGroup>();
            }
            
            TargetObject = targetObject;
            
            EditorGUILayout.BeginVertical("HelpBox");
            
            GUILayout.Space(3.5f);
            
            EditorGUI.BeginChangeCheck();
            
            targetObject.Fp("isGroup").intValue =
                GUILayout.Toolbar(targetObject.Fp("isGroup").intValue, Options);
            
            GUILayout.Space(3.5f);
            EditorGUILayout.BeginVertical("Box");
            
            EditorGUILayout.PropertyField(targetObject.Fp("playInstantly"));
            
            switch (targetObject.Fp("isGroup").intValue)
            {
                case 0:

                    if (string.IsNullOrEmpty(targetObject.Fp("request").stringValue))
                    {
                        if (GUILayout.Button("Select Clip"))
                        {
                            SearchProviderInstancing.SearchProviderLibrary.SelectionMade.Clear();
                            SearchProviderInstancing.SearchProviderLibrary.SelectionMade.Add(SelectClip);
                            
                            SearchProviderInstancing.SearchProviderLibrary.Open();
                        }
                        
                        GUILayout.Space(1.5f);
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUI.BeginDisabledGroup(true);
                        
                        if (!string.IsNullOrEmpty(targetObject.Fp("request").stringValue))
                        {
                            if (ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.LibraryLookup.ContainsKey(targetObject.Fp("request").stringValue))
                            {
                                EditorGUILayout.TextField("Request", ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef
                                    .LibraryLookup[targetObject.Fp("request").stringValue].key);
                            }
                            else
                            {
                                targetObject.Fp("request").stringValue = string.Empty;
                                EditorGUILayout.EndHorizontal();
                                
                                targetObject.ApplyModifiedProperties();
                                targetObject.Update();
                                
                                return;
                            }
                        }
                        else
                        {
                            EditorGUILayout.TextField(string.Empty);
                        }
                        
                        EditorGUI.EndDisabledGroup();
                        
                        if (GUILayout.Button("Change Clip", GUILayout.Width(100)))
                        {
                            SearchProviderInstancing.SearchProviderLibrary.SelectionMade.Clear();
                            SearchProviderInstancing.SearchProviderLibrary.SelectionMade.Add(SelectClip);
                            
                            SearchProviderInstancing.SearchProviderLibrary.Open();
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    }
                    
                    break;
                case 1:

                    if (string.IsNullOrEmpty(targetObject.Fp("groupRequest").stringValue))
                    {
                        if (GUILayout.Button("Select Group"))
                        {
                            SearchProviderInstancing.SearchProviderGroups.SelectionMade.Clear();
                            SearchProviderInstancing.SearchProviderGroups.SelectionMade.Add(SelectGroup);
                            
                            SearchProviderInstancing.SearchProviderGroups.Open();
                        }
                        
                        GUILayout.Space(1.5f);
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUI.BeginDisabledGroup(true);
                        
                        if (!string.IsNullOrEmpty(targetObject.Fp("groupRequest").stringValue))
                        {
                            if (ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.GroupsLookup.ContainsKey(targetObject.Fp("groupRequest").stringValue))
                            {
                                EditorGUILayout.TextField("Group Request", ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef
                                    .GroupsLookup[targetObject.Fp("groupRequest").stringValue].GroupName);
                            }
                            else
                            {
                                targetObject.Fp("groupRequest").stringValue = string.Empty;
                                
                                EditorGUILayout.EndHorizontal();
                                
                                targetObject.ApplyModifiedProperties();
                                targetObject.Update();
                                
                                return;
                            }
                        }
                        else
                        {
                            EditorGUILayout.TextField(string.Empty);
                        }
                        
                        EditorGUI.EndDisabledGroup();
                        
                        if (GUILayout.Button("Change Group", GUILayout.Width(100)))
                        {
                            SearchProviderInstancing.SearchProviderGroups.SelectionMade.Clear();
                            SearchProviderInstancing.SearchProviderGroups.SelectionMade.Add(SelectGroup);
                            
                            SearchProviderInstancing.SearchProviderGroups.Open(ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.GroupsLookup[targetObject.Fp("groupRequest").stringValue].GroupName);
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    }
                    
                    break;
            }
            
            
            if (EditorGUI.EndChangeCheck())
            {
                targetObject.ApplyModifiedProperties();
                targetObject.Update();
            }
            
            GUILayout.Space(3.5f);
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Runs when a clip is selected from the search provider.
        /// </summary>
        /// <param name="treeEntry">The entry selected in the search provider.</param>
        private static void SelectClip(SearchTreeEntry treeEntry)
        {
            SearchProviderInstancing.SearchProviderLibrary.SelectionMade.Remove(SelectClip);
            
            TargetObject.Fp("request").stringValue = ((AudioData)treeEntry.userData).id;
            TargetObject.ApplyModifiedProperties();
            TargetObject.Update();  
        }
        
        
        /// <summary>
        /// Runs when a group is selected from the search provider.
        /// </summary>
        /// <param name="treeEntry">The entry selected in the search provider.</param>
        private static void SelectGroup(SearchTreeEntry treeEntry)
        {
            SearchProviderInstancing.SearchProviderGroups.SelectionMade.Remove(SelectGroup);
            
            TargetObject.Fp("groupRequest").stringValue = ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.GroupsLookup.ToList().First(t => t.Value.GroupName.Equals(((GroupData)treeEntry.userData).GroupName)).Key;
            TargetObject.ApplyModifiedProperties();
            TargetObject.Update();
        }
    }
}