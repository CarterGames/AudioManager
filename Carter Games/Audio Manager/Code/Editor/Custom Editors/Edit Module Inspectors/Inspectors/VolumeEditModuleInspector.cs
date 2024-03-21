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
    /// The editor GUI logic for the volume edit module.
    /// </summary>
    public sealed class VolumeEditModuleInspector : EditModuleInspectorBase
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The options for the toolbar in this editor.
        /// </summary>
        private static readonly string[] Options = new string[3]
        {
            "Standard", "Range", "Variance"
        };

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// The properties the edit module has to edit.
        /// </summary>
        protected override Dictionary<string, string> EditPropertiesDefaults { get; set; } = new Dictionary<string, string>
        {
            { "showModule", "False" },
            { "enabled", "True" },
            { "moduleMode", "0" },
            { "normalEditValue", "1" },
            { "rangeEditValue", "[0.0, 1.0]" },
            { "varianceEditValue", "[1.0, 0.1, 0.0, 1.0]" },
        };
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Draws the inspector GUI for the module.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="index">The index of the module in the object.</param>
        public override void DrawInspector(SerializedObject targetObject, int index)
        {
            InitializeValues(targetObject, index);
            
            EditorGUILayout.BeginVertical("HelpBox");

            DrawDropDown(targetObject, index, "Volume Edit");

            GUILayout.Space(2.5f);
            
            if (ShouldReturn)
            {
                ShouldReturn = false;
                return;
            }
            
            if (bool.Parse(EditModuleInspectorHelper.GetValue(targetObject, index, "showModule")))
            {
                UtilEditor.DrawHorizontalGUILine();
                
                EditModuleInspectorHelper.SetValue(targetObject, index, "moduleMode", GUILayout.Toolbar(int.Parse(EditModuleInspectorHelper.GetValue(targetObject, index, "moduleMode")), Options).ToString());

                switch (int.Parse(EditModuleInspectorHelper.GetValue(targetObject, index, "moduleMode")))
                {
                    case 0:
                        
                        EditModuleInspectorHelper.SetValue(targetObject, index, "normalEditValue",
                            EditorGUILayout.Slider("Volume:", float.Parse(EditModuleInspectorHelper.GetValue(targetObject, index, "normalEditValue")),0f,1f).ToString());
                        
                        break;
                    case 1:
                        
                        var min = float.Parse(EditModuleInspectorHelper.GetValue(targetObject, index, "rangeEditValue").Split(',')[0].Replace("[", string.Empty));
                        var max = float.Parse(EditModuleInspectorHelper.GetValue(targetObject, index, "rangeEditValue").Split(',')[1].Replace("]", string.Empty));

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Min:", GUILayout.Width(65));
                        EditorGUILayout.LabelField("");
                        EditorGUILayout.LabelField("Max:", GUILayout.Width(65));
                        EditorGUILayout.EndHorizontal();
                        
                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginChangeCheck();
                        min = EditorGUILayout.FloatField(min, GUILayout.Width(65));
                        GUILayout.Space(2.5f);
                        EditorGUILayout.MinMaxSlider(ref min, ref max, 0f, 1f);
                        GUILayout.Space(2.5f);
                        max = EditorGUILayout.FloatField(max, GUILayout.Width(65));
                        EditorGUILayout.EndHorizontal();
                        
                        if (EditorGUI.EndChangeCheck())
                        {
                            min = Mathf.Clamp(min, 0f, max);
                            max = Mathf.Clamp(max, min > 0f ? min : 0f, 1f);
                            
                            EditModuleInspectorHelper.SetValue(targetObject, index, "rangeEditValue", $"[{min},{max}]");
                        }
                        
                        break;
                    
                    case 2:

                        var starting = float.Parse(EditModuleInspectorHelper.GetValue(targetObject, index, "varianceEditValue").Split(',')[0].Replace("[", string.Empty));
                        var offset = float.Parse(EditModuleInspectorHelper.GetValue(targetObject, index, "varianceEditValue").Split(',')[1]);
                        var minVariance = float.Parse(EditModuleInspectorHelper.GetValue(targetObject, index, "varianceEditValue").Split(',')[2]);
                        var maxVariance = float.Parse(EditModuleInspectorHelper.GetValue(targetObject, index, "varianceEditValue").Split(',')[3].Replace("]", string.Empty));
                        
                        EditorGUI.BeginChangeCheck();

                        starting = EditorGUILayout.FloatField("Starting Value:", starting);
                        offset = EditorGUILayout.FloatField("Offset:", offset);
                        minVariance = EditorGUILayout.FloatField("Min:", minVariance);
                        maxVariance = EditorGUILayout.FloatField("Max:", maxVariance);
                        

                        if (EditorGUI.EndChangeCheck())
                        {
                            starting = Mathf.Clamp01(starting);
                            offset = Mathf.Clamp01(offset);
                            minVariance = Mathf.Clamp(minVariance, 0f, maxVariance);
                            maxVariance = Mathf.Clamp(maxVariance, minVariance > 0f ? minVariance : 0f, 1f);
                            
                            EditModuleInspectorHelper.SetValue(targetObject, index, "varianceEditValue", $"[{starting},{offset},{minVariance},{maxVariance}]");
                        }
                        
                        break;
                }
            }
            
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }
    }
}