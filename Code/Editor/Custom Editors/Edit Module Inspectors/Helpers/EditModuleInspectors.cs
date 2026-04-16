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
using CarterGames.Shared.AudioManager;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// A helper class to determine the editor scripts for edit modules.
    /// </summary>
    public static class EditModuleInspectors
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private static Dictionary<Type, IEditModuleEditor> cacheInspectors;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// A lookup of all the editors for the edit modules that have custom editors for.
        /// </summary>
        public static Dictionary<Type, IEditModuleEditor> Inspectors
        {
            get
            {
                if (cacheInspectors is { } && cacheInspectors.Count > 0) return cacheInspectors;
                cacheInspectors = new Dictionary<Type, IEditModuleEditor>();

                foreach (var editor in AssemblyHelper.GetClassesOfType<IEditModuleEditor>())
                {
                    cacheInspectors.Add(editor.EditModule, editor);
                }

                return cacheInspectors;
            }
        }
    }
}