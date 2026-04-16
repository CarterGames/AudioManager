using System.Linq;
using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
	public static class GroupsGUIGroupInfo
	{
		private static readonly GUIContent GroupName = new GUIContent("Group Name:", "The name to refer to this group as, it cannot match another group name.");
		
		
		public static void Draw(SerializedProperty data)
		{
			EditorGUILayout.BeginVertical("HelpBox");

			EditorGUILayout.BeginHorizontal();
			GUI.contentColor = EditorColors.PrimaryYellow;
			EditorGUILayout.LabelField("Group", EditorStyles.boldLabel);
			GUI.contentColor = Color.white;
			
			GroupsGUIDangerZone.Draw(data);
			EditorGUILayout.EndHorizontal();
            
			UtilEditor.DrawHorizontalGUILine();
            
			DrawGroupNameField(data.Fpr("value").Fpr("groupName"));
			EditorGUILayout.PropertyField(data.Fpr("value").Fpr("groupPlayMode"));

			EditorGUILayout.Space(1.5f);
			EditorGUILayout.EndVertical();
		}
		
		
		/// <summary>
		/// Draws the group name field.
		/// </summary>
		/// <param name="groupName">The property to edit.</param>
		private static void DrawGroupNameField(SerializedProperty groupName)
		{
			EditorGUILayout.BeginHorizontal();

			EditorGUI.BeginChangeCheck();
			
			EditorGUILayout.PropertyField(groupName, GroupName);
			
			if (EditorGUI.EndChangeCheck())
			{
				ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.ApplyModifiedProperties();
				ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.Update();
			}
			
			if (GUILayout.Button("Copy Key", GUILayout.Width(80)))
			{
				Clipboard.Copy(groupName.stringValue);
				EditorUtility.DisplayDialog("Copy Group Key", "Key copied to clipboard", "Continue");
			}
			
			EditorGUILayout.EndHorizontal();
            
            
			if (!NameValid(groupName.stringValue))
			{
				EditorGUILayout.HelpBox(
					"Group name invalid, please ensure the group name is not empty & does not match an existing group name.",
					MessageType.Warning);
			}
		}
		
		
		/// <summary>
		/// Checks to see if the name if valid.
		/// </summary>
		/// <param name="name">The name entered.</param>
		/// <returns>If its valid or not.</returns>
		public static bool NameValid(string name)
		{
			if (name.Length <= 0)
			{
				return false;
			}
            
			return ScriptableRef
				.GetAssetDef<AudioLibrary>().AssetRef.GroupsLookup
				.Count(t => t.Value.GroupName.Equals(name))
				.Equals(1);
		}
	}
}