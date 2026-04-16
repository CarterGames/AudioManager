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
    /// A class to handle any common logic needed at runtime.
    /// </summary>
    public static class UtilRuntime
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public static readonly string DefaultAudioPlayerPrefabResourcesPath = $"Prefabs/{AudioPlayerPrefabName}";
        public static readonly string DefaultAudioSourcePrefabResourcesPath = $"Prefabs/{AudioSourcePrefabName}";
        
        
        // Default Names
        /* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        public const string AudioPlayerPrefabName = "+[AudioManager] - Audio Player";
        public const string AudioSourcePrefabName = "+[AudioManager] - Source";


        public static bool SettingCanPlayAudio => AmAssetAccessor.GetAsset<AmAssetSettings>().CanPlayAudio;
        public static PlayState SettingAudioPlayState => AmAssetAccessor.GetAsset<AmAssetSettings>().PlayAudioState;
    }
}