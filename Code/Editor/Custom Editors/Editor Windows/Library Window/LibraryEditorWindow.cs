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
    /// Handles the library editor window.
    /// </summary>
    public sealed class LibraryEditorWindow : EditorWindow
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private static AudioLibrary Library => ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef;
        private static Rect deselectRect;

        private static LibraryEditorWindow window;
        private static ILibraryTab libraryTab;
        private static ILibraryTab groupsTab;
        private static ILibraryTab mixerTab;

        private static bool isInitialized;

        private readonly string[] tabNames = new string[3] { "Library", "Groups", "Mixers" };

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
        [MenuItem("Tools/Carter Games/Audio Manager/Audio Library", priority = 11)]
        public static void ShowWindow()
        {
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

            ShownTab = TabPos switch
            {
                0 => libraryTab,
                1 => groupsTab,
                2 => mixerTab,
                _ => ShownTab
            };

            // if (UtilEditor.Library.LibraryTotal <= 0)
            // {
            //     AudioScanner.ScanForAudio(false);
            // }

            libraryTab.Initialize();
            groupsTab.Initialize();
            mixerTab.Initialize();
        }

        private void OnDisable()
        {
            EditorAudioClipPlayer.StopAll();
        }

        /// <summary>
        /// Draws the tab buttons on the window.
        /// </summary>
        private void DrawTabButtons()
        {
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            TabPos = GUILayout.Toolbar(TabPos, tabNames, GUILayout.Height(22.5f));

            if (EditorGUI.EndChangeCheck())
            {
                ShownTab = TabPos switch
                {
                    0 => libraryTab,
                    1 => groupsTab,
                    2 => mixerTab,
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