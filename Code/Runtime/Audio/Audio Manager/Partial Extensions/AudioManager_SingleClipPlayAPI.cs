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

using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    public static partial class AudioManager
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Play Methods (Single Request)
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer Play(string request, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var instance = InternalPrepare(request, edits);
            instance.Play();
            return instance;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="startTime">The start time for the clip.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayFromTime(string request, float startTime, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var instance = InternalPrepare(request, new StartTimeEdit(startTime), edits);
            instance.Play();
            return instance;
        }
        
       
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="delay">The start delay for the clip.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayWithDelay(string request, float delay, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var instance = InternalPrepare(request, new DelayEdit(delay), edits);
            instance.Play();
            return instance;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayAtLocation(string request, Vector2 position, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var instance = InternalPrepare(request, new PositionEdit(position), edits);
            instance.Play();
            return instance;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayAtLocation(string request, Vector3 position, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var instance = InternalPrepare(request, new PositionEdit(position), edits);
            instance.Play();
            return instance;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="useLocalPosition">Should use the local position on the inputted transform?.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayAtLocation(string request, Transform position, bool useLocalPosition, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var instance = InternalPrepare(request, new PositionEdit(position, useLocalPosition), edits);
            instance.Play();
            return instance;
        }


        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer Play(string request, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var instance = InternalPrepare(request, new IEditModule[]
            {
                new VolumeEdit(volume ?? 1f), 
                new PitchEdit(pitch ?? 1f)
            }, edits);
            
            instance.Play();
            return instance;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="startTime">The start time for the clip.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayFromTime(string request, float startTime, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var instance = InternalPrepare(request, new IEditModule[] 
            {  
                new VolumeEdit(volume ?? 1f), 
                new PitchEdit(pitch ?? 1f), 
                new StartTimeEdit(startTime)
            }, edits);
            
            instance.Play();
            return instance;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="delay">The start delay for the clip.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayWithDelay(string request, float delay, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var instance = InternalPrepare(request, new IEditModule[] 
            {  
                new VolumeEdit(volume ?? 1f), 
                new PitchEdit(pitch ?? 1f), 
                new DelayEdit(delay)
            }, edits);
            
            instance.Play();
            return instance;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayAtLocation(string request, Vector2 position, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var instance = InternalPrepare(request, new IEditModule[] 
            {  
                new VolumeEdit(volume ?? 1f), 
                new PitchEdit(pitch ?? 1f), 
                new PositionEdit(position)
            }, edits);
            
            instance.Play();
            return instance;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayAtLocation(string request, Vector3 position, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var instance = InternalPrepare(request, new IEditModule[] 
            {  
                new VolumeEdit(volume ?? 1f), 
                new PitchEdit(pitch ?? 1f), 
                new PositionEdit(position)
            }, edits);
            
            instance.Play();
            return instance;
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="useLocalPosition">Should use the local position on the inputted transform?.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayer PlayAtLocation(string request, Transform position, bool useLocalPosition, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            if (Settings.PlayAudioState == PlayState.Disabled) return null;
            
            var instance = InternalPrepare(request, new IEditModule[] 
            {  
                new VolumeEdit(volume ?? 1f), 
                new PitchEdit(pitch ?? 1f), 
                new PositionEdit(position, useLocalPosition)
            }, edits);
            
            instance.Play();
            return instance;
        }
    }
}