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

using CarterGames.Shared.AudioManager.Serializiation;

namespace CarterGames.Assets.AudioManager
{
    public sealed class EditParameters
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private SerializableDictionary<string, object> edits;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets the edits that have been defined in the params.
        /// </summary>
        private SerializableDictionary<string, object> Edits
        {
            get
            {
                if (edits != null) return edits;
                edits = new SerializableDictionary<string, object>();
                return edits;
            }
        }
        
        
        /// <summary>
        /// Gets the number of param edits made.
        /// </summary>
        public int ParamsCount => edits.Count;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Constructors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        public EditParameters()
        {
            Edits.Add("dynamicTime", true);
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Gets if the key exists.
        /// </summary>
        /// <param name="key">The key to find.</param>
        /// <returns>If the key exists.</returns>
        public bool HasKey(string key) => edits.ContainsKey(key);
        
        
        /// <summary>
        /// Gets a value from the params of the entered key.
        /// </summary>
        /// <param name="key">The key to find.</param>
        /// <typeparam name="T">The type to cast to.</typeparam>
        /// <returns>The value found.</returns>
        public T GetValue<T>(string key)
        {
            if (Edits.ContainsKey(key))
            {
                return (T) Edits[key];
            }
            
            return default;
        }


        /// <summary>
        /// Tries to get a value from the params of the entered key.
        /// </summary>
        /// <param name="key">The key to find.</param>
        /// <param name="value">The value found.</param>
        /// <typeparam name="T">The type to cast to.</typeparam>
        /// <returns>If it was successful or not.</returns>
        public bool TryGetValue<T>(string key, out T value)
        {
            value = GetValue<T>(key);
            return value != null;
        }


        /// <summary>
        /// Sets a value in the params to the entered key and value.
        /// </summary>
        /// <param name="key">The key to find.</param>
        /// <param name="value">The value to set to.</param>
        public void SetValue(string key, object value)
        {
            if (Edits.ContainsKey(key))
            {
                Edits[key] = value;
                return;
            }
            
            Edits.Add(key, value);
        }


        /// <summary>
        /// Removes a key from the params when called.
        /// </summary>
        /// <param name="key">The key to find.</param>
        public void RemoveValue(string key)
        {
            if (!Edits.ContainsKey(key)) return;
            Edits.Remove(key);
        }


        /// <summary>
        /// Clears all the parameters in the edit.
        /// </summary>
        public void ClearAllParams()
        {
            Edits.Clear();
            Edits.Add("dynamicTime", true);
        }
    }
}