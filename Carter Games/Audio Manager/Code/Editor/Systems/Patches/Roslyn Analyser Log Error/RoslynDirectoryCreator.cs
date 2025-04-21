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
using UnityEditor;
using UnityEngine;

// Credit: https://gist.github.com/SolidAlloy/68b02da3c774c6691da7dab2eba190cc | SolidAlloy
// Modified a tad to work a bit better.

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Attempts to fix a random Unity error message on moving some files about from the asset...
    /// </summary>
    [InitializeOnLoad]
    internal static class RoslynDirectoryCreator
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Constructors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        static RoslynDirectoryCreator()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
            Application.logMessageReceived += OnLogMessageReceived;
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static void OnLogMessageReceived(string message, string _, LogType logType)
        {
            if (logType != LogType.Exception) return;
            
            if (message.Contains("DirectoryNotFoundException: Could not find a part of the path") || message.Contains("Temp/RoslynAnalysisRunner")) 
            {
                Directory.CreateDirectory("Temp/RoslynAnalysisRunner");
            }
        }
    }
}