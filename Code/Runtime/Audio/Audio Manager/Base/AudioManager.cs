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

using CarterGames.Shared.AudioManager;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// The main manager class of the asset to play audio clips.
    /// </summary>
    public static partial class AudioManager
    {
        /// <summary>
        /// An internal ref to the audio library for better API.
        /// </summary>
        private static AudioLibrary Library => AmAssetAccessor.GetAsset<AudioLibrary>();
        
        
        /// <summary>
        /// An internal ref to the audio settings for better API.
        /// </summary>
        private static AmAssetSettings Settings => AmAssetAccessor.GetAsset<AmAssetSettings>();
        
        
        /// <summary>
        /// Use to change the play state.
        /// </summary>
        /// <param name="playState">The state to set to.</param>
        public static void ChangePlayState(PlayState playState)
        {
            Settings.PlayAudioState = playState;
        }
    }
}