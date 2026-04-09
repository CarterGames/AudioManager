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
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using GUIStyle = UnityEngine.GUIStyle;

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

        private static SerializedObject LibObj => ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef;
        
        
        /// <summary>
        /// The currently selected property/element of the library clips.
        /// </summary>
        private static SerializedProperty SelectedProperty
        {
            get
            {
                if (selectedPropertyCache != null) return selectedPropertyCache;
                
                if (PerUserSettings.LastLibraryIndexShown > -1 && LibObj.Fp("library").Fpr("list").arraySize > 0)
                {
                    if (PerUserSettings.LastLibraryIndexShown > LibObj.Fp("library").Fpr("list").arraySize - 1)
                    {
                        PerUserSettings.LastLibraryIndexShown = LibObj.Fp("library").Fpr("list").arraySize - 1;
                    }
                    
                    selectedPropertyCache = LibObj.Fp("library").Fpr("list").GetIndex(PerUserSettings.LastLibraryIndexShown);
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
            if (LibObj.Fp("library").Fpr("list").arraySize <= 0)
            {
                EditorGUILayout.HelpBox("No clips in the library so there is nothing to show.", MessageType.Info);
                
                EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);
                
                GUI.backgroundColor = EditorColors.PrimaryYellow;
                if (GUILayout.Button("Scan For Audio", GUILayout.Height(25)))
                {
                    AudioScanner.ManualScan();
                }
                GUI.backgroundColor = Color.white;
                
                EditorGUI.EndDisabledGroup();
                
                return;
            }
            
            DrawSearchButton();
            
            DisplaySections();

            UtilEditor.CreateDeselectZone(ref deselectRect);
        }


        /// <summary>
        /// Changes the left control GUI to something else.
        /// </summary>
        protected override void LeftSectionControl()
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.MaxWidth(250));
            GUILayout.Space(5f);
            
            GUI.backgroundColor = EditorColors.PrimaryYellow;
            if (GUILayout.Button("Update Clips Struct", GUILayout.MaxHeight(25)))
            {
                if (EditorUtility.DisplayDialog("Update Clips Struct",
                        "Are you sure you want to update the clips struct?", "Update Clips Struct", "Cancel"))
                {
                    StructHandler.RefreshClips();
                }
            }
            GUI.backgroundColor = Color.white;
            
            if (PerUserSettings.ShowHelpBoxes)
            {
                EditorGUILayout.HelpBox("Press ^^^ to update the Clip.??? struct to have the latest changes",
                    MessageType.None);
            }
            
            EditorGUILayout.Space(1.5f);
            UtilEditor.DrawHorizontalGUILine();
            EditorGUILayout.Space(1.5f);
            
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
            if (SelectedProperty == null || !ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.LibraryLookup.ContainsKey(SelectedPropertyKey)) return;
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
            if (LibObj.Fp("library").Fpr("list").arraySize <= 0) return;
            
            for (var i = 0; i < LibObj.Fp("library").Fpr("list").arraySize; i++)
            {
                if (LibObj.Fp("library").Fpr("list").GetIndex(i) == null) continue;
                
                if (LibObj.Fp("library").Fpr("list").GetIndex(i).Fpr("key").stringValue == (SelectedPropertyKey))
                {
                    if (SelectedProperty != null)
                    {
                        if (SelectedProperty.Fpr("value").Fpr("key").stringValue.Equals(LibObj
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

                var toDisplay = new GUIContent(LibObj
                    .Fp("library").Fpr("list").GetIndex(i).Fpr("value").Fpr("key").stringValue);

                if (toDisplay.text.Length > 25)
                {
                    toDisplay.text = toDisplay.text.Substring(0, Mathf.Clamp(toDisplay.text.Length, 20, 35)) + "...";
                }
                
                if (GUILayout.Button(toDisplay))
                {
                    PerUserSettings.LastLibraryIndexShown = i;
                    
                    SelectedProperty = LibObj.Fp("library").Fpr("list")
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
            if (!CanUpdate) return;
            if (SelectedProperty == null || !ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.LibraryLookup.ContainsKey(SelectedPropertyKey)) return;
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
            LibObj.ApplyModifiedProperties();
            LibObj.Update();
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

            GUI.backgroundColor = EditorColors.PrimaryOrange;
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button(SearchButton, GUILayout.MaxHeight(25)))
            {
                SearchProviderInstancing.SearchProviderLibrary.SelectionMade.Add(ShowSelected);
                SearchProviderInstancing.SearchProviderLibrary.Open();
            }
            
            GUI.backgroundColor = Color.white;

            if (GUILayout.Button(TagsGUI.TagStdButtonContent, GUILayout.MaxWidth(50), GUILayout.MaxHeight(25)))
            {
                // open filter options...
                TagsEditor.ShowWindow();
            }

            EditorGUILayout.EndHorizontal();
            
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
                selectedPropertyCache = null;
                return;
            }
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            
            GUILayout.Space(2.5f);
            
            // File details for entry...
            EditorGUILayout.BeginVertical("HelpBox");
            EditorGUILayout.BeginHorizontal();
            
            GUI.contentColor = EditorColors.PrimaryYellow;
            EditorGUILayout.LabelField("File", EditorStyles.boldLabel);
            GUI.contentColor = Color.white;

            DrawPreviewButton(ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.LibraryLookup[prop.Fpr("key").stringValue]);
            EditorGUILayout.EndHorizontal();
            
            UtilEditor.DrawHorizontalGUILine();
            
            LibraryEditorClip.DrawLibraryEditor(prop);
            EditorGUILayout.EndVertical();
            
            
            // Spacer.
            EditorGUILayout.Space(5f);
            
            
            // Extra options for entry...
            EditorGUILayout.BeginVertical("HelpBox");
            
            GUI.contentColor = EditorColors.PrimaryYellow;
            EditorGUILayout.LabelField("Extra Options", EditorStyles.boldLabel);
            GUI.contentColor = Color.white;

            UtilEditor.DrawHorizontalGUILine();

            LibraryEditorDefaultClipSettings.DrawLibraryEditor(prop);
            LibraryEditorDynamicTime.DrawLibraryEditor(prop);
            
            EditorGUILayout.EndVertical();
            
            // Spacer.
            EditorGUILayout.Space(5f);
            
            // Metadata
            // EditorGUILayout.PropertyField(prop.Fpr("value").Fpr("metaData").Fpr("category"));
            
            EditorGUILayout.LabelField("Tags");
            UtilEditor.DrawHorizontalGUILine();

            TagsGUI.DrawClipSection(prop);
            
            EditorGUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }


        /// <summary>
        /// Shows the entry select when called.
        /// </summary>
        /// <param name="entry">The entry selected.</param>
        private void ShowSelected(SearchTreeEntry entry)
        {
            SearchProviderInstancing.SearchProviderLibrary.SelectionMade.Remove(ShowSelected);
            
            if (!LibraryEditorWindow.ShownTab.Equals(this)) return;
            
            PerUserSettings.LastLibraryIndexShown = ScriptableRef.GetAssetDef<AudioLibrary>().AssetRef.GetIndexOfData((AudioData)entry.userData);
            SelectedProperty = LibObj.Fp("library").Fpr("list").GetIndex(PerUserSettings.LastLibraryIndexShown);
        }
        
        
        /// <summary>
        /// Draws the preview button for the clip.
        /// </summary>
        /// <param name="data">The clip to play if pressed.</param>
        private static void DrawPreviewButton(AudioData data)
        {
            if (!EditorAudioClipPlayer.IsClipPlaying())
            {
                GUI.backgroundColor = EditorColors.PrimaryGreen;
                    
                if (GUILayout.Button(PreviewButton, GUILayout.Height(20), GUILayout.Width(125)))
                {
                    EditorAudioClipPlayer.Play(data);
                }
            }
            else if (EditorAudioClipPlayer.IsClipPlaying() && EditorAudioClipPlayer.CurrentClip == data.value)
            {
                GUI.backgroundColor = EditorColors.PrimaryRed;
                    
                if (GUILayout.Button(StopButton,GUILayout.Height(20), GUILayout.Width(125)))
                {
                    EditorAudioClipPlayer.StopAll();
                }
            }
            else
            {
                GUI.backgroundColor = EditorColors.PrimaryGreen;
                    
                if (GUILayout.Button(PreviewButton, GUILayout.Height(20), GUILayout.Width(125)))
                {
                    EditorAudioClipPlayer.Play(data);
                }
            }

            GUI.backgroundColor = Color.white;
        }
    }
}