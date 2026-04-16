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
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// The editor GUI logic for the pitch edit module.
    /// </summary>
    public sealed class PitchEditModuleInspector : EditModuleInspectorBase
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
        
        public override Type EditModule => typeof(PitchEdit);
        
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

            DrawDropDown("Pitch Edit");
            
            GUILayout.Space(2.5f);
            
            if (ShouldReturn)
            {
                ShouldReturn = false;
                return;
            }
            
            if (bool.Parse(GetValue("showModule")))
            {
                UtilEditor.DrawHorizontalGUILine();
                
                SetValue("moduleMode", GUILayout.Toolbar(int.Parse(GetValue("moduleMode")), Options).ToString());

                switch (int.Parse(GetValue("moduleMode")))
                {
                    case 0:
                        
                        SetValue("normalEditValue",
                            EditorGUILayout.Slider("Pitch:", float.Parse(GetValue("normalEditValue")),-3f,3f).ToString());
                        
                        break;
                    case 1:
                        
                        var min = float.Parse(GetValue("rangeEditValue").Split(',')[0].Replace("[", string.Empty));
                        var max = float.Parse(GetValue("rangeEditValue").Split(',')[1].Replace("]", string.Empty));

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Min:", GUILayout.Width(65));
                        EditorGUILayout.LabelField("");
                        EditorGUILayout.LabelField("Max:", GUILayout.Width(65));
                        EditorGUILayout.EndHorizontal();
                        
                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginChangeCheck();
                        min = EditorGUILayout.FloatField(min, GUILayout.Width(65));
                        GUILayout.Space(2.5f);
                        EditorGUILayout.MinMaxSlider(ref min, ref max, -3f, 3f);
                        GUILayout.Space(2.5f);
                        max = EditorGUILayout.FloatField(max, GUILayout.Width(65));
                        EditorGUILayout.EndHorizontal();
                        
                        if (EditorGUI.EndChangeCheck())
                        {
                            min = Mathf.Clamp(min, -3f, max);
                            max = Mathf.Clamp(max, min > -3f ? min : -3f, 3f);
                            
                            SetValue("rangeEditValue", $"[{min},{max}]");
                        }
                        
                        break;
                    
                    case 2:

                        var starting = float.Parse(GetValue("varianceEditValue").Split(',')[0].Replace("[", string.Empty));
                        var offset = float.Parse(GetValue("varianceEditValue").Split(',')[1]);
                        var minVariance = float.Parse(GetValue("varianceEditValue").Split(',')[2]);
                        var maxVariance = float.Parse(GetValue("varianceEditValue").Split(',')[3].Replace("]", string.Empty));
                        
                        EditorGUI.BeginChangeCheck();

                        starting = EditorGUILayout.FloatField("Starting Value:", starting);
                        offset = EditorGUILayout.FloatField("Offset:", offset);
                        

                        if (EditorGUI.EndChangeCheck())
                        {
                            starting = Mathf.Clamp(starting, -3f, 3f);
                            offset = Mathf.Clamp01(offset);
                            minVariance = Mathf.Clamp(minVariance, -3f, maxVariance);
                            maxVariance = Mathf.Clamp(maxVariance, minVariance > -3f ? minVariance : -3f, 3f);
                            
                            SetValue("varianceEditValue", $"[{starting},{offset},{minVariance},{maxVariance}]");
                        }
                        
                        break;
                }
            }
            
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }
    }
}