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

using System.IO;
using System.Text.RegularExpressions;
using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the creation/editing of the struct classes use as helper classes for the asset...
    /// </summary>
    public static class StructHandler
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private static string DefaultUtilityFilePaths => $"Assets/Plugins/{AssetVersionData.AssetName}/Constants/";
        
        // Honestly regex is not something I'm super familiar with, but this works xD
        private const string ValueNamePattern = @"[\W]";
        private const string DoubleUnderscorePattern = @"[_+]";
        private const string NumberPattern = @"[0-9]";

        private static ClipClassGenerator clipGeneratorCache;
        private static GroupClassGenerator groupGeneratorCache;
        private static MixerClassGenerator mixerGeneratorCache;
   
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// The clip class generator (The "Clip" Struct)
        /// </summary>
        private static ClipClassGenerator ClipGenerator
        {
            get
            {
                if (clipGeneratorCache != null) return clipGeneratorCache;
                clipGeneratorCache = new ClipClassGenerator();
                return clipGeneratorCache;
            }
        }
        
        
        /// <summary>
        /// The group class generator (The "Group" Struct)
        /// </summary>
        private static GroupClassGenerator GroupGenerator
        {
            get
            {
                if (groupGeneratorCache != null) return groupGeneratorCache;
                groupGeneratorCache = new GroupClassGenerator();
                return groupGeneratorCache;
            }
        }


        /// <summary>
        /// The mixer class generator (The "Mixer" Struct)
        /// </summary>
        private static MixerClassGenerator MixerGenerator
        {
            get
            {
                if (mixerGeneratorCache != null) return mixerGeneratorCache;
                mixerGeneratorCache = new MixerClassGenerator();
                return mixerGeneratorCache;
            }
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Menu Items
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        [MenuItem("Tools/Carter Games/Audio Manager/Reset Clips Helper Class", priority = 33)]
        private static void ResetClipsClass()
        {
            GenerateEmptyClass(ClipGenerator);
        }
        
        
        [MenuItem("Tools/Carter Games/Audio Manager/Reset Groups Helper Class", priority = 34)]
        private static void ResetGroupsClass()
        {
            GenerateEmptyClass(GroupGenerator);
        }


        [MenuItem("Tools/Carter Games/Audio Manager/Reset Mixers Helper Class", priority = 35)]
        private static void ResetMixerClass()
        {
            GenerateEmptyClass(MixerGenerator);
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Utility Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Updates the clips struct class when called.
        /// </summary>
        public static void RefreshClips(bool updateAssets = false)
        {
            ResetClipsClass();
            ClipGenerator.Generate();

            if (!updateAssets) return;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// Updates the groups struct class when called.
        /// </summary>
        public static void RefreshGroups(bool updateAssets = false)
        {
            ResetGroupsClass();
            GroupGenerator.Generate();
            
            if (!updateAssets) return;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// Updates the mixers struct class when called.
        /// </summary>
        public static void RefreshMixers(bool updateAssets = false)
        {
            ResetMixerClass();
            MixerGenerator.Generate();
            
            if (!updateAssets) return;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Builder Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Builds the header line for the class. 
        /// </summary>
        /// <param name="file">The file to edit.</param>
        /// <param name="structName">The name of the struct to create.</param>
        public static void WriteHeader(TextWriter file, string structName)
        {
            file.WriteLine("namespace CarterGames.Assets.AudioManager");
            file.WriteLine("{");
            file.WriteLine($"    public struct {structName}");
            file.WriteLine("    {");
        }
        
        
        /// <summary>
        /// Builds the footer of the class.
        /// </summary>
        /// <param name="file">The file to edit.</param>
        public static void WriteFooter(TextWriter file)
        {
            file.WriteLine("    }");
            file.WriteLine("}");
            file.Close();
        }
        
        
        /// <summary>
        /// Writes a line for an entry into the class.
        /// </summary>
        /// <param name="file">The file to edit.</param>
        /// <param name="prefix">The prefix for the field.</param>
        /// <param name="fieldName">The field name use.</param>
        /// <param name="data">The data to assign to this field.</param>
        public static void WriteLine(TextWriter file, string prefix, string fieldName, string data)
        {
            file.WriteLine($"{prefix} {fieldName.ParseFieldName()} = {data};");
        }


        /// <summary>
        /// Parse the name entered using Regex to make sure it is valid.
        /// </summary>
        /// <param name="input">The input to edit.</param>
        /// <returns>The corrected input.</returns>
        public static string ParseFieldName(this string input)
        {
            // Starts by making sure the string is a valid variable name, replacing anything that isn't with "".
            var altered = Regex.Replace(input, ValueNamePattern, "");
            
            // Alters the result to remove any double underscores as that will break the variable name.
            altered = Regex.Replace(altered, DoubleUnderscorePattern, "_");
            
            // Adds the prefix of _ if the clip starts with a number.
            var charString = altered[0].ToString();

            var matched = Regex.Match(charString, NumberPattern);
            
            if (matched.Success)
            {
                altered = altered.Insert(0, "_");
            }

            return altered;
        }


        /// <summary>
        /// Generates an empty class for resetting a class to its default setup.
        /// </summary>
        /// <param name="structGenerator">The data to use to reset from.</param>
        private static void GenerateEmptyClass(StructGeneratorBase structGenerator)
        {
            var path = string.IsNullOrEmpty(structGenerator.ClassPath)
                ? DefaultUtilityFilePaths + structGenerator.ClassName + ".cs"
                : structGenerator.ClassPath;
            
            if (!File.Exists(path))
            {
                FileEditorUtil.CreateToDirectory(path.Replace(structGenerator.ClassName + ".cs", string.Empty));
                
                var fileStream = new FileStream(path, FileMode.Create);
                fileStream.Close();
            }
            
            using (var file = new StreamWriter(path))
            {
                WriteHeader(file, structGenerator.ClassName);
                WriteFooter(file);
            }
            
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// Resets all the structs that use audio clips when called.
        /// </summary>
        public static void ResetLibraryStructs()
        {
            ResetClipsClass();
            ResetGroupsClass();
        }
    }
}