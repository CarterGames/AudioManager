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