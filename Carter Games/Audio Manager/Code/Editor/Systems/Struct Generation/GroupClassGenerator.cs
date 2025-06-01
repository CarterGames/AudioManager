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