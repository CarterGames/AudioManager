/*
 * Copyright (c) 2025 Carter Games
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
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// The base class for edit module inspectors.
    /// </summary>
    public abstract class EditModuleInspectorBase : IEditModuleEditor
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
        
        
        protected SerializedObject TargetObject { get; set; }
        protected int Index { get; set; }
        
        
        /// <summary>
        /// Gets the SerializedProperty for the lookup of an edit module's editor settings.
        /// </summary>
        /// <returns>The SerializedProperty for the lookup.</returns>
        protected SerializedProperty Lookup => TargetObject
            .Fp("editModuleSettings")
            .Fpr("list")
            .GetIndex(Index)
            .Fpr("value")
            .Fpr("list");

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Initializes the values for all the fields in the editor.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="index">The index of the module in the object.</param>
        public void InitializeValues(SerializedObject targetObject, int index)
        {
            TargetObject = targetObject;
            Index = index;
            
            foreach (var property in EditPropertiesDefaults)
            {
                if (GetValue(property.Key) != null) continue;
                AddValue(property.Key, property.Value);
            }
        }

        
        /// <summary>
        /// Draws a drop down for the module editor.
        /// </summary>
        /// <param name="dropDownLabel">The label to show on the dropdown.</param>
        protected void DrawDropDown(string dropDownLabel)
        {
            EditorGUILayout.BeginHorizontal();
            
            GUILayout.Space(2.5f);
            
            SetValue("enabled", EditorGUILayout.Toggle(GUIContent.none, bool.Parse(GetValue("enabled")), GUILayout.Width(25)).ToString());
            
            GUILayout.Space(5f);
            
            EditorGUI.BeginDisabledGroup(!bool.Parse(GetValue("enabled")));
            
            SetValue("showModule", EditorGUILayout.Foldout(bool.Parse(GetValue("showModule")), dropDownLabel).ToString());
            
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginDisabledGroup(Index == 0);
            
            if (GUILayout.Button("Up", GUILayout.Width(35)))
            {
                TargetObject.Fp("editModuleSettings").Fpr("list").MoveArrayElement(Index, Index - 1);
                TargetObject.ApplyModifiedProperties();
                TargetObject.Update();
                ShouldReturn = true;
                return;
            }
            
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(Index == TargetObject.Fp("editModuleSettings").Fpr("list").arraySize - 1);
            
            if (GUILayout.Button("Dwn", GUILayout.Width(35)))
            {
                TargetObject.Fp("editModuleSettings").Fpr("list").MoveArrayElement(Index, Index + 1);
                TargetObject.ApplyModifiedProperties();
                TargetObject.Update();
                ShouldReturn = true;
                return;
            }
            
            EditorGUI.EndDisabledGroup();

            GUI.backgroundColor = UtilEditor.Red;
            
            if (GUILayout.Button("-", GUILayout.Width(25)))
            {
                TargetObject.Fp("editModuleSettings").Fpr("list").DeleteIndex(Index);
                TargetObject.ApplyModifiedProperties();
                TargetObject.Update();
                ShouldReturn = true;
                return;
            }
            
            GUI.backgroundColor = Color.white;
            
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginDisabledGroup(!bool.Parse(GetValue("enabled")));
        }


        public abstract Type EditModule { get; }

        
        /// <summary>
        /// A method to implement logic for the editor for a module.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="index">The index of the module in the object.</param>
        public abstract void DrawInspector(SerializedObject targetObject, int index);
        
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets the index of a property in the edit module settings dictionary. 
        /// </summary>
        /// <param name="key">The key to find in the lookup for the settings of the module.</param>
        /// <returns>The index found.</returns>
        private int IndexOfProperty(string key)
        {
            for (var i = 0; i < Lookup.arraySize; i++)
            {
                if (Lookup.GetIndex(i).Fpr("key").stringValue != key) continue;
                return i;
            }
            
            return -1;
        }
        
        
        /// <summary>
        /// Adds a value to the module inspector settings dictionary.
        /// </summary>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="value">The value to set for the new entry.</param>
        protected void AddValue(string key, string value)
        {
            Lookup.InsertIndex(Lookup.arraySize);
            SetValue(Lookup.arraySize - 1, key, value);
        }
        
        
        /// <summary>
        /// Gets the value of an entry in the edit module inspector settings lookup.
        /// </summary>
        /// <param name="key">The key to find in the lookup for the settings of the module.</param>
        /// <returns>The string value of the key entry.</returns>
        protected string GetValue(string key)
        {
            for (var i = 0; i < Lookup.arraySize; i++)
            {
                if (Lookup.GetIndex(i).Fpr("key").stringValue == key)
                {
                    return Lookup.GetIndex(i).Fpr("value").stringValue;
                }
            }
            
            return null;
        }

        
        /// <summary>
        /// Sets the value of an entry in the edit module inspector settings lookup.
        /// </summary>
        /// <param name="indexOfElement">The index of the element to set to.</param>
        /// <param name="key">The key to find in the lookup for the settings of the module.</param>
        /// <param name="value">The value to set to.</param>
        private void SetValue(int indexOfElement, string key, string value)
        {
            Lookup.GetIndex(indexOfElement).Fpr("key").stringValue = key;
            Lookup.GetIndex(indexOfElement).Fpr("value").stringValue = value;

            TargetObject.ApplyModifiedProperties();
            TargetObject.Update();
        }
        

        /// <summary>
        /// Sets the value of an entry in the edit module inspector settings lookup.
        /// </summary>
        /// <param name="key">The key to find in the lookup for the settings of the module.</param>
        /// <param name="value">The value to set to.</param>
        protected void SetValue(string key, string value)
        {
            SetValue(IndexOfProperty(key), key, value);
        }
    }
}