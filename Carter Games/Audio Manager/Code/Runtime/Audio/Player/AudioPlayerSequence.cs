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
using CarterGames.Common;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles one or many audio players for a request to be fulfilled.
    /// </summary>
    public sealed class AudioPlayerSequence : MonoBehaviour
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private readonly List<AudioPlayer> assignedPlayers = new List<AudioPlayer>();
        private ISequenceHandler sequenceHandler;
        private List<string> requests;
        private List<AudioClipSettings> clipSettingsList;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets/Sets if the sequence is initialized.
        /// </summary>
        private bool IsInitialized { get; set; }
        
        
        /// <summary>
        /// Gets all the audio players assigned to this sequence.
        /// </summary>
        public List<AudioPlayer> Players => assignedPlayers;
        
        
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
        public bool IsPlaying => Players.Any(t => t.IsPlaying);
        
        
        public bool RecycleOnComplete { get; set; }

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
        public void AssignNewPlayer()
        {
            var player = AudioPool.AssignPlayer();
            assignedPlayers.Add(player);
            player.transform.SetParent(transform);
        }


        /// <summary>
        /// Initializes the sequence when called.
        /// </summary>
        private void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;

            assignedPlayers.AddRange(GetComponentsInChildren<AudioPlayer>(true));
        }

        
        /// <summary>
        /// Prepares the sequence for a single clip when called.
        /// </summary>
        /// <param name="request">The request to use.</param>
        /// <param name="requestSettings">The settings for the request.</param>
        public void Prepare(string request, AudioClipSettings requestSettings)
        {
            Initialize();
            
            sequenceHandler = new SingleRequestSequence(this, request, requestSettings);
            sequenceHandler.Setup();
        }


        /// <summary>
        /// Prepares the sequence for a groups of clips when called.
        /// </summary>
        /// <param name="request">The request to use.</param>
        /// <param name="requestSettings">The settings for the request.</param>
        public void PrepareGroup(string request, AudioClipSettings requestSettings)
        {
            Initialize();

            var data = AssetAccessor.GetAsset<AudioLibrary>().GetGroup(request);

            if (data == null) return;
            
            switch (data.PlayMode)
            {
                case GroupPlayMode.Random:
                    sequenceHandler = new RandomGroupRequestSequence(this, data, requestSettings);
                    break;
                case GroupPlayMode.Sequential:
                    sequenceHandler = new SequentialGroupRequestSequence(this, data, requestSettings);
                    break;
                case GroupPlayMode.Combined:
                    sequenceHandler = new CombinedGroupRequestSequence(this, data, requestSettings);
                    break;
            }
            
            sequenceHandler.Setup();
        }
        
        
        /// <summary>
        /// Prepares the sequence for a groups of clips when called.
        /// </summary>
        /// <param name="request">The request to use.</param>
        /// <param name="playMode">The play mode for the group request.</param>
        /// <param name="requestSettings">The settings for the request.</param>
        public void PrepareGroup(IEnumerable<string> request, GroupPlayMode playMode, AudioClipSettings requestSettings)
        {
            Initialize();

            switch (playMode)
            {
                case GroupPlayMode.Random:
                    sequenceHandler = new RandomGroupRequestSequence(this, new GroupData(Guid.NewGuid().ToString(), request), requestSettings);
                    break;
                case GroupPlayMode.Sequential:
                    sequenceHandler = new SequentialGroupRequestSequence(this, new GroupData(Guid.NewGuid().ToString(), request), requestSettings);
                    break;
                case GroupPlayMode.Combined:
                    sequenceHandler = new CombinedGroupRequestSequence(this, new GroupData(Guid.NewGuid().ToString(), request), requestSettings);
                    break;
            }
            
            sequenceHandler.Setup();
        }


        /// <summary>
        /// Plays the sequence when called.
        /// </summary>
        public void Play()
        {
            Started.Raise();
            gameObject.SetActive(true);
            PlayersCompleted = 0;
            sequenceHandler.Play();
        }


        /// <summary>
        /// Pauses the sequence when called.
        /// </summary>
        public void Pause()
        {
            sequenceHandler.Pause();
            Paused.Raise();
        }
        
        
        /// <summary>
        /// Resumes the sequence when called.
        /// </summary>
        public void Resume()
        {
            sequenceHandler.Resume();
            Resumed.Raise();
        }
        

        /// <summary>
        /// Stops the sequence when called.
        /// </summary>
        public void Stop()
        {
            sequenceHandler.Stop();
            Stopped.Raise();
        }


        /// <summary>
        /// Runs when a player in the sequence is complete.
        /// </summary>
        public void PlayerComplete()
        {
            PlayersCompleted++;

            if (!PlayersCompleted.Equals(Players.Count)) return;
            
            if (IsLooped)
            {
                Loop();
                return;
            }
                
            PlayersCompleted = 0;
                
            Completed.Raise();

            if (!RecycleOnComplete) return;

            foreach (var player in Players)
            {
                AudioPool.Return(player);
            }
            
            AudioPool.Return(this);
        }


        /// <summary>
        /// Runs when the sequence is called to loop.
        /// </summary>
        private void Loop()
        {
            if (!PlayersCompleted.Equals(Players.Count)) return;
            
            LoopInfo.CurrentLoopCount++;

            if (IsLoopCompleted)
            {
                Completed.Raise();
                return;
            }

            foreach (var player in Players)
            {
                player.ProcessLoopEdits();
            }
            
            sequenceHandler.OnLoop();
            Looped.Raise();
            Play();
        }


        /// <summary>
        /// Call to manually recycle this element into the pooling setup.
        /// </summary>
        public void RecycleManually()
        {
            foreach (var player in Players)
            {
                AudioPool.Return(player);
            }
            
            AudioPool.Return(this);
        }
    }
}