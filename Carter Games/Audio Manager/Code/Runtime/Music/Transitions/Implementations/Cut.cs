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

using System.Collections;
using CarterGames.Common;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles a cut transition for music tracks.
    /// </summary>
    public sealed class Cut : IMusicTransition
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        // Cached Id's to avoid spelling mistakes elsewhere in the script.
        private const string RoutineId = "transition_cut";
        private const string ClipId = "musicClip";
        private const string ClipStartTimeId = "musicClipStartTime";
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets if the transition is completed or not.
        /// </summary>
        public bool IsComplete { get; set; }

        
        /// <summary>
        /// Gets if the transition is in progress or not.
        /// </summary>
        public bool InProgress { get; set; }


        /// <summary>
        /// Contains the generic data for the transition.
        /// </summary>
        public TransitionData Data { get; set; }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Events
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Raises when the transition has been completed.
        /// </summary>
        public Evt Completed { get; set; } = new Evt();
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Conmstructors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        public Cut()
        {
            Data = new TransitionData();
            // Debug.LogError("dhjsfh");
            IsComplete = false;
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public void Transition(TransitionDirection transitionDirection)
        {
            IsComplete = false;
            
            MusicRoutineHandler.RunRoutine(RoutineId, Co_RunTransition());
            
            MusicManager.TransitionStarted.Raise();
            MusicManager.TransitionStartedCtx.Raise(Data.Id, this);
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Coroutines
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Actually does the transition logic.
        /// </summary>
        public IEnumerator Co_RunTransition()
        {
            InProgress = true;
            
            yield return null;
            
            MusicManager.MusicSource.Standard.MainSource.clip = Data.GetParam<AudioClip>(ClipId);

            // Debug.LogError(Data.GetParam<AudioClip>(ClipId));
            
            MusicManager.MusicSource.Standard.MainSource.time = Data.GetParam(ClipStartTimeId, 0f);
            MusicManager.MusicSource.Standard.MainSource.volume = MusicManager.PlayerVolume;
            MusicManager.MusicSource.Standard.MainSource.Play();
            
            IsComplete = true;
            Completed.Raise();
            
            MusicManager.TransitionCompleted.Raise();
            MusicManager.TransitionCompletedCtx.Raise(Data.Id, this);

            InProgress = false;
        }
    }
}