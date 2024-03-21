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

        private const string UniqueIdId = "CarterGames_AudioManager_Editor_UUID";

        private static readonly string AutoValidationAutoCheckId = $"{UniqueId}_CarterGames_AudioManager_Editor_AutoVersionCheck";
        private static readonly string DeveloperDebugModeId = $"{UniqueId}_CarterGames_AudioManager_Editor_Developer_DebugMode";
        
        private static readonly string ScannerInitializedId = $"{UniqueId}_CarterGames_AudioManager_Editor_Scanner_Initialized";
        private static readonly string ScannerHasNewAudioClipId = $"{UniqueId}_CarterGames_AudioManager_Editor_Scanner_HasNewAudioClip";
        private static readonly string ScannerHasScannedProjectId = $"{UniqueId}_CarterGames_AudioManager_Editor_Scanner_HasScanned";
        
        private static readonly string EditorOptionsId = $"{UniqueId}_CarterGames_AudioManager_EditorSettings_ShowEditorOptions";
        private static readonly string HelpBoxesId = $"{UniqueId}_CarterGames_AudioManager_EditorSettings_ShowHelpBoxes";
        private static readonly string AudioOptionsId = $"{UniqueId}_CarterGames_AudioManager_EditorSettings_ShowAudioOptions";
        
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
        private static string UniqueId => (string)GetOrCreateValue<string>(UniqueIdId, SettingType.PlayerPref, Guid.NewGuid().ToString());


        /// <summary>
        /// Has the scanner been initialized.
        /// </summary>
        public static bool VersionValidationAutoCheckOnLoad
        {
            get => (bool) GetOrCreateValue<bool>(AutoValidationAutoCheckId, SettingType.EditorPref);
            set => SetValue<bool>(AutoValidationAutoCheckId, SettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The dev debug mode toggle, shows the behind the scenes on custom editors mostly, intended to aid in debugging the asset when an issue occurs.
        /// </summary>
        public static bool DeveloperDebugMode
        {
            get => (bool) GetOrCreateValue<bool>(DeveloperDebugModeId, SettingType.EditorPref);
            set => SetValue<bool>(DeveloperDebugModeId, SettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// Has the scanner been initialized.
        /// </summary>
        public static bool ScannerInitialized
        {
            get => (bool) GetOrCreateValue<bool>(ScannerInitializedId, SettingType.SessionState);
            set => SetValue<bool>(ScannerInitializedId, SettingType.SessionState, value);
        }
        
        
        /// <summary>
        /// Does the scanner have a new audio clip to add to the library.
        /// </summary>
        public static bool ScannerHasNewAudioClip
        {
            get => (bool) GetOrCreateValue<bool>(ScannerHasNewAudioClipId, SettingType.EditorPref);
            set => SetValue<bool>(ScannerHasNewAudioClipId, SettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// Has the scanner performed a scan.
        /// </summary>
        public static bool ScannerHasScanned
        {
            get => (bool) GetOrCreateValue<bool>(ScannerHasScannedProjectId, SettingType.EditorPref);
            set => SetValue<bool>(ScannerHasScannedProjectId, SettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The tab position of the audio library editor window.
        /// </summary>
        public static int EditorTabPosition 
        {
            get => (int) GetOrCreateValue<int>(EditorTabPositionId, SettingType.EditorPref);
            set => SetValue<int>(EditorTabPositionId, SettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// Defines if the editor options dropdown is active in the settings provider.
        /// </summary>
        public static bool ShowEditorOptions
        {
            get => (bool) GetOrCreateValue<bool>(EditorOptionsId, SettingType.EditorPref);
            set => SetValue<bool>(EditorOptionsId, SettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// Defines if helps boxes should appear on the GUI.
        /// </summary>
        public static bool ShowHelpBoxes
        {
            get => (bool) GetOrCreateValue<bool>(HelpBoxesId, SettingType.EditorPref);
            set => SetValue<bool>(HelpBoxesId, SettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// Defines if the audio options dropdown show in the settings provider.
        /// </summary>
        public static bool ShowAudioOptions
        {
            get => (bool) GetOrCreateValue<bool>(AudioOptionsId, SettingType.EditorPref);
            set => SetValue<bool>(AudioOptionsId, SettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// Get the last library entry shown.
        /// </summary>
        public static int LastLibraryIndexShown
        {
            get => (int) GetOrCreateValue<int>(LastLibraryEntryId, SettingType.EditorPref, -1);
            set => SetValue<int>(LastLibraryEntryId, SettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The scroll rect pos of the library tab left GUI section.
        /// </summary>
        public static Vector2 LibBtnScrollRectPos
        {
            get => (Vector2) GetOrCreateValue<Vector2>(LibBtnScrollRectPosId, SettingType.EditorPref);
            set => SetValue<Vector2>(LibBtnScrollRectPosId, SettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The scroll rect pos of the library tab right GUI section.
        /// </summary>
        public static Vector2 LibScrollRectPos
        {
            get => (Vector2) GetOrCreateValue<Vector2>(LibScrollRectPosId, SettingType.EditorPref);
            set => SetValue<Vector2>(LibScrollRectPosId, SettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// Get the last group shown.
        /// </summary>
        public static int LastLibraryGroupEntry
        {
            get => (int) GetOrCreateValue<int>(LastLibraryGroupEntryId, SettingType.EditorPref, -1);
            set => SetValue<int>(LastLibraryGroupEntryId, SettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The scroll rect pos of the group tab left GUI section.
        /// </summary>
        public static Vector2 GroupBtnScrollRectPos
        {
            get => (Vector2) GetOrCreateValue<Vector2>(GroupBtnScrollRectPosId, SettingType.EditorPref);
            set => SetValue<Vector2>(GroupBtnScrollRectPosId, SettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The scroll rect pos of the group tab right GUI section.
        /// </summary>
        public static Vector2 GroupScrollRectPos
        {
            get => (Vector2) GetOrCreateValue<Vector2>(GroupScrollRectPosId, SettingType.EditorPref);
            set => SetValue<Vector2>(GroupScrollRectPosId, SettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The last mixer that was selected.
        /// </summary>
        public static int LastLibMixerEntry
        {
            get => (int) GetOrCreateValue<int>(LastLibMixerEntryId, SettingType.EditorPref, -1);
            set => SetValue<int>(LastLibMixerEntryId, SettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The scroll rect pos of the mixer tab left GUI section.
        /// </summary>
        public static Vector2 MixerBtnScrollRectPos
        {
            get => (Vector2) GetOrCreateValue<Vector2>(MixerBtnScrollRectPosId, SettingType.EditorPref);
            set => SetValue<Vector2>(MixerBtnScrollRectPosId, SettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The scroll rect pos of the group tab right GUI section.
        /// </summary>
        public static Vector2 MixerScrollRectPos
        {
            get => (Vector2) GetOrCreateValue<Vector2>(MixerScrollRectPosId, SettingType.EditorPref);
            set => SetValue<Vector2>(MixerScrollRectPosId, SettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The last track list selected.
        /// </summary>
        public static int LastLibMusicEntry
        {
            get => (int) GetOrCreateValue<int>(LastLibMusicEntryId, SettingType.EditorPref, -1);
            set => SetValue<int>(LastLibMusicEntryId, SettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The scroll rect pos of the music tab left GUI section.
        /// </summary>
        public static Vector2 MusicBtnScrollRectPos
        {
            get => (Vector2) GetOrCreateValue<Vector2>(MusicBtnScrollRectPosId, SettingType.EditorPref);
            set => SetValue<Vector2>(MusicBtnScrollRectPosId, SettingType.EditorPref, value);
        }
        
        
        /// <summary>
        /// The scroll rect pos of the music track list tab right GUI section.
        /// </summary>
        public static Vector2 MusicScrollRectPos
        {
            get => (Vector2) GetOrCreateValue<Vector2>(MusicScrollRectPosId, SettingType.EditorPref);
            set => SetValue<Vector2>(MusicScrollRectPosId, SettingType.EditorPref, value);
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private static object GetOrCreateValue<T>(string key, SettingType type, object defaultValue = null)
        {
            switch (type)
            {
                case SettingType.EditorPref:

                    if (EditorPrefs.HasKey(key))
                    {
                        switch (typeof(T))
                        {
                            case var x when x == typeof(bool):
                                return EditorPrefs.GetBool(key);
                            case var x when x == typeof(int):
                                return EditorPrefs.GetInt(key);
                            case var x when x == typeof(float):
                                return EditorPrefs.GetFloat(key);
                            case var x when x == typeof(string):
                                return EditorPrefs.GetString(key);
                            case var x when x == typeof(Vector2):
                                return JsonUtility.FromJson<Vector2>(EditorPrefs.GetString(key));
                            default:
                                return null;
                        }
                    }

                    switch (typeof(T))
                    {
                        case var x when x == typeof(bool):
                            EditorPrefs.SetBool(key, defaultValue == null ? false : (bool)defaultValue);
                            return EditorPrefs.GetBool(key);
                        case var x when x == typeof(int):
                            EditorPrefs.SetInt(key, defaultValue == null ? 0 : (int)defaultValue);
                            return EditorPrefs.GetInt(key);
                        case var x when x == typeof(float):
                            EditorPrefs.SetFloat(key, defaultValue == null ? 0 : (float)defaultValue);
                            return EditorPrefs.GetFloat(key);
                        case var x when x == typeof(string):
                            EditorPrefs.SetString(key, (string)defaultValue);
                            return EditorPrefs.GetString(key);
                        case var x when x == typeof(Vector2):
                            EditorPrefs.SetString(key,
                                defaultValue == null
                                    ? JsonUtility.ToJson(Vector2.zero)
                                    : JsonUtility.ToJson(defaultValue));
                            return JsonUtility.FromJson<Vector2>(EditorPrefs.GetString(key));
                        default:
                            return null;
                    }
                    
                case SettingType.PlayerPref:
                    
                    if (PlayerPrefs.HasKey(key))
                    {
                        switch (typeof(T))
                        {
                            case var x when x == typeof(bool):
                                return PlayerPrefs.GetInt(key) == 1;
                            case var x when x == typeof(int):
                                return PlayerPrefs.GetInt(key);
                            case var x when x == typeof(float):
                                return PlayerPrefs.GetFloat(key);
                            case var x when x == typeof(string):
                                return PlayerPrefs.GetString(key);
                            case var x when x == typeof(Vector2):
                                return JsonUtility.FromJson<Vector2>(PlayerPrefs.GetString(key));
                            default:
                                return null;
                        }
                    }

                    switch (typeof(T))
                    {
                        case var x when x == typeof(bool):
                            PlayerPrefs.SetInt(key,
                                defaultValue == null ? 0 : defaultValue.ToString().ToLower() == "true" ? 1 : 0);
                            return PlayerPrefs.GetInt(key) == 1;
                        case var x when x == typeof(int):
                            PlayerPrefs.SetInt(key, defaultValue == null ? 0 : (int)defaultValue);
                            return PlayerPrefs.GetInt(key);
                        case var x when x == typeof(float):
                            PlayerPrefs.SetFloat(key, defaultValue == null ? 0 : (float)defaultValue);
                            return PlayerPrefs.GetFloat(key);
                        case var x when x == typeof(string):
                            PlayerPrefs.SetString(key, (string)defaultValue);
                            return PlayerPrefs.GetString(key);
                        case var x when x == typeof(Vector2):
                            PlayerPrefs.SetString(key,
                                defaultValue == null
                                    ? JsonUtility.ToJson(Vector2.zero)
                                    : JsonUtility.ToJson(defaultValue));
                            return JsonUtility.FromJson<Vector2>(PlayerPrefs.GetString(key));
                        default:
                            return null;
                    }
                    
                case SettingType.SessionState:

                    switch (typeof(T))
                    {
                        case var x when x == typeof(bool):
                            return SessionState.GetBool(key, defaultValue == null ? false : (bool)defaultValue);
                        case var x when x == typeof(int):
                            return SessionState.GetInt(key, defaultValue == null ? 0 : (int)defaultValue);
                        case var x when x == typeof(float):
                            return SessionState.GetFloat(key, defaultValue == null ? 0 : (float)defaultValue);
                        case var x when x == typeof(string):
                            return SessionState.GetString(key, (string)defaultValue);
                        case var x when x == typeof(Vector2):
                            return JsonUtility.FromJson<Vector2>(SessionState.GetString(key,
                                JsonUtility.ToJson(defaultValue)));
                        default:
                            return null;
                    }
                    
                default:
                    return null;
            }
        }


        private static void SetValue<T>(string key, SettingType type, object value)
        {
            switch (type)
            {
                case SettingType.EditorPref:
                    
                    switch (typeof(T))
                    {
                        case var x when x == typeof(bool):
                            EditorPrefs.SetBool(key, (bool)value);
                            break;
                        case var x when x == typeof(int):
                            EditorPrefs.SetInt(key, (int)value);
                            break;
                        case var x when x == typeof(float):
                            EditorPrefs.SetFloat(key, (float)value);
                            break;
                        case var x when x == typeof(string):
                            EditorPrefs.SetString(key, (string)value);
                            break;
                        case var x when x == typeof(Vector2):
                            EditorPrefs.SetString(key, JsonUtility.ToJson(value));
                            break;
                    }
                    
                    break;
                case SettingType.PlayerPref:
                    
                    switch (typeof(T))
                    {
                        case var x when x == typeof(bool):
                            PlayerPrefs.SetInt(key, ((bool)value) ? 1 : 0);
                            break;
                        case var x when x == typeof(int):
                            PlayerPrefs.SetInt(key, (int)value);
                            break;
                        case var x when x == typeof(float):
                            PlayerPrefs.SetFloat(key, (float)value);
                            break;
                        case var x when x == typeof(string):
                            PlayerPrefs.SetString(key, (string)value);
                            break;
                        case var x when x == typeof(Vector2):
                            PlayerPrefs.SetString(key, JsonUtility.ToJson(value));
                            break;
                    }
                    
                    PlayerPrefs.Save();
                    
                    break;
                case SettingType.SessionState:
                    
                    switch (typeof(T))
                    {
                        case var x when x == typeof(bool):
                            SessionState.SetBool(key, (bool)value);
                            break;
                        case var x when x == typeof(int):
                            SessionState.SetInt(key, (int)value);
                            break;
                        case var x when x == typeof(float):
                            SessionState.SetFloat(key, (float)value);
                            break;
                        case var x when x == typeof(string):
                            SessionState.SetString(key, (string)value);
                            break;
                        case var x when x == typeof(Vector2):
                            SessionState.SetString(key, JsonUtility.ToJson(value));
                            break;
                    }
                    
                    break;
            }
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
            VersionValidationAutoCheckOnLoad = true;
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