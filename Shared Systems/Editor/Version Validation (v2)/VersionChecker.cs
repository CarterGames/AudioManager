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

namespace CarterGames.Shared.AudioManager.Editor
{
    /// <summary>
    /// Handles checking for the latest version.
    /// </summary>
    public static class VersionChecker
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The download URL for the latest version.
        /// </summary>
        public static string DownloadURL => VersionInfo.DownloadBaseUrl + VersionsPacket.Version;
        

        /// <summary>
        /// Gets if the latest version is this version.
        /// </summary>
        public static bool IsLatestVersion => VersionsPacket.VersionNumber.Equals(new Version(VersionInfo.ProjectVersionNumber));
        
        
        /// <summary>
        /// Gets if the version here is higher that the latest version.
        /// </summary>
        public static bool IsNewerVersion => new Version(VersionInfo.ProjectVersionNumber).CompareTo(VersionsPacket.VersionNumber) > 0;
        
        
        /// <summary>
        /// Gets the version data downloaded.
        /// </summary>
        public static VersionPacketSuccess VersionsPacket { get; private set; }

        
        /// <summary>
        /// The latest version string.
        /// </summary>
        public static string LatestVersionNumberString => VersionsPacket.Version;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Events
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Raises when the data has been downloaded.
        /// </summary>
        public static Evt ResponseReceived { get; private set; } = new Evt();
        
        
        /// <summary>
        /// Raises when the request failed for some reason.
        /// </summary>
        public static Evt<VersionPacketError> ErrorReceived { get; private set; } = new Evt<VersionPacketError>();
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets the latest version data when called.
        /// </summary>
        public static void GetLatestVersions()
        {
            RequestLatestVersionData();
        }


        /// <summary>
        /// Makes the web request & handles the response.
        /// </summary>
        private static void RequestLatestVersionData()
        {
            VersioningAPI.Query(OnSuccess, OnFailed);
            return;
            
            void OnSuccess(VersionPacketSuccess data)
            {
                VersionsPacket = data;
                ResponseReceived.Raise();
            }

            void OnFailed(VersionPacketError errorPacket)
            {
                ErrorReceived.Raise(errorPacket);
            }
        }
    }
}