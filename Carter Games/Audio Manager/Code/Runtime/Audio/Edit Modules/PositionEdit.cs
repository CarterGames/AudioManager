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