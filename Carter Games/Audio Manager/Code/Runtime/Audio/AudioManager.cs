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
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// The main manager class of the asset to play audio clips.
    /// </summary>
    public static class AudioManager
    {
        /// <summary>
        /// Use to change the play state.
        /// </summary>
        /// <param name="playState">The state to set to.</param>
        public static void ChangePlayState(PlayState playState)
        {
            AssetAccessor.GetAsset<SettingsAssetRuntime>().PlayAudioState = playState;
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Play Methods (Base)
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Handles actually preparing the request.
        /// </summary>
        /// <param name="request">The request edit specifically.</param>
        /// <param name="edits">Any extra edits the user wants to add.</param>
        /// <returns>The AudioClipPlayer setup with the params requested.</returns>
        private static AudioPlayerSequence PrepareBase(string request, params IEditModule[] edits)
            => PlayBase(request, edits, false);


        /// <summary>
        /// Handles actually playing the request.
        /// </summary>
        /// <param name="request">The request edit specifically.</param>
        /// <param name="methodEdit">The edit specific for the method call.</param>
        /// <param name="edits">Any extra edits the user wants to add.</param>
        /// <returns>The AudioClipPlayer setup with the params requested.</returns>
        private static AudioPlayerSequence PlayBase(string request, IEditModule methodEdit, params IEditModule[] edits)
            => PlayBase(request, edits.Concat(new IEditModule[] {methodEdit}).ToArray());
        
        
        /// <summary>
        /// Handles actually playing the request.
        /// </summary>
        /// <param name="methodEdits">The edits specific for the method call.</param>
        /// <param name="edits">Any extra edits the user wants to add.</param>
        /// <returns>The AudioClipPlayer setup with the params requested.</returns>
        private static AudioPlayerSequence PlayBase(string request, IEnumerable<IEditModule> methodEdits, params IEditModule[] edits)
            => PlayBase(request, edits.Concat(methodEdits).ToArray());
        
        
        /// <summary>
        /// Handles actually playing/preparing the request.
        /// </summary>
        /// <param name="edits">The edits to apply.</param>
        /// <param name="autoPlay">Should the clip player instantly play? DEF: True</param>
        /// <returns>The AudioClipPlayer setup with the params requested.</returns>
        private static AudioPlayerSequence PlayBase(string request, IEditModule[] edits, bool autoPlay = true)
        {
            // Checks to see if the audio manager can actually play audio.
            // if not, do nothing as it won't be heard...
            if (!AssetAccessor.GetAsset<SettingsAssetRuntime>().CanPlayAudio)
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.AudioDisabled));
                return null;
            }

            var clipSettings = new AudioClipSettings(edits);
            var sequence = AudioPool.AssignSequence();

            sequence.Prepare(request, clipSettings);

            if (autoPlay)
            {
                sequence.Play();
            }
            
            return sequence;
        }



        private static AudioPlayerSequence PrepareGroupBase(string groupRef)
        {
            return PlayGroupBase(groupRef, Array.Empty<IEditModule>(), false);
        }


        private static AudioPlayerSequence PrepareGroupBase(string groupRef, IEditModule[] edits)
        {
            return PlayGroupBase(groupRef, edits, false);
        }
        
        private static AudioPlayerSequence PrepareGroupBase(string[] groupRef, GroupPlayMode playMode)
        {
            return PlayGroupBase(groupRef, playMode, Array.Empty<IEditModule>(), false);
        }
        
        private static AudioPlayerSequence PrepareGroupBase(string[] groupRef, GroupPlayMode playMode, IEditModule[] edits)
        {
            return PlayGroupBase(groupRef, playMode, edits, false);
        }
        
        private static AudioPlayerSequence PlayGroupBase(string request, IEditModule methodEdit, IEditModule[] edits, bool autoPlay = true)
        {
            return PlayGroupBase(request, edits.Concat(new IEditModule[] { methodEdit }).ToArray(), autoPlay);
        }
        
        
        private static AudioPlayerSequence PlayGroupBase(string request, IEditModule[] methodEdit, IEditModule[] edits, bool autoPlay = true)
        {
            return PlayGroupBase(request, edits.Concat(methodEdit).ToArray(), autoPlay);
        }
        
        private static AudioPlayerSequence PlayGroupBase(string request, IEditModule[] edits, bool autoPlay = true)
        {
            // Checks to see if the audio manager can actually play audio.
            // if not, do nothing as it won't be heard...
            if (!AssetAccessor.GetAsset<SettingsAssetRuntime>().CanPlayAudio)
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.AudioDisabled));
                return null;
            }

            var clipSettings = new AudioClipSettings(edits);
            var sequence = AudioPool.AssignSequence();

            sequence.PrepareGroup(request, clipSettings);

            if (autoPlay)
            {
                sequence.Play();
            }
            
            return sequence;
        }
        
        
        private static AudioPlayerSequence PlayGroupBase(string[] request, GroupPlayMode playMode, IEditModule methodEdit, IEditModule[] edits, bool autoPlay = true)
        {
            return PlayGroupBase(request, playMode, edits.Concat(new IEditModule[] { methodEdit }).ToArray(), autoPlay);
        }
        
        
        private static AudioPlayerSequence PlayGroupBase(string[] request, GroupPlayMode playMode, IEditModule[] methodEdit, IEditModule[] edits, bool autoPlay = true)
        {
            return PlayGroupBase(request, playMode, edits.Concat(methodEdit).ToArray(), autoPlay);
        }
        
        
        private static AudioPlayerSequence PlayGroupBase(string[] request, GroupPlayMode playMode, IEditModule[] edits, bool autoPlay = true)
        {
            // Checks to see if the audio manager can actually play audio.
            // if not, do nothing as it won't be heard...
            if (!AssetAccessor.GetAsset<SettingsAssetRuntime>().CanPlayAudio)
            {
                AmDebugLogger.Error(AudioManagerErrorMessages.GetMessage(AudioManagerErrorCode.AudioDisabled));
                return null;
            }

            var clipSettings = new AudioClipSettings(edits);
            var sequence = AudioPool.AssignSequence();

            sequence.PrepareGroup(request, playMode, clipSettings);

            if (autoPlay)
            {
                sequence.Play();
            }
            
            return sequence;
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Prepare Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Prepares the audio clip requested to be played, but doesn't play it automatically.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence Prepare(string request, params IEditModule[] edits)
        {
            return PrepareBase(request, edits);
        }
        
        
        /// <summary>
        /// Prepares the audio clip requested to be played, but doesn't play it automatically.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The play mode for the group.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PrepareGroup(string[] request, GroupPlayMode playMode, params IEditModule[] edits)
        {
            return PrepareGroupBase(request, playMode, edits);
        }
        
        
        /// <summary>
        /// Prepares the audio clip requested to be played, but doesn't play it automatically.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PrepareGroup(string request)
        {
            return PrepareGroupBase(request);
        }
        
        
        /// <summary>
        /// Prepares the audio clip requested to be played, but doesn't play it automatically.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PrepareGroup(string request, params IEditModule[] edits)
        {
            return PrepareGroupBase(request, edits);
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Play Methods (Single Request)
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence Play(string request, params IEditModule[] edits)
        {
            return PlayBase(request, edits);
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="startTime">The start time for the clip.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayFromTime(string request, float startTime, params IEditModule[] edits)
        {
            return PlayBase(request, new StartTimeEdit(startTime), edits);
        }
        
       
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="delay">The start delay for the clip.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayWithDelay(string request, float delay, params IEditModule[] edits)
        {
            return PlayBase(request, new DelayEdit(delay), edits);
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayAtLocation(string request, Vector2 position, params IEditModule[] edits)
        {
            return PlayBase(request, new PositionEdit(position), edits);
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayAtLocation(string request, Vector3 position, params IEditModule[] edits)
        {
            return PlayBase(request, new PositionEdit(position), edits);
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="useLocalPosition">Should use the local position on the inputted transform?.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayAtLocation(string request, Transform position, bool useLocalPosition, params IEditModule[] edits)
        {
            return PlayBase(request, new PositionEdit(position, useLocalPosition), edits);
        }


        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence Play(string request, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            return PlayBase(request, new IEditModule[] { new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f) }, edits);
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
        public static AudioPlayerSequence PlayFromTime(string request, float startTime, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            return PlayBase(request, new IEditModule[] 
            {  
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f), new StartTimeEdit(startTime)
            }, edits);
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
        public static AudioPlayerSequence PlayWithDelay(string request, float delay, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            return PlayBase(request, new IEditModule[] 
            {
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new DelayEdit(delay)
            }, edits);
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
        public static AudioPlayerSequence PlayAtLocation(string request, Vector2 position, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            return PlayBase(request, new IEditModule[] 
            {
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new PositionEdit(position)
            }, edits);
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
        public static AudioPlayerSequence PlayAtLocation(string request, Vector3 position, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            return PlayBase(request, new IEditModule[] 
            { 
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new PositionEdit(position)
            }, edits);
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
        public static AudioPlayerSequence PlayAtLocation(string request, Transform position, bool useLocalPosition, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            return PlayBase(request, new IEditModule[] 
            {
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new PositionEdit(position, useLocalPosition)
            }, edits);
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Play Methods (Group)
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayGroup(string request, params IEditModule[] edits)
        {
            return PlayGroupBase(request, edits);
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="delay">The start delay for the clip.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayGroupWithDelay(string request, float delay, params IEditModule[] edits)
        {
            return PlayGroupBase(request, new DelayEdit(delay), edits);
        }

        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayGroupAtLocation(string request, Vector2 position, params IEditModule[] edits)
        {
            return PlayGroupBase(request, new PositionEdit(position), edits);
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayGroupAtLocation(string request, Vector3 position, params IEditModule[] edits)
        {
            return PlayGroupBase(request, new PositionEdit(position), edits);
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="useLocalPosition">Should use the local position on the inputted transform?.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayGroupAtLocation(string request, Transform position, bool useLocalPosition, params IEditModule[] edits)
        {
            return PlayGroupBase(request, new PositionEdit(position, useLocalPosition), edits);
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip name.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayGroup(string request, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            return PlayGroupBase(request, new IEditModule[] { new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f) }, edits);
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
        public static AudioPlayerSequence PlayGroupWithDelay(string request, float delay, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            return PlayGroupBase(request, new IEditModule[]
            { 
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new DelayEdit(delay)
            }, edits);
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
        public static AudioPlayerSequence PlayGroupAtLocation(string request, Vector2 position, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            return PlayGroupBase(request, new IEditModule[]
            {
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new PositionEdit(position)
            }, edits);
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
        public static AudioPlayerSequence PlayGroupAtLocation(string request, Vector3 position, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            return PlayGroupBase(request, new IEditModule[]
            {
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new PositionEdit(position)
            }, edits);
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
        public static AudioPlayerSequence PlayGroupAtLocation(string request, Transform position, bool useLocalPosition, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            return PlayGroupBase(request, new IEditModule[]
            {
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new PositionEdit(position, useLocalPosition)
            }, edits);
        }
        
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayGroup(string[] request, GroupPlayMode playMode, params IEditModule[] edits)
        {
            return PlayGroupBase(request, playMode, edits);
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="delay">The start delay for the clip.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayGroupWithDelay(string[] request, GroupPlayMode playMode, float delay, params IEditModule[] edits)
        {
            return PlayGroupBase(request, playMode, new DelayEdit(delay), edits);
        }

        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayGroupAtLocation(string[] request, GroupPlayMode playMode, Vector2 position, params IEditModule[] edits)
        {
            return PlayGroupBase(request, playMode, new PositionEdit(position), edits);
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayGroupAtLocation(string[] request, GroupPlayMode playMode, Vector3 position, params IEditModule[] edits)
        {
            return PlayGroupBase(request, playMode, new PositionEdit(position), edits);
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="useLocalPosition">Should use the local position on the inputted transform?.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayGroupAtLocation(string[] request, GroupPlayMode playMode, Transform position, bool useLocalPosition, params IEditModule[] edits)
        {
            return PlayGroupBase(request, playMode, new PositionEdit(position, useLocalPosition), edits);
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayGroup(string[] request, GroupPlayMode playMode, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            return PlayGroupBase(request, playMode, new IEditModule[] { new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f) }, edits);
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="delay">The start delay for the clip.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayGroupWithDelay(string[] request, GroupPlayMode playMode, float delay, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            return PlayGroupBase(request, playMode, new IEditModule[]
            { 
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new DelayEdit(delay)
            }, edits);
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayGroupAtLocation(string[] request, GroupPlayMode playMode, Vector2 position, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            return PlayGroupBase(request, playMode, new IEditModule[]
            {
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new PositionEdit(position)
            }, edits);
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayGroupAtLocation(string[] request, GroupPlayMode playMode, Vector3 position, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            return PlayGroupBase(request, playMode, new IEditModule[]
            {
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new PositionEdit(position)
            }, edits);
        }
        
        
        /// <summary>
        /// Plays the audio clip requested.
        /// </summary>
        /// <param name="request">The requested clip names.</param>
        /// <param name="playMode">The playmode to play the group as.</param>
        /// <param name="position">The position for the clip player to play from.</param>
        /// <param name="useLocalPosition">Should use the local position on the inputted transform?.</param>
        /// <param name="volume">The volume to set to.</param>
        /// <param name="pitch">The pitch to set to.</param>
        /// <param name="edits">Any edits before playing.</param>
        /// <returns>The player that has been set & started.</returns>
        public static AudioPlayerSequence PlayGroupAtLocation(string[] request, GroupPlayMode playMode, Transform position, bool useLocalPosition, float? volume = 1f, float? pitch = 1f, params IEditModule[] edits)
        {
            return PlayGroupBase(request, playMode, new IEditModule[]
            {
                new VolumeEdit(volume ?? 1f), new PitchEdit(pitch ?? 1f),
                new PositionEdit(position, useLocalPosition)
            }, edits);
        }
    }
}