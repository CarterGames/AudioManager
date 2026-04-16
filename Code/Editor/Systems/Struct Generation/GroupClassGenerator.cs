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
using CarterGames.Assets.AudioManager.Logging;
using CarterGames.Shared.AudioManager.Editor;
using CarterGames.Shared.AudioManager.Serializiation;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles generating the Group.? struct class entries.
    /// </summary>
    public sealed class GroupClassGenerator : StructGeneratorBase
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static GroupData[] groups;
        private static List<string> groupsGenerated = new List<string>();
        private static Dictionary<string, int> groupDupNameLookup = new Dictionary<string, int>();

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public override string LinePrefix => "        public static readonly string";

        public override string ClassPath => "Assets/Audio Manager/Generated Struct Classes/Group.cs";
        
        public override string ClassName => "Group";

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        public override void Generate()
        {
            using (var file = new StreamWriter(ClassPath))
            {
                StructHandler.WriteHeader(file, ClassName);

                groupsGenerated.Clear();
                
                
                if (ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.GroupsLookup.Count <= 0)
                {
                    AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.StructGeneratorNoData));
                    StructHandler.WriteFooter(file);
                    return;
                }

                foreach (var keypair in ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.GroupsLookup)
                {
                    var parsedName = keypair.Value.GroupName.ParseFieldName();
                    var dupeChangedName = string.Empty;

                    if (groupsGenerated.Count > 0)
                    {
                        if (groupDupNameLookup.ContainsKey(parsedName))
                        {
                            groupDupNameLookup[parsedName]++;
                            dupeChangedName = $"{parsedName}{groupDupNameLookup[parsedName]}";

                            AmDebugLogger.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.StructElementNameAlreadyExists));
                        }

                        try
                        {
                            StructHandler.WriteLine(file, LinePrefix,
                                dupeChangedName.Length > 0 ? dupeChangedName : parsedName, GetGroupRef(keypair));
                            groupsGenerated.Add(dupeChangedName.Length > 0 ? dupeChangedName : parsedName);

                            if (!groupDupNameLookup.ContainsKey(dupeChangedName.Length > 0
                                    ? dupeChangedName
                                    : parsedName))
                                groupDupNameLookup.Add(dupeChangedName.Length > 0 ? dupeChangedName : parsedName, 0);
                        }
#pragma warning disable
                        catch (Exception e)
                        {
                            AmDebugLogger.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.StructGeneratorElementFailed));
                        }
#pragma warning restore
                    }
                    else
                    {
                        try
                        {
                            StructHandler.WriteLine(file, LinePrefix, parsedName, GetGroupRef(keypair));
                            groupsGenerated.Add(parsedName);

                            if (!groupDupNameLookup.ContainsKey(dupeChangedName.Length > 0
                                    ? dupeChangedName
                                    : parsedName))
                            {
                                groupDupNameLookup.Add(parsedName, 0);
                            }
                        }
#pragma warning disable
                        catch (Exception e)
                        {
                            AmDebugLogger.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.StructGeneratorElementFailed));
                        }
#pragma warning restore
                    }
                }
                
                StructHandler.WriteFooter(file);
            }
        }


        private static string GetGroupRef(SerializableKeyValuePair<string, GroupData> data)
        {
            return $"\"{data.key}\"";
        }


        private static string GetListInLine(IReadOnlyList<string> toParse)
        {
            var line = "{";

            for (var i = 0; i < toParse.Count; i++)
            {
                if (i.Equals(0))
                    line += $"\"{toParse[i]}\"";
                else
                    line += $",\"{toParse[i]}\"";
            }

            line += "}";
            return line;
        }
    }
}