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

using System;
using System.Collections.Generic;
using System.Linq;
using CarterGames.Shared.AudioManager;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles one or many audio players for a request to be fulfilled.
    /// </summary>
    public sealed class AudioPlayer : MonoBehaviour
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        [SerializeField] private AudioSourceInstance standardSource;
        [SerializeField] private List<AudioSourceInstance> additionalSources = new List<AudioSourceInstance>();

        private List<AudioSourceInstance> allSources;
        private IPlayMethod playMethod;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets/Sets if the player is initialized.
        /// </summary>
        private bool IsInitialized { get; set; }
        
        
        /// <summary>
        /// Gets the default audio player source.
        /// </summary>
        public AudioSourceInstance Source => standardSource;
        
        
        /// <summary>
        /// Gets all the additional audio players assigned to this sequence.
        /// </summary>
        public List<AudioSourceInstance> AdditionalSources => additionalSources;
        
        
        /// <summary>
        /// Gets all the additional audio players assigned to this sequence.
        /// </summary>
        public List<AudioSourceInstance> AllSources => allSources;
        
        
        /// <summary>
        /// Gets the loop edit info for this sequence to use.
        /// </summary>
        public LoopEdit LoopInfo { get; set; }
        
        
        /// <summary>
        /// Gets the number of completed players in the sequence.
        /// </summary>
        private int PlayersCompleted { get; set; }
        
        
        /// <summary>
        /// Gets if the sequence is a looping sequence or not.
        /// </summary>
        private bool IsLooped => LoopInfo != null;
        
        
        /// <summary>
        /// Gets if the sequence has completed its loop or not.
        /// </summary>
        private bool IsLoopCompleted => !LoopInfo.IsInfiniteLoop && LoopInfo.CurrentLoopCount.Equals(LoopInfo.LoopCount);


        /// <summary>
        /// Returns if any audio is currently being played from this sequence.
        /// </summary>
        public bool IsPlaying => AllSources.Any(t => t.IsPlaying);
        
        
        /// <summary>
        /// Defines if the player recycles to the pool again once complete.
        /// </summary>
        public bool RecycleOnComplete { get; private set; }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Events
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Raises when the sequence has started playing.
        /// </summary>
        public readonly Evt Started = new Evt();
        
        
        /// <summary>
        /// Raises when the sequence has looped.
        /// </summary>
        public readonly Evt Looped = new Evt();
        
        
        /// <summary>
        /// Raises when the sequence has completed playing.
        /// </summary>
        public readonly Evt Completed = new Evt();
        
        
        /// <summary>
        /// Raises when the sequence has stopped playing.
        /// </summary>
        public readonly Evt Stopped = new Evt();
        
        
        /// <summary>
        /// Raises when the sequence has been paused.
        /// </summary>
        public readonly Evt Paused = new Evt();
        
        
        /// <summary>
        /// Raises when the sequence has been resumed.
        /// </summary>
        public readonly Evt Resumed = new Evt();
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Assigns a new audio player from the pool to this sequence when called.
        /// </summary>
        public void AssignNewInstance()
        {
            var player = AudioPool.AssignSource();
            additionalSources.Add(player);
            player.transform.SetParent(transform);
            AllSources.Add(player);
        }


        /// <summary>
        /// Initializes the sequence when called.
        /// </summary>
        public void Initialize(string request, AudioClipSettings requestSettings)
        {
            if (IsInitialized) return;

            allSources = new List<AudioSourceInstance>()
            {
                standardSource
            };
            
            if (!AmAssetAccessor.GetAsset<AudioLibrary>().TryGetClip(request, out var data)) return;

            RecycleOnComplete = true;
            playMethod = SingleSourcePlayMethod.InitializePlayMethod(this, data, requestSettings);
            
            IsInitialized = true;
        }
        
        
        /// <summary>
        /// Initializes the sequence when called.
        /// </summary>
        public void InitializeGroup(string request, AudioClipSettings requestSettings)
        {
            if (IsInitialized) return;

            var data = AmAssetAccessor.GetAsset<AudioLibrary>().GetGroup(request);

            if (data == null) return;

            allSources = new List<AudioSourceInstance>()
            {
                standardSource
            };

            RecycleOnComplete = true;
            
            switch (data.PlayMode)
            {
                case GroupPlayMode.Random:
                    playMethod = GroupSourcePlayerRandom.InitializePlayMethod(this, data, requestSettings);
                    break;
                case GroupPlayMode.Sequential:
                    playMethod = new SequentialGroupRequestSequence(this, data, requestSettings);
                    break;
                case GroupPlayMode.Combined:
                    playMethod = new CombinedGroupRequestSequence(this, data, requestSettings);
                    break;
            }
            
            IsInitialized = true;
        }


        public void InitializeGroup(IEnumerable<string> request, GroupPlayMode playMode, AudioClipSettings requestSettings = null)
        {
            if (IsInitialized) return;

            var data = new GroupData(Guid.NewGuid().ToString(), request, playMode);

            RecycleOnComplete = true;
            
            switch (data.PlayMode)
            {
                case GroupPlayMode.Random:
                    playMethod = GroupSourcePlayerRandom.InitializePlayMethod(this, data, requestSettings);
                    break;
                case GroupPlayMode.Sequential:
                    playMethod = new SequentialGroupRequestSequence(this, data, requestSettings);
                    break;
                case GroupPlayMode.Combined:
                    playMethod = new CombinedGroupRequestSequence(this, data, requestSettings);
                    break;
            }
            
            IsInitialized = true;
        }


        /// <summary>
        /// Plays the sequence when called.
        /// </summary>
        public void Play()
        {
            Started.Raise();
            gameObject.SetActive(true);
            PlayersCompleted = 0;
            playMethod.Play();
        }


        /// <summary>
        /// Pauses the sequence when called.
        /// </summary>
        public void Pause()
        {
            playMethod.Pause();
            Paused.Raise();
        }
        
        
        /// <summary>
        /// Resumes the sequence when called.
        /// </summary>
        public void Resume()
        {
            playMethod.Resume();
            Resumed.Raise();
        }
        

        /// <summary>
        /// Stops the sequence when called.
        /// </summary>
        public void Stop()
        {
            playMethod.Stop();
            Stopped.Raise();
        }


        /// <summary>
        /// Runs when a player in the sequence is complete.
        /// </summary>
        public void PlayerComplete()
        {
            PlayersCompleted++;

            if (!PlayersCompleted.Equals(AllSources.Count)) return;
            
            if (IsLooped)
            {
                Loop();
                return;
            }
                
            PlayersCompleted = 0;
            Completed.Raise();

            if (!RecycleOnComplete) return;

            ReturnPlayerToPool();
        }


        /// <summary>
        /// Runs when the sequence is called to loop.
        /// </summary>
        private void Loop()
        {
            if (!PlayersCompleted.Equals(AllSources.Count)) return;
            
            LoopInfo.CurrentLoopCount++;

            if (IsLoopCompleted)
            {
                Completed.Raise();
                return;
            }

            foreach (var player in AllSources)
            {
                player.ProcessLoopEdits();
            }
            
            playMethod.OnLoop();
            Looped.Raise();
            Play();
        }


        private void ReturnPlayerToPool()
        {
            foreach (var sourceInstance in additionalSources)
            {
                sourceInstance.ResetSourceInstance(true);
                AudioPool.Return(sourceInstance);
            }
            
            additionalSources.Clear();
            allSources = new List<AudioSourceInstance>()
            {
                Source
            };

            standardSource.ResetSourceInstance(true);
            AudioPool.Return(this);
            IsInitialized = false;
        }
    }
}