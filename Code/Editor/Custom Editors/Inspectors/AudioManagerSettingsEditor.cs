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

using CarterGames.Assets.AudioManager.Logging;
using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the custom editor for the asset settings object.
    /// </summary>
    [CustomEditor(typeof(AmAssetSettings))]
    public sealed class AudioManagerSettingsEditor : UnityEditor.Editor
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private bool debugProp;
        private static Color defaultGUIBackground;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private void OnEnable()
        {
            Initialize();
        }


        public override void OnInspectorGUI()
        {
            Initialize();
            
            GUILayout.Space(12.5f);
            
            UtilEditor.DrawSoScriptSection((AmAssetSettings) target);
            
            GUILayout.Space(1.5f);
            DrawAudioOptions();
            GUILayout.Space(1.5f);
            DrawExtraSettings();
            GUILayout.Space(1.5f);
            DrawButtons();
            
            serializedObject.Update();
            
            if (!PerUserSettings.DeveloperDebugMode) return;
            
            EditorGUILayout.LabelField("DEVELOPER DEBUG", EditorStyles.boldLabel);
            UtilEditor.DrawHorizontalGUILine();
            
            base.OnInspectorGUI();
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Editor Drawer Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private void Initialize()
        {
            debugProp = PerUserSettingsRuntime.ShowDebugLogs;
            defaultGUIBackground = GUI.backgroundColor;
            AudioManagerEditorEvents.OnSettingsReset.Add(Repaint);
        }
        
        
        private static void DrawButtons()
        {
            GUI.backgroundColor = UtilEditor.Green;
            
            if (GUILayout.Button("Edit Settings", GUILayout.Height(25f)))
                SettingsService.OpenProjectSettings(UtilEditor.SettingsLocationPath);
            
            GUI.backgroundColor = UtilEditor.Red;
            
            if (GUILayout.Button("Reset Settings", GUILayout.Height(25f)))
            {
                if (EditorUtility.DisplayDialog("Reset Audio Manager Settings",
                        "Are you sure you want to reset the asset settings?\n\nNote this will not reset any editor only settings, just the prefabs & runtime settings",
                        "Reset", "Cancel"))
                {
                    ScriptableRef.GetAssetDef<AmAssetSettings>().AssetRef.ResetSettings();

                    GameObject audioPrefab = null;

                    if (UtilEditor.FileExistsByFilter(UtilEditor.AudioPrefabName)) 
                        audioPrefab = UtilEditor.AudioPrefab;
                    else
                        AmDebugLogger.Error("Unable to find the default audio prefab, setting to null.");
                    
                    AudioManagerEditorEvents.OnSettingsReset.Raise();
                }
            }
            
            GUI.backgroundColor = defaultGUIBackground;
        }


        private void DrawAudioOptions()
        {
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(1.5f);
            
            EditorGUILayout.LabelField("Audio Clip Options", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(true);
            
            EditorGUILayout.PropertyField(serializedObject.Fp("playAudioState"));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.Fp("playerPrefab"));

            GUI.backgroundColor = UtilEditor.Yellow;
            EditorGUI.EndDisabledGroup();
            
            if (GUILayout.Button("Edit", GUILayout.Width(55)))
            {
                 EditorGUIUtility.ShowObjectPicker<AudioSourceInstance>(null, false, "t:GameObject", 0);
            }
            
            if ((Event.current.commandName == "ObjectSelectorClosed" || Event.current.commandName == "ObjectSelectorUpdated") && EditorGUIUtility.GetObjectPickerControlID() == 0)
            {
                if (EditorGUIUtility.GetObjectPickerObject() != null)
                {
                    if (((GameObject)EditorGUIUtility.GetObjectPickerObject()).GetComponent<AudioSourceInstance>() != null)
                    {
                        serializedObject.Fp("playerPrefab").objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
                        serializedObject.ApplyModifiedProperties();
                        serializedObject.Update();
                    }
                }
                else
                {
                    serializedObject.Fp("playerPrefab").objectReferenceValue = null;
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                }
            }
                
            GUI.backgroundColor = Color.white;   
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.BeginDisabledGroup(true);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.Fp("sourceInstancePrefab"));

            GUI.backgroundColor = UtilEditor.Yellow;
            EditorGUI.EndDisabledGroup();
            
            if (GUILayout.Button("Edit", GUILayout.Width(55)))
            {
                EditorGUIUtility.ShowObjectPicker<AudioPlayer>(null, false, "t:GameObject", 1);
            }
            
            if ((Event.current.commandName == "ObjectSelectorClosed" || Event.current.commandName == "ObjectSelectorUpdated") && EditorGUIUtility.GetObjectPickerControlID() == 1)
            {
                if (EditorGUIUtility.GetObjectPickerObject() != null)
                {
                    if (((GameObject)EditorGUIUtility.GetObjectPickerObject()).GetComponent<AudioPlayer>() != null)
                    {
                        serializedObject.Fp("sourceInstancePrefab").objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
                        serializedObject.ApplyModifiedProperties();
                        serializedObject.Update();
                    }
                }
                else
                {
                    serializedObject.Fp("sourceInstancePrefab").objectReferenceValue = null;
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                }
            }
                
            GUI.backgroundColor = Color.white;   
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.PropertyField(serializedObject.Fp("clipAudioMixer"));
            
            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
        }
        
        
        private void DrawExtraSettings()
        {
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(1.5f);
            
            EditorGUILayout.LabelField("Additional Options", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(true);
            
            EditorGUILayout.Toggle(new GUIContent("Show Debug Logs"), debugProp);

            EditorGUILayout.PropertyField(serializedObject.Fp("audioPoolInitSize"));
            EditorGUILayout.PropertyField(serializedObject.Fp("useGlobalVariance"));
            
            EditorGUILayout.PropertyField(serializedObject.Fp("volumeVarianceOffset"));
            EditorGUILayout.PropertyField(serializedObject.Fp("pitchVarianceOffset"));
            
            EditorGUILayout.PropertyField(serializedObject.Fp("dynamicDetectionOffset"));

            EditorGUI.EndDisabledGroup();
            
            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
        }
    }
}