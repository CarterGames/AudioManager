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
    /// Handles the editor GUI for the dynamic time element of a clip.
    /// </summary>
    public static class LibraryEditorDynamicTime
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static readonly GUIContent FoldoutLabel = new GUIContent("Dynamic Start Time");
        private static readonly string[] OptionLabels = new string[] { "Automatic", "Manual" };
        private static SerializedProperty property;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static SerializedProperty ShowProp => property.Fpr("dynamicStartTime");
        private static SerializedProperty ThresholdProp => property.Fpr("dynamicStartTime").Fpr("threshold");
        private static SerializedProperty OptionProp => property.Fpr("dynamicStartTime").Fpr("option");
        private static SerializedProperty TimeProp => property.Fpr("dynamicStartTime").Fpr("time");
        private static SerializedProperty ValueProp => property.Fpr("value");
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Draws the library editor for dynamic time.
        /// </summary>
        /// <param name="propertyReference">The property to base off.</param>
        public static void DrawLibraryEditor(SerializedProperty propertyReference)
        {
            EditorGUILayout.BeginVertical(propertyReference.Fpr("value").Fpr("dynamicStartTime").isExpanded ? "HelpBox" : "Box");
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Space(1.5f);
            
            property = propertyReference.Fpr("value");
            ShowProp.isExpanded = EditorGUILayout.Foldout(ShowProp.isExpanded, FoldoutLabel);
            
            if (ShowProp.isExpanded)
            {
                EditorGUILayout.Space(1.5f);
                
                UtilEditor.DrawHorizontalGUILine();
                
                EditorGUILayout.BeginVertical();
                EditorGUILayout.Space(1.5f);
                
                OptionProp.intValue = GUILayout.Toolbar(OptionProp.intValue, OptionLabels, GUILayout.Height(27.5f));

                EditorGUILayout.Space();
                
                if (OptionProp.intValue.Equals(0))
                {
                    DrawAutomaticSection();
                }
                else
                {
                    DrawManualSection();
                }
                
                EditorGUILayout.Space(2.5f);

                EditorGUILayout.BeginVertical("HelpBox");
                DrawWaveform();
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.Space(1.5f);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(ShowProp.isExpanded ? 3f : .5f);
            
            if (!EditorGUI.EndChangeCheck()) return;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }


        /// <summary>
        /// Draws the automatic section.
        /// </summary>
        private static void DrawAutomaticSection()
        {
            EditorGUILayout.BeginVertical("HelpBox");
            EditorGUILayout.Space(1.5f);
            
            EditorGUILayout.PropertyField(ThresholdProp);

            GUI.backgroundColor = EditorColors.PastelGreen;
            if (GUILayout.Button("Estimate Start Time"))
            {
                if (DynamicTimeDetector.TryDetectStartTime((AudioClip) ValueProp.objectReferenceValue, ThresholdProp.floatValue, out var time))
                {
                    TimeProp.floatValue = time.time;

                    property.serializedObject.ApplyModifiedProperties();
                    property.serializedObject.Update();
                    
                    Undo.RecordObject(property.serializedObject.targetObject, "Dynamic Time Estimated");
                }
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
        }
        
        
        /// <summary>
        /// Draws the manual section.
        /// </summary>
        private static void DrawManualSection()
        {
            EditorGUILayout.BeginVertical("HelpBox");
            EditorGUILayout.Space(1.5f);
            
            EditorGUILayout.Slider(TimeProp, 0f, ((AudioClip) ValueProp.objectReferenceValue).length);
            
            EditorGUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Draws the waveform of the clip.
        /// </summary>
        private static void DrawWaveform()
        {
            var rect = EditorGUILayout.GetControlRect();
            
            WaveformHandler.RenderPreview(new Rect(rect.x, rect.y + 15, rect.width, 85), (AudioClip) ValueProp.objectReferenceValue);
            
            var normalisedStartTime = TimeProp.floatValue / ((AudioClip) ValueProp.objectReferenceValue).length;
            var newRect = rect;
            
            newRect.x += normalisedStartTime * rect.width;
            newRect.y += 15;
            newRect.height += 60;
            newRect.width = 2f;
            
            EditorGUI.DrawRect(newRect, new Color(1, 1, 1, .5f));
            
            EditorGUILayout.Space(80);
        }
    }
}