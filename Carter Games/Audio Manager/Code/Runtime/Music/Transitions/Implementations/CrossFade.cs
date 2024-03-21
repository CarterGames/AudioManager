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
    /// Handles a cross fade volume transition for music tracks.
    /// </summary>
    public sealed class CrossFade : IMusicTransition
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        // Cached Id's to avoid spelling mistakes elsewhere in the script.
        private const string RoutineId = "transition_crossFade";
        private const string DirectionId = "direction";
        private const string DurationId = "duration";
        private const string ClipId = "musicClip";
        private const string ClipStartTimeId = "musicClipStartTime";
        private const string TimeScaleId = "unscaledTime";
        
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
        |   Constructors
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public CrossFade(float duration)
        {
            Data = new TransitionData(duration);
            IsComplete = false;
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Runs the transition when called.
        /// </summary>
        /// <param name="transitionDirection">The transition type to run.</param>
        public void Transition(TransitionDirection transitionDirection)
        {
            IsComplete = false;
            
            Data.CreateOrSetParam(DirectionId, transitionDirection);
            
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
            
            var unscaled = Data.GetParam<bool>(TimeScaleId);
            var dir = Data.GetParam<TransitionDirection>(DirectionId);
            var duration = Data.GetParam<float>(DurationId);
            var t = 0f;

            if (dir != TransitionDirection.InAndOut)
            {
                yield return Co_TransitionStandard(dir, t, duration, unscaled);
                
                IsComplete = true;
                Completed.Raise();
                
                MusicRoutineHandler.StopRoutine(RoutineId);
                
                MusicManager.TransitionCompleted.Raise();
                MusicManager.TransitionCompletedCtx.Raise(Data.Id, this);
            }
            else
            {
                yield return Co_TransitionInOut(t, duration, unscaled);

                IsComplete = true;
                Completed.Raise();
                
                MusicRoutineHandler.StopRoutine(RoutineId);
                
                MusicManager.TransitionCompleted.Raise();
                MusicManager.TransitionCompletedCtx.Raise(Data.Id, this);
            }

            InProgress = false;
        }
        
        
        /// <summary>
        /// Runs a in or out transition.
        /// </summary>
        /// <param name="dir">The direction to transition to.</param>
        /// <param name="t">The time currently in the transition.</param>
        /// <param name="duration">The duration of the transition.</param>
        /// <param name="unscaled">Should it use unscaled time.</param>
        private IEnumerator Co_TransitionStandard(TransitionDirection dir, float t, float duration, bool unscaled)
        {
            var startingVolume = MusicManager.MusicSource.Standard.MainSource.volume;
            
            while (t < duration)
            {
                t += unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                
                if (dir == TransitionDirection.In)
                {
                    MusicManager.MusicSource.Standard.MainSource.volume = Mathf.Clamp((t / duration), 0f, startingVolume);
                }
                else
                {
                    MusicManager.MusicSource.Standard.MainSource.volume = Mathf.Clamp(1 - (t / duration), 0f, startingVolume);
                }
                
                yield return null;
            }
        }
        
        
        /// <summary>
        /// Runs a in or out transition.
        /// </summary>
        /// <param name="t">The time currently in the transition.</param>
        /// <param name="duration">The duration of the transition.</param>
        /// <param name="unscaled">Should it use unscaled time.</param>
        private IEnumerator Co_TransitionInOut(float t, float duration, bool unscaled)
        {
            var startingVolume = MusicManager.MusicSource.Standard.MainSource.volume;
            
            if (Data.HasParam(ClipId))
            {
                MusicManager.MusicSource.Standard.TransitionSource.clip = Data.GetParam<AudioClip>(ClipId);
                MusicManager.MusicSource.Standard.TransitionSource.time = Data.GetParam(ClipStartTimeId, 0f);
                MusicManager.MusicSource.Standard.TransitionSource.Play();
            }
            
            while (t < duration)
            {
                t += unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                
                MusicManager.MusicSource.Standard.MainSource.volume = Mathf.Clamp(1 - (t / duration), 0f, startingVolume);
                MusicManager.MusicSource.Standard.TransitionSource.volume = Mathf.Clamp((t / duration), 0f, startingVolume);
                
                yield return null;
            }
            
            MusicManager.MusicSource.Standard.SwitchSourceReferences();
            MusicManager.MusicSource.Standard.TransitionSource.Stop();
        }
    }
}