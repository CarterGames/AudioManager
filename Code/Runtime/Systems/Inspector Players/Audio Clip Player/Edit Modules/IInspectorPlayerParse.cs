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

using CarterGames.Shared.AudioManager.Serializiation;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// The interface to implement to parse a edit module from a dictionary of string key values.
    /// </summary>
    public interface IInspectorPlayerParse
    {
        /// <summary>
        /// Parses the module from the inspector setup when called.
        /// </summary>
        /// <param name="data">The data to parse with.</param>
        /// <returns>The setup module</returns>
        IEditModule Parse(SerializableDictionary<string, string> data);
    }
}