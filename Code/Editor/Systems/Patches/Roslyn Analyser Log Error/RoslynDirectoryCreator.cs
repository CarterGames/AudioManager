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