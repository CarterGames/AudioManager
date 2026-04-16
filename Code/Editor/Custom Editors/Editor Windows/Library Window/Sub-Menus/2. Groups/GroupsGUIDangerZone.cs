using System.Linq;
using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
	public static class GroupsGUIDangerZone
	{
		public static void Draw(SerializedProperty data)
		{
            GUI.backgroundColor = EditorColors.PrimaryRed;
            
            if (GUILayout.Button("Clear Group", GUILayout.Height(22.5f), GUILayout.Width(100)))
            {
                if (EditorUtility.DisplayDialog("Clear Clip Group",
                        $"Are you sure you want to clear all clips from the group '{data.Fpr("value").Fpr("groupName").stringValue}'",
                        "Clear", "Cancel"))
                {
	                var groupDic = ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.Fp("groups").Fpr("list");
	                
	                groupDic.GetIndex(ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.GroupsLookup.Keys.ToList().IndexOf(data.Fpr("key").stringValue)).Fpr("value").Fpr("clipNames").ClearArray();
	                groupDic.serializedObject.ApplyModifiedProperties();
	                groupDic.serializedObject.Update();
                    return;
                }
            }
            
            
            if (GUILayout.Button("Delete Group", GUILayout.Height(22.5f), GUILayout.Width(100)))
            {
                if (EditorUtility.DisplayDialog("Remove Clip Group",
                        $"Are you sure you want to remove the group '{data.Fpr("value").Fpr("groupName").stringValue}'",
                        "Remove", "Cancel"))
                {
	                var groupDic = ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.Fp("groups").Fpr("list");
	                
	                groupDic.DeleteIndex(ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.GroupsLookup.Keys.ToList().IndexOf(data.Fpr("key").stringValue));
	                groupDic.serializedObject.ApplyModifiedProperties();
	                groupDic.serializedObject.Update();
                    
                    LibraryEditorGroupsTab.ResetSelectedProperty();
                    return;
                }
            }
            
            GUI.backgroundColor = Color.white;
		}
	}
}