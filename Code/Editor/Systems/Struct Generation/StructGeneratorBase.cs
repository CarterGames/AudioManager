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

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Provides the base logic for the struct generators.
    /// </summary>
    public abstract class StructGeneratorBase
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The prefix line for a line.
        /// </summary>
        public abstract string LinePrefix { get; }
        
        
        /// <summary>
        /// The path for the class.
        /// </summary>
        public abstract string ClassPath { get; }
        
        
        /// <summary>
        /// The name of the class.
        /// </summary>
        public abstract string ClassName { get; }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Generates the struct when called. To be implemented by inheritors.
        /// </summary>
        public abstract void Generate();
    }
}