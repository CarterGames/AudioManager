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