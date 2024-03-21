/*
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

using System;
using System.Collections.Generic;
using System.Linq;
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

        // GUI Content Meta Data
        /* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        private static readonly GUIContent GroupName = new GUIContent("Group Name:", "The name to refer to this group as, it cannot match another group name.");
        
        
        // General Fields
        /* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        private static Vector2 scrollRect;
        private static Rect deselectZone;
        private static readonly LibrarySearchProvider SearchProvider = ScriptableObject.CreateInstance<LibrarySearchProvider>();

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static SerializedProperty SelectedProperty { get; set; }
        private static bool IsEditingClip { get; set; }
        private static int ClipEditIndex { get; set; }

        private static SerializedProperty GroupsDictionary => UtilEditor.LibraryObject.Fp("groups").Fpr("list");
        private static SerializedProperty GroupsReverseDictionary => UtilEditor.LibraryObject.Fp("groupsReverseLookup").Fpr("list");
        private SerializedProperty GroupsDictionaryLastElement => GroupsDictionary.GetIndex(GroupsDictionary.arraySize - 1);

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   ILibraryTab Implementation
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Initializes the tab when called.
        /// </summary>
        public void Initialize()
        {
            if (UtilEditor.Library.GroupsLookup.Count > 0 && PerUserSettings.LastLibraryGroupEntry >= 0)
            {
                SelectedProperty = GroupsDictionary.GetIndex(PerUserSettings.LastLibraryGroupEntry);
            }
            
            LibrarySearchProvider.OnSearchTreeSelectionMade.Add(OnSelectionMade);
        }
        
        
        /// <summary>
        /// Displays the tab when called.
        /// </summary>
        public void Display()
        {
            if (UtilEditor.LibraryObject.Fp("groups").Fpr("list").arraySize <= 0)
            {
                EditorGUILayout.HelpBox("No groups in the library so there is nothing to show.", MessageType.Info);
                
                EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);
                GUI.backgroundColor = UtilEditor.Green;
                
                if (GUILayout.Button("Make Group", GUILayout.Height(25)))
                {
                    GroupsDictionary.InsertIndex(GroupsDictionary.arraySize);
                    GroupsDictionaryLastElement.Fpr("key").stringValue = Guid.NewGuid().ToString();

                    var newGroupData = GroupDataLookup(GroupsDictionaryLastElement.Fpr("value"));
                
                    newGroupData["groupName"].stringValue = "New Group " + GroupsDictionaryLastElement.Fpr("key").stringValue.Substring(0, 8);
                    newGroupData["groupPlayMode"].intValue = 0;
                    newGroupData["clipNames"].arraySize = 0;

                    GroupsReverseDictionary.InsertIndex(GroupsReverseDictionary.arraySize);
                    GroupsReverseDictionary.GetIndex(GroupsReverseDictionary.arraySize - 1).Fpr("key").stringValue = newGroupData["groupName"].stringValue;
                    GroupsReverseDictionary.GetIndex(GroupsReverseDictionary.arraySize - 1).Fpr("value").stringValue = GroupsDictionaryLastElement.Fpr("key").stringValue;
                
                    UtilEditor.LibraryObject.ApplyModifiedProperties();
                    UtilEditor.LibraryObject.Update();
                
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
            
            GUI.backgroundColor = UtilEditor.Yellow;
            if (GUILayout.Button("Update Groups Struct", GUILayout.MaxHeight(25)))
            {
                StructHandler.RefreshGroups();
            }
            GUI.backgroundColor = Color.white;
            
            if (PerUserSettings.ShowHelpBoxes)
            {
                EditorGUILayout.HelpBox("Press ^^^ to update the Group.??? struct to have the latest changes",
                    MessageType.None);
            }
            
            GUILayout.Space(7.5f);
            UtilEditor.DrawHorizontalGUILine();
            GUILayout.Space(7.5f);
            
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
            var groupKeys = UtilEditor.Library.GroupsLookup.Keys.ToArray();
            
            GUI.backgroundColor = UtilEditor.Green;
            
            if (GUILayout.Button("+ Add New Group"))
            {
                GroupsDictionary.InsertIndex(GroupsDictionary.arraySize);
                GroupsDictionaryLastElement.Fpr("key").stringValue = Guid.NewGuid().ToString();

                var newGroupData = GroupDataLookup(GroupsDictionaryLastElement.Fpr("value"));
                
                newGroupData["groupName"].stringValue = "New Group " + GroupsDictionaryLastElement.Fpr("key").stringValue.Substring(0, 8);
                newGroupData["groupPlayMode"].intValue = 0;
                newGroupData["clipNames"].arraySize = 0;

                GroupsReverseDictionary.InsertIndex(GroupsReverseDictionary.arraySize);
                GroupsReverseDictionary.GetIndex(GroupsReverseDictionary.arraySize - 1).Fpr("key").stringValue = newGroupData["groupName"].stringValue;
                GroupsReverseDictionary.GetIndex(GroupsReverseDictionary.arraySize - 1).Fpr("value").stringValue = GroupsDictionaryLastElement.Fpr("key").stringValue;
                
                UtilEditor.LibraryObject.ApplyModifiedProperties();
                UtilEditor.LibraryObject.Update();
                
                SelectedProperty = GroupsDictionaryLastElement;
            }
            
            GUI.backgroundColor = Color.white;
            
            if (groupKeys.Length > 0)
            {
                GUILayout.Space(5f);
                UtilEditor.DrawHorizontalGUILine();
                GUILayout.Space(5f);
            }


            
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

                if (UtilEditor.Library.GroupsLookup[key].Clips.Count > 1)
                {
                    suffix = "Clips";
                }
                else
                {
                    if (UtilEditor.Library.GroupsLookup[key].Clips.Count.Equals(1))
                    {
                        suffix = "Clip";
                    }
                    else
                    {
                        suffix = "Clips";
                    }
                }
                
                if (GUILayout.Button($"{UtilEditor.Library.GroupsLookup[key].GroupName} ({UtilEditor.Library.GroupsLookup[key].Clips.Count} {suffix})"))
                {
                    PerUserSettings.LastLibraryGroupEntry = UtilEditor.Library.GroupsLookup.Keys.ToList().IndexOf(key);
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
        /// Draws the remove group button on call.
        /// </summary>
        /// <param name="data">The property to use.</param>
        private static void DrawGroupRemoveButton(SerializedProperty data)
        {
            EditorGUILayout.BeginVertical();

            GUILayout.Space(2.5f);
            
            GUI.contentColor = UtilEditor.Yellow;
            EditorGUILayout.LabelField("Danger Zone", EditorStyles.boldLabel);
            GUI.contentColor = Color.white;
            
            UtilEditor.DrawHorizontalGUILine();
            
            GUILayout.Space(1.5f);
            
            GUI.backgroundColor = UtilEditor.Red;
            
            if (GUILayout.Button("Clear Group"))
            {
                if (EditorUtility.DisplayDialog("Clear Clip Group",
                        $"Are you sure you want to clear all clips from the group '{data.Fpr("value").Fpr("groupName").stringValue}'",
                        "Clear", "Cancel"))
                {
                    GroupsDictionary.GetIndex(UtilEditor.Library.GroupsLookup.Keys.ToList().IndexOf(data.Fpr("key").stringValue)).Fpr("value").Fpr("clipNames").ClearArray();
                    GroupsDictionary.serializedObject.ApplyModifiedProperties();
                    GroupsDictionary.serializedObject.Update();
                    
                    Undo.RecordObject(GroupsDictionary.serializedObject.targetObject, "Clear Group");
                    return;
                }
            }
            
            
            if (GUILayout.Button("Delete Group"))
            {
                if (EditorUtility.DisplayDialog("Remove Clip Group",
                        $"Are you sure you want to remove the group '{data.Fpr("value").Fpr("groupName").stringValue}'",
                        "Remove", "Cancel"))
                {
                    GroupsDictionary.DeleteIndex(UtilEditor.Library.GroupsLookup.Keys.ToList().IndexOf(data.Fpr("key").stringValue));
                    GroupsDictionary.serializedObject.ApplyModifiedProperties();
                    GroupsDictionary.serializedObject.Update();
                    
                    Undo.RecordObject(GroupsDictionary.serializedObject.targetObject, "Removed Group");
                    
                    SelectedProperty = null;
                    return;
                }
            }
            
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Draws the group name field.
        /// </summary>
        /// <param name="groupName">The property to edit.</param>
        private static void DrawGroupNameField(SerializedProperty groupName, SerializedProperty uuidProp)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(groupName, GroupName);
            if (EditorGUI.EndChangeCheck())
            {
                var oldIndexReverse = -1;
                
                for (var i = 0; i < UtilEditor.LibraryObject.Fp("groupsReverseLookup").Fpr("list").arraySize; i++)
                {
                    if (UtilEditor.LibraryObject.Fp("groupsReverseLookup").Fpr("list").GetIndex(i).Fpr("value").stringValue == uuidProp.stringValue)
                    {
                        oldIndexReverse = i;
                    }
                }
                
                if (oldIndexReverse > -1)
                {
                    UtilEditor.LibraryObject.Fp("groupsReverseLookup").Fpr("list").DeleteIndex(oldIndexReverse);
                }
                
                UtilEditor.LibraryObject.Fp("groupsReverseLookup").Fpr("list").InsertIndex(UtilEditor.LibraryObject.Fp("groupsReverseLookup").Fpr("list").arraySize);
                UtilEditor.LibraryObject.Fp("groupsReverseLookup").Fpr("list").GetIndex(UtilEditor.LibraryObject.Fp("groupsReverseLookup").Fpr("list").arraySize - 1).Fpr("key").stringValue = groupName.stringValue;
                UtilEditor.LibraryObject.Fp("groupsReverseLookup").Fpr("list").GetIndex(UtilEditor.LibraryObject.Fp("groupsReverseLookup").Fpr("list").arraySize - 1).Fpr("value").stringValue = uuidProp.stringValue;
                
                GroupsDictionary.serializedObject.ApplyModifiedProperties();
                GroupsDictionary.serializedObject.Update();
                
                Undo.RecordObject(GroupsDictionary.serializedObject.targetObject, "Group Rename");
            }

            GUI.backgroundColor = UtilEditor.Yellow;

            if (GUILayout.Button("Copy Key", GUILayout.Width(80)))
            {
                Clipboard.Copy(groupName.stringValue);
                EditorUtility.DisplayDialog("Copy Group Key", "Key copied to clipboard", "Continue");
            }

            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();
            
            
            if (!NameValid(groupName.stringValue))
            {
                EditorGUILayout.HelpBox(
                    "Group name invalid, please ensure the group name is not empty & does not match an existing group name.",
                    MessageType.Warning);
            }

        }


        /// <summary>
        /// Draws the select clip button for group clips.
        /// </summary>
        /// <param name="groupClips">the clips property to edit.</param>
        private static void DrawGroupSelectClipButton(SerializedProperty groupClips)
        {
            if (GUILayout.Button("Select Clip"))
            {
                // SelectedProperty = groupClips.GetIndex(index);
                LibrarySearchProvider.ToExclude.Clear();
        
                for (var j = 0; j < groupClips.arraySize; j++)
                {
                    LibrarySearchProvider.ToExclude.Add(groupClips.GetIndex(j).stringValue);
                }
        
                SearchWindow.Open(
                    new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                    SearchProvider);
            }
        }


        /// <summary>
        /// Draws the group when called.
        /// </summary>
        /// <param name="data">The property to edit.</param>
        private static void DrawGroup(SerializedProperty data)
        {
            if (data == null) return;
         
            GroupsDictionary.serializedObject.Update();

            var groupData = GroupDataLookup(data.Fpr("value"));
            
            var isValid = NameValid(groupData["groupName"].stringValue);

            var clipsSuffixLabel = groupData["clipNames"].arraySize > 1 ? "Clips" : "Clip";
            var groupLabel = $" {groupData["groupName"].stringValue} ({groupData["clipNames"].arraySize} {clipsSuffixLabel})";

            var content = isValid
                ? new GUIContent(groupLabel)
                : new GUIContent(groupLabel, UtilEditor.WarningIcon,
                    "The group name is invalid, either it is blank or matches another existing group name. Please correct this to remove this warning!");
            
            
            EditorGUILayout.BeginVertical("HelpBox");
            
            GUILayout.Space(2.5f);
            
            EditorGUILayout.BeginVertical();

            GUI.contentColor = UtilEditor.Yellow;
            EditorGUILayout.LabelField("Meta Data", EditorStyles.boldLabel);
            GUI.contentColor = Color.white;
            
            UtilEditor.DrawHorizontalGUILine();
            
            DrawGroupNameField(groupData["groupName"], SelectedProperty.Fpr("key"));
            EditorGUILayout.PropertyField(groupData["groupPlayMode"]);

            EditorGUILayout.EndVertical();

            
            GUILayout.Space(15f);
            
            
            EditorGUILayout.BeginVertical("HelpBox");
            
            if (groupData["clipNames"].arraySize.Equals(0))
            {
                EditorGUILayout.HelpBox("No clips defined in this group, add some below.", MessageType.Info);
            }
            else
            {
                GUI.contentColor = UtilEditor.Yellow;
                EditorGUILayout.LabelField("Clips", EditorStyles.boldLabel);
                GUI.contentColor = Color.white;
                
                UtilEditor.DrawHorizontalGUILine();

                for (var i = 0; i < groupData["clipNames"].arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();


                    if (groupData["clipNames"].GetIndex(i).stringValue.Length <= 0)
                    {
                        DrawGroupSelectClipButton(groupData["clipNames"]);
                    }
                    else
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.TextField(UtilEditor.Library.LibraryLookup[groupData["clipNames"].GetIndex(i).stringValue].key);
                        EditorGUI.EndDisabledGroup();

                        if (GUILayout.Button("Edit Clip", GUILayout.Width(80)))
                        {
                            LibrarySearchProvider.ToExclude.Clear();

                            for (var j = 0; j < groupData["clipNames"].arraySize; j++)
                            {
                                if (string.IsNullOrEmpty(groupData["clipNames"].GetIndex(j).stringValue)) continue;
                                LibrarySearchProvider.ToExclude.Add(groupData["clipNames"].GetIndex(j).stringValue);
                            }

                            ClipEditIndex = i;
                            IsEditingClip = true;
                            
                            SearchWindow.Open(
                                new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                                SearchProvider);
                        }
                    }

                    GUI.backgroundColor = UtilEditor.Red;

                    if (GUILayout.Button("-", GUILayout.Width(20f)))
                    {
                        groupData["clipNames"].DeleteIndex(i);
                        Undo.RecordObject(GroupsDictionary.serializedObject.targetObject, "CGAM:Remove Clip From Group");
                    }

                    GUI.backgroundColor = Color.white;

                    EditorGUILayout.EndHorizontal();
                }
            }
            
            GUI.backgroundColor = UtilEditor.Green;
            
            if (GUILayout.Button("Add New Clip"))
            {
                groupData["clipNames"].InsertIndex(groupData["clipNames"].arraySize);
                groupData["clipNames"].GetIndex(groupData["clipNames"].arraySize - 1).stringValue = string.Empty;
                
                // SelectedProperty = groupClips.GetIndex(index);
                LibrarySearchProvider.ToExclude.Clear();
        
                for (var j = 0; j < groupData["clipNames"].arraySize; j++)
                {
                    if (string.IsNullOrEmpty(groupData["clipNames"].GetIndex(j).stringValue)) continue;
                    LibrarySearchProvider.ToExclude.Add(groupData["clipNames"].GetIndex(j).stringValue);
                }
        
                SearchWindow.Open(
                    new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                    SearchProvider);
                
                Undo.RecordObject(GroupsDictionary.serializedObject.targetObject, "Group Clip Change");
            }
            
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(15f);
            
            EditorGUILayout.BeginVertical("HelpBox");
            
            DrawGroupRemoveButton(data);
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
            
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
        /// Checks to see if the name if valid.
        /// </summary>
        /// <param name="name">The name entered.</param>
        /// <returns>If its valid or not.</returns>
        private static bool NameValid(string name)
        {
            if (name.Length <= 0)
            {
                return false;
            }
            
            return UtilEditor.Library.GroupsLookup.Count(t => t.Value.GroupName.Equals(name)).Equals(1);
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
    }
}