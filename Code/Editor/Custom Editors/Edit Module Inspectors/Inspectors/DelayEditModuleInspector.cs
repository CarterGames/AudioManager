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
    /// The editor GUI logic for the delay edit module.
    /// </summary>
    public sealed class DelayEditModuleInspector : EditModuleInspectorBase
    {
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
            { "delay", "0" },
        };
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        public override Type EditModule => typeof(DelayEdit);
        

        /// <summary>
        /// Draws the inspector GUI for the module.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="index">The index of the module in the object.</param>
        public override void DrawInspector(SerializedObject targetObject, int index)
        {
            InitializeValues(targetObject, index);
            
            EditorGUILayout.BeginVertical("HelpBox");

            DrawDropDown("Delay Edit");
            
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
                
                var delay = EditorGUILayout.FloatField("Delay:", float.Parse(GetValue("delay")));

                if (EditorGUI.EndChangeCheck())
                {
                    // Stops invalid input for the delay...
                    if (delay < 0)
                    {
                        delay = 0;
                        AmDebugLogger.Warning($"{AudioManagerErrorCode.InvalidAudioClipInspectorInput}\nDelay edit cannot have a value below 0.");
                    }
                    
                    SetValue("delay", delay.ToString());
                }
            }
            
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }
    }
}