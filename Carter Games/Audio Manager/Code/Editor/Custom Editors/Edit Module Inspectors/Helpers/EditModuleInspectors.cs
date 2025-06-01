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