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
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// The editor GUI logic for the global variance edit module.
    /// </summary>
    public sealed class GlobalVarianceEditModuleInspector : EditModuleInspectorBase
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// The properties the edit module has to edit.
        /// </summary>
        protected override Dictionary<string, string> EditPropertiesDefaults { get; set; } =
            new Dictionary<string, string>
            {
                { "showModule", "False" },
                { "enabled", "True" },
                { "useGlobalVariance", "True" },
            };
        
        public override Type EditModule => typeof(GlobalVarianceEdit);

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

            DrawDropDown("Global Variance Edit");

            GUILayout.Space(2.5f);

            if (ShouldReturn)
            {
                ShouldReturn = false;
                return;
            }

            if (bool.Parse(GetValue("showModule")))
            {
                UtilEditor.DrawHorizontalGUILine();

                EditorGUI.BeginChangeCheck();

                var useGlobalVariance = EditorGUILayout.Toggle("Use Global Variance:",
                    bool.Parse(GetValue("useGlobalVariance")));

                if (EditorGUI.EndChangeCheck())
                {
                    SetValue("useGlobalVariance", useGlobalVariance.ToString());
                }
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }
    }
}