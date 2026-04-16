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