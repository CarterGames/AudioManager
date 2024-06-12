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

using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the library editor window.
    /// </summary>
    public sealed class LibraryEditorWindow : EditorWindow
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static AudioLibrary library;
        private static Rect deselectRect;

        private static LibraryEditorWindow window;
        private static ILibraryTab libraryTab;
        private static ILibraryTab groupsTab;
        private static ILibraryTab mixerTab;
        private static ILibraryTab musicTab;

        private static bool isInitialized;

        private readonly string[] tabNames = new string[4] { "Library", "Groups", "Mixers", "Music" };
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The toolbar position for the window for this user.
        /// </summary>
        private static int TabPos
        {
            get => PerUserSettings.EditorTabPosition;
            set => PerUserSettings.EditorTabPosition = value;
        }

        
        /// <summary>
        /// The shown tab to the user at the moment.
        /// </summary>
        public static ILibraryTab ShownTab { get; private set; }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Menu Items
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Shows the library window when called.
        /// </summary>
        [MenuItem("Tools/Carter Games/Audio Manager/Library Editor", priority = 11)]
        public static void ShowWindow()
        {
            if (HasOpenInstances<LibraryEditorWindow>())
            {
                return;
            }

            window = GetWindow<LibraryEditorWindow>();

            window.titleContent = new GUIContent("Audio Library")
            {
                image = UtilEditor.FlatMusicalNoteIcon
            };

            window.minSize = new Vector2(500f, 300f);
            window.maxSize = new Vector2(950f, 750f);
            window.Show();
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Override Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The window GUI.
        /// </summary>
        private void OnGUI()
        {
            WindowInit();
            
            Undo.undoRedoPerformed -= ForceUpdate;
            Undo.undoRedoPerformed += ForceUpdate;
            
            if (library == null)
            {
                library = UtilEditor.Library;
            }

            DrawHeader();
            DrawTabButtons();
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Shows the window on a specific tab.
        /// </summary>
        /// <param name="tab">The pos to show.</param>
        public static void ShowWindowOnTab(int tab)
        {
            ShowWindow();
            PerUserSettings.EditorTabPosition = tab;
            GetWindow<LibraryEditorWindow>().Repaint();
        }
        

        /// <summary>
        /// Initializes the window when called.
        /// </summary>
        private static void WindowInit()
        {
            if (isInitialized) return;
            isInitialized = true;
            
            libraryTab = new LibraryEditorLibraryTab();
            groupsTab = new LibraryEditorGroupsTab();
            mixerTab = new LibraryEditorMixerTab();
            musicTab = new LibraryEditorMusicTab();
            
            ShownTab = TabPos switch
            {
                0 => libraryTab,
                1 => groupsTab,
                2 => mixerTab,
                3 => musicTab,
                _ => ShownTab
            };
            
            // if (UtilEditor.Library.LibraryTotal <= 0)
            // {
            //     AudioScanner.ScanForAudio(false);
            // }
            
            libraryTab.Initialize();
            groupsTab.Initialize();
            mixerTab.Initialize();
            musicTab.Initialize();
        }
        
        
        /// <summary>
        /// Draws the header for the editor window.
        /// </summary>
        private static void DrawHeader()
        {
            if (!UtilEditor.AudioManagerBanner) return;
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button(UtilEditor.OpenBookIcon, GUIStyle.none, GUILayout.MaxHeight(75)))
            {
                GUI.FocusControl(null);
            }
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }


        /// <summary>
        /// Draws the tab buttons on the window.
        /// </summary>
        private void DrawTabButtons()
        {
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            
            TabPos = GUILayout.Toolbar(TabPos, tabNames);
            
            if (EditorGUI.EndChangeCheck())
            {
                ShownTab = TabPos switch
                {
                    0 => libraryTab,
                    1 => groupsTab,
                    2 => mixerTab,
                    3 => musicTab,
                    _ => ShownTab
                };
            }
            
            GUILayout.Space(7.5f);

            switch (TabPos)
            {
                case 0:
                    libraryTab.Display();
                    break;
                case 1:
                    groupsTab.Display();
                    break;
                case 2:
                    mixerTab.Display();
                    break;
                case 3:
                    musicTab.Display();
                    break;
            }
        }


        /// <summary>
        /// Forces the GUI to update when called.
        /// </summary>
        public static void ForceUpdate()
        {
            if (!HasOpenInstances<LibraryEditorWindow>()) return;
            
            window ??= GetWindow<LibraryEditorWindow>();
            window.Repaint();
        }
    }
}