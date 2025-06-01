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
using System.IO;
using System.Reflection;
using CarterGames.Assets.AudioManager.Logging;
using CarterGames.Shared.AudioManager;
using CarterGames.Shared.AudioManager.Serializiation;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles generating the Mixer.? struct class entries.
    /// </summary>
    public sealed class MixerClassGenerator : StructGeneratorBase
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public override string LinePrefix => "        public const string";
        public override string ClassPath => "Assets/Audio Manager/Generated Struct Classes/Mixer.cs";
        public override string ClassName => "Mixer";
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public override void Generate()
        {
            using (var file = new StreamWriter(ClassPath))
            {
                StructHandler.WriteHeader(file, ClassName);

                var data = (SerializableDictionary<string, MixerData>) GetMixerGroups(AmAssetAccessor.GetAsset<AudioLibrary>()).GetValue(AmAssetAccessor.GetAsset<AudioLibrary>());

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
                        foreach (var mixer in data)
                        {
                            try
                            {
                                StructHandler.WriteLine(file, LinePrefix, mixer.Value.Key, $"\"{mixer.Key}\"");
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


        private static FieldInfo GetMixerGroups(AudioLibrary lib)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            return lib.GetType().GetField("mixers", flags);
        }
    }
}