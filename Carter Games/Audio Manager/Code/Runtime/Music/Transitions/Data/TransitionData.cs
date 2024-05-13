/*
 * Copyright (c) 2024 Carter Games
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
 * FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using CarterGames.Assets.AudioManager.Logging;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Contains all the data needed for a music transition to run.
    /// </summary>
    [Serializable]
    public sealed class TransitionData
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        // Id's cached to avoid spelling mistakes mostly.
        private const string DurationId = "duration";
        private const string TimeScaleId = "unscaledTime";
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The id of the transition.
        /// </summary>
        public string Id { get; private set; }
        
        
        /// <summary>
        /// The parameters for the transition.
        /// </summary>
        public Dictionary<string, object> Parameters { get; private set; } = new Dictionary<string, object>();
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Constructors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Makes a new transition data class when called.
        /// </summary>
        /// <param name="duration">The duration of the transition, Def: 1f</param>
        /// <param name="unscaledTime">Should the transition be in unscaled time? Def: false</param>
        public TransitionData(float duration = 1f, bool unscaledTime = false)
        {
            Id = Guid.NewGuid().ToString();
            
            Parameters.Add(DurationId, duration);
            Parameters.Add(TimeScaleId, unscaledTime);
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Gets if the param exists in the class.
        /// </summary>
        /// <param name="id">The id to search for.</param>
        /// <returns>If the id exists.</returns>
        public bool HasParam(string id)
        {
            return Parameters.ContainsKey(id);
        }


        /// <summary>
        /// Adds a parameter to the class, or modifies it if it already exists.
        /// </summary>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The value to set.</param>
        public void AddParam(string key, object value)
        {
            if (HasParam(key))
            {
                Parameters[key] = value;
            }
            else
            {
                Parameters.Add(key, value);
            }
        }
        
        
        /// <summary>
        /// Creates or sets a param with the entered data.
        /// </summary>
        /// <param name="key">The key to look for or create.</param>
        /// <param name="value">The value to set.</param>
        public void CreateOrSetParam(string key, object value)
        {
            if (Parameters.ContainsKey(key))
            {
                Parameters[key] = value;
                return;
            }

            Parameters.Add(key, value);
        }


        /// <summary>
        /// Gets the param of the entered key.
        /// </summary>
        /// <param name="key">The key to set.</param>
        /// <typeparam name="T">The type to get.</typeparam>
        /// <returns>The found param value or the default if not.</returns>
        public T GetParam<T>(string key)
        {
            if (Parameters.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            AmDebugLogger.Warning($"{AudioManagerErrorCode.TransitionDataParameterNotFound}\nUnable to get parameter {key}.");
            return default;
        }
        
        
        /// <summary>
        /// Gets the param of the entered key.
        /// </summary>
        /// <param name="key">The key to set.</param>
        /// <param name="defValue">The default value to get if not found.</param>
        /// <typeparam name="T">The type to get.</typeparam>
        /// <returns>The found param value or the default value provided if not found.</returns>
        public T GetParam<T>(string key, T defValue)
        {
            if (Parameters.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            AmDebugLogger.Warning($"{AudioManagerErrorCode.TransitionDataParameterNotFound}\nUnable to get parameter {key}. Using provided default {defValue} instead.");
            return defValue;
        }
        
        
        /// <summary>
        /// Tries to get the param of the entered key.
        /// </summary>
        /// <param name="key">The key to set.</param>
        /// <param name="value">The found value.</param>
        /// <typeparam name="T">The type to get.</typeparam>
        /// <returns>If it was successful or not.</returns>
        public bool TryGetParam<T>(string key, out T value)
        {
            if (Parameters.TryGetValue(key, out var foundValue))
            {
                value = (T)foundValue;
                return true;
            }

            AmDebugLogger.Warning($"{AudioManagerErrorCode.TransitionDataParameterNotFound}\nUnable to get parameter {key}.");
            value = default;
            return false;
        }
    }
}