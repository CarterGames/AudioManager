using System.Collections.Generic;
using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
	public static class MixerGUIMixerInspector
	{
		public static void Draw(SerializedProperty property, Dictionary<string, UnityEditor.Editor> editorsCache)
		{
			EditorGUILayout.BeginVertical("HelpBox");
			GUILayout.Space(1.5f);
			
			GUI.contentColor = EditorColors.PrimaryOrange;
			EditorGUILayout.LabelField("Inspector", EditorStyles.boldLabel);
			GUI.contentColor = Color.white;
            
			UtilEditor.DrawHorizontalGUILine();
			GUILayout.Space(2.5f);
            
			if (editorsCache.ContainsKey(property.Fpr("key").stringValue))
			{
				editorsCache[property.Fpr("key").stringValue].OnInspectorGUI();
			}
			else
			{
				editorsCache.Add(property.Fpr("key").stringValue, UnityEditor.Editor.CreateEditor(property.Fpr("value").Fpr("mixerGroup").objectReferenceValue));
				editorsCache[property.Fpr("key").stringValue].OnInspectorGUI();
			}
			
			GUILayout.Space(1.5f);
			EditorGUILayout.EndVertical();
		}
	}
}