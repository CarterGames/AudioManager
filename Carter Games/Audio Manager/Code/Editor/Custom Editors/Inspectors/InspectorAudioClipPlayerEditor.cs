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

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the custom editor for the inspector audio clip player.
    /// </summary>
    [CustomEditor(typeof(InspectorAudioClipPlayer))]
    public sealed class InspectorAudioClipPlayerEditor : UnityEditor.Editor
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static EditModuleSearchProvider moduleSearchProvider;
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
                if (EditModuleInspectors.Inspectors.ContainsKey(EditModuleLookupProp.GetIndex(i).Fpr("key").stringValue))
                {
                    EditModuleInspectors.Inspectors[EditModuleLookupProp.GetIndex(i).Fpr("key").stringValue]
                        .DrawInspector(EditModuleLookupProp.serializedObject, i);
                }
            }
            
            if (GUILayout.Button("Add Edit Module", GUILayout.Height(25)))
            {
                moduleSearchProvider ??= CreateInstance<EditModuleSearchProvider>();
                        
                EditModuleSearchProvider.ToExclude.Clear();
                        
                for (var j = 0; j < serializedObject.Fp("editModuleSettings").Fpr("list").arraySize; j++)
                {
                    EditModuleSearchProvider.ToExclude.Add(serializedObject.Fp("editModuleSettings").Fpr("list").GetIndex(j).Fpr("key").stringValue.Replace("CarterGames.Assets.AudioManager.", string.Empty).Replace("Edit", string.Empty));
                }
                
                EditModuleSearchProvider.OnSearchTreeSelectionMade.Clear();
                EditModuleSearchProvider.OnSearchTreeSelectionMade.AddAnonymous("editModuleSelect", SelectClip);
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), moduleSearchProvider);
            }
            
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
            
            if (!PerUserSettings.DeveloperDebugMode) return;
            
            EditorGUILayout.LabelField("DEVELOPER DEBUG", EditorStyles.boldLabel);
            UtilEditor.DrawHorizontalGUILine();
            
            base.OnInspectorGUI();
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private void SelectClip(SearchTreeEntry treeEntry)
        {
            LibrarySearchProvider.OnSearchTreeSelectionMade.RemoveAnonymous("editModuleSelect");

            var total = 0;
            
            for (var i = 0; i < EditModuleLookupProp.arraySize; i++)
            {
                if (EditModuleLookupProp.GetIndex(i).Fpr("key").stringValue != treeEntry.userData.ToString()) continue;
                total++;
            }

            if (total > 0) return;
            
            EditModuleLookupProp.InsertIndex(EditModuleLookupProp.arraySize);
            EditModuleLookupProp.GetIndex(EditModuleLookupProp.arraySize - 1).Fpr("key").stringValue = treeEntry.userData.ToString();
            EditModuleLookupProp.GetIndex(EditModuleLookupProp.arraySize - 1).Fpr("value").Fpr("list").ClearArray();
                
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
    }
}