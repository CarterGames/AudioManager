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

using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles initializing the settings asset of the asset.
    /// </summary>
    public sealed class SettingsInitialize : IAssetEditorInitialize, IAssetEditorReload
    {
        private static SerializedObject SettingsObj => ScriptableRef.GetAssetDef<AmAssetSettings>().ObjectRef;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   IAssetEditorInitialize Implementation
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public int InitializeOrder => 200;
        
        
        public void OnEditorInitialized()
        {
            TryInitializeSettings();
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   IAssetEditorReload Implementation
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public void OnEditorReloaded()
        {
            TryInitializeSettings();
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static void TryInitializeSettings()
        {
            var changed = false;
            
            if (SettingsObj.Fp("playerPrefab").objectReferenceValue == null)
            {
                SettingsObj.Fp("playerPrefab").objectReferenceValue = Resources.Load(UtilRuntime.DefaultAudioPlayerPrefabResourcesPath);
                changed = true;
            }
            
            if (SettingsObj.Fp("sourceInstancePrefab").objectReferenceValue == null)
            {
                SettingsObj.Fp("sourceInstancePrefab").objectReferenceValue = Resources.Load(UtilRuntime.DefaultAudioSourcePrefabResourcesPath);
                changed = true;
            }

            if (!changed) return;

            SettingsObj.ApplyModifiedProperties();
            SettingsObj.Update();
        }
    }
}