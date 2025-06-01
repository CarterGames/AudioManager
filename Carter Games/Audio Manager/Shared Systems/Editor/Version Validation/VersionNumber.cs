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
using UnityEngine;

namespace CarterGames.Shared.AudioManager.Editor
{
    // Warning disable for overriding Equals() but not the hashcode method.
#pragma warning disable 0659
    /// <summary>
    /// A data class to hold a x.x.x version number for comparisons.
    /// </summary>
    [Serializable]
    public sealed class VersionNumber
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        [SerializeField] private int major;
        [SerializeField] private int minor;
        [SerializeField] private int patch;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The major version number.
        /// </summary>
        public int Major => major;
        
        
        /// <summary>
        /// The minor version number.
        /// </summary>
        public int Minor => minor;
        
        
        /// <summary>
        /// The patch version number.
        /// </summary>
        public int Patch => patch;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Constructors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The blank constructor.
        /// </summary>
        public VersionNumber() { }

        
        /// <summary>
        /// Makes a new version number with the string entered.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        public VersionNumber(string value)
        {
            var split = value.Split('.');

            if (split.Length != 3) return;

            major = int.Parse(split[0]);
            minor = int.Parse(split[1]);
            patch = int.Parse(split[2]);
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets if the entry version number matches the entered string.
        /// </summary>
        /// <param name="toCompare">The version string to compare.</param>
        /// <returns>If the entry is a match or not on all values (major/minor/patch).</returns>
        public bool Match(string toCompare)
        {
            var versionNumberA = this;
            var versionNumberB = new VersionNumber(toCompare);

            return versionNumberA.Major.Equals(versionNumberB.Major) &&
                   versionNumberA.Minor.Equals(versionNumberB.Minor) &&
                   versionNumberA.Patch.Equals(versionNumberB.Patch);
        }


        /// <summary>
        /// Gets if the entry is a higher version than the converted version.
        /// </summary>
        /// <param name="toCompare">The version string to compare.</param>
        /// <returns>If the entry is greater on any (major/minor/patch) value.</returns>
        public bool IsHigherVersion(string toCompare)
        {
            var versionNumberA = this;
            var versionNumberB = new VersionNumber(toCompare);

            if (Match(toCompare))
            {
                return false;
            }

            return (versionNumberA.Major < versionNumberB.Major) ||
                   (versionNumberA.Minor < versionNumberB.Minor) || 
                   (versionNumberA.Patch < versionNumberB.Patch);
        }



        public override bool Equals(object obj)
        {
            if ((VersionNumber) obj == null) return false;
            return (Major == ((VersionNumber) obj).Major) && (Minor == ((VersionNumber) obj).Minor) && (Patch == ((VersionNumber) obj).Patch);
        }
        

        public static bool operator >(VersionNumber a, VersionNumber b)
        {
            if (a.Major > b.Major) return true;
            if (a.Minor > b.Minor) return true;
            return (a.Patch > b.Patch);
        }
        

        public static bool operator <(VersionNumber a, VersionNumber b)
        {
            if (a.Major < b.Major) return true;
            if (a.Minor < b.Minor) return true;
            return (a.Patch < b.Patch);
        }
    }
#pragma warning restore
}