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

using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// The custom inspector for the audio manager settings asset.
    /// </summary>
    [CustomEditor(typeof(AudioManagerSettings))]
    public class AudioManagerSettingsEditor : UnityEditor.Editor
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private SerializedProperty baseAudioScanPath;
        private SerializedProperty isUsingStatic;
        private SerializedProperty showDebugMessages;
        private Color defaultBackgroundColor;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private void OnEnable()
        {
            baseAudioScanPath = serializedObject.FindProperty("baseAudioScanPath");
            isUsingStatic = serializedObject.FindProperty("isUsingStatic");
            showDebugMessages = serializedObject.FindProperty("showDebugMessages");

            defaultBackgroundColor = GUI.backgroundColor;
        }
        

        public override void OnInspectorGUI()
        {
            AudioManagerEditorUtil.SettingsHeader();
            DrawScriptSection();
            DrawOptions();
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Draws the script section of the custom inspector.
        /// </summary>
        private void DrawScriptSection()
        {
            GUILayout.Space(4.5f);
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(1.5f);
            
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromScriptableObject(target as AudioManagerSettings), typeof(AudioManagerSettings), false);
            GUI.enabled = true;

            DrawButton();
            
            GUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Draws the options for the custom inspector.
        /// </summary>
        private void DrawOptions()
        {
            GUILayout.Space(4.5f);
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(1.5f);
            
            GUI.enabled = false;
            EditorGUILayout.PropertyField(baseAudioScanPath);
            EditorGUILayout.PropertyField(isUsingStatic);
            EditorGUILayout.PropertyField(showDebugMessages);
            GUI.enabled = true;
            
            GUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
        }
        
        
        /// <summary>
        /// Draws the edit settings button in the custom inspector.
        /// </summary>
        private void DrawButton()
        {
            GUILayout.Space(2f);

            GUI.backgroundColor = AudioManagerEditorUtil.Green;
            if (GUILayout.Button("Edit Settings", GUILayout.Height(25)))
            {
                SettingsService.OpenProjectSettings("Project/Carter Games/Audio Manager");
            }
            GUI.backgroundColor = defaultBackgroundColor;
        }
    }
}