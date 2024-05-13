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
using CarterGames.Assets.AudioManager.Logging;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the settings provider GUI for the asset.
    /// </summary>
    public static class AudioManagerSettingsWindow
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static SettingsProvider provider;
        private static Color defaultGUIColor;
        private static Rect rect;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static bool EventsSubbed { get; set; }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Menu Items
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        [MenuItem("Tools/Carter Games/Audio Manager/Edit Settings", priority = 0)]
        private static void OpenSettingsWindow()
        {
            SettingsService.OpenProjectSettings(UtilEditor.SettingsLocationPath);
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Settings Provider
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        [SettingsProvider]
        public static SettingsProvider AudioManagerSettingsProvider()
        {
            provider = new SettingsProvider(UtilEditor.SettingsLocationPath, SettingsScope.Project)
            {
                guiHandler = (searchContext) =>
                {
                    if (!EventsSubbed)
                    {
                        EventsSubbed = true;
                        AudioManagerEditorEvents.OnSettingsReset.Add(provider.Repaint);
                    }
                    
                    defaultGUIColor = GUI.color;
                    
                    DrawHeaderSection();

                    EditorGUI.BeginChangeCheck();
                    
                    GUILayout.Space(7.5f);
                    DrawDisclaimer();                    
                    
                    GUILayout.Space(2f);
                    DrawAssetMetaData();

                    GUILayout.Space(2.5f);
                    
                    EditorGUILayout.BeginVertical("HelpBox");
                    
                    EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
                    UtilEditor.DrawHorizontalGUILine();
                    
                    GUILayout.Space(2.5f);
                    DrawEditorSettings();
                    
                    GUILayout.Space(2.5f);
                    DrawAudioPrefabOptions();
                    
                    EditorGUILayout.EndVertical();

                    if (EditorGUI.EndChangeCheck())
                    {
                        UtilEditor.SettingsObject.ApplyModifiedProperties();
                        UtilEditor.SettingsObject.Update();
                    }
                    
                    GUILayout.Space(2.5f);
                    DrawButtons();
                    
                    UtilEditor.CreateDeselectZone(ref rect);
                },
                
                keywords = new HashSet<string>(new[] { "Carter Games", "External Assets", "Tools", "Audio", "Audio Manager", "Sound Manager", "Music", "Sound" })
            };

            return provider;
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Draws a disclaimer section warning users of possible bugs & instability in the new rewrite...
        /// </summary>
        private static void DrawDisclaimer()
        {
            GUI.backgroundColor = UtilEditor.Yellow;
            
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(1.5f);


            EditorGUILayout.LabelField("Disclaimer", EditorStyles.boldLabel);
            
            GUI.backgroundColor = Color.white;
            UtilEditor.DrawHorizontalGUILine();
            
            EditorGUILayout.HelpBox("This is an early version of a total re-write of the asset. Bugs are to be expected as I simply don't have the time to test every edge-case before release.\nPlease let me know of any issues that arise so I can fix them.", MessageType.Info);

            GUI.backgroundColor = UtilEditor.Yellow;
            
            if (GUILayout.Button("Report Issue"))
            {
                Application.OpenURL("https://carter.games/report/");
            }
            
            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
            
            GUI.backgroundColor = Color.white;
        }
        
        
        /// <summary>
        /// Draws the header section of the settings window.
        /// </summary>
        private static void DrawHeaderSection()
        {
            var managerHeader = UtilEditor.AudioManagerBanner;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
                    
            if (managerHeader)
            {
                if (GUILayout.Button(managerHeader, GUIStyle.none, GUILayout.MaxHeight(125)))
                {
                    GUI.FocusControl(null);
                }
            }
                    
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }


        /// <summary>
        /// Draws the meta data for the section.
        /// </summary>
        private static void DrawAssetMetaData()
        {
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(1.5f);
            
            EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);
            UtilEditor.DrawHorizontalGUILine();
            
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField(new GUIContent("Version"),  new GUIContent(AssetVersionData.VersionNumber));
            VersionEditorGUI.DrawCheckForUpdatesButton();
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.LabelField(new GUIContent("Release Date"), new GUIContent(AssetVersionData.ReleaseDate));

            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Draws the prefab options.
        /// </summary>
        private static void DrawAudioPrefabOptions()
        {
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(1.5f);
            
            PerUserSettings.ShowAudioOptions = EditorGUILayout.Foldout(PerUserSettings.ShowAudioOptions, "Audio");

            if (PerUserSettings.ShowAudioOptions)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical("Box");
                
                EditorGUILayout.LabelField("Audio Clips", EditorStyles.boldLabel);
                UtilEditor.DrawHorizontalGUILine();

                EditorGUILayout.PropertyField(UtilEditor.SettingsObject.Fp("playAudioState"));
                
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(UtilEditor.SettingsObject.Fp("sequencePrefab"));
                EditorGUI.EndDisabledGroup();
                GUI.backgroundColor = UtilEditor.Yellow;
                if (GUILayout.Button("Edit", GUILayout.Width(55)))
                {
                    EditorGUIUtility.ShowObjectPicker<AudioPlayerSequence>(null, false, "t:GameObject", 10);
                }
            
                if ((Event.current.commandName == "ObjectSelectorClosed" || Event.current.commandName == "ObjectSelectorUpdated") && EditorGUIUtility.GetObjectPickerControlID() == 10)
                {
                    if (EditorGUIUtility.GetObjectPickerObject() != null)
                    {
                        if (((GameObject)EditorGUIUtility.GetObjectPickerObject()).GetComponent<AudioPlayerSequence>() != null)
                        {
                            UtilEditor.SettingsObject.Fp("sequencePrefab").objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
                            UtilEditor.SettingsObject.ApplyModifiedProperties();
                            UtilEditor.SettingsObject.Update();
                        }
                        else
                        {
                            AmDebugLogger.Warning($"{AudioManagerErrorCode.PrefabNotValid}\nThe prefab you tried to assign did not have a AudioSequence class attached.");
                        }
                    }
                    else
                    {
                        UtilEditor.SettingsObject.Fp("sequencePrefab").objectReferenceValue = null;
                        UtilEditor.SettingsObject.ApplyModifiedProperties();
                        UtilEditor.SettingsObject.Update();
                    }
                }
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();
              
                
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(UtilEditor.SettingsObject.Fp("audioPrefab"));
                EditorGUI.EndDisabledGroup();
                GUI.backgroundColor = UtilEditor.Yellow;
                if (GUILayout.Button("Edit", GUILayout.Width(55)))
                {
                    EditorGUIUtility.ShowObjectPicker<AudioPlayer>(null, false, "t:GameObject", 11);
                }
            
                if ((Event.current.commandName == "ObjectSelectorClosed" || Event.current.commandName == "ObjectSelectorUpdated") && EditorGUIUtility.GetObjectPickerControlID() == 11)
                {
                    if (EditorGUIUtility.GetObjectPickerObject() != null)
                    {
                        if (((GameObject)EditorGUIUtility.GetObjectPickerObject()).GetComponent<AudioPlayer>() != null)
                        {
                            UtilEditor.SettingsObject.Fp("audioPrefab").objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
                            UtilEditor.SettingsObject.ApplyModifiedProperties();
                            UtilEditor.SettingsObject.Update();
                        }
                        else
                        {
                            AmDebugLogger.Warning($"{AudioManagerErrorCode.PrefabNotValid}\nThe prefab you tried to assign did not have a AudioPlayer class attached.");
                        }
                    }
                    else
                    {
                        UtilEditor.SettingsObject.Fp("audioPrefab").objectReferenceValue = null;
                        UtilEditor.SettingsObject.ApplyModifiedProperties();
                        UtilEditor.SettingsObject.Update();
                    }
                }
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.PropertyField(UtilEditor.SettingsObject.Fp("clipAudioMixer"), new GUIContent("Default Clip Mixer"));
                
                GUILayout.Space(12.5f);
                
                EditorGUILayout.LabelField("Music Tracks", EditorStyles.boldLabel);
                UtilEditor.DrawHorizontalGUILine();
                EditorGUILayout.PropertyField(UtilEditor.SettingsObject.Fp("playMusicState"));
                EditorGUILayout.PropertyField(UtilEditor.SettingsObject.Fp("musicAudioMixer"), new GUIContent("Default Music Mixer"));
                
                GUILayout.Space(12.5f);
                
                EditorGUILayout.LabelField("Pooling", EditorStyles.boldLabel);
                UtilEditor.DrawHorizontalGUILine();
                EditorGUILayout.PropertyField(UtilEditor.SettingsObject.Fp("audioPoolInitSize"));

                GUILayout.Space(12.5f);
                
                EditorGUILayout.LabelField("Variance", EditorStyles.boldLabel);
                UtilEditor.DrawHorizontalGUILine();
                    
                EditorGUILayout.PropertyField(UtilEditor.SettingsObject.Fp("volumeVarianceOffset"),
                    new GUIContent("Volume Variance", "The amount of +/- from the default value to change the volume when using global variance."));
                
                EditorGUILayout.PropertyField(UtilEditor.SettingsObject.Fp("pitchVarianceOffset"),
                    new GUIContent("Pitch Variance",
                        "The amount of +/- from the default value to change the pitch when using global variance."));
                
                GUILayout.Space(12.5f);
                
                EditorGUILayout.LabelField("Dynamic Time", EditorStyles.boldLabel);
                UtilEditor.DrawHorizontalGUILine();
                
                EditorGUILayout.PropertyField(UtilEditor.SettingsObject.Fp("dynamicDetectionOffset"),
                    new GUIContent("Dynamic Start Offset",
                        "The amount of samples reduced from the found start time when detecting the start time of a clip."));
                
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
            }

            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
        }
        
        
        /// <summary>
        /// Draws the editor only settings.
        /// </summary>
        private static void DrawEditorSettings()
        {
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(1.5f);

            PerUserSettings.ShowEditorOptions = EditorGUILayout.Foldout(PerUserSettings.ShowEditorOptions, "Editor");
            
            if (PerUserSettings.ShowEditorOptions)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginVertical("Box");

                PerUserSettings.VersionValidationAutoCheckOnLoad = EditorGUILayout.Toggle(new GUIContent("Update Check On Load",
                        "Checks for any updates to the asset from the GitHub page when you load the project."),
                    PerUserSettings.VersionValidationAutoCheckOnLoad);
                
                PerUserSettings.ShowHelpBoxes = EditorGUILayout.Toggle(new GUIContent("Help Boxes",
                        "Show prompts under some buttons to help aid in understanding what does what?"),
                    PerUserSettings.ShowHelpBoxes);

                PerUserSettingsRuntime.ShowDebugLogs = EditorGUILayout.Toggle(
                    new GUIContent("Debug Logs", "Show log messages in the console for this asset?"),
                    PerUserSettingsRuntime.ShowDebugLogs);
                
                PerUserSettings.DeveloperDebugMode = EditorGUILayout.Toggle(
                    new GUIContent("Dev Mode", "Toggle Developer mode, for debugging ONLY!"),
                    PerUserSettings.DeveloperDebugMode);

                if (GUILayout.Button("Reset Settings"))
                {
                    if (EditorUtility.DisplayDialog("Reset Audio Manager Settings",
                            "Are you sure that you want to reset all settings for the asset to their defaults. This only applies to you. Other users of the project will have to do this for their settings themselves.",
                            "Reset Settings", "Cancel"))
                    {
                        PerUserSettings.ResetPrefs();
                        PerUserSettingsRuntime.ResetPrefs();
                    }
                }
                
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
            }
            
            GUILayout.Space(2.55f);
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Draws the buttons at the bottom of the provider.
        /// </summary>
        private static void DrawButtons()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Buy Me A Coffee", GUILayout.Height(30), GUILayout.MinWidth(100)))
            {
                Application.OpenURL("https://carter.games/donate");
            }
            
            if (GUILayout.Button("GitHub", GUILayout.Height(30), GUILayout.MinWidth(100)))
            {
                Application.OpenURL("https://github.com/CarterGames/AudioManager");
            }

            if (GUILayout.Button("Documentation", GUILayout.Height(30), GUILayout.MinWidth(100)))
            {
                Application.OpenURL("https://carter.games/audiomanager/");
            }

            if (GUILayout.Button("Support", GUILayout.Height(30), GUILayout.MinWidth(100)))
            {
                Application.OpenURL("https://carter.games/contact");
            }

            EditorGUILayout.EndHorizontal();

            var carterGamesBanner = UtilEditor.CarterGamesBanner;

            if (carterGamesBanner != null)
            {
                GUI.contentColor = new Color(1, 1, 1, .75f);

                if (GUILayout.Button(carterGamesBanner, GUILayout.MaxHeight(40)))
                {
                    Application.OpenURL("https://carter.games");
                }

                GUI.contentColor = defaultGUIColor;
            }
            else
            {
                if (GUILayout.Button("Carter Games", GUILayout.MaxHeight(40)))
                {
                    Application.OpenURL("https://carter.games");
                }
            }
        }
    }
}