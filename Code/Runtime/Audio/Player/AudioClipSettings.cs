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
using System.Linq;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// A class to define settings that are needed to set an audio source or play a clip.
    /// </summary>
    public sealed class AudioClipSettings
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Gets all the edits stored for the clip player.
        /// </summary>
        public Dictionary<Type, IEditModule> Edits { get; }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Constructor
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Creates a new clip settings class with the options desired.
        /// </summary>
        /// <param name="options">The options to apply.</param>
        public AudioClipSettings(IEnumerable<IEditModule> options)
        {
            Edits = new Dictionary<Type, IEditModule>();
            
            if (options == null) return;
            
            foreach (var option in options)
            {
                Edits.Add(option.GetType(), option);
            }
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Processes all the edits when called.
        /// </summary>
        /// <param name="source">The source to apply to.</param>
        public void ProcessEdits(AudioSourceInstance source, params Type[] ignoreModules)
        {
            foreach (var edit in Edits.Values)
            {
                if (ignoreModules.Any(t => t.FullName != null && t.FullName.Equals(edit.GetType().FullName))) continue;
                edit.Process(source);
            }
        }

        
        /// <summary>
        /// Reverts all the edits when called.
        /// </summary>
        /// <param name="source">The source to apply to.</param>
        public void RevertEdits(AudioSourceInstance source, params Type[] ignoreModules)
        {
            foreach (var edit in Edits.Values)
            {
                if (ignoreModules.Any(t => t.FullName != null && t.FullName.Equals(edit.GetType().FullName))) continue;
                edit.Revert(source);
            }
        }
    }
}