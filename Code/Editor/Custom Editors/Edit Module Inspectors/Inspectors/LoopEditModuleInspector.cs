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
using CarterGames.Assets.AudioManager.Logging;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// The editor GUI logic for the loop edit module.
    /// </summary>
    public sealed class LoopEditModuleInspector : EditModuleInspectorBase
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The options for the toolbar in this editor.
        /// </summary>
        private static readonly string[] Options = new string[2]
        {
            "X Times", "Infinite"
        };
        
        public override Type EditModule => typeof(LoopEdit);
        
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
                { "moduleMode", "0" },
                { "loopCount", "0" },
                { "ignoreDelayAfterFirst", "True" },
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

            DrawDropDown("Loop Edit");

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

                        EditorGUI.BeginChangeCheck();
                        
                        if (int.Parse(GetValue("loopCount")) < 0f)
                        {
                            SetValue("loopCount", "0");
                        }
                        
                        var loopCount = EditorGUILayout.IntField("Times To Loop:",
                            int.Parse(GetValue("loopCount")));

                        if (EditorGUI.EndChangeCheck())
                        {
                            if (loopCount < 0)
                            {
                                loopCount = 0;
                                AmDebugLogger.Warning($"{AudioManagerErrorCode.InvalidAudioClipInspectorInput}\nLoop count cannot have a value below 0 when using x times mode.");
                            }
                            
                            SetValue("loopCount", loopCount.ToString());
                        }
                        
                        break;
                    case 1:

                        if (int.Parse(GetValue("loopCount")) > 0)
                        {
                            SetValue("loopCount", "-1");
                        }
                        
                        break;
                }
                
                
                EditorGUI.BeginChangeCheck();
                        
                var delayAfterFirst = EditorGUILayout.Toggle("Use delay on repeats:",
                    bool.Parse(GetValue("ignoreDelayAfterFirst")));

                if (EditorGUI.EndChangeCheck())
                {
                    SetValue("ignoreDelayAfterFirst", delayAfterFirst.ToString());
                }
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }
    }
}