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

using System.Collections.Generic;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the removal of any null entries in the defined tracks in any track list.
    /// </summary>
    public sealed class TracksCleanupScanProcess : IScanProcess
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Holds all the track list keys and clip keys to clear.
        /// </summary>
        private Dictionary<string, List<string>> ClipsIdsToClear { get; set; } = new Dictionary<string, List<string>>();
        
        
        /// <summary>
        /// Defines the oder this scan processor runs in.
        /// </summary>
        public int Priority => 22;
        
        
        /// <summary>
        /// Defines if this processor did something or not.
        /// </summary>
        public bool DidSomething { get; set; }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Works out if the library needs updates based on what this processor is used for.
        /// </summary>
        /// <returns>Gets if the library should be updated by this processor or not.</returns>
        public bool ShouldUpdateLibrary()
        {
            ClipsIdsToClear.Clear();
            
            foreach (var trackList in UtilEditor.Library.MusicTrackLookup)
            {
                foreach (var trackInList in trackList.Value.GetTracksRaw())
                {
                    if (UtilEditor.Library.LibraryLookup.ContainsKey(trackInList.ClipId)) continue;

                    if (ClipsIdsToClear.ContainsKey(trackList.Key))
                    {
                        ClipsIdsToClear[trackList.Key].Add(trackInList.ClipId);
                    }
                    else
                    {
                        ClipsIdsToClear.Add(trackList.Key, new List<string>() { trackInList.ClipId });
                    }
                }
            }

            return ClipsIdsToClear.Count > 0;
        }

        
        /// <summary>
        /// Updates the library with the required changes when called.
        /// </summary>
        public void UpdateLibrary()
        {
            DidSomething = false;

            if (!ShouldUpdateLibrary()) return;

            var trackListRef = UtilEditor.LibraryObject.Fp("tracks").Fpr("list");
            
            foreach (var trackListKey in ClipsIdsToClear.Keys)
            {
                foreach (var clipIdsToClear in ClipsIdsToClear[trackListKey])
                {
                    for (var i = 0; i < trackListRef.arraySize; i++)
                    {
                        if (!trackListRef.GetIndex(i).Fpr("key").stringValue.Equals(trackListKey)) continue;

                        for (var j = 0; j < trackListRef.GetIndex(i).Fpr("value").arraySize; j++)
                        {
                            if (!trackListRef.GetIndex(i).Fpr("value").GetIndex(j).Fpr("clipId").stringValue.Equals(clipIdsToClear)) continue;
                            trackListRef.GetIndex(i).Fpr("value").DeleteIndex(j);
                            break;
                        }
                    }
                }
            }

            UtilEditor.LibraryObject.ApplyModifiedProperties();
            UtilEditor.LibraryObject.Update();

            DidSomething = true;
        }
    }
}