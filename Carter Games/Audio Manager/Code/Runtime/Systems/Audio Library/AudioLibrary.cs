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

using System;
using System.Collections.Generic;
using System.Linq;
using CarterGames.Assets.AudioManager.Logging;
using CarterGames.Common.Serializiation;
using UnityEngine;
using UnityEngine.Audio;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Holds all the clips
    /// </summary>
    public class AudioLibrary : AudioManagerAsset
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        [SerializeField] private SerializableDictionary<string, AudioData> library;
        [SerializeField] private SerializableDictionary<string, string> libraryReverseLookup;
        
        [SerializeField] private SerializableDictionary<string, GroupData> groups = new SerializableDictionary<string, GroupData>();
        [SerializeField] private SerializableDictionary<string, string> groupsReverseLookup;

        [SerializeField] private SerializableDictionary<string, MixerData> mixers;
        [SerializeField] private SerializableDictionary<string, string> mixersReverseLookup;
        
        [SerializeField] private SerializableDictionary<string, MusicPlaylist> tracks;
        [SerializeField] private SerializableDictionary<string, string> tracksReverseLookup;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Gets the total entries in the library.
        /// </summary>
        public int LibraryTotal => LibraryLookup.Count;
        
        
        /// <summary>
        /// The lookup of all the library entries.
        /// </summary>
        public SerializableDictionary<string, AudioData> LibraryLookup => library;


        /// <summary>
        /// A lookup of all the clips with clip key as the id and the uuid as the value.
        /// </summary>
        public SerializableDictionary<string, string> ReverseLibraryLookup => libraryReverseLookup;
        

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
        public SerializableDictionary<string, string> ReverseGroupsLookup => groupsReverseLookup;
        
        
        /// <summary>
        /// Gets a lookup of all the mixers in the library.
        /// </summary>
        public Dictionary<string, MixerData> MixerLookup => mixers;
        
        
        /// <summary>
        /// A lookup of all the mixers with the user key as the id and the uuid as the value.
        /// </summary>
        public SerializableDictionary<string, string> ReverseMixerLookup => mixersReverseLookup;
        
        
        /// <summary>
        /// Gets a lookup of all the music tracks in the library.
        /// </summary>
        public Dictionary<string, MusicPlaylist> MusicTrackLookup => tracks;
        
        
        /// <summary>
        /// A lookup of all the clips with clip key as the id and the uuid as the value.
        /// </summary>
        public SerializableDictionary<string, string> ReverseTrackListLookup => tracksReverseLookup;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets the clip requested if it is possible...
        /// </summary>
        /// <param name="request">The clip to find...</param>
        /// <returns>The audio data received...</returns>
        public AudioData GetData(string request)
        {
            if (ReverseLibraryLookup.ContainsKey(request))
            {
                return LibraryLookup[ReverseLibraryLookup[request]];
            }
            
            if (LibraryLookup.ContainsKey(request))
            {
                return LibraryLookup[request];
            }
            
            AmDebugLogger.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.ClipCannotBeFound));
            return null;
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
            
            AmDebugLogger.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.MixerCannotBeFound));
            return null;
        }
        
        
        /// <summary>
        /// Gets the track list requested.
        /// </summary>
        /// <param name="request">The requested string.</param>
        /// <returns>The track list found.</returns>
        public MusicPlaylist GetTrackList(string request)
        {
            if (ReverseTrackListLookup.ContainsKey(request))
            {
                return MusicTrackLookup[ReverseTrackListLookup[request]];
            }
            
            if (MusicTrackLookup.ContainsKey(request))
            {
                return MusicTrackLookup[request];
            }
            
            AmDebugLogger.Warning(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.TrackListCannotBeFound));
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
                mixersReverseLookup.Add(key, uuid);
            }
        }

        
        public void ResetLibraryToDefault()
        {
            library.Clear();
            libraryReverseLookup.Clear();
            
            mixers.Clear();
            mixersReverseLookup.Clear();
        }
    }
}