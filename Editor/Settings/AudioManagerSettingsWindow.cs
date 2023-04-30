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

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// The settings provider window for the asset.
    /// </summary>
    public class AudioManagerSettingsWindow : EditorWindow
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static SettingsProvider _provider;
        private static SerializedObject _settingsAssetObject;
        private static bool _listeningToEvents;
        private static int _directoryIndex;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static SerializedObject SettingsAssetObject
        {
            get
            {
                if (_settingsAssetObject != null) return _settingsAssetObject;
                _settingsAssetObject = new SerializedObject(AudioManagerEditorUtil.Settings);
                return _settingsAssetObject;
            }
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Events
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        public static Action OnBaseDirectoryUpdated = delegate { };
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Settings Provider
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        [SettingsProvider]
        public static SettingsProvider MultiSceneSettingsDrawer()
        {
            var provider = new SettingsProvider(AudioManagerEditorUtil.SettingsWindowPath, SettingsScope.Project)
            {
                guiHandler = (searchContext) =>
                {
                    if (!AudioManagerEditorUtil.HasSettingsFile) return;

                    DrawHeader();
                    DrawInfo();
                    
                    EditorGUILayout.BeginVertical("HelpBox");
                    GUILayout.Space(1.5f);
            
                    EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
                    GUILayout.Space(1.5f);
                    
                    EditorGUI.BeginChangeCheck();
                    
                    DrawSettings();
                    
                    if (EditorGUI.EndChangeCheck())
                    {
                        SettingsAssetObject.ApplyModifiedProperties();
                        SettingsAssetObject.Update();
                    }

                    GUILayout.Space(2.5f);
                    EditorGUILayout.EndVertical();
                    
                    DrawButtons();
                },
                
                keywords = new HashSet<string>(new[] { "Carter Games", "External Assets", "Tools", "Audio Manager", "Audio", "Audio Management", "Music", "Sound" })
            };
            
            return provider;
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Draws the default Banner Logo header for the asset...
        /// </summary>
        private static void DrawHeader()
        {
            var managerHeader = AudioManagerEditorUtil.ManagerHeader;
            
            GUILayout.Space(5f);
                    
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
                    
            if (managerHeader != null)
            {
                if (GUILayout.Button(managerHeader, GUIStyle.none, GUILayout.MaxHeight(110)))
                {
                    GUI.FocusControl(null);
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
                    
            GUILayout.Space(5f);
        }
        
        
        /// <summary>
        /// Draws the info section of the settings provider.
        /// </summary>
        private static void DrawInfo()
        {
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(1.5f);
            
            EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);
            
            EditorGUILayout.HelpBox("The 2.x version of this asset will no longer receive official support by the end of 2023. The new 3.x version will be released this year to replace it.", MessageType.Info);

            EditorGUILayout.LabelField(new GUIContent("Version", "The version of the asset in use."),  new GUIContent(AssetVersionData.VersionNumber));
            EditorGUILayout.LabelField(new GUIContent("Release Date", "The date this version of the asset was published on."), new GUIContent(AssetVersionData.ReleaseDate));

            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Draws the settings section of the settings provider.
        /// </summary>
        private static void DrawSettings()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Base Scan Path ", GUILayout.Width(150));
            SettingsAssetObject.FindProperty("baseAudioScanPath").stringValue = DirectorySelectHelper.ConvertIntToDir(EditorGUILayout.Popup(DirectorySelectHelper.ConvertStringToIndex(SettingsAssetObject.FindProperty("baseAudioScanPath").stringValue), DirectorySelectHelper.GetAllDirectories().ToArray()));

            if (GUILayout.Button("Refresh", GUILayout.Width(100)))
            {
                DirectorySelectHelper.RefreshAllDirectories();
            }
            
            EditorGUILayout.EndHorizontal();
            
            if (EditorGUI.EndChangeCheck())
            {
                SettingsAssetObject.ApplyModifiedProperties();
                OnBaseDirectoryUpdated?.Invoke();
            }

            GUI.enabled = false;
            EditorGUILayout.PropertyField(SettingsAssetObject.FindProperty("isUsingStatic"));
            GUI.enabled = true;
            
            EditorGUILayout.PropertyField(SettingsAssetObject.FindProperty("showDebugMessages"));
        }


        /// <summary>
        /// Draws the buttons section of the settings provider.
        /// </summary>
        private static void DrawButtons()
        {
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("GitHub", GUILayout.Height(30), GUILayout.MinWidth(100)))
                Application.OpenURL("https://github.com/CarterGames/AudioManager");

            if (GUILayout.Button("Documentation", GUILayout.Height(30), GUILayout.MinWidth(100)))
                Application.OpenURL("https://carter.games/audiomanager");

            if (GUILayout.Button("Support", GUILayout.Height(30), GUILayout.MinWidth(100)))
                Application.OpenURL("https://carter.games/contact");

            EditorGUILayout.EndHorizontal();

            if (AudioManagerEditorUtil.CarterGamesBanner != null)
            {
                var defaultTextColour = GUI.contentColor;
                GUI.contentColor = new Color(1, 1, 1, .75f);
            
                if (GUILayout.Button(AudioManagerEditorUtil.CarterGamesBanner, GUILayout.MaxHeight(40)))
                    Application.OpenURL("https://carter.games");
            
                GUI.contentColor = defaultTextColour;
            }
            else
            {
                if (GUILayout.Button("Carter Games", GUILayout.MaxHeight(40)))
                    Application.OpenURL("https://carter.games");
            }
        }
    }
}