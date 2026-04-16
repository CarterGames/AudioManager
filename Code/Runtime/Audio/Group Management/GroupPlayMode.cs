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

using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    public enum GroupPlayMode
    {
        /// <summary>
        /// A random clip is played from the group.
        /// </summary>
        [InspectorName("Random (1-Clip)")] Random,
        
        
        /// <summary>
        /// All clips are played in the order defined in the group.
        /// </summary>
        [InspectorName("Sequential (All Clips)")] Sequential,
        
        
        /// <summary>
        /// All clips are played at the same time from the group.
        /// </summary>
        [InspectorName("Combined (All Clips)")] Combined,
    }
}