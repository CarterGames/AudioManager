﻿/*
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
        
        private static SerializedProperty ShowProp => property.Fpr("showDynamicTime");
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
            EditorGUILayout.BeginVertical("HelpBox");
            EditorGUILayout.BeginVertical();
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Space();
            
            property = propertyReference.Fpr("value");
            ShowProp.boolValue = EditorGUILayout.Foldout(ShowProp.boolValue, FoldoutLabel);
            
            EditorGUILayout.Space();
            
            
            if (ShowProp.boolValue)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.Space(1.5f);
                
                OptionProp.intValue = GUILayout.Toolbar(OptionProp.intValue, OptionLabels);

                EditorGUILayout.Space();
                
                if (OptionProp.intValue.Equals(0))
                {
                    DrawAutomaticSection();
                }
                else
                {
                    DrawManualSection();
                }
                
                EditorGUILayout.Space();
                UtilEditor.DrawHorizontalGUILine();

                DrawWaveform();
                
                EditorGUILayout.Space(1.5f);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
            
            if (!EditorGUI.EndChangeCheck()) return;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }


        /// <summary>
        /// Draws the automatic section.
        /// </summary>
        private static void DrawAutomaticSection()
        {
            EditorGUILayout.PropertyField(ThresholdProp);
            
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
        }
        
        
        /// <summary>
        /// Draws the manual section.
        /// </summary>
        private static void DrawManualSection()
        {
            EditorGUILayout.Slider(TimeProp, 0f, ((AudioClip) ValueProp.objectReferenceValue).length);
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