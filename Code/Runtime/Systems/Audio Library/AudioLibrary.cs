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
using System.Collections.Generic;
using System.Linq;
using CarterGames.Assets.AudioManager.Logging;
using CarterGames.Shared.AudioManager;
using CarterGames.Shared.AudioManager.Serializiation;
using UnityEngine;
using UnityEngine.Audio;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Holds all the clips
    /// </summary>
    [Serializable]
    public class AudioLibrary : AmDataAsset
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        [SerializeField] private SerializableDictionary<string, AudioData> library;
        [SerializeField] private SerializableDictionary<string, GroupData> groups = new SerializableDictionary<string, GroupData>();
        [SerializeField] private SerializableDictionary<string, MixerData> mixers;
        [SerializeField] private List<string> categories;
        [SerializeField] private List<string> tags;
        
        private SerializableDictionary<string, string> cacheLibraryReverseLookup;
        private SerializableDictionary<string, string> cacheGroupsReverseLookup;
        private SerializableDictionary<string, string> cacheMixersReverseLookup;
        private SerializableDictionary<string, List<string>> cacheClipByTagLookup;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The lookup of all the library entries.
        /// </summary>
        public SerializableDictionary<string, AudioData> LibraryLookup => library;


        /// <summary>
        /// A lookup of all the clips with clip key as the id and the uuid as the value.
        /// </summary>
        public SerializableDictionary<string, string> ReverseLibraryLookup
        {
            get
            {
                if (cacheLibraryReverseLookup is { } && cacheLibraryReverseLookup.Count > 0) return cacheLibraryReverseLookup;
                cacheLibraryReverseLookup = new SerializableDictionary<string, string>();

                foreach (var libraryEntry in LibraryLookup)
                {
                    cacheLibraryReverseLookup.Add(libraryEntry.Value.key, libraryEntry.Key);
                }
                
                return cacheLibraryReverseLookup;
            }
        }


        /// <summary>
        /// Gets the number of clips stored in the library...
        /// </summary>
        public int ClipCount
        {
            get
            {
                if (library == null) return 0;
                return library.Count;
            }
        }


        /// <summary>
        /// Gets the groups stored in the library...
        /// </summary>
        public List<GroupData> Groups => groups.Values.ToList();


        /// <summary>
        /// Gets the total number of mixers in the library.
        /// </summary>
        public int MixerCount => mixers.Count;

        
        /// <summary>
        /// Gets a lookup of all the groups in the library.
        /// </summary>
        public Dictionary<string, GroupData> GroupsLookup => groups;
        
        
        /// <summary>
        /// A lookup of all the clips with clip key as the id and the uuid as the value.
        /// </summary>
        public SerializableDictionary<string, string> ReverseGroupsLookup
        {
            get
            {
                if (cacheGroupsReverseLookup is { } && cacheGroupsReverseLookup.Count > 0) return cacheGroupsReverseLookup;
                cacheGroupsReverseLookup = new SerializableDictionary<string, string>();

                foreach (var libraryEntry in GroupsLookup)
                {
                    cacheGroupsReverseLookup.Add(libraryEntry.Value.GroupName, libraryEntry.Key);
                }
                
                return cacheGroupsReverseLookup;
            }
        }
        
        
        /// <summary>
        /// Gets a lookup of all the mixers in the library.
        /// </summary>
        public Dictionary<string, MixerData> MixerLookup => mixers;
        
        
        /// <summary>
        /// A lookup of all the mixers with the user key as the id and the uuid as the value.
        /// </summary>
        public SerializableDictionary<string, string> ReverseMixerLookup
        {
            get
            {
                if (cacheMixersReverseLookup is { } && cacheMixersReverseLookup.Count > 0) return cacheMixersReverseLookup;
                cacheMixersReverseLookup = new SerializableDictionary<string, string>();

                foreach (var libraryEntry in MixerLookup)
                {
                    cacheMixersReverseLookup.Add(libraryEntry.Value.Key, libraryEntry.Key);
                }
                
                return cacheMixersReverseLookup;
            }
        }
        
        
        
        public SerializableDictionary<string, List<string>> ClipByTagLookup
        {
            get
            {
                if (cacheClipByTagLookup is { } && cacheClipByTagLookup.Count > 0) return cacheClipByTagLookup;
                cacheClipByTagLookup = new SerializableDictionary<string, List<string>>();

                foreach (var libraryEntry in LibraryLookup)
                {
                    var data = libraryEntry.Value;
                    
                    foreach (var tagOnData in data.Tags)
                    {
                        if (!cacheClipByTagLookup.ContainsKey(tagOnData))
                        {
                            cacheClipByTagLookup.Add(tagOnData, new List<string>());
                        }
                        
                        cacheClipByTagLookup[tagOnData].Add(data.key);
                    }
                }
                
                return cacheClipByTagLookup;
            }
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets the clip requested if it is possible...
        /// </summary>
        /// <param name="request">The clip to find...</param>
        /// <returns>The audio data received...</returns>
        public bool TryGetClip(string request, out AudioData data)
        {
            data = null;
            
            if (!LibraryHelper.HasClip(request))
            {
                AmDebugLogger.Error(AudioManagerErrorCode.ClipCannotBeFound.CodeToMessage());
                return false;
            }
            
            if (ReverseLibraryLookup.ContainsKey(request))
            {
                data = LibraryLookup[ReverseLibraryLookup[request]];
                return true;
            }
            
            if (LibraryLookup.ContainsKey(request))
            {
                data = LibraryLookup[request];
                return true;
            }

            return false;
        }
        
        
        /// <summary>
        /// Gets the group of the requested string.
        /// </summary>
        /// <param name="request">The group to get.</param>
        /// <returns>The group found.</returns>
        public GroupData GetGroup(string request)
        {
            if (ReverseGroupsLookup.ContainsKey(request))
            {
                return GroupsLookup[ReverseGroupsLookup[request]];
            }
            
            if (GroupsLookup.ContainsKey(request))
            {
                return GroupsLookup[request];
            }
            
            AmDebugLogger.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.GroupCannotBeFound));
            return null;
        }


        /// <summary>
        /// Gets the mixer requested.
        /// </summary>
        /// <param name="request">The mixer to get.</param>
        /// <returns>The mixer group.</returns>
        public AudioMixerGroup GetMixer(string request) 
        {
            if (ReverseMixerLookup.ContainsKey(request))
            {
                return MixerLookup[ReverseMixerLookup[request]].MixerGroup;
            }
            
            if (MixerLookup.ContainsKey(request))
            {
                return MixerLookup[request].MixerGroup;
            }

            Debug.Log(request);
            AmDebugLogger.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.MixerCannotBeFound));
            return null;
        }
        

        
        /// <summary>
        /// Gets the index of the data in the library.
        /// </summary>
        /// <param name="audioData">The data to </param>
        /// <returns>The index of the data entered in the library.</returns>
        public int GetIndexOfData(AudioData audioData)
        {
            if (library.ContainsKey(audioData.id))
            {
                return library.Values.ToList().IndexOf(audioData);
            }

            return 0;
        }


        /// <summary>
        /// (EDITOR USE ONLY) Orders the library by the key for each clip when called.
        /// </summary>
        public void OrderLibrary()
        {
            if (Application.isPlaying)
            {
                AmDebugLogger.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.EditorOnlyMethod));
                return;
            }

            var dictionary = new SerializableDictionary<string, AudioData>();
            
            foreach (var pair in library.OrderBy(t => t.Key))
            {
                dictionary.Add(pair.Key, pair.Value);
            }
            
            library = dictionary;
        }


        /// <summary>
        /// (EDITOR USE ONLY) Sets the mixer groups in the library when called.
        /// </summary>
        /// <param name="inputMixerArray">The values to set.</param>
        public void SetMixerGroups(AudioMixerGroup[] inputMixerArray)
        {
            if (inputMixerArray == null) return;
            
            if (Application.isPlaying)
            {
                AmDebugLogger.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.EditorOnlyMethod));
                return;
            }

            foreach (var mixerValue in inputMixerArray)
            {
                if (mixers.Any(t => t.Value.MixerGroup.Equals(mixerValue))) continue;

                var uuid = Guid.NewGuid().ToString();
                var key = $"{mixerValue.name}{uuid.Substring(0, 7)}";
                
                mixers.Add(uuid, new MixerData(uuid,key, mixerValue));
                cacheMixersReverseLookup.Add(key, uuid);
            }
        }

        
        public void ResetLibraryToDefault()
        {
            library.Clear();
            cacheLibraryReverseLookup.Clear();
            
            mixers.Clear();
            cacheMixersReverseLookup.Clear();
        }
    }
}