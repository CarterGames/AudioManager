/*
 * Copyright (c) 2018-Present Carter Games
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
        
        private const string AutoValidationAutoCheckId = "AudioManager_Editor_AutoVersionCheck";
        private const string DeveloperDebugModeId = "AudioManager_Editor_Developer_DebugMode";
        
        private const string ScannerInitializedId = "AudioManager_Editor_Scanner_Initialized";
        private const string ScannerHasNewAudioClipId = "AudioManager_Editor_Scanner_HasNewAudioClip";
        private const string ScannerHasScannedProjectId = "AudioManager_Editor_Scanner_HasScanned";
        
        private const string EditorOptionsId = "AudioManager_EditorSettings_ShowEditorOptions";
        private const string HelpBoxesId = "AudioManager_EditorSettings_ShowHelpBoxes";
        private const string AudioOptionsId = "AudioManager_EditorSettings_ShowAudioOptions";
        
        private const string EditorTabPositionId = "AudioManager_EditorWindow_EditorTabPos";
        
        private const string LastLibraryEntryId = "AudioManager_EditorWindow_Tab01_LibIndex";
        private const string LibBtnScrollRectPosId = "AudioManager_EditorWindow_Tab01_LibBtnScrollRectPos";
        private const string LibScrollRectPosId = "AudioManager_EditorWindo_Tab01_LibScrollRectPos";
        
        private const string LastLibraryGroupEntryId = "AudioManager_EditorWindow_Tab02_LibGroupIndex";
        private const string GroupScrollRectPosId = "AudioManager_EditorWindow_Tab02_ScrollRectPos";
        
        private const string LastLibMixerEntryId = "AudioManager_EditorWindow_Tab03_LibMixerIndex";
        private const string MixerScrollRectPosId = "AudioManager_EditorWindow_Tab03_ScrollRectPos";
        
        private const string LastLibMusicEntryId = "AudioManager_EditorWindow_Tab04_LibMusicIndex";
        private const string MusicScrollRectPosId = "AudioManager_EditorWindow_Tab04_ScrollRectPos";
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
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
        /// The scroll rect pos of the group tab left GUI section.
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
        /// The scroll rect pos of the music track list tab left GUI section.
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
            if (type == SettingType.EditorPref)
            {
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
                            defaultValue == null ? JsonUtility.ToJson(Vector2.zero) : JsonUtility.ToJson(defaultValue));
                        return JsonUtility.FromJson<Vector2>(EditorPrefs.GetString(key));
                    default:
                        return null;
                }
            }
            else
            {
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
                        return JsonUtility.FromJson<Vector2>(SessionState.GetString(key, JsonUtility.ToJson(defaultValue)));
                    default:
                        return null;
                }
            }
        }


        private static void SetValue<T>(string key, SettingType type, object value)
        {
            if (type == SettingType.EditorPref)
            {
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
                    default:
                        break;
                }
            }
            else
            {
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
                    default:
                        break;
                }
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