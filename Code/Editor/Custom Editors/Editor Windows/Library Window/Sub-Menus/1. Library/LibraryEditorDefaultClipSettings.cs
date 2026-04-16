using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
	public static class LibraryEditorDefaultClipSettings
	{
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static readonly GUIContent FoldoutLabel = new GUIContent("Base Clip Settings");
        private static SerializedProperty property;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static SerializedProperty ShowProp => property.Fpr("defaultSettings");
        private static SerializedProperty VolumeProp => property.Fpr("defaultSettings").Fpr("volume");
        private static SerializedProperty PitchProp => property.Fpr("defaultSettings").Fpr("pitch");
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Draws the library editor for dynamic time.
        /// </summary>
        /// <param name="propertyReference">The property to base off.</param>
        public static void DrawLibraryEditor(SerializedProperty propertyReference)
        {
            EditorGUILayout.BeginVertical(propertyReference.Fpr("value").Fpr("defaultSettings").isExpanded ? "HelpBox" : "Box");
            
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

                EditorGUILayout.PropertyField(VolumeProp);
                EditorGUILayout.PropertyField(PitchProp);
                
                EditorGUILayout.Space(1.5f);
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(3.5f);
            }

            EditorGUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space(ShowProp.isExpanded ? 3f : .5f);
            
            if (!EditorGUI.EndChangeCheck()) return;
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
        }
	}
}