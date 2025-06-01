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