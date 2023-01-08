/*
 * Copyright (c) 2018-Present Carter Games
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
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// The custom inspector for the Audio Manager File.
    /// </summary>
    [CustomEditor(typeof(AudioManagerFile))]
    public class AudioManagerFileEditor : UnityEditor.Editor
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */ 
        
        private readonly string[] tabTitles = new string[2] {"Settings", "Library"};
        
        private SerializedProperty audioPrefab;
        private SerializedProperty isPopulated;
        private SerializedProperty audioMixers;
        private SerializedProperty directories;
        private SerializedProperty library;
        private SerializedProperty tabPos;
        
        private Color normalBackgroundCol;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */ 
        
        private void OnEnable()
        {
            RefreshReferences();
        }


        public override void OnInspectorGUI()
        {
            AudioManagerEditorUtil.Header("Audio Manager File");
            DrawScriptSection();
            RenderTabBar();
            RenderSettings(tabPos.intValue);
            RenderLibrary(tabPos.intValue);

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
        
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */ 
        
        /// <summary>
        /// Updates the references for all the serialized properties.
        /// </summary>
        private void RefreshReferences()
        {
            audioPrefab = serializedObject.FindProperty("soundPrefab");
            isPopulated = serializedObject.FindProperty("isPopulated");
            audioMixers = serializedObject.FindProperty("audioMixer");
            directories = serializedObject.FindProperty("directory");
            library = serializedObject.FindProperty("library");
            tabPos = serializedObject.FindProperty("tabPos");
            normalBackgroundCol = GUI.backgroundColor;
        }


        /// <summary>
        /// Draws the tab options for the inspector.
        /// </summary>
        private void RenderTabBar()
        {
            GUILayout.Space(5f);
            tabPos.intValue = GUILayout.Toolbar(tabPos.intValue, tabTitles);
            GUILayout.Space(5f);
        }

        
        /// <summary>
        /// Draws the script section of the inspector.
        /// </summary>
        private void DrawScriptSection()
        {
            GUILayout.Space(4.5f);
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(1.5f);
            
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromScriptableObject(target as AudioManagerFile), typeof(AudioManagerFile), false);
            GUI.enabled = true;
            
            GUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
        }
        

        /// <summary>
        /// Draws the settings section of the inspector.
        /// </summary>
        /// <param name="pos">The position of the tab is currently on.</param>
        private void RenderSettings(int pos)
        {
            if (!pos.Equals(0)) return;

            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(2.5f);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("File Settings", EditorStyles.boldLabel, GUILayout.MaxWidth(120f));
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(2.5f);
            
            EditorGUILayout.PropertyField(audioPrefab, new GUIContent("Sound Prefab:"));
            
            GUI.enabled = false;
            EditorGUILayout.PropertyField(isPopulated);
            GUI.enabled = true;
            
            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
            
            //
            //
            
            GUILayout.Space(3.5f);
            
            //
            //
            
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(2.5f);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mixers", EditorStyles.boldLabel, GUILayout.MaxWidth(120f));
            EditorGUILayout.EndHorizontal();

            if (audioMixers.arraySize.Equals(0))
            {
                audioMixers.InsertArrayElementAtIndex(0);
                return;
            }
            
            for (var i = 0; i < audioMixers.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"#{(i).ToString()}", GUILayout.Width(AudioManagerEditorUtil.TextWidth($"#{(i+1).ToString()} ")));
                EditorGUILayout.PropertyField(audioMixers.GetArrayElementAtIndex(i), GUIContent.none);

                GUI.backgroundColor = AudioManagerEditorUtil.Green;
                
                if (GUILayout.Button("+", GUILayout.Width(25f)))
                    audioMixers.InsertArrayElementAtIndex(i + 1);
                
                GUI.backgroundColor = AudioManagerEditorUtil.Red;

                if (GUILayout.Button("-", GUILayout.Width(25f)))
                    audioMixers.DeleteArrayElementAtIndex(i);

                GUI.backgroundColor = normalBackgroundCol;
                
                EditorGUILayout.EndHorizontal();
            }    
            
            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
            
            //
            //

            GUILayout.Space(3.5f);
            
            //
            //

            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(2.5f);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Directories", EditorStyles.boldLabel, GUILayout.MaxWidth(120f));
            EditorGUILayout.EndHorizontal();

            if (directories.arraySize.Equals(0))
            {
                if (GUILayout.Button("Add First Element"))
                {
                    directories.InsertArrayElementAtIndex(0);
                }
                
                return;
            }

            var options = DirectorySelectHelper.GetDirectoriesFromBase(false);
            
            for (var i = 0; i < directories.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"#{(i+1).ToString()}", GUILayout.Width(AudioManagerEditorUtil.TextWidth($"#{(i+1).ToString()} ")));
                directories.GetArrayElementAtIndex(i).stringValue = DirectorySelectHelper.ConvertIntToDir(EditorGUILayout.Popup(DirectorySelectHelper.ConvertStringToIndex(directories.GetArrayElementAtIndex(i).stringValue, options), options.ToArray()), options);
                
                GUI.backgroundColor = AudioManagerEditorUtil.Green;
                
                if (GUILayout.Button("+", GUILayout.Width(25f)))
                    directories.InsertArrayElementAtIndex(i + 1);

                GUI.backgroundColor = AudioManagerEditorUtil.Red;
                
                if (GUILayout.Button("-", GUILayout.Width(25f)))
                    directories.DeleteArrayElementAtIndex(i);

                GUI.backgroundColor = normalBackgroundCol;
                
                EditorGUILayout.EndHorizontal();
            }
            
            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Draws the library of all the clips in the inspector.
        /// </summary>
        /// <param name="pos">The position of the tab is currently on.</param>
        private void RenderLibrary(int pos)
        {
            if (!pos.Equals(1)) return;
            
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(2.5f);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Controls", EditorStyles.boldLabel, GUILayout.MaxWidth(120f));
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Force Update Library"))
            {
                // library.ClearArray();
                var strings = new List<string>();
                var list = new List<AudioClip>();
                AudioManagerScriptHelper.AddAudioClips(directories, isPopulated, list);
                AudioManagerScriptHelper.AddStrings(list, strings);
                
                for (var i = 0; i < list.Count; i++)
                {
                    if (library.arraySize > i)
                    {
                        library.GetArrayElementAtIndex(i).FindPropertyRelative("key").stringValue = strings[i];
                        library.GetArrayElementAtIndex(i).FindPropertyRelative("value").objectReferenceValue = list[i];
                    }
                    else
                    {
                        library.InsertArrayElementAtIndex(i);
                        library.GetArrayElementAtIndex(i).FindPropertyRelative("key").stringValue = strings[i];
                        library.GetArrayElementAtIndex(i).FindPropertyRelative("value").objectReferenceValue = list[i];
                    }
                }

                EditorUtility.DisplayDialog("Audio Manager | Library Updated", "The file has been updated with the latest scan of the directories entered.", "Continue");
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
            
            if (GUILayout.Button("Clear Library"))
            {
                library.ClearArray();
            }
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
            
            //
            //

            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(2.5f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Audio Library", EditorStyles.boldLabel, GUILayout.MaxWidth(120f));
            EditorGUILayout.EndHorizontal();

            if (library.arraySize > 0)
            {
                EditorGUILayout.HelpBox("Shows all the clips that are present in the library of this file.", MessageType.None);

                GUILayout.Space(5f);
            
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Clip Name", EditorStyles.boldLabel, GUILayout.Width(Screen.width / 2.25f));
                EditorGUILayout.LabelField("Audio Clip File", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();

                GUI.enabled = false;

                for (var i = 0; i < library.arraySize; i++)
                {
                    var _fileName = library.GetArrayElementAtIndex(i).FindPropertyRelative("key");
                    var _fileClip = library.GetArrayElementAtIndex(i).FindPropertyRelative("value");

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(_fileName, GUIContent.none, GUILayout.Width(Screen.width / 2.25f));

                    EditorGUILayout.PropertyField(_fileClip, GUIContent.none);
                    EditorGUILayout.EndHorizontal();
                }

                GUI.enabled = true;
            }
            else
            {
                EditorGUILayout.HelpBox("No clips currently in this library.", MessageType.None);
            }
            
            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
        }
    }
}
