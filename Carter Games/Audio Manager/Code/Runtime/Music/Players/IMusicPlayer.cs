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

using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Implement to make a music player.
    /// </summary>
    public interface IMusicPlayer
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets if the player is playing.
        /// </summary>
        bool IsPlaying { get; set; }
        
        
        /// <summary>
        /// Gets if the player is transitioning.
        /// </summary>
        bool IsTransitioning { get; set; }
        
        
        /// <summary>
        /// The base volume of the source for this player.
        /// </summary>
        float Volume { get; set; }
        
        
        /// <summary>
        /// Gets the track list.
        /// </summary>
        MusicPlaylist Playlist { get; set; }
        
        
        /// <summary>
        /// Gets the default transition.
        /// </summary>
        IMusicTransition DefaultVolumeTransition { get; set; }
        
        
        /// <summary>
        /// Gets the active transition.
        /// </summary>
        IMusicTransition Transition { get; }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Call to play the player.
        /// </summary>
        void Play();
        
        
        /// <summary>
        /// Call to pause the player.
        /// </summary>
        void Pause();
        
        
        /// <summary>
        /// Call to resume the player.
        /// </summary>
        void Resume();
        
        
        /// <summary>
        /// Call to stop the player.
        /// </summary>
        void Stop();
        
        
        /// <summary>
        /// Call to transition in the track.
        /// </summary>
        void TransitionIn();
        
        
        /// <summary>
        /// Call to transition out the track.
        /// </summary>
        void TransitionOut();
        
        
        /// <summary>
        /// Skips the track forwards.
        /// </summary>
        void SkipForwards(bool smoothTransition = true);
        
        
        /// <summary>
        /// Skips the track backwards.
        /// </summary>
        /// <param name="replayCurrentFirst">Should the current track restart first? Def: true</param>
        /// <param name="smoothTransition">Should the switching transition run when going backwards. Def: true</param>
        void SkipBackwards(bool replayCurrentFirst = true, bool smoothTransition = true);


        /// <summary>
        /// The first track in to play.
        /// </summary>
        /// <param name="audioClip">The clip to set. It must be in the track list...</param>
        void SetFirstTrack(AudioClip audioClip);
        
        
        /// <summary>
        /// The first track in to play.
        /// </summary>
        /// <param name="audioClip">The clip to set. It must be in the track list...</param>
        void SetTrack(AudioClip audioClip);


        /// <summary>
        /// Sets the transition to a custom one, overriding the default.
        /// </summary>
        /// <param name="musicTransition">The transition to use.</param>
        void SetTransition(IMusicTransition musicTransition);
    }
}