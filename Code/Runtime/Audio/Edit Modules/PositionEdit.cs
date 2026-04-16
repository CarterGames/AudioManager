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
    /// <summary>
    /// An audio clip player edit that changes the world/local position of the player.
    /// </summary>
    public sealed class PositionEdit : IEditModule
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private Vector3 position;
        private bool setLocal;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   IAudioEditModule
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets if the edits should process when looping
        /// </summary>
        public bool ProcessOnLoop => false;
        
        
        /// <summary>
        /// Processes the edit when called.
        /// </summary>
        /// <param name="source">The AudioSource to edit.</param>
        public void Process(AudioSourceInstance source)
        {
            if (setLocal)
            {
                source.transform.localPosition = position;
            }
            else
            {
                source.transform.position = position;
            }
        }

        
        /// <summary>
        /// Revers the edit to default when called.
        /// </summary>
        /// <param name="source">The AudioSource to edit.</param>
        public void Revert(AudioSourceInstance source)
        {
            if (setLocal)
            {
                source.transform.localPosition = Vector3.zero;
            }
            else
            {
                source.transform.position = Vector3.zero;
            }
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Makes a new position edit with the setting entered.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public PositionEdit(Vector2 value)
        {
            position = value;
        }
        
        
        /// <summary>
        /// Makes a new position edit with the setting entered.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public PositionEdit(Vector3 value)
        {
            position = value;
        }
        
        
        /// <summary>
        /// Makes a new position edit with the setting entered.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        /// <param name="local">Use local position, DEF: False</param>
        public PositionEdit(Transform value, bool local = false)
        {
            position = local ? value.localPosition : value.position;
            setLocal = local;
        }
    }
}