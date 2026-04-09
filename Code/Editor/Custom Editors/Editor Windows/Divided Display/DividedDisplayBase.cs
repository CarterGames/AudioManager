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

using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// A base class for the divided up editor look used in the library editor GUI.
    /// </summary>
    public abstract class DividedDisplayBase
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Draws the sections in the right order by default when called.
        /// </summary>
        protected void DisplaySections()
        {
            EditorGUILayout.BeginHorizontal();

            LeftSectionControl();
            RightSectionControl();

            EditorGUILayout.EndHorizontal();
        }


        /// <summary>
        /// Draws the left side when called.
        /// </summary>
        protected virtual void LeftSectionControl()
        {
            OnLeftGUI();
        }


        /// <summary>
        /// Draws the right side when called.
        /// </summary>
        protected virtual void RightSectionControl()
        {
            OnRightGUI();
        }


        /// <summary>
        /// Implement to do some GUI on the left side of the display.
        /// </summary>
        protected abstract void OnLeftGUI();


        /// <summary>
        /// Implement to do some GUI on the right side of the display.
        /// </summary>
        protected abstract void OnRightGUI();
    }
}