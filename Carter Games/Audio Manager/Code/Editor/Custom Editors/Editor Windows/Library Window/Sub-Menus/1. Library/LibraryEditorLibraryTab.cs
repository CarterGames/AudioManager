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

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the library tab logic for the audio library editor window.
    /// </summary>
    public sealed class LibraryEditorLibraryTab : DividedDisplayBase, ILibraryTab
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private static readonly GUIContent PreviewButton = new GUIContent(" Preview Clip", UtilEditor.PlayIcon);
        private static readonly GUIContent StopButton = new GUIContent(" Stop Clip", UtilEditor.StopIcon);
        private static readonly GUIContent SearchButton = new GUIContent(" Search Library", UtilEditor.SearchIcon, "Opens a search provider to let you search through all entries in the library by name.");
        
        private static readonly Dictionary<SerializedProperty, Dictionary<string, SerializedProperty>> LibraryDisplayDataCache = new Dictionary<SerializedProperty, Dictionary<string, SerializedProperty>>();
        
        private static Rect deselectRect;
        private static SerializedProperty selectedPropertyCache;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// The currently selected property/element of the library clips.
        /// </summary>
        private static SerializedProperty SelectedProperty
        {
            get
            {
                if (selectedPropertyCache != null) return selectedPropertyCache;
                
                if (PerUserSettings.LastLibraryIndexShown > -1 && UtilEditor.LibraryObject.Fp("library").Fpr("list").arraySize > 0)
                {
                    if (PerUserSettings.LastLibraryIndexShown >
                        UtilEditor.LibraryObject.Fp("library").Fpr("list").arraySize - 1)
                    {
                        PerUserSettings.LastLibraryIndexShown =
                            UtilEditor.LibraryObject.Fp("library").Fpr("list").arraySize - 1;
                    }
                    
                    selectedPropertyCache = UtilEditor.LibraryObject.Fp("library").Fpr("list")
                        .GetIndex(PerUserSettings.LastLibraryIndexShown);
                }
                else
                {
                    selectedPropertyCache = null;
                }

                return selectedPropertyCache;
            }
            set
            {
                selectedPropertyCache = value;
                SelectedPropertyKey = value.Fpr("key").stringValue;
            }
        }


        public static string SelectedPropertyKey
        {
            get => SessionState.GetString("library-key", string.Empty);
            set => SessionState.SetString("library-key", value);
        }


        public static bool CanUpdate { get; set; } = true;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   ILibraryTab Implementation
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Initializes the tab for use.
        /// </summary>
        public void Initialize()
        {
            AudioManagerEditorEvents.OnLibraryRefreshed.Remove(RefreshLibraryCache);
            AudioManagerEditorEvents.OnLibraryRefreshed.Add(RefreshLibraryCache);
        }


        /// <summary>
        /// Displays the tab when called.
        /// </summary>
        public void Display()
        {
            if (UtilEditor.LibraryObject.Fp("library").Fpr("list").arraySize <= 0)
            {
                EditorGUILayout.HelpBox("No clips in the library so there is nothing to show.", MessageType.Info);
                
                EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);
                GUI.backgroundColor = UtilEditor.Yellow;
                
                if (GUILayout.Button("Scan For Audio", GUILayout.Height(25)))
                {
                    AudioScanner.ManualScan();
                }

                GUI.backgroundColor = Color.white;
                EditorGUI.EndDisabledGroup();
                
                return;
            }
            
            DisplaySections();

            UtilEditor.CreateDeselectZone(ref deselectRect);
        }


        /// <summary>
        /// Changes the left control GUI to something else.
        /// </summary>
        protected override void LeftSectionControl()
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.MaxWidth(250));
            
            GUI.backgroundColor = UtilEditor.Yellow;
            if (GUILayout.Button("Update Clips Struct", GUILayout.MaxHeight(25)))
            {
                StructHandler.RefreshClips();
            }
            GUI.backgroundColor = Color.white;
            
            if (PerUserSettings.ShowHelpBoxes)
            {
                EditorGUILayout.HelpBox("Press ^^^ to update the Clip.??? struct to have the latest changes",
                    MessageType.None);
            }
            
            GUILayout.Space(7.5f);
            UtilEditor.DrawHorizontalGUILine();
            GUILayout.Space(7.5f);
            
            PerUserSettings.LibBtnScrollRectPos = EditorGUILayout.BeginScrollView(PerUserSettings.LibBtnScrollRectPos);
            base.LeftSectionControl();
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Changes the right GUI control to something else.
        /// </summary>
        protected override void RightSectionControl()
        {
            PerUserSettings.LibScrollRectPos = EditorGUILayout.BeginScrollView(PerUserSettings.LibScrollRectPos);
            EditorGUILayout.BeginVertical("Box");
            base.RightSectionControl();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }


        /// <summary>
        /// Adds the implemented for the left GUI.
        /// </summary>
        protected override void OnLeftGUI()
        {
            if (UtilEditor.LibraryObject.Fp("library").Fpr("list").arraySize <= 0) return;
            
            for (var i = 0; i < UtilEditor.LibraryObject.Fp("library").Fpr("list").arraySize; i++)
            {
                if (UtilEditor.LibraryObject.Fp("library").Fpr("list").GetIndex(i) == null) continue;
                
                if (UtilEditor.LibraryObject.Fp("library").Fpr("list").GetIndex(i).Fpr("key").stringValue == (SelectedPropertyKey))
                {
                    if (SelectedProperty != null)
                    {
                        if (SelectedProperty.Fpr("value").Fpr("key").stringValue.Equals(UtilEditor.LibraryObject
                                .Fp("library").Fpr("list").GetIndex(i).Fpr("value").Fpr("key").stringValue))
                        {
                            GUI.backgroundColor = UtilEditor.Grey;
                        }
                        else
                        {
                            GUI.backgroundColor = Color.white;
                        }
                    }
                }

                if (GUILayout.Button(UtilEditor.LibraryObject.Fp("library").Fpr("list").GetIndex(i).Fpr("value").Fpr("key").stringValue))
                {
                    PerUserSettings.LastLibraryIndexShown = i;
                    
                    SelectedProperty = UtilEditor.LibraryObject.Fp("library").Fpr("list")
                        .GetIndex(PerUserSettings.LastLibraryIndexShown);
                }
                
                GUI.backgroundColor = Color.white;
            }
        }


        /// <summary>
        /// Adds the logic for the right side GUI.
        /// </summary>
        protected override void OnRightGUI()
        {
            DrawSearchButton();
            
            GUILayout.Space(10);

            if (SelectedProperty == null)
            {
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.BeginVertical("Box");
                EditorGUILayout.LabelField("Select a clip to see its contents here.");
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndVertical();
                return;
            }
            
            if (!CanUpdate) return;
            
            if (SelectedProperty == null || !UtilEditor.Library.LibraryLookup.ContainsKey(SelectedPropertyKey)) return;
            DrawLibraryRow(SelectedProperty);
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Utility Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// refreshes the library object when called.
        /// </summary>
        private void RefreshLibraryCache()
        {
            LibraryDisplayDataCache.Clear();
            UtilEditor.LibraryObject.ApplyModifiedProperties();
            UtilEditor.LibraryObject.Update();
            LibraryEditorWindow.ForceUpdate();
            CanUpdate = true;
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Drawer Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Draws the search & clear search buttons.
        /// </summary>
        private void DrawSearchButton()
        {
            EditorGUILayout.BeginVertical();

            var librarySearchProvider = ScriptableObject.CreateInstance<LibrarySearchProvider>();

            GUI.backgroundColor = UtilEditor.Yellow;
            
            if (GUILayout.Button(SearchButton, GUILayout.MaxHeight(25)))
            {
                LibrarySearchProvider.ToExclude.Clear();
                LibrarySearchProvider.OnSearchTreeSelectionMade.Add(ShowSelected);
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), librarySearchProvider);
            }
            
            GUI.backgroundColor = Color.white;

            if (PerUserSettings.ShowHelpBoxes)
            {
                EditorGUILayout.HelpBox(
                    "Press ^^^ to search through the entire library of scanned clips for a particular clip.",
                    MessageType.None);
            }

            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Draws a single row of the a library entry.
        /// </summary>
        /// <param name="prop">The property to display.</param>
        private static void DrawLibraryRow(SerializedProperty prop)
        {
            if (prop.Fpr("value") == null) return;

            if (prop.Fpr("value").Fpr("value").objectReferenceValue == null)
            {
                // AudioRemover.RemoveNullLibraryEntries();
                selectedPropertyCache = null;
                return;
            }
            
            EditorGUILayout.BeginHorizontal("HelpBox");
            EditorGUILayout.BeginVertical();

            var data = UtilEditor.Library.LibraryLookup[prop.Fpr("key").stringValue];
            
            DrawPreviewButton(data);
            
            EditorGUILayout.Space();
            UtilEditor.DrawHorizontalGUILine();
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginVertical("HelpBox");
            
            GUI.contentColor = UtilEditor.Yellow;
            EditorGUILayout.LabelField("File", EditorStyles.boldLabel);
            GUI.contentColor = Color.white;
            
            LibraryEditorClip.DrawLibraryEditor(prop);
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space();
            UtilEditor.DrawHorizontalGUILine();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical("HelpBox");
            
            GUI.contentColor = UtilEditor.Yellow;
            EditorGUILayout.LabelField("Extra's", EditorStyles.boldLabel);
            GUI.contentColor = Color.white;

            EditorGUILayout.Space();

            LibraryEditorDynamicTime.DrawLibraryEditor(prop);
            
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.Space();
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }


        /// <summary>
        /// Shows the entry select when called.
        /// </summary>
        /// <param name="entry">The entry selected.</param>
        private void ShowSelected(SearchTreeEntry entry)
        {
            LibrarySearchProvider.OnSearchTreeSelectionMade.Remove(ShowSelected);
            
            if (!LibraryEditorWindow.ShownTab.Equals(this)) return;
            
            PerUserSettings.LastLibraryIndexShown = UtilEditor.Library.GetIndexOfData((AudioData)entry.userData);
            selectedPropertyCache = UtilEditor.LibraryObject.Fp("library").Fpr("list")
                .GetIndex(PerUserSettings.LastLibraryIndexShown);
        }


        /// <summary>
        /// Draws the preview button for the clip.
        /// </summary>
        /// <param name="data">The clip to play if pressed.</param>
        private static void DrawPreviewButton(AudioData data)
        {
            EditorGUILayout.Space(1.5f);
                
            if (!EditorAudioClipPlayer.IsClipPlaying())
            {
                GUI.backgroundColor = UtilEditor.Green;
                    
                if (GUILayout.Button(PreviewButton, GUILayout.Height(20)))
                {
                    EditorAudioClipPlayer.Play(data);
                }
            }
            else if (EditorAudioClipPlayer.IsClipPlaying() && EditorAudioClipPlayer.CurrentClip == data.value)
            {
                GUI.backgroundColor = UtilEditor.Red;
                    
                if (GUILayout.Button(StopButton,GUILayout.Height(20)))
                {
                    EditorAudioClipPlayer.StopAll();
                }
            }
            else
            {
                GUI.backgroundColor = UtilEditor.Green;
                    
                if (GUILayout.Button(PreviewButton, GUILayout.Height(20)))
                {
                    EditorAudioClipPlayer.Play(data);
                }
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.Space(1.5f);
        }
    }
}