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
    /// Handles all the per user settings.
    /// </summary>
    public static class PerUserSettings
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static readonly string DeveloperDebugModeId = $"{UniqueId}_CarterGames_AudioManager_Editor_Developer_DebugMode";
        
        private static readonly string ScannerInitializedId = $"{UniqueId}_CarterGames_AudioManager_Editor_Scanner_Initialized";
        private static readonly string ScannerHasNewAudioClipId = $"{UniqueId}_CarterGames_AudioManager_Editor_Scanner_HasNewAudioClip";
        private static readonly string ScannerHasScannedProjectId = $"{UniqueId}_CarterGames_AudioManager_Editor_Scanner_HasScanned";
        
        private static readonly string EditorOptionsId = $"{UniqueId}_CarterGames_AudioManager_EditorSettings_ShowEditorOptions";
        private static readonly string HelpBoxesId = $"{UniqueId}_CarterGames_AudioManager_EditorSettings_ShowHelpBoxes";
        private static readonly string AudioOptionsId = $"{UniqueId}_CarterGames_AudioManager_EditorSettings_ShowAudioOptions";
        private static readonly string PoolingOptionsId = $"{UniqueId}_CarterGames_AudioManager_EditorSettings_ShowPoolingOptions";
        
        private static readonly string EditorTabPositionId = $"{UniqueId}_CarterGames_AudioManager_EditorWindow_EditorTabPos";
        
        private static readonly string LastLibraryEntryId = $"{UniqueId}_CarterGames_AudioManager_EditorWindow_Tab01_LibIndex";
        private static readonly string LibBtnScrollRectPosId = $"{UniqueId}_CarterGames_AudioManager_EditorWindow_Tab01_LibBtnScrollRectPos";
        private static readonly string LibScrollRectPosId = $"{UniqueId}_CarterGames_AudioManager_EditorWindow_Tab01_LibScrollRectPos";
        
        private static readonly string LastLibraryGroupEntryId = $"{UniqueId}_CarterGames_AudioManager_EditorWindow_Tab02_LibGroupIndex";
        private static readonly string GroupBtnScrollRectPosId = $"{UniqueId}_CarterGames_AudioManager_EditorWindow_Tab02_GroupBtnScrollRectPos";
        private static readonly string GroupScrollRectPosId = $"{UniqueId}_CarterGames_AudioManager_EditorWindow_Tab02_ScrollRectPos";
        
        private static readonly string LastLibMixerEntryId = $"{UniqueId}_CarterGames_AudioManager_EditorWindow_Tab03_LibMixerIndex";
        private static readonly string MixerBtnScrollRectPosId = $"{UniqueId}_CarterGames_AudioManager_EditorWindow_Tab03_MixerBtnScrollRectPos";
        private static readonly string MixerScrollRectPosId = $"{UniqueId}_CarterGames_AudioManager_EditorWindow_Tab03_ScrollRectPos";
        
        private static readonly string LastLibMusicEntryId = $"{UniqueId}_CarterGames_AudioManager_EditorWindow_Tab04_LibMusicIndex";
        private static readonly string MusicBtnScrollRectPosId = $"{UniqueId}_CarterGames_AudioManager_EditorWindow_Tab04_MusicBtnScrollRectPos";
        private static readonly string MusicScrollRectPosId = $"{UniqueId}_CarterGames_AudioManager_EditorWindow_Tab04_ScrollRectPos";
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// The unique if for the assets settings to be per project...
        /// </summary>
        private static string UniqueId => SharedPerUserSettings.UniqueId;
        
        
        /// <summary>
        /// The dev debug mode toggle, shows the behind the scenes on custom editors mostly, intended to aid in debugging the asset when an issue occurs.
        /// </summary>
        public static bool DeveloperDebugMode
        {
            get => (bool) GetOrCreateValue<bool>(DeveloperDebugModeId, PerUserSettingType.EditorPref);
            set => SetValue<bool>(DeveloperDebugModeId, PerUserSettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// Has the scanner been initialized.
        /// </summary>
        public static bool ScannerInitialized
        {
            get => (bool) GetOrCreateValue<bool>(ScannerInitializedId, PerUserSettingType.SessionState);
            set => SetValue<bool>(ScannerInitializedId, PerUserSettingType.SessionState, value);
        }
        
        
        /// <summary>
        /// Does the scanner have a new audio clip to add to the library.
        /// </summary>
        public static bool ScannerHasNewAudioClip
        {
            get => (bool) GetOrCreateValue<bool>(ScannerHasNewAudioClipId, PerUserSettingType.EditorPref);
            set => SetValue<bool>(ScannerHasNewAudioClipId, PerUserSettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// Has the scanner performed a scan.
        /// </summary>
        public static bool ScannerHasScanned
        {
            get => (bool) GetOrCreateValue<bool>(ScannerHasScannedProjectId, PerUserSettingType.EditorPref);
            set => SetValue<bool>(ScannerHasScannedProjectId, PerUserSettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The tab position of the audio library editor window.
        /// </summary>
        public static int EditorTabPosition 
        {
            get => (int) GetOrCreateValue<int>(EditorTabPositionId, PerUserSettingType.EditorPref);
            set => SetValue<int>(EditorTabPositionId, PerUserSettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// Defines if the editor options dropdown is active in the settings provider.
        /// </summary>
        public static bool ShowEditorOptions
        {
            get => (bool) GetOrCreateValue<bool>(EditorOptionsId, PerUserSettingType.EditorPref);
            set => SetValue<bool>(EditorOptionsId, PerUserSettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// Defines if helps boxes should appear on the GUI.
        /// </summary>
        public static bool ShowHelpBoxes
        {
            get => (bool) GetOrCreateValue<bool>(HelpBoxesId, PerUserSettingType.EditorPref);
            set => SetValue<bool>(HelpBoxesId, PerUserSettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// Defines if the audio options dropdown show in the settings provider.
        /// </summary>
        public static bool ShowAudioOptions
        {
            get => (bool) GetOrCreateValue<bool>(AudioOptionsId, PerUserSettingType.EditorPref);
            set => SetValue<bool>(AudioOptionsId, PerUserSettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// Defines if the pooling options dropdown show in the settings provider.
        /// </summary>
        public static bool ShowPoolingOptions
        {
            get => (bool) GetOrCreateValue<bool>(PoolingOptionsId, PerUserSettingType.EditorPref);
            set => SetValue<bool>(PoolingOptionsId, PerUserSettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// Get the last library entry shown.
        /// </summary>
        public static int LastLibraryIndexShown
        {
            get => (int) GetOrCreateValue<int>(LastLibraryEntryId, PerUserSettingType.EditorPref, -1);
            set => SetValue<int>(LastLibraryEntryId, PerUserSettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The scroll rect pos of the library tab left GUI section.
        /// </summary>
        public static Vector2 LibBtnScrollRectPos
        {
            get => (Vector2) GetOrCreateValue<Vector2>(LibBtnScrollRectPosId, PerUserSettingType.EditorPref);
            set => SetValue<Vector2>(LibBtnScrollRectPosId, PerUserSettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The scroll rect pos of the library tab right GUI section.
        /// </summary>
        public static Vector2 LibScrollRectPos
        {
            get => (Vector2) GetOrCreateValue<Vector2>(LibScrollRectPosId, PerUserSettingType.EditorPref);
            set => SetValue<Vector2>(LibScrollRectPosId, PerUserSettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// Get the last group shown.
        /// </summary>
        public static int LastLibraryGroupEntry
        {
            get => (int) GetOrCreateValue<int>(LastLibraryGroupEntryId, PerUserSettingType.EditorPref, -1);
            set => SetValue<int>(LastLibraryGroupEntryId, PerUserSettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The scroll rect pos of the group tab left GUI section.
        /// </summary>
        public static Vector2 GroupBtnScrollRectPos
        {
            get => (Vector2) GetOrCreateValue<Vector2>(GroupBtnScrollRectPosId, PerUserSettingType.EditorPref);
            set => SetValue<Vector2>(GroupBtnScrollRectPosId, PerUserSettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The scroll rect pos of the group tab right GUI section.
        /// </summary>
        public static Vector2 GroupScrollRectPos
        {
            get => (Vector2) GetOrCreateValue<Vector2>(GroupScrollRectPosId, PerUserSettingType.EditorPref);
            set => SetValue<Vector2>(GroupScrollRectPosId, PerUserSettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The last mixer that was selected.
        /// </summary>
        public static int LastLibMixerEntry
        {
            get => (int) GetOrCreateValue<int>(LastLibMixerEntryId, PerUserSettingType.EditorPref, -1);
            set => SetValue<int>(LastLibMixerEntryId, PerUserSettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The scroll rect pos of the mixer tab left GUI section.
        /// </summary>
        public static Vector2 MixerBtnScrollRectPos
        {
            get => (Vector2) GetOrCreateValue<Vector2>(MixerBtnScrollRectPosId, PerUserSettingType.EditorPref);
            set => SetValue<Vector2>(MixerBtnScrollRectPosId, PerUserSettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The scroll rect pos of the group tab right GUI section.
        /// </summary>
        public static Vector2 MixerScrollRectPos
        {
            get => (Vector2) GetOrCreateValue<Vector2>(MixerScrollRectPosId, PerUserSettingType.EditorPref);
            set => SetValue<Vector2>(MixerScrollRectPosId, PerUserSettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The last track list selected.
        /// </summary>
        public static int LastLibMusicEntry
        {
            get => (int) GetOrCreateValue<int>(LastLibMusicEntryId, PerUserSettingType.EditorPref, -1);
            set => SetValue<int>(LastLibMusicEntryId, PerUserSettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The scroll rect pos of the music tab left GUI section.
        /// </summary>
        public static Vector2 MusicBtnScrollRectPos
        {
            get => (Vector2) GetOrCreateValue<Vector2>(MusicBtnScrollRectPosId, PerUserSettingType.EditorPref);
            set => SetValue<Vector2>(MusicBtnScrollRectPosId, PerUserSettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The scroll rect pos of the music track list tab right GUI section.
        /// </summary>
        public static Vector2 MusicScrollRectPos
        {
            get => (Vector2) GetOrCreateValue<Vector2>(MusicScrollRectPosId, PerUserSettingType.EditorPref);
            set => SetValue<Vector2>(MusicScrollRectPosId, PerUserSettingType.EditorPref, value);
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private static object GetOrCreateValue<T>(string key, PerUserSettingType type, object defaultValue = null)
        {
            return PerUserSettingsEditor.GetOrCreateValue<T>(key, type, defaultValue);
        }


        private static void SetValue<T>(string key, PerUserSettingType type, object value)
        {
            PerUserSettingsEditor.SetValue<T>(key, type, value);
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Menu Items
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        [MenuItem("Tools/Carter Games/Audio Manager/Reset Prefs")]
        public static void ResetPrefs()
        {
            EditorTabPosition = 0;
            ShowEditorOptions = false;
            ShowHelpBoxes = false;
            ShowAudioOptions = false;
            LastLibraryIndexShown = -1;
            LibBtnScrollRectPos = Vector2.zero;
            LibScrollRectPos = Vector2.zero;
            LastLibraryGroupEntry = -1;
            GroupScrollRectPos = Vector2.zero;
            LastLibMixerEntry = -1;
            MixerScrollRectPos = Vector2.zero;
            LastLibMusicEntry = -1;
            MusicScrollRectPos = Vector2.zero;
            
            PerUserSettingsRuntime.ResetPrefs();
        }
    }
}