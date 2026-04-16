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
    /// Handles the inspector GUI for a clip's file & meta data section.
    /// </summary>
    public static class LibraryEditorClip
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static readonly GUIContent ValueContent = new GUIContent("Audio File", "The file that was scanned into this entry.");
        private static readonly GUIContent PathContent = new GUIContent("File Path", "The location of this file in the project.");
        private static SerializedProperty property;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static SerializedProperty KeyProp => property.Fpr("key");
        private static SerializedProperty ValueKeyProp => property.Fpr("value").Fpr("key");
        private static SerializedProperty DefKeyProp => property.Fpr("value").Fpr("defaultKey");
        private static SerializedProperty ClipProp => property.Fpr("value").Fpr("value");
        private static SerializedProperty PathProp => property.Fpr("value").Fpr("path");

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Calls to draw the clip data when called.
        /// </summary>
        /// <param name="propertyReference">The property reference to use.</param>
        public static void DrawLibraryEditor(SerializedProperty propertyReference)
        {
            EditorGUI.BeginChangeCheck();
            
            property = propertyReference;

            DrawClipFields();

            if (!EditorGUI.EndChangeCheck()) return;
            KeyProp.serializedObject.ApplyModifiedProperties();
            KeyProp.serializedObject.Update();
            Undo.RecordObject(property.serializedObject.targetObject, "Clip Edited");
        }
        
        
        /// <summary>
        /// Draws the actual visuals for the clip data when called.
        /// </summary>
        private static void DrawClipFields()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space(1.5f);
            
            EditorGUILayout.BeginHorizontal();
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(ValueKeyProp);
            if (EditorGUI.EndChangeCheck())
            {
                ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.ApplyModifiedProperties();
                ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.Update();
            }
            
            if (!DefKeyProp.stringValue.Equals(KeyProp.stringValue))
            {
                if (GUILayout.Button("Reset Key", GUILayout.Width(100)))
                {
                    if (EditorUtility.DisplayDialog("Reset Clip Key",
                            "Are you sure you want to reset the key to its default value?", "Reset key", "Cancel"))
                    {
                        KeyProp.stringValue = DefKeyProp.stringValue;
                        property.serializedObject.ApplyModifiedProperties();
                        property.serializedObject.Update();
                        
                        Undo.RecordObject(property.serializedObject.targetObject, "Clip Key Reset");
                    }
                    
                    GUI.FocusControl(null);
                }
            }

            
            if (GUILayout.Button("Copy Key", GUILayout.Width(100)))
            {
                Clipboard.Copy(KeyProp.stringValue);
            }
            

            EditorGUILayout.EndHorizontal();

            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();

            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.PropertyField(ClipProp, ValueContent);
            EditorGUILayout.PropertyField(PathProp, PathContent);

            EditorGUI.EndDisabledGroup();

            
            EditorGUILayout.EndVertical();
            

            if (Selection.activeObject != null)
            {
                EditorGUI.BeginDisabledGroup(Selection.activeObject.Equals(ClipProp.objectReferenceValue));
            }

            
            if (GUILayout.Button("Select File", GUILayout.Width(100),
                    GUILayout.Height((EditorGUIUtility.singleLineHeight * 2) + 1.5f)))
            {
                Selection.activeObject = ClipProp.objectReferenceValue;
            }
            

            if (Selection.activeObject != null)
            {
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
        }
    }
}