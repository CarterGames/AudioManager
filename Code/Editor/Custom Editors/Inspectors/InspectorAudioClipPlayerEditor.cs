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
using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the custom editor for the inspector audio clip player.
    /// </summary>
    [CustomEditor(typeof(InspectorAudioClipPlayer), true)]
    public sealed class InspectorAudioClipPlayerEditor : UnityEditor.Editor
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private SerializedProperty editModuleLookupProp;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private SerializedProperty EditModuleLookupProp
        {
            get
            {
                if (editModuleLookupProp != null) return editModuleLookupProp;
                editModuleLookupProp ??= serializedObject.Fp("editModuleSettings").Fpr("list");
                return editModuleLookupProp;
            }
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public override void OnInspectorGUI()
        {
            GUILayout.Space(5f);
            
            UtilEditor.DrawMonoScriptSection((InspectorAudioClipPlayer) target);
            
            GUILayout.Space(7.5f);
            
            EditorGUILayout.LabelField("Request", EditorStyles.boldLabel);
            UtilEditor.DrawHorizontalGUILine();
            
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            RequestInspector.DrawInspector(serializedObject);
            EditorGUI.EndDisabledGroup();
            
            GUILayout.Space(15f);
            
            EditorGUILayout.LabelField("Edits", EditorStyles.boldLabel);
            UtilEditor.DrawHorizontalGUILine();
            
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            
            for (var i = 0; i < EditModuleLookupProp.arraySize; i++)
            {
                var type = Type.GetType(EditModuleLookupProp.GetIndex(i).Fpr("key").stringValue);
                
                if (type == null) continue;
                
                if (EditModuleInspectors.Inspectors.ContainsKey(type))
                {
                    EditModuleInspectors.Inspectors[type].DrawInspector(EditModuleLookupProp.serializedObject, i);
                }
            }

            GUILayout.Space(2.5f);
            GUI.backgroundColor = EditorColors.PrimaryGreen;
            
            if (GUILayout.Button("+ Add Edit Module", GUILayout.Height(25)))
            {
                var used = new List<string>();
                
                for (var j = 0; j < serializedObject.Fp("editModuleSettings").Fpr("list").arraySize; j++)
                {
                    used.Add(serializedObject.Fp("editModuleSettings").Fpr("list").GetIndex(j).Fpr("key").stringValue);
                }
                
                SearchProviderInstancing.SearchProviderEditModule.SelectionMade.Clear();
                SearchProviderInstancing.SearchProviderEditModule.SelectionMade.AddAnonymous("editModuleSelect", SelectClip);
                
                SearchProviderInstancing.SearchProviderEditModule.Open(used);
            }
            
            GUI.backgroundColor = Color.white;
            
            EditorGUI.EndDisabledGroup();
            
            GUILayout.Space(15f);
            
            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
            UtilEditor.DrawHorizontalGUILine();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel++;
            serializedObject.Fp("showEvents").boolValue =
                EditorGUILayout.Foldout(serializedObject.Fp("showEvents").boolValue, "Show Events");
            EditorGUI.indentLevel--;
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
            
            if (serializedObject.Fp("showEvents").boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.Fp("onStarted"));
                EditorGUILayout.PropertyField(serializedObject.Fp("onLooped"));
                EditorGUILayout.PropertyField(serializedObject.Fp("onCompleted"));
            }
            
            EditorGUI.EndDisabledGroup();

            DrawPropertiesExcluding(serializedObject, "m_Script", "playInstantly", "isGroup", "request", "groupRequest",
                "editModuleSettings", "showEvents", "onStarted", "onLooped", "onCompleted");
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private void SelectClip(SearchTreeEntry treeEntry)
        {
            SearchProviderInstancing.SearchProviderLibrary.SelectionMade.RemoveAnonymous("editModuleSelect");

            var total = 0;
            
            for (var i = 0; i < EditModuleLookupProp.arraySize; i++)
            {
                if (EditModuleLookupProp.GetIndex(i).Fpr("key").stringValue != ((Type) treeEntry.userData).AssemblyQualifiedName) continue;
                total++;
            }

            if (total > 0) return;
            
            EditModuleLookupProp.InsertIndex(EditModuleLookupProp.arraySize);
            EditModuleLookupProp.GetIndex(EditModuleLookupProp.arraySize - 1).Fpr("key").stringValue = ((Type) treeEntry.userData).AssemblyQualifiedName;
            EditModuleLookupProp.GetIndex(EditModuleLookupProp.arraySize - 1).Fpr("value").Fpr("list").ClearArray();
                
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
    }
}