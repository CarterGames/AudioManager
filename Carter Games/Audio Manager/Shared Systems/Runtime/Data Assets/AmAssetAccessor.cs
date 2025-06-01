﻿/*
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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CarterGames.Shared.AudioManager
{
    /// <summary>
    /// Handles accessing the scriptable object data assets for this asset.
    /// </summary>
    public static class AmAssetAccessor
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
     
        private const string IndexPath = "[Audio Manager] Asset Index";
        
        // A cache of all the assets found...
        private static AmDataAssetIndex indexCache;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Gets all the assets from the build versions asset...
        /// </summary>
        private static AmDataAssetIndex Index
        {
            get
            {
                if (indexCache != null) return indexCache;
                indexCache = (AmDataAssetIndex) Resources.Load(IndexPath, typeof(AmDataAssetIndex));
                return indexCache;
            }
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Gets the Save Manager Asset requested.
        /// </summary>
        /// <typeparam name="T">The save manager asset to get.</typeparam>
        /// <returns>The asset if it exists.</returns>
        public static T GetAsset<T>() where T : AmDataAsset
        {
            if (Index.Lookup.ContainsKey(typeof(T).ToString()))
            {
                return (T)Index.Lookup[typeof(T).ToString()][0];
            }

            return null;
        }
        
        
        /// <summary>
        /// Gets the Save Manager Asset requested.
        /// </summary>
        /// <typeparam name="T">The save manager asset to get.</typeparam>
        /// <returns>The asset if it exists.</returns>
        public static List<T> GetAssets<T>() where T : AmDataAsset
        {
            if (Index.Lookup.ContainsKey(typeof(T).ToString()))
            {
                return Index.Lookup[typeof(T).ToString()].Cast<T>().ToList();
            }

            return null;
        }
    }
}