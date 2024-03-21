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
    /// Removes AudioClips from the library when the clip is removed the project...
    /// </summary>
    public sealed class AudioRemover : UnityEditor.AssetModificationProcessor
    {
        private static int loggedDataRemovals = 0;
        private static int loggedMixerRemovals = 0;
        

        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            RemoveNullEntriesInLibrary(assetPath);
            return AssetDeleteResult.DidNotDelete;
        }
        
        
        private static void RemoveNullEntriesInLibrary(string removed)
        {
            loggedDataRemovals = 0;
            loggedMixerRemovals = 0;
            
            
            // Finds any path removals and removes them from the library...
            if (removed.Contains(".mixer"))
            {
                TryRemovePathMixerEntry(removed);
            }
            else if (UtilEditor.LibraryObject.Fp("library").Fpr("list").arraySize > 0)
            {
                TryRemovePathAudioDataEntry(removed);
            }
            
            
            // Removed any entries from the library than have a null/missing value assigned to them...
            RemoveNullLibraryEntries();
            

            // Updates the structs if any of the removals have been valid...
            if (loggedDataRemovals > 0)
            {
                StructHandler.RefreshClips();
            }

            if (loggedMixerRemovals > 0)
            {
                StructHandler.RefreshMixers();
            }
        }



        private static void TryRemovePathMixerEntry(string path)
        {
            var indexOfPath = -1;
            var indexOfReversedPath = -1;
            
            for (var i = 0; i < UtilEditor.LibraryObject.Fp("mixers").Fpr("list").arraySize; i++)
            {
                var keyPair = UtilEditor.LibraryObject.Fp("mixers").Fpr("list").GetIndex(i);

                if (keyPair.Fpr("value").Fpr("path").stringValue.Equals(path))
                {
                    indexOfPath = i;
                    break;
                }
            }
            
            // if not found a matching path, give up now....
            if (indexOfPath.Equals(-1)) return;

            var entry = UtilEditor.LibraryObject.Fp("mixers").Fpr("list").GetIndex(indexOfPath);
            
            for (var i = 0; i < UtilEditor.LibraryObject.Fp("mixersReverseLookup").Fpr("list").arraySize; i++)
            {
                var keyPair = UtilEditor.LibraryObject.Fp("mixersReverseLookup").Fpr("list").GetIndex(i);

                if (keyPair.Fpr("value").stringValue.Equals(entry.Fpr("key").stringValue))
                {
                    indexOfReversedPath = i;
                    break;
                }
            }

            if (PerUserSettings.LastLibMixerEntry == indexOfPath)
            {
                PerUserSettings.LastLibMixerEntry = -1;
            }
            
            UtilEditor.LibraryObject.Fp("mixers").Fpr("list").DeleteIndex(indexOfPath);
            UtilEditor.LibraryObject.Fp("mixersReverseLookup").Fpr("list").DeleteIndex(indexOfReversedPath);

            loggedMixerRemovals++;
            
            UtilEditor.LibraryObject.ApplyModifiedProperties();
            UtilEditor.LibraryObject.Update();
        }


        private static void TryRemovePathAudioDataEntry(string path)
        {
            var indexOfPath = -1;
            var indexOfReversedPath = -1;
            
            for (var i = 0; i < UtilEditor.LibraryObject.Fp("library").Fpr("list").arraySize; i++)
            {
                var keyPair = UtilEditor.LibraryObject.Fp("library").Fpr("list").GetIndex(i);

                if (keyPair.Fpr("value").Fpr("path").stringValue.Equals(path))
                {
                    indexOfPath = i;
                    break;
                }
            }
            
            
            // if not found a matching path, give up now....
            if (indexOfPath.Equals(-1)) return;

            var entry = UtilEditor.LibraryObject.Fp("library").Fpr("list").GetIndex(indexOfPath);

            RemoveEntryFromAnyGroups(entry);
            RemoveEntryFromAnyTrackList(entry);
            
            for (var i = 0; i < UtilEditor.LibraryObject.Fp("libraryReverseLookup").Fpr("list").arraySize; i++)
            {
                var keyPair = UtilEditor.LibraryObject.Fp("libraryReverseLookup").Fpr("list").GetIndex(i);

                if (keyPair.Fpr("value").stringValue.Equals(entry.Fpr("key").stringValue))
                {
                    indexOfReversedPath = i;
                    break;
                }
            }
            
            if (PerUserSettings.LastLibraryIndexShown == indexOfPath)
            {
                PerUserSettings.LastLibraryIndexShown = -1;
            }
            
            UtilEditor.LibraryObject.Fp("library").Fpr("list").DeleteIndex(indexOfPath);
            UtilEditor.LibraryObject.Fp("libraryReverseLookup").Fpr("list").DeleteIndex(indexOfReversedPath);
            
            loggedDataRemovals++;
            
            UtilEditor.LibraryObject.ApplyModifiedProperties();
            UtilEditor.LibraryObject.Update();
        }


        private static void RemoveEntryFromAnyGroups(SerializedProperty entry)
        {
            var clipName = entry.Fpr("value").Fpr("key").stringValue;
            
            for (var i = 0; i < UtilEditor.LibraryObject.Fp("groups").Fpr("list").arraySize; i++)
            {
                var keyPair = UtilEditor.LibraryObject.Fp("groups").Fpr("list").GetIndex(i);
                var adjusted = 0;
                    
                for (var j = 0; j < keyPair.Fpr("value").Fpr("clipNames").arraySize; j++)
                {
                    if (clipName != keyPair.Fpr("value").Fpr("clipNames").GetIndex(j - adjusted).stringValue) continue;
                    keyPair.Fpr("value").Fpr("clipNames").DeleteIndex(j - adjusted);
                    adjusted += 1;
                }
            }
        }


        private static void RemoveEntryFromAnyTrackList(SerializedProperty entry)
        {
            var clipName = entry.Fpr("value").Fpr("key").stringValue;
            
            for (var i = 0; i < UtilEditor.LibraryObject.Fp("tracks").Fpr("list").arraySize; i++)
            {
                var keyPair = UtilEditor.LibraryObject.Fp("tracks").Fpr("list").GetIndex(i);
                var adjusted = 0;
                    
                for (var j = 0; j < keyPair.Fpr("value").Fpr("tracks").arraySize; j++)
                {
                    if (clipName != keyPair.Fpr("value").Fpr("tracks").GetIndex(j - adjusted).Fpr("clipKey").stringValue) continue;
                    keyPair.Fpr("value").Fpr("tracks").DeleteIndex(j - adjusted);
                    adjusted += 1;
                }
            }
        }


        public static void RemoveNullLibraryEntries()
        {
            var indexAdjustment = 0;
            var loops = UtilEditor.LibraryObject.Fp("library").Fpr("list").arraySize;
            
            // Audio Data...
            for (var i = 0; i < loops; i++)
            {
                var adjustedIndex = i - indexAdjustment;
                var reversedIndex = -1;
                var entry = UtilEditor.LibraryObject.Fp("library").Fpr("list").GetIndex(adjustedIndex);

                if (entry.Fpr("value").Fpr("value").objectReferenceValue != null) continue;

                RemoveEntryFromAnyGroups(entry);
                RemoveEntryFromAnyTrackList(entry);

                for (var k = 0; k < UtilEditor.LibraryObject.Fp("libraryReverseLookup").Fpr("list").arraySize; k++)
                {
                    var keyPair = UtilEditor.LibraryObject.Fp("libraryReverseLookup").Fpr("list").GetIndex(k);

                    if (keyPair.Fpr("value").stringValue.Equals(entry.Fpr("key").stringValue))
                    {
                        reversedIndex = k;
                        break;
                    }
                }
                
                if (PerUserSettings.LastLibraryIndexShown == adjustedIndex)
                {
                    PerUserSettings.LastLibraryIndexShown = -1;
                }
                
                UtilEditor.LibraryObject.Fp("library").Fpr("list").DeleteIndex(adjustedIndex);
                UtilEditor.LibraryObject.Fp("libraryReverseLookup").Fpr("list").DeleteIndex(reversedIndex);
                indexAdjustment++;
                loggedDataRemovals++;
            }
            
            // Mixers...
            indexAdjustment = 0;
            loops = UtilEditor.LibraryObject.Fp("mixers").Fpr("list").arraySize;

            for (var i = 0; i < loops; i++)
            {
                var adjustedIndex = i - indexAdjustment;
                var reversedIndex = -1;
                var entry = UtilEditor.LibraryObject.Fp("mixers").Fpr("list").GetIndex(adjustedIndex);
                
                if (entry.Fpr("value").Fpr("mixerGroup").objectReferenceValue != null) continue;

                for (var k = 0; k < UtilEditor.LibraryObject.Fp("mixersReverseLookup").Fpr("list").arraySize; k++)
                {
                    var keyPair = UtilEditor.LibraryObject.Fp("mixersReverseLookup").Fpr("list").GetIndex(k);

                    if (keyPair.Fpr("value").stringValue.Equals(entry.Fpr("key").stringValue))
                    {
                        reversedIndex = k;
                        break;
                    }
                }
                
                if (PerUserSettings.LastLibMixerEntry == adjustedIndex)
                {
                    PerUserSettings.LastLibMixerEntry = -1;
                }
                
                UtilEditor.LibraryObject.Fp("mixers").Fpr("list").DeleteIndex(adjustedIndex);
                UtilEditor.LibraryObject.Fp("mixersReverseLookup").Fpr("list").DeleteIndex(reversedIndex);
                indexAdjustment++;
                loggedMixerRemovals++;
            }
            
            UtilEditor.LibraryObject.ApplyModifiedProperties();
            UtilEditor.LibraryObject.Update();
        }
    }
}