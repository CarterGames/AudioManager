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
            
            EditorGUILayout.HelpBox("NOTE: 2.6.x will be the last version in the 2.x line. This version will be supported for 3 years after the release of the next version (3.x) with bug fixes & QOL updates only.", MessageType.Info);

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
            
            if (GUILayout.Button("Asset Store", GUILayout.Height(30), GUILayout.MinWidth(100)))
                Application.OpenURL("https://assetstore.unity.com/packages/tools/audio/audio-manager-cg-149123");
            
            if (GUILayout.Button("GitHub", GUILayout.Height(30), GUILayout.MinWidth(100)))
                Application.OpenURL("https://github.com/CarterGames/AudioManager");

            if (GUILayout.Button("Documentation", GUILayout.Height(30), GUILayout.MinWidth(100)))
                Application.OpenURL("https://carter.games/audiomanager");

            if (GUILayout.Button("Change Log", GUILayout.Height(30), GUILayout.MinWidth(100)))
                Application.OpenURL("https://carter.games/audiomanager/changelog");

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Email", GUILayout.Height(30), GUILayout.MinWidth(100)))
                Application.OpenURL("mailto:hello@carter.games?subject=Audio Manager Asset Enquiry");

            if (GUILayout.Button("Discord", GUILayout.Height(30), GUILayout.MinWidth(100)))
                Application.OpenURL("https://carter.games/discord");

            if (GUILayout.Button("Report Issues", GUILayout.Height(30), GUILayout.MinWidth(100)))
                Application.OpenURL("https://carter.games/report");

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