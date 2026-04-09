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

using System.Collections.Generic;
using System.Linq;
using CarterGames.Shared.AudioManager.Editor;
using CarterGames.Shared.AudioManager.Serializiation;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Provides a load of handy references & systems for the editor part of the asset.
    /// </summary>
    public static class UtilEditor
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        // Paths
        /* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        public const string SettingsLocationPath = "Carter Games/Assets/Audio Manager";
        

        // Default Names
        /* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        public const string AudioPrefabName = UtilRuntime.AudioPlayerPrefabName;

        
        // Filters
        /* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        // Graphics
        private const string AudioManagerBannerFilter = "T_AudioManager_LogoIcon";
        private const string CarterGamesBannerFilter = "T_AudioManager_CarterGamesBanner";
        private const string OpenBookIconFilter = "T_AudioManager_LibraryIcon";
        private const string FlatMusicalNoteIconFilter = "T_AudioManager_MusicalNoteFlatIcon";
        private const string CogIconFilter = "T_AudioManager_SettingsIcon";
        private const string DataIconFilter = "T_AudioManager_DatabaseIcon";
        private const string PlayIconFilter = "T_AudioManager_PlayButtonIcon";
        private const string SearchIconFilter = "T_AudioManager_SearchButtonIcon";
        private const string StopIconFilter = "T_AudioManager_StopButtonIcon";
        private const string WarningFilledFilter = "T_AudioManager_WarningFilledIcon";
        

        // Asset Caches
        /* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        // Graphics
        private static Texture2D audioManagerBannerCache;
        private static Texture2D cachedManagerHeaderLogoTransparentImg;
        private static Texture2D carterGamesBannerCache;
        private static Texture2D openBookIconCache;
        private static Texture2D flatMusicalNoteIconCache;
        private static Texture2D musicalNoteIconCache;
        private static Texture2D cogIconCache;
        private static Texture2D dataIconCache;
        private static Texture2D playIconCache;
        private static Texture2D searchIconCache;
        private static Texture2D stopIconCache;
        private static Texture2D warningFilledCache;


        // Objects
        private static GameObject audioPrefabCache;
        private static GameObject musicPrefabCache;
        
        
        // Colours
        /* ────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Green Apple (Green) Color
        /// </summary>
        /// <see cref="https://www.computerhope.com/cgi-bin/htmlcolor.pl?c=4CC417"/>
        public static readonly Color Green = new Color32(76, 196, 23, 255);

        
        /// <summary>
        /// Rubber Ducky Yellow (Yellow) Color
        /// </summary>
        /// <see cref="https://www.computerhope.com/cgi-bin/htmlcolor.pl?c=FFD801"/>
        public static readonly Color Yellow = new Color32(255, 216, 1, 255);
        

        /// <summary>
        /// Scarlet Red (Red) Color
        /// </summary>
        /// <see cref="https://www.computerhope.com/cgi-bin/htmlcolor.pl?c=FF2400"/>
        public static readonly Color Red = new Color32(255, 36, 23, 255);
        
        
        
        /// <summary>
        /// Carbon Grey (Grey) Color
        /// </summary>
        /// <see cref="https://www.computerhope.com/cgi-bin/htmlcolor.pl?c=625D5D"/>
        public static readonly Color Grey = new Color32(98, 93, 93, 255);
        

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The audio prefab.
        /// </summary>
        public static GameObject AudioPrefab => FileEditorUtil.GetOrAssignCache(ref audioPrefabCache, AudioPrefabName);


        /// <summary>
        /// Gets the open book icon.
        /// </summary>
        public static Texture2D OpenBookIcon => FileEditorUtil.GetOrAssignCache(ref openBookIconCache, OpenBookIconFilter);
        
        
        /// <summary>
        /// Gets the open book icon.
        /// </summary>
        public static Texture2D FlatMusicalNoteIcon => FileEditorUtil.GetOrAssignCache(ref flatMusicalNoteIconCache, FlatMusicalNoteIconFilter);
        
        
        /// <summary>
        /// Gets the cog icon.
        /// </summary>
        public static Texture2D CogIcon => FileEditorUtil.GetOrAssignCache(ref cogIconCache, CogIconFilter);
        
        
        /// <summary>
        /// Gets the data icon.
        /// </summary>
        public static Texture2D DataIcon => FileEditorUtil.GetOrAssignCache(ref dataIconCache, DataIconFilter);
        
        
        /// <summary>
        /// Gets the play icon.
        /// </summary>
        public static Texture2D PlayIcon => FileEditorUtil.GetOrAssignCache(ref playIconCache, PlayIconFilter);


        /// <summary>
        /// Gets the stop icon.
        /// </summary>
        public static Texture2D StopIcon => FileEditorUtil.GetOrAssignCache(ref stopIconCache, StopIconFilter);        
        
        
        /// <summary>
        /// Gets the warning icon
        /// </summary>
        public static Texture2D WarningIcon => FileEditorUtil.GetOrAssignCache(ref warningFilledCache, WarningFilledFilter);
        
        
        /// <summary>
        /// Gets the search icon
        /// </summary>
        public static Texture2D SearchIcon => FileEditorUtil.GetOrAssignCache(ref searchIconCache, SearchIconFilter);
        
        
        /// <summary>
        /// Gets the audio manager banner.
        /// </summary>
        public static Texture2D AudioManagerBanner => FileEditorUtil.GetOrAssignCache(ref audioManagerBannerCache, AudioManagerBannerFilter);

        
        /// <summary>
        /// Gets the carter games banner.
        /// </summary>
        public static Texture2D CarterGamesBanner => FileEditorUtil.GetOrAssignCache(ref carterGamesBannerCache, CarterGamesBannerFilter);
        

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Checks to see if a file exists in the editor.
        /// </summary>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>Bool</returns>
        public static bool FileExistsByFilter(string filter)
        {
            return AssetDatabase.FindAssets(filter, null).Length > 0;
        }


        /// <summary>
        /// Gets the path of a file by its class name.
        /// </summary>
        /// <param name="className">The class name to find.</param>
        /// <param name="toMatch">The rough end of the path to match.</param>
        /// <returns>The path found.</returns>
        public static string GetPathOfFile(string className, string toMatch)
        {
            var asset = AssetDatabase.FindAssets(className, null);
            var find = asset.FirstOrDefault(t => AssetDatabase.GUIDToAssetPath(t).Contains(toMatch));
            return AssetDatabase.GUIDToAssetPath(find);
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Library Reflection Edit Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets the library data currently stored via reflection.
        /// </summary>
        public static SerializableDictionary<string, AudioData> GetLibraryData()
        {
            return ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.LibraryLookup;
        }
        
        
        /// <summary>
        /// Sets the library data via reflection.
        /// </summary>
        /// <param name="value">The data to set to.</param>
        public static void SetLibraryData(Dictionary<AudioData, bool> value, bool clearData = false)
        {
            var library = ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef;
            var libraryObject = ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef;
            
            if (clearData)
            {
                libraryObject.Fp("library").Fpr("list").ClearArray();
            }

            var keysList = value.Keys.ToList();
            var valuesList = value.Values.ToList();

            for (var i = 0; i < value.Count; i++)
            {
                if (!valuesList[i]) continue;
                if (library.LibraryLookup.ContainsKey(keysList[i].id)) continue;
                
                library.LibraryLookup.Add(keysList[i].id, keysList[i]);
            }
            
            libraryObject.ApplyModifiedProperties();
            libraryObject.Update();

            library.OrderLibrary();

            libraryObject.ApplyModifiedProperties();
            libraryObject.Update();
        }
        
        
        /// <summary>
        /// Sets the library mixer groups to the value passed in.
        /// </summary>
        /// <param name="value">The value to set to.</param>
        public static void SetLibraryMixerGroups(AudioMixerGroup[] value)
        {
            ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.SetMixerGroups(value);
            EditorUtility.SetDirty(ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Editor Drawer Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Draws the header logo for the asset.
        /// </summary>
        public static void DrawHeaderWithTexture(Texture texture)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (texture != null)
            {
                if (GUILayout.Button(texture, GUIStyle.none, GUILayout.MaxHeight(110)))
                    GUI.FocusControl(null);
            }
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        
        
        /// <summary>
        /// Draws the script fields in the custom inspector...
        /// </summary>
        public static void DrawMonoScriptSection<T>(T target) where T : MonoBehaviour
        {
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(1.5f);
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(target), typeof(T), false);
            EditorGUI.EndDisabledGroup();
            
            GUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
        }
        
        
        /// <summary>
        /// Draws the script fields in the custom inspector...
        /// </summary>
        public static void DrawSoScriptSection<T>(T target) where T : ScriptableObject
        {
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(1.5f);
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script:", MonoScript.FromScriptableObject(target), typeof(T), false);
            EditorGUI.EndDisabledGroup();
            
            GUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
        }
        
        
        
        /// <summary>
        /// Creates a deselect zone to let users click outside of any editor window to unfocus from their last selected field.
        /// </summary>
        /// <param name="rect">The rect to draw on.</param>
        public static void CreateDeselectZone(ref Rect rect)
        {
            if (rect.width <= 0)
            {
                rect = new Rect(0, 0, Screen.width, Screen.height);
            }

            if (GUI.Button(rect, string.Empty, GUIStyle.none))
            {
                GUI.FocusControl(null);
            }
        }
        
        
        /// <summary>
        /// Draws a horizontal line
        /// </summary>
        public static void DrawHorizontalGUILine()
        {
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.background = new Texture2D(1, 1);
            boxStyle.normal.background.SetPixel(0, 0, Color.gray);
            boxStyle.normal.background.Apply();

            var one = EditorGUILayout.BeginHorizontal();
            GUILayout.Box("", boxStyle, GUILayout.ExpandWidth(true), GUILayout.Height(2));
            EditorGUILayout.EndHorizontal();
        }
        
        
        /// <summary>
        /// Draws a horizontal line
        /// </summary>
        public static void DrawHorizontalGUILine(Color lineCol)
        {
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.background = new Texture2D(1, 1);
            boxStyle.normal.background.SetPixel(0, 0, lineCol);
            boxStyle.normal.background.Apply();

            var one = EditorGUILayout.BeginHorizontal();
            GUILayout.Box("", boxStyle, GUILayout.ExpandWidth(true), GUILayout.Height(2));
            EditorGUILayout.EndHorizontal();
        }
        
        
        /// <summary>
        /// Draws a vertical line
        /// </summary>
        public static Rect DrawVerticalGUILine()
        {
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.background = new Texture2D(1, 1);
            boxStyle.normal.background.SetPixel(0, 0, Color.grey);
            boxStyle.normal.background.Apply();

            var one = EditorGUILayout.BeginVertical();
            GUILayout.Box("", boxStyle, GUILayout.ExpandHeight(true), GUILayout.Width(2));
            EditorGUILayout.EndVertical();

            return one;
        }
        
        
        /// <summary>
        /// Draws a vertical line
        /// </summary>
        public static void DrawVerticalGUILine(Color lineCol)
        {
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.background = new Texture2D(1, 1);
            boxStyle.normal.background.SetPixel(0, 0, lineCol);
            boxStyle.normal.background.Apply();

            var one = EditorGUILayout.BeginVertical();
            GUILayout.Box("", boxStyle, GUILayout.ExpandHeight(true), GUILayout.Width(2));
            EditorGUILayout.EndVertical();
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Editor Extension Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets the width of a string's GUI.
        /// </summary>
        /// <param name="text">The text to size.</param>
        /// <returns>The resulting size.</returns>
        public static float Width(this string text)
        {
            return GUI.skin.label.CalcSize(new GUIContent(text)).x;
        }
        
        
        /// <summary>
        /// Copies data from one instance of an asset to another.
        /// </summary>
        /// <param name="read">The asset to read from.</param>
        /// <param name="target">The asset to assign to.</param>
        public static void TransferProperties(SerializedObject read, SerializedObject target)
        {
            var dest = target;
            var propIterator = read.GetIterator();
            
            if (propIterator.NextVisible(true))
            {
                while (propIterator.NextVisible(true))
                {
                    var propElement = dest.FindProperty(propIterator.name);
                    
                    if (propElement == null || propElement.propertyType != propIterator.propertyType) continue;
                    dest.CopyFromSerializedProperty(propIterator);
                }
            }
            
            dest.ApplyModifiedProperties();
        }
    }
}