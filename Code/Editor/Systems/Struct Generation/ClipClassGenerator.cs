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
using System.IO;
using System.Reflection;
using CarterGames.Assets.AudioManager.Logging;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles generating the Clip.? struct class entries.
    /// </summary>
    public sealed class ClipClassGenerator : StructGeneratorBase
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static readonly Dictionary<string, int> DupeLookup = new Dictionary<string, int>();

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public override string LinePrefix => "        public const string";
        public override string ClassPath => "Assets/Audio Manager/Generated Struct Classes/Clip.cs";
        public override string ClassName => "Clip";

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public override void Generate()
        {
            // var allCurrentEntries = typeof(Clip).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            // Debug.Log(allCurrentEntries[0].Name);
            
            DupeLookup.Clear();
            var data = UtilEditor.GetLibraryData();
            
            using (var file = new StreamWriter(ClassPath))
            {
                StructHandler.WriteHeader(file, ClassName);

                if (data == null)
                {
                    AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.StructGeneratorNoData));
                }
                else
                {
                    if (data.Count <= 0)
                    {
                        AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.StructGeneratorNoData));
                    }
                    else
                    {
                        foreach (var entry in data)
                        {
                            var parsedName = entry.Key.ParseFieldName();
                            var dupeChangedName = string.Empty;

                            if (DupeLookup.ContainsKey(parsedName))
                            {
                                DupeLookup[parsedName]++;
                                dupeChangedName = $"{parsedName}{DupeLookup[parsedName]}";

                                AmDebugLogger.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.StructElementNameAlreadyExists));
                            }


                            try
                            {
                                StructHandler.WriteLine(file, LinePrefix,
                                    dupeChangedName.Length > 0 ? dupeChangedName : entry.Value.key, $"\"{entry.Value.id}\"");

                                if (DupeLookup.ContainsKey(parsedName)) continue;
                                DupeLookup.Add(parsedName, 0);
                            }
#pragma warning disable
                            catch (Exception e)
                            {
                                AmDebugLogger.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.StructGeneratorElementFailed));
                            }
#pragma warning restore
                        }
                    }
                }

                StructHandler.WriteFooter(file);
            }
        }
    }
}