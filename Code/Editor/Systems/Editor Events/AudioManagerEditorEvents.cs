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

using CarterGames.Shared.AudioManager;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles events for the manager that are only for use in editor code.
    /// </summary>
    public static class AudioManagerEditorEvents
    {
        /// <summary>
        /// Raises if the library has been edited via scanning or clearing.
        /// </summary>
        public static readonly Evt OnLibraryRefreshed = new Evt();
        
        
        /// <summary>
        /// Raises when the settings is reset by the user in the editor (not deleted, just reset via the option).
        /// </summary>
        public static readonly Evt OnSettingsReset = new Evt();
    }
}