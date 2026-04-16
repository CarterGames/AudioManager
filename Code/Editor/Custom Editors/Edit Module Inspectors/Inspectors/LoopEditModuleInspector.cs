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