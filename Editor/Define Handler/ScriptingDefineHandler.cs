/*
 * Copyright (c) 2018-Present Carter Games
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

using System.Linq;
using UnityEditor;
using UnityEditor.Build;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// A helper class to handle editing the scripting defines of the project.
    /// </summary>
    public class ScriptingDefineHandler : IActiveBuildTargetChanged
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static readonly string[] AssetDefinitions = new string[2] { "Use_CGAudioManager_Static", "USE_CG_AM_STATIC" };
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets the defines in the current target.
        /// </summary>
        /// <param name="buildTarget">The current build target.</param>
        /// <returns>A string of all the defines.</returns>
        private static string GetScriptingDefines(BuildTarget buildTarget) 
        {
            var group = BuildPipeline.GetBuildTargetGroup(buildTarget);
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        }

        
        /// <summary>
        /// Sets the defines to the entered string.
        /// </summary>
        /// <param name="scriptingDefines">The defines to apply.</param>
        /// <param name="buildTarget">The current build target.</param>
        private static void SetScriptingDefines(string scriptingDefines, BuildTarget buildTarget) 
        {
            var group = BuildPipeline.GetBuildTargetGroup(buildTarget);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, scriptingDefines);
        }

        
        /// <summary>
        /// Gets the scripting defines as a collection to look through.
        /// </summary>
        /// <param name="buildTarget">The current build target.</param>
        /// <returns>A collection of the defines.</returns>
        private static string[] GetScriptingDefinesCollection(BuildTarget buildTarget) 
        {
            var scriptingDefines = GetScriptingDefines(buildTarget);
            var separateScriptingDefines = scriptingDefines.Split(';');
            return separateScriptingDefines;
        }
        
        
        /// <summary>
        /// Gets whether or not the define exists at the moment.
        /// </summary>
        /// <returns>If the define is in the project.</returns>
        public static bool IsScriptingDefinePresent() 
        {
            var scriptingDefines = GetScriptingDefinesCollection(EditorUserBuildSettings.activeBuildTarget);
            return scriptingDefines.Contains(AssetDefinitions[0]) || scriptingDefines.Contains(AssetDefinitions[1]);
        }
        
        
        /// <summary>
        /// Gets whether or a the define exists at the moment.
        /// </summary>
        /// <param name="scriptingDefine">The define to search for.</param>
        /// <param name="buildTarget">The current build target.</param>
        /// <returns>If the define is in the project.</returns>
        private static bool IsScriptingDefinePresent(string scriptingDefine, BuildTarget buildTarget) 
        {
            var scriptingDefines = GetScriptingDefinesCollection(buildTarget);
            return scriptingDefines.Contains(scriptingDefine);
        }

        
        /// <summary>
        /// Adds a define to the scripting define. 
        /// </summary>
        /// <param name="define">The define to add.</param>
        /// <param name="buildTargets">The build targets.</param>
        public static void AddScriptingDefine(string define, params BuildTarget[] buildTargets)
        {
            if (buildTargets == null) return;

            foreach (var buildTarget in buildTargets) 
            {
                if (IsScriptingDefinePresent(define, buildTarget)) continue;
                
                var scriptingDefines = GetScriptingDefines(buildTarget) + ";" + define;
                SetScriptingDefines(scriptingDefines, buildTarget);
            }
        }

        
        /// <summary>
        /// Removed a define from the project.
        /// </summary>
        /// <param name="define">The define to remove.</param>
        /// <param name="buildTargets">The build targets.</param>
        public static void RemoveScriptingDefine(string define, params BuildTarget[] buildTargets)
        {
            if (buildTargets == null) return;

            foreach (var buildTarget in buildTargets)
            {
                if (!IsScriptingDefinePresent(define, buildTarget)) continue;
                
                var scriptingDefines = GetScriptingDefinesCollection(buildTarget).ToList();
                var removeIndex = scriptingDefines.FindIndex(item => item == define);
                scriptingDefines.RemoveAt(removeIndex);
                var updatedScriptingDefines = string.Join(";", scriptingDefines);
                SetScriptingDefines(updatedScriptingDefines, buildTarget);
            }
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   IActiveBuildTargetChanged Implementation
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public int callbackOrder { get; }
        
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            if (!AudioManagerEditorUtil.Settings.isUsingStatic) return;
            if (!IsScriptingDefinePresent(AssetDefinitions[0], EditorUserBuildSettings.activeBuildTarget)) return; 
            if (!IsScriptingDefinePresent(AssetDefinitions[1], EditorUserBuildSettings.activeBuildTarget)) return;
            AddScriptingDefine(AssetDefinitions[1]);
        }
    }
}