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
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.IO;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// 
    /// </summary>
    public static class AudioManagerEditorUtil
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public const string SettingsWindowPath = "Project/Carter Games/Audio Manager";
        private const string SettingsAssetDefaultFullPath = "Assets/Resources/Carter Games/Audio Manager/Audio Manager Settings.asset";
        private const string SettingsAssetFilter = "t:audiomanagersettings";
        private const string AssetHeaderGraphicFilter = "AudioManagerEditorHeader";
        private const string AssetLogoSpeakerGraphicFilter = "AudioManagerSpeakerLogo";
        private const string AssetLogoMusicGraphicFilter = "AudioManagerMusicLogo";
        private const string SettingsIconTransparentGraphicFilter = "SettingsIcon";
        private const string PlayIconTransparentGraphicFilter = "PlayButton";
        private const string StopIconTransparentGraphicFilter = "StopButton";
        private const string CarterGamesBannerGraphicFilter = "CarterGamesBanner";
        
        public static readonly Color Green = new Color32(72, 222, 55, 255);
        public static readonly Color Yellow = new Color32(245, 234, 56, 255);
        public static readonly Color Red = new Color32(255, 150, 157, 255);
        public static readonly Color Blue = new Color32(151, 196, 255, 255);
        public static readonly Color Hidden = new Color(0, 0, 0, .3f);
        
        private static Texture2D cachedSpeakerLogoImg;
        private static Texture2D cachedMusicLogoImg;
        private static Texture2D cachedManagerHeaderImg;
        private static Texture2D cachedCarterGamesBannerImg;
        private static Texture2D cachedSettingsLogoImg;
        private static Texture2D cachedPlayButtonImg;
        private static Texture2D cachedStopButtonImg;
        
        private static AudioManagerSettings _settingsAsset;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets the header graphic of the asset.
        /// </summary>
        public static Texture2D ManagerHeader
        {
            get
            {
                if (cachedManagerHeaderImg) return cachedManagerHeaderImg;
                cachedManagerHeaderImg = GetTextureFile(AssetHeaderGraphicFilter);
                return cachedManagerHeaderImg;
            }
        }
        
        
        /// <summary>
        /// Gets the carter games banner graphic.
        /// </summary>
        public static Texture2D CarterGamesBanner 
        {
            get
            {
                if (cachedCarterGamesBannerImg) return cachedCarterGamesBannerImg;
                cachedCarterGamesBannerImg = GetTextureFile(CarterGamesBannerGraphicFilter);
                return cachedCarterGamesBannerImg;
            }
        }

        
        /// <summary>
        /// Gets the speaker logo.
        /// </summary>
        public static Texture2D SpeakerLogo
        {
            get
            {
                if (cachedSpeakerLogoImg) return cachedSpeakerLogoImg;
                cachedSpeakerLogoImg = GetTextureFile(AssetLogoSpeakerGraphicFilter);
                return cachedSpeakerLogoImg;
            }
        }
        
        
        /// <summary>
        /// Gets the music logo.
        /// </summary>
        public static Texture2D MusicLogo
        {
            get
            {
                if (cachedMusicLogoImg) return cachedMusicLogoImg;
                cachedMusicLogoImg = GetTextureFile(AssetLogoMusicGraphicFilter);
                return cachedMusicLogoImg;
            }
        }
        
        
        /// <summary>
        /// Gets the settings logo.
        /// </summary>
        public static Texture2D SettingsLogoTransparent
        {
            get
            {
                if (cachedSettingsLogoImg) return cachedSettingsLogoImg;
                cachedSettingsLogoImg = GetTextureFile(SettingsIconTransparentGraphicFilter);
                return cachedSettingsLogoImg;
            }
        }
        
        
        /// <summary>
        /// Gets the play icon.
        /// </summary>
        public static Texture2D PlayButton
        {
            get
            {
                if (cachedPlayButtonImg) return cachedPlayButtonImg;
                cachedPlayButtonImg = GetTextureFile(PlayIconTransparentGraphicFilter);
                return cachedPlayButtonImg;
            }
        }
        
        
        /// <summary>
        /// Gets the stop icon.
        /// </summary>
        public static Texture2D StopButton
        {
            get
            {
                if (cachedStopButtonImg) return cachedStopButtonImg;
                cachedStopButtonImg = GetTextureFile(StopIconTransparentGraphicFilter);
                return cachedStopButtonImg;
            }
        }


        /// <summary>
        /// Gets whether or not there is an audio manager file in the project.
        /// </summary>
        public static bool HasAnyAMF => AssetDatabase.FindAssets("t:audiomanagerfile", null).Length > 0;


        /// <summary>
        /// Confirms if there is a settings asset in the project or not...
        /// </summary>
        public static bool HasSettingsFile => Settings != null;
        
        
        /// <summary>
        /// Gets the settings asset for use (Editor Space Only!)...
        /// </summary>
        public static AudioManagerSettings Settings 
        {
            get
            {
                if (_settingsAsset != null) return _settingsAsset;

                if (AssetDatabase.FindAssets(SettingsAssetFilter, null).Length <= 0)
                {
                    CreateFile<AudioManagerSettings>(SettingsAssetDefaultFullPath);
                }
                
                _settingsAsset = (AudioManagerSettings)GetFile<AudioManagerSettings>(SettingsAssetFilter);
                return _settingsAsset;
            }
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   File Management Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Creates a file of the type requested...
        /// </summary>
        /// <param name="path">The path to create the file in (Editor Only)</param>
        /// <typeparam name="T">The type the file is</typeparam>
        private static void CreateFile<T>(string path)
        {
            var instance = ScriptableObject.CreateInstance(typeof(T));

            var currentPath = string.Empty;
            
            foreach (var element in path.Split('/'))
            {
                if (!element.Equals("Assets"))
                    currentPath += "/" + element;
                else
                    currentPath = element;
                
                if (Directory.Exists(element.Replace("Audio Manager Settings.asset", ""))) continue;
                Directory.CreateDirectory(currentPath.Replace("Audio Manager Settings.asset", ""));
            }
            
            AssetDatabase.CreateAsset(instance, path);
            AssetDatabase.Refresh();
        }
        
        
        /// <summary>
        /// Gets the first file of the type requested that isn't the class (Editor Only)
        /// </summary>
        /// <param name="filter">the search filter</param>
        /// <typeparam name="T">The type to get</typeparam>
        /// <returns>object</returns>
        public static object GetFile<T>(string filter)
        {
            var asset = AssetDatabase.FindAssets(filter, null);
            var path = AssetDatabase.GUIDToAssetPath(asset[0]);
            return AssetDatabase.LoadAssetAtPath(path, typeof(T));
        }


        /// <summary>
        /// Runs GetFile() but for a texture & returns the result...
        /// </summary>
        /// <param name="filter">The filter to apply</param>
        /// <returns>The texture found</returns>
        private static Texture2D GetTextureFile(string filter)
        {
            return (Texture2D)GetFile<Texture2D>(filter);
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        
        /// <summary>
        /// Gets the header for a settings asset.
        /// </summary>
        public static void SettingsHeader()
        {
            GUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Shows either the Carter Games Logo or an alternative for if the icon is deleted/not included when you import the package
            // Note: if you are using an older version of the asset, the directory/name of the logo may not match this and therefore will display the text title only
            if (SettingsLogoTransparent)
            {
                if (GUILayout.Button(SettingsLogoTransparent, GUIStyle.none, GUILayout.MaxHeight(65)))
                {
                    GUI.FocusControl(null);
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2.5f);
        }
        
        
        /// <summary>
        /// Gets the header for most custom inspectors to help reduce duplicate code.
        /// </summary>
        /// <param name="scriptName">The name of the script to show.</param>
        public static void Header(string scriptName)
        {
            GUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Shows either the Carter Games Logo or an alternative for if the icon is deleted/not included when you import the package
            // Note: if you are using an older version of the asset, the directory/name of the logo may not match this and therefore will display the text title only
            if (scriptName.Contains("Music") ? MusicLogo : SpeakerLogo)
            {
                if (GUILayout.Button(scriptName.Contains("Music") ? MusicLogo : SpeakerLogo, GUIStyle.none, GUILayout.Width(50), GUILayout.Height(50)))
                {
                    GUI.FocusControl(null);
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);

            // Label that shows the name of the script / tool & the Version number for user reference sake.
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(scriptName, EditorStyles.boldLabel, GUILayout.Width(TextWidth($"{scriptName}   ")));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10f);
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Utility Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets the text width of the text string entered.
        /// </summary>
        /// <param name="text">The text to get the width of.</param>
        /// <returns>The width of the text entered.</returns>
        public static float TextWidth(string text)
        {
            return GUI.skin.label.CalcSize(new GUIContent(text)).x;
        }
    }
}