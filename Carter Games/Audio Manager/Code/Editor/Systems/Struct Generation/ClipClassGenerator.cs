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