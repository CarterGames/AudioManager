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

using UnityEditor;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// A helper class for applying changes to settings edited in a edit module inspectors.
    /// </summary>
    public static class EditModuleInspectorHelper
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets the SerializedProperty for the lookup of an edit module's editor settings.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="indexOfModule">The index of the module in the dictionary.</param>
        /// <returns>The SerializedProperty for the lookup.</returns>
        private static SerializedProperty Lookup(SerializedObject targetObject, int indexOfModule)
        {
            return targetObject.Fp("editModuleSettings").Fpr("list")
                .GetIndex(indexOfModule).Fpr("value").Fpr("list");
        }
        
        
        /// <summary>
        /// Gets the index of a property in the edit module settings dictionary. 
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="moduleIndex">The index of the module in the lookup.</param>
        /// <param name="key">The key to find in the lookup for the settings of the module.</param>
        /// <returns>The index found.</returns>
        private static int IndexOfProperty(SerializedObject targetObject, int moduleIndex, string key)
        {
            for (var i = 0; i < Lookup(targetObject, moduleIndex).arraySize; i++)
            {
                if (Lookup(targetObject, moduleIndex).GetIndex(i).Fpr("key").stringValue == key)
                {
                    return i;
                }
            }
            
            return -1;
        }
        
        
        /// <summary>
        /// Adds a value to the module inspector settings dictionary.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="indexOfModule">The index of the module in the dictionary.</param>
        /// <param name="key">The key of the entry to add.</param>
        /// <param name="value">The value to set for the new entry.</param>
        public static void AddValue(SerializedObject targetObject, int indexOfModule, string key, string value)
        {
            Lookup(targetObject, indexOfModule).InsertIndex(Lookup(targetObject, indexOfModule).arraySize);
            SetValue(targetObject, indexOfModule, Lookup(targetObject, indexOfModule).arraySize - 1, key, value);
        }
        
        
        /// <summary>
        /// Gets the value of an entry in the edit module inspector settings lookup.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="moduleIndex">The index of the module in the lookup.</param>
        /// <param name="key">The key to find in the lookup for the settings of the module.</param>
        /// <returns>The string value of the key entry.</returns>
        public static string GetValue(SerializedObject targetObject, int moduleIndex, string key)
        {
            for (var i = 0; i < Lookup(targetObject, moduleIndex).arraySize; i++)
            {
                if (Lookup(targetObject, moduleIndex).GetIndex(i).Fpr("key").stringValue == key)
                {
                    return Lookup(targetObject, moduleIndex).GetIndex(i).Fpr("value").stringValue;
                }
            }
            
            return null;
        }

        
        /// <summary>
        /// Sets the value of an entry in the edit module inspector settings lookup.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="indexOfModule">The index of the module in the lookup.</param>
        /// <param name="indexOfElement">The index of the element to set to.</param>
        /// <param name="key">The key to find in the lookup for the settings of the module.</param>
        /// <param name="value">The value to set to.</param>
        private static void SetValue(SerializedObject targetObject, int indexOfModule, int indexOfElement, string key,
            string value)
        {
            Lookup(targetObject, indexOfModule).GetIndex(indexOfElement).Fpr("key").stringValue = key;
            Lookup(targetObject, indexOfModule).GetIndex(indexOfElement).Fpr("value").stringValue = value;

            targetObject.ApplyModifiedProperties();
            targetObject.Update();
        }
        

        /// <summary>
        /// Sets the value of an entry in the edit module inspector settings lookup.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="indexOfModule">The index of the module in the lookup.</param>
        /// <param name="key">The key to find in the lookup for the settings of the module.</param>
        /// <param name="value">The value to set to.</param>
        public static void SetValue(SerializedObject targetObject, int indexOfModule, string key,
            string value)
        {
            SetValue(targetObject, indexOfModule, IndexOfProperty(targetObject, indexOfModule, key), key, value);
        }
    }
}