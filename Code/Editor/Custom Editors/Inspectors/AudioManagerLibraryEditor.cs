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

using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the custom editor for the audio library asset.
    /// </summary>
    [CustomEditor(typeof(AudioLibrary))]
    public sealed class AudioManagerLibraryEditor : UnityEditor.Editor
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private AudioLibrary library;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private void OnEnable()
        {
            library = target as AudioLibrary;
        }

        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUI.BeginChangeCheck();
            
            DrawEditor();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }


            if (!PerUserSettings.DeveloperDebugMode) return;
            
            EditorGUILayout.LabelField("DEVELOPER DEBUG", EditorStyles.boldLabel);
            UtilEditor.DrawHorizontalGUILine();
            
            base.OnInspectorGUI();
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Editor Drawer Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Draws the editor GUI.
        /// </summary>
        private void DrawEditor()
        {
            GUILayout.Space(12.5f);
            UtilEditor.DrawSoScriptSection((AudioLibrary) target);
            GUILayout.Space(1.5f);
            
            GUILayout.Space(1.5f);

            DrawStats();
            EditorGUILayout.Space(2.5f);
            DrawTools();

            GUILayout.Space(1.5f);
        }


        /// <summary>
        /// Draws the stats for the library.
        /// </summary>
        private void DrawStats()
        {
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(1.5f);
            
            EditorGUILayout.LabelField("Summary", EditorStyles.boldLabel);

            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Audio files scanned: {library.ClipCount}");
    
            if (GUILayout.Button("View Library", GUILayout.Width(110f)))
            {
                LibraryEditorWindow.ShowWindowOnTab(0);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField($"Groups defined: {library.Groups?.Count ?? 0}");
            
            if (GUILayout.Button("Edit Groups", GUILayout.Width(110f)))
            {
                LibraryEditorWindow.ShowWindowOnTab(1);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.LabelField($"Mixer groups found: {library.MixerCount}");
            
            if (GUILayout.Button("Show Mixers", GUILayout.Width(110f)))
            {
                LibraryEditorWindow.ShowWindowOnTab(2);
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Draws the buttons for the editor GUI.
        /// </summary>
        private void DrawTools()
        {
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(1.5f);

            EditorGUILayout.LabelField("Tools", EditorStyles.boldLabel);

            GUI.backgroundColor = UtilEditor.Green;
            if (GUILayout.Button("Open Library Window"))
            {
                LibraryEditorWindow.ShowWindowOnTab(0);
            }

            GUI.backgroundColor = Color.white;

            GUILayout.Space(1f);

            GUI.backgroundColor = UtilEditor.Yellow;
            if (GUILayout.Button("Perform Manual Scan"))
            {
                AudioScanner.ManualScan();
            }
            GUI.backgroundColor = Color.white;

            GUILayout.Space(1f);


            EditorGUI.BeginDisabledGroup(library.ClipCount <= 0);

            GUI.backgroundColor = UtilEditor.Red;

            if (GUILayout.Button("Clear Library"))
            {
                if (EditorUtility.DisplayDialog("Clear Audio Manager Library",
                        "Are you sure you want to clear the audio library. This will remove all entries from the library, as well as all groups and track lists defined.\n\nOnly do this if you find clips missing or something has gone wrong with the library.",
                        "Clear", "Cancel"))
                {
                    LibraryEditorLibraryTab.CanUpdate = false;
                    
                    PerUserSettings.LastLibraryIndexShown = -1;
                    PerUserSettings.LastLibraryGroupEntry = -1;
                    PerUserSettings.LastLibMusicEntry = -1;

                    var libObj = ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef;
                    
                    libObj.Fp("library").Fpr("list").ClearArray();
                    libObj.Fp("groups").Fpr("list").ClearArray();
                    libObj.Fp("tracks").Fpr("list").ClearArray();
                    
                    StructHandler.ResetLibraryStructs();

                    libObj.ApplyModifiedProperties();
                    libObj.Update();
                    
                    AudioManagerEditorEvents.OnLibraryRefreshed.Raise();
                }
            }

            GUI.backgroundColor = Color.white;
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(1f);
            EditorGUILayout.EndVertical();
        }
    }
}