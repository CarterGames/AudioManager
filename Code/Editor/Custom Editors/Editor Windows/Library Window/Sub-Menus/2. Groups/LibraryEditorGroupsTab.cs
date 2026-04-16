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

using System;
using System.Collections.Generic;
using System.Linq;
using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the groups tab logic for the audio library editor window.
    /// </summary>
    public sealed class LibraryEditorGroupsTab : DividedDisplayBase, ILibraryTab
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        // General Fields
        /* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        private static Vector2 scrollRect;
        private static Rect deselectZone;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private static AudioLibrary LibAsset => ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef;
        private static SerializedObject LibObj => ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef;
        private static SerializedProperty SelectedProperty { get; set; }
        public static bool IsEditingClip { get; set; }
        public static int ClipEditIndex { get; set; }
        

        private static SerializedProperty GroupsDictionary => ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.Fp("groups").Fpr("list");
        private SerializedProperty GroupsDictionaryLastElement => GroupsDictionary.GetIndex(GroupsDictionary.arraySize - 1);

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   ILibraryTab Implementation
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Initializes the tab when called.
        /// </summary>
        public void Initialize()
        {
            if (LibAsset.GroupsLookup.Count > 0 && PerUserSettings.LastLibraryGroupEntry >= 0)
            {
                SelectedProperty = GroupsDictionary.GetIndex(PerUserSettings.LastLibraryGroupEntry);
            }
            
            SearchProviderInstancing.SearchProviderLibrary.SelectionMade.Add(OnSelectionMade);
        }
        
        
        /// <summary>
        /// Displays the tab when called.
        /// </summary>
        public void Display()
        {
            if (LibObj.Fp("groups").Fpr("list").arraySize <= 0)
            {
                EditorGUILayout.HelpBox("No groups in the library so there is nothing to show.", MessageType.Info);
                
                EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);
                GUI.backgroundColor = EditorColors.PrimaryGreen;
                
                if (GUILayout.Button("+ Make Group", GUILayout.Height(25)))
                {
                    GroupsDictionary.InsertIndex(GroupsDictionary.arraySize);
                    GroupsDictionaryLastElement.Fpr("key").stringValue = Guid.NewGuid().ToString();

                    var newGroupData = GroupDataLookup(GroupsDictionaryLastElement.Fpr("value"));
                
                    newGroupData["groupName"].stringValue = "New Group " + GroupsDictionaryLastElement.Fpr("key").stringValue.Substring(0, 8);
                    newGroupData["groupPlayMode"].intValue = 0;
                    newGroupData["clipNames"].arraySize = 0;

                    LibObj.ApplyModifiedProperties();
                    LibObj.Update();
                
                    SelectedProperty = GroupsDictionaryLastElement;
                }

                GUI.backgroundColor = Color.white;
                EditorGUI.EndDisabledGroup();
                
                return;
            }
            
            DisplaySections();
            UtilEditor.CreateDeselectZone(ref deselectZone);
        }


        /// <summary>
        /// Changes the left control GUI to something else.
        /// </summary>
        protected override void LeftSectionControl()
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.MaxWidth(250));
            GUILayout.Space(5f);
            
            GUI.backgroundColor = EditorColors.PrimaryYellow;
            if (GUILayout.Button("Update Groups Struct", GUILayout.MaxHeight(25)))
            {
                if (EditorUtility.DisplayDialog("Update Groups Struct",
                        "Are you sure you want to update the groups struct?", "Update Groups Struct", "Cancel"))
                {
                    StructHandler.RefreshGroups();
                }
            }
            GUI.backgroundColor = Color.white;
            
            if (PerUserSettings.ShowHelpBoxes)
            {
                EditorGUILayout.HelpBox("Press ^^^ to update the Group.??? struct to have the latest changes",
                    MessageType.None);
            }
            
            GUILayout.Space(1.5f);
            UtilEditor.DrawHorizontalGUILine();
            GUILayout.Space(1.5f);
            
            
            GUI.backgroundColor = EditorColors.PrimaryGreen;
            
            if (GUILayout.Button("+ Add New Group", GUILayout.MaxHeight(25)))
            {
                GroupsDictionary.InsertIndex(GroupsDictionary.arraySize);
                GroupsDictionaryLastElement.Fpr("key").stringValue = Guid.NewGuid().ToString();

                var newGroupData = GroupDataLookup(GroupsDictionaryLastElement.Fpr("value"));
                
                newGroupData["groupName"].stringValue = "New Group " + GroupsDictionaryLastElement.Fpr("key").stringValue.Substring(0, 8);
                newGroupData["groupPlayMode"].intValue = 0;
                newGroupData["clipNames"].arraySize = 0;

                LibObj.ApplyModifiedProperties();
                LibObj.Update();
                
                SelectedProperty = GroupsDictionaryLastElement;
            }
            
            GUI.backgroundColor = Color.white;
            
            GUILayout.Space(5f);
            UtilEditor.DrawHorizontalGUILine();
            GUILayout.Space(2.5f);
            
            
            PerUserSettings.GroupBtnScrollRectPos = EditorGUILayout.BeginScrollView(PerUserSettings.GroupBtnScrollRectPos);
            base.LeftSectionControl();
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Changes the right GUI control to something else.
        /// </summary>
        protected override void RightSectionControl()
        {
            PerUserSettings.GroupScrollRectPos = EditorGUILayout.BeginScrollView(PerUserSettings.GroupScrollRectPos);
            EditorGUILayout.BeginVertical("Box");
            base.RightSectionControl();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }


        /// <summary>
        /// Adds the implemented for the left GUI.
        /// </summary>
        protected override void OnLeftGUI()
        {
            var groupKeys = LibAsset.GroupsLookup.Keys.ToArray();

            foreach (var key in groupKeys)
            {
                if (key.Equals(string.Empty)) continue;
                
                if (SelectedProperty != null)
                {
                    if (SelectedProperty.Fpr("key").stringValue.Equals(key))
                    {
                        GUI.backgroundColor = UtilEditor.Grey;
                    }
                    else
                    {
                        GUI.backgroundColor = Color.white;
                    }
                }

                string suffix;

                if (LibAsset.GroupsLookup[key].Clips.Count > 1)
                {
                    suffix = "Clips";
                }
                else
                {
                    if (LibAsset.GroupsLookup[key].Clips.Count.Equals(1))
                    {
                        suffix = "Clip";
                    }
                    else
                    {
                        suffix = "Clips";
                    }
                }
                
                var toDisplay = new GUIContent(LibAsset.GroupsLookup[key].GroupName);

                if (toDisplay.text.Length > 25)
                {
                    toDisplay.text = toDisplay.text.Substring(0, 22) + "...";
                }
                
                
                if (GUILayout.Button($"{toDisplay.text} ({LibAsset.GroupsLookup[key].Clips.Count} {suffix})"))
                {
                    PerUserSettings.LastLibraryGroupEntry = LibAsset.GroupsLookup.Keys.ToList().IndexOf(key);
                    SelectedProperty = GroupsDictionary.GetIndex(PerUserSettings.LastLibraryGroupEntry);
                    GUI.FocusControl(null);
                }
                
                GUI.backgroundColor = Color.white;
            }
        }


        /// <summary>
        /// Adds the logic for the right side GUI.
        /// </summary>
        protected override void OnRightGUI()
        {
            if (SelectedProperty == null)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Select a group to see its contents here.");
                EditorGUILayout.EndVertical();
                return;
            }
            
            DrawGroup(SelectedProperty);
        }


        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Drawer Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Draws the group when called.
        /// </summary>
        /// <param name="data">The property to edit.</param>
        private static void DrawGroup(SerializedProperty data)
        {
            if (data == null) return;
         
            GroupsDictionary.serializedObject.Update();

            var groupData = GroupDataLookup(data.Fpr("value"));
            
            var isValid = GroupsGUIGroupInfo.NameValid(data.Fpr("value").Fpr("groupName").stringValue);

            var clipsSuffixLabel = groupData["clipNames"].arraySize > 1 ? "Clips" : "Clip";
            var groupLabel = $" {groupData["groupName"].stringValue} ({groupData["clipNames"].arraySize} {clipsSuffixLabel})";

            var content = isValid
                ? new GUIContent(groupLabel)
                : new GUIContent(groupLabel, UtilEditor.WarningIcon,
                    "The group name is invalid, either it is blank or matches another existing group name. Please correct this to remove this warning!");
            
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.Space(2.5f);
            
            GroupsGUIGroupInfo.Draw(data);
            
            EditorGUILayout.Space(5f);
            
            GroupsGUIClips.Draw(data.Fpr("value"));
            
            EditorGUILayout.Space(5f);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            
            GroupsDictionary.serializedObject.ApplyModifiedProperties();
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Helper Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Runs when a clip selection is made.
        /// </summary>
        /// <param name="searchTreeEntry">The selected entry.</param>
        private void OnSelectionMade(SearchTreeEntry searchTreeEntry)
        {
            if (!LibraryEditorWindow.ShownTab.Equals(this)) return;
            
            var baseClipProperty = SelectedProperty.Fpr("value").Fpr("clipNames");

            if (!IsEditingClip)
            {
                baseClipProperty.InsertIndex(baseClipProperty.arraySize);
                baseClipProperty.GetIndex(baseClipProperty.arraySize - 1).stringValue = string.Empty;
            }
            
            var propertyToEdit = IsEditingClip 
                ? baseClipProperty.GetIndex(ClipEditIndex) 
                : baseClipProperty.GetIndex(baseClipProperty.arraySize - 1);
            
            
            propertyToEdit.stringValue = ((AudioData)searchTreeEntry.userData).id;
            
            IsEditingClip = false;
            
            GroupsDictionary.serializedObject.ApplyModifiedProperties();
            GroupsDictionary.serializedObject.Update();
            GUI.FocusControl(null);
        }

        
        /// <summary>
        /// A lookup of properties for the editor.
        /// </summary>
        /// <param name="keyPairProperty">The property to use as a base.</param>
        /// <returns>The lookup for the properties.</returns>
        private static Dictionary<string, SerializedProperty> GroupDataLookup(SerializedProperty keyPairProperty)
        {
            return new Dictionary<string, SerializedProperty>()
            {
                { "groupName", keyPairProperty.Fpr("groupName") },
                { "groupPlayMode", keyPairProperty.Fpr("groupPlayMode") },
                { "clipNames", keyPairProperty.Fpr("clipNames") }
            };
        }


        public static void ResetSelectedProperty()
        {
            SelectedProperty = null;
        }
    }
}