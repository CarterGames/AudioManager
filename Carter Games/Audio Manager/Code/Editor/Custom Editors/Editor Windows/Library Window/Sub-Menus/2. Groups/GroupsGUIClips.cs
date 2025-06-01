using System.Collections.Generic;
using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
	public static class GroupsGUIClips
	{
		public static void Draw(SerializedProperty data)
		{
			EditorGUILayout.BeginVertical("HelpBox");
            
            if (data.Fpr("clipNames").arraySize.Equals(0))
            {
                EditorGUILayout.HelpBox("No clips defined in this group, add some below.", MessageType.Info);
            }
            else
            {
                GUI.contentColor = UtilEditor.Yellow;
                EditorGUILayout.LabelField("Clips", EditorStyles.boldLabel);
                GUI.contentColor = Color.white;
                
                UtilEditor.DrawHorizontalGUILine();

                for (var i = 0; i < data.Fpr("clipNames").arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();


                    if (data.Fpr("clipNames").GetIndex(i).stringValue.Length <= 0)
                    {
                        DrawGroupSelectClipButton(data.Fpr("clipNames"));
                    }
                    else
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.TextField(ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.LibraryLookup[data.Fpr("clipNames").GetIndex(i).stringValue].key);
                        EditorGUI.EndDisabledGroup();

                        if (GUILayout.Button("Edit Clip", GUILayout.Width(80)))
                        {
                            var clipsToIgnore = new List<string>();

                            for (var j = 0; j < data.Fpr("clipNames").arraySize; j++)
                            {
                                if (string.IsNullOrEmpty(data.Fpr("clipNames").GetIndex(j).stringValue)) continue;
                                clipsToIgnore.Add(data.Fpr("clipNames").GetIndex(j).stringValue);
                            }

                            LibraryEditorGroupsTab.ClipEditIndex = i;
                            LibraryEditorGroupsTab.IsEditingClip = true;
                            
                            SearchProviderInstancing.SearchProviderLibrary.Open(clipsToIgnore);
                        }
                    }

                    GUI.backgroundColor = UtilEditor.Red;

                    if (GUILayout.Button("-", GUILayout.Width(20f)))
                    {
                        data.Fpr("clipNames").DeleteIndex(i);
                        data.serializedObject.ApplyModifiedProperties();
                        data.serializedObject.Update();
                    }

                    GUI.backgroundColor = Color.white;

                    EditorGUILayout.EndHorizontal();
                }
            }
            
            EditorGUILayout.Space(1.5f);
            
            GUI.backgroundColor = EditorColors.PrimaryGreen;
            if (GUILayout.Button("+ Add Clip To Group", GUILayout.Height(22.5f)))
            {
                var clipsToIgnore = new List<string>();
        
                for (var j = 0; j < data.Fpr("clipNames").arraySize; j++)
                {
                    if (string.IsNullOrEmpty(data.Fpr("clipNames").GetIndex(j).stringValue)) continue;
                    clipsToIgnore.Add(data.Fpr("clipNames").GetIndex(j).stringValue);
                }
        
                SearchProviderInstancing.SearchProviderLibrary.Open(clipsToIgnore);
                data.serializedObject.ApplyModifiedProperties();
                data.serializedObject.Update();
            }
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
		}
        
        
        
        /// <summary>
        /// Draws the select clip button for group clips.
        /// </summary>
        /// <param name="groupClips">the clips property to edit.</param>
        private static void DrawGroupSelectClipButton(SerializedProperty groupClips)
        {
            if (GUILayout.Button("Select Clip"))
            {
                var entriesToIgnore = new List<string>();
                
                for (var j = 0; j < groupClips.arraySize; j++)
                {
                    entriesToIgnore.Add(groupClips.GetIndex(j).stringValue);
                }
        
                SearchProviderInstancing.SearchProviderLibrary.Open(entriesToIgnore);
            }
        }
	}
}