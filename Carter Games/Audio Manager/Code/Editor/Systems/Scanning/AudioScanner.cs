/*
 * Copyright (c) 2018-Present Carter Games
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
using System.Linq;
using CarterGames.Common.Serializiation;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Scans for audio clips when a new audio clip is added to the project...
    /// </summary>
    public sealed class AudioScanner : AssetPostprocessor
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private static bool LibraryExists => ScriptableRef.HasLibraryFile;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Menu Items
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        [MenuItem("Tools/Carter Games/Audio Manager/Perform Manual Scan", priority = 22)]
        public static void ManualScan()
        {
            if (!PerUserSettings.ScannerInitialized) return;

            var option = EditorUtility.DisplayDialogComplex("Manual Audio Scan",
                "Do you want to do a clean scan of all files or ust find new ones not in the library?",
                "Clean Scan",
                "New Only Scan", "Cancel");

            if (option.Equals(2)) return;
            
            if (option.Equals(0))
            {
                ScanForAudio(true);
            }
            else
            {
                ScanForAudio(false);
            }
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   AssetPostprocessors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (importedAssets.Length > 0 && movedAssets.Length <= 0 && movedFromAssetPaths.Length <= 0)
            {
                if (!UtilEditor.HasInitialized)
                {
                    UtilEditor.Initialize();
                }

                CheckForAudioIfEmpty();
                AudioRemover.RemoveNullLibraryEntries();

                if (importedAssets.Any(t => t.Contains(".mixer")))
                {
                    UtilEditor.SetLibraryMixerGroups(GetAllMixersInProject());
                    StructHandler.RefreshMixers();
                }

                if (!PerUserSettings.ScannerHasNewAudioClip) return;

                if (GetAllClipsInProject(false, out var lookup))
                {
                    UtilEditor.SetLibraryData(lookup, false);

                    StructHandler.RefreshClips();

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                PerUserSettings.ScannerHasNewAudioClip = false;
            }
        }


        private void OnPostprocessAudio(AudioClip a)
        {
            PerUserSettings.ScannerHasNewAudioClip = true;
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public static void ScanForAudio(bool cleanScan)
        {
            AudioRemover.RemoveNullLibraryEntries();
            
            if (GetAllClipsInProject(cleanScan, out var lookup))
            {
                UtilEditor.SetLibraryData(lookup, cleanScan);

                var mixers = GetAllMixersInProject();
                UtilEditor.SetLibraryMixerGroups(mixers);
                
                StructHandler.RefreshClips();
                
                if (mixers != null)
                {
                    StructHandler.RefreshMixers();
                }
                
                EditorUtility.SetDirty(UtilEditor.Library);
            
                UtilEditor.LibraryObject.ApplyModifiedProperties();
                UtilEditor.LibraryObject.Update();
            
                AudioManagerEditorEvents.OnLibraryRefreshed.Raise();
            }

            PerUserSettings.ScannerHasScanned = true;
        }
        
        
        
        private static void CheckForAudioIfEmpty()
        {
            if (!LibraryExists)
            {
                CreateLibrary();
                return;
            }

            if (PerUserSettings.ScannerHasScanned) return;
            ManualScan();
        }
        
        
        private static void CreateLibrary()
        {
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance(typeof(AudioLibrary)), UtilEditor.LibraryAssetPath);
            ManualScan();
        }


        private static bool GetAllClipsInProject(bool cleanScan, out Dictionary<AudioData, bool> lookup)
        {
            lookup = new SerializableDictionary<AudioData, bool>();
            var alreadyIn = new List<AudioClip>();
            
            AssetDatabase.Refresh();
            
            var lib = AssetAccessor.GetAsset<AudioLibrary>();
            var assets = AssetDatabase.FindAssets("t:AudioClip", null);

            if (assets.Length <= 0) return false;
            
            var clips = new AudioClip[assets.Length];
            var foundNew = false;

            for (var i = 0; i < assets.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(assets[i]);
                clips[i] = (AudioClip)AssetDatabase.LoadAssetAtPath(path, typeof(AudioClip));
            }
            
            for (var i = 0; i < clips.Length; i++)
            {
                if (lib == null) break;
                
                // Probs a bit in-efficient, but it will work...
                if (lib.LibraryTotal > 0)
                {
                    if (!cleanScan)
                    {
                        for (int j = 0; j < UtilEditor.LibraryObject.Fp("library").Fpr("list").arraySize; j++)
                        {
                            if (UtilEditor.LibraryObject.Fp("library").Fpr("list").GetIndex(j).Fpr("value").Fpr("value")
                                    .objectReferenceValue == clips[i])
                            {
                                // Debug.Log($"Adding | {UtilEditor.LibraryObject.Fp("library").Fpr("list").GetIndex(j).Fpr("key").stringValue}");
                                lookup.Add(UtilEditor.Library.GetData(UtilEditor.LibraryObject.Fp("library").Fpr("list").GetIndex(j).Fpr("key").stringValue), false);
                                alreadyIn.Add((AudioClip) UtilEditor.LibraryObject.Fp("library").Fpr("list").GetIndex(j).Fpr("value").Fpr("value").objectReferenceValue);
                            }
                        }
                    }
                }
                
                if (alreadyIn.Contains(clips[i]))
                {
                    // Debug.Log("Already IN - Skipped");
                    continue;
                }
                    
                var toAdd = new AudioData(clips[i].name, clips[i], AssetDatabase.GetAssetPath(clips[i]));

                if (!DynamicTimeDetector.TryDetectStartTime(clips[i], out var time))
                {
                    lookup.Add(toAdd, true);
                    foundNew = true;
                    continue;
                }
                
                toAdd.dynamicStartTime = time;
                lookup.Add(toAdd, true);
                foundNew = true;
            }

            return foundNew;
        }
        
        
        private static AudioMixerGroup[] GetAllMixersInProject()
        {
            var assets = AssetDatabase.FindAssets("t:AudioMixerGroup", null);

            if (assets.Length <= 0) return null;

            var mixers = new List<AudioMixerGroup>();

            for (var i = 0; i < assets.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(assets[i]);
                var mixer = (AudioMixer)AssetDatabase.LoadAssetAtPath(path, typeof(AudioMixer));

                for (var j = 0; j < mixer.FindMatchingGroups(string.Empty).Length; j++)
                {
                    if (mixers.Contains(mixer.FindMatchingGroups(string.Empty)[j])) continue;
                    mixers.Add(mixer.FindMatchingGroups(string.Empty)[j]);
                }
            }

            return mixers.ToArray();
        }
    }
}