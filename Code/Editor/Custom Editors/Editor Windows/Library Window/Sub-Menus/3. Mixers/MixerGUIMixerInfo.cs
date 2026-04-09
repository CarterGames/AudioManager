using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
	public static class MixerGUIMixerInfo
	{
		public static void Draw(SerializedProperty property)
		{
			EditorGUILayout.BeginVertical("HelpBox");

			GUI.contentColor = EditorColors.PrimaryYellow;
			EditorGUILayout.LabelField("Mixer", EditorStyles.boldLabel);
			GUI.contentColor = Color.white;
            
			UtilEditor.DrawHorizontalGUILine();
			GUILayout.Space(5f);
            
			EditorGUILayout.BeginHorizontal();
            
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(property.Fpr("value").Fpr("key"));
			if (EditorGUI.EndChangeCheck())
			{
				property.serializedObject.ApplyModifiedProperties();
				property.serializedObject.Update();
			}
            
			
			if (GUILayout.Button("Copy Key", GUILayout.Width(100)))
			{
				Clipboard.Copy(property.Fpr("value").Fpr("key").stringValue);
				EditorUtility.DisplayDialog("Copy Mixer Key", "Key copied to clipboard", "Continue");
			}

            
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
            
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.PropertyField(property.Fpr("value").Fpr("mixerGroup"), new GUIContent("Mixer Reference", "The mixer this entry is for."));
			EditorGUI.EndDisabledGroup();
            
			if (GUILayout.Button("Select Mixer", GUILayout.Width(100)))
			{
				Selection.activeObject = property.Fpr("value").Fpr("mixerGroup").objectReferenceValue;
			}

			EditorGUILayout.EndHorizontal();
			GUILayout.Space(2.5f);
			EditorGUILayout.EndVertical();
		}
	}
}