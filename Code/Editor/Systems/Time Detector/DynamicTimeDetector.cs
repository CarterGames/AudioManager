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
using CarterGames.Shared.AudioManager.Editor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles finding the start/end time of audio clips.
    /// </summary>
    public static class DynamicTimeDetector
    {
        /// <summary>
        /// Attempts to find the start time of a clip based on when the clip has volume on its samples.
        /// </summary>
        /// <param name="audioData">The clip to read.</param>
        /// <param name="dynamicStartTime">The start time found.</param>
        /// <param name="division">The sample division to use for detecting the start, higher is better for music, clips should be lower.</param>
        /// <returns>If it was able to find a start time.</returns>
        public static bool TryDetectStartTime(AudioClip audioData, out DynamicTime dynamicStartTime, int division = -1)
        {
            return TryDetectStartTime(audioData, .025f, out dynamicStartTime);
        }


        /// <summary>
        /// Some math stuff I don't get xD
        /// </summary>
        private static float GetExpandedSample(float input, float min, float max)
        {
            return (((input - 0) / (1 - 0)) * (max - min) + min);
        }
        
        
        /// <summary>
        /// Some math stuff I don't get xD
        /// </summary>
        private static double RootMeanSquare(IEnumerable<float> x)
        {
            return Math.Sqrt(x.Average(i => (double)i * i));
        }


        /// <summary>
        /// Tries to detect the start time of the clip when called.
        /// </summary>
        /// <returns>If it was successful.</returns>
        public static bool TryDetectStartTime(AudioClip audioData, float thresholdFraction, out DynamicTime dynamicStartTime)
        {
            var samples = new float[audioData.samples * audioData.channels];

            dynamicStartTime = new DynamicTime();
            
            if (audioData.loadType != AudioClipLoadType.DecompressOnLoad) return false;
            
            audioData.GetData(samples, 0);


            var highestPoint = RootMeanSquare(samples.ToList());

            if (highestPoint <= 0)
            {
                highestPoint *= -1;
            }
            

            var threshold = highestPoint * thresholdFraction;
            

            for (var i = 0; i < samples.Length; i++)
            {
                if (GetExpandedSample(samples[i],0, (float)highestPoint) > threshold || GetExpandedSample(samples[i],0, (float)highestPoint) < -threshold)
                {
                    dynamicStartTime.time =
                        (((i - ScriptableRef.GetAssetDef<AmAssetSettings>().AssetRef.DynamicDetectionOffset) / audioData.frequency) /
                         (float)audioData.channels);

                    if (dynamicStartTime.time < 0)
                    {
                        dynamicStartTime.time = 0;
                    }
                    
                    dynamicStartTime.threshold = (float) threshold;
                    return true;
                }
            }

            dynamicStartTime.time = 0;
            return false;
        }
    }
}