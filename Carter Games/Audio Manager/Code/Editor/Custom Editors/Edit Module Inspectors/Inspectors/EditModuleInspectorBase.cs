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

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// The base class for edit module inspectors.
    /// </summary>
    public abstract class EditModuleInspectorBase
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The properties the edit module has to edit.
        /// </summary>
        protected abstract Dictionary<string, string> EditPropertiesDefaults { get; set; }
        
        
        /// <summary>
        /// Gets if the module editor should call to return out of the inspector logic or not.
        /// </summary>
        protected bool ShouldReturn { get; set; }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Initializes the values for all the fields in the editor.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="index">The index of the module in the object.</param>
        protected void InitializeValues(SerializedObject targetObject, int index)
        {
            foreach (var property in EditPropertiesDefaults)
            {
                if (EditModuleInspectorHelper.GetValue(targetObject, index, property.Key) != null) continue;
                EditModuleInspectorHelper.AddValue(targetObject, index, property.Key, property.Value);
            }
        }

        
        /// <summary>
        /// Draws a drop down for the module editor.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="index">The index of the module in the object.</param>
        /// <param name="dropDownLabel">The label to show on the dropdown.</param>
        protected void DrawDropDown(SerializedObject targetObject, int index, string dropDownLabel)
        {
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.Space(2.5f);
            
            EditModuleInspectorHelper.SetValue(targetObject, index, "enabled",
                EditorGUILayout.Toggle(GUIContent.none, bool.Parse(EditModuleInspectorHelper.GetValue(targetObject, index, "enabled")), GUILayout.Width(25)).ToString());
            
            GUILayout.Space(5f);
            
            EditorGUI.BeginDisabledGroup(!bool.Parse(EditModuleInspectorHelper.GetValue(targetObject, index, "enabled")));
            
            EditModuleInspectorHelper.SetValue(targetObject, index, "showModule", EditorGUILayout.Foldout(bool.Parse(EditModuleInspectorHelper.GetValue(targetObject, index, "showModule")), dropDownLabel).ToString());
            
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginDisabledGroup(index == 0);
            
            if (GUILayout.Button("Up", GUILayout.Width(35)))
            {
                targetObject.Fp("editModuleSettings").Fpr("list").MoveArrayElement(index, index - 1);
                targetObject.ApplyModifiedProperties();
                targetObject.Update();
                ShouldReturn = true;
                return;
            }
            
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(index == targetObject.Fp("editModuleSettings").Fpr("list").arraySize - 1);
            
            if (GUILayout.Button("Dwn", GUILayout.Width(35)))
            {
                targetObject.Fp("editModuleSettings").Fpr("list").MoveArrayElement(index, index + 1);
                targetObject.ApplyModifiedProperties();
                targetObject.Update();
                ShouldReturn = true;
                return;
            }
            
            EditorGUI.EndDisabledGroup();

            GUI.backgroundColor = UtilEditor.Red;
            
            if (GUILayout.Button("-", GUILayout.Width(25)))
            {
                targetObject.Fp("editModuleSettings").Fpr("list").DeleteIndex(index);
                targetObject.ApplyModifiedProperties();
                targetObject.Update();
                ShouldReturn = true;
                return;
            }
            
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginDisabledGroup(!bool.Parse(EditModuleInspectorHelper.GetValue(targetObject, index, "enabled")));
        }
        
        
        /// <summary>
        /// A method to implement logic for the editor for a module.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="index">The index of the module in the object.</param>
        public abstract void DrawInspector(SerializedObject targetObject, int index);
    }
}