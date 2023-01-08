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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Editor | The Audio Manager custom inspector editor script, should be placed in an /editor folder so to allow unity to make builds. 
    /// </summary>
    [CustomEditor(typeof(AudioManager)), CanEditMultipleObjects]
    public class AudioManagerEditor : UnityEditor.Editor
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */   
        
        private static AudioSource Source;
        private static string[] Names;
        
        private Color normalColour;
        private Color normalBackgroundColour;
        private Color normalContentColour;

        private List<AudioClip> audioList;
        private List<string> audioStrings;

        private AudioManager audioManagerScript;
        
        private bool isSetup = false;
        private int lastTotal = 0;
        private int totalClipsInFile = 0;
        
        private AudioSource cachedSource;
        
        private SerializedProperty file;
        private SerializedProperty lastFile;
        private SerializedObject fileObj;
        private SerializedProperty soundPrefab;
        private SerializedProperty audioMixerList;
        private SerializedProperty fileDirs;
        private SerializedProperty fileLib;
        private SerializedProperty fileIsPopulated;

        private bool hasAmf;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */   
        
        /// <summary>
        /// Assigns the script and sets the library up is it is null.
        /// </summary>
        private void OnEnable()
        {
            audioManagerScript = (AudioManager)target;            // References the Audio Manager Script
            ReferenceSetup();
            UpdateNormalEditorColours();
            hasAmf = AudioManagerEditorUtil.HasAnyAMF;

            AudioManagerSettingsWindow.OnBaseDirectoryUpdated -= CallUpdateDirectories;
            AudioManagerSettingsWindow.OnBaseDirectoryUpdated += CallUpdateDirectories;

            CallUpdateDirectories();
        }


        /// <summary>
        /// OnInspectorGUI | Overrides the default inspector of the Audio Manager Script with this custom one.
        /// </summary>
        public override void OnInspectorGUI()
        {
            UpdateNormalEditorColours();
            fileObj?.Update();
            serializedObject.Update();
            
            #region First Setup
            // If the audio source is not attached
            if (!isSetup)
            {
                // Init Setup if needed (makes an audio folder and audio manager file if not already there and adds an audio source to the game object this is on so it can preview sounds)
                FirstSetup();
                isSetup = true;     // Confirms that the setup has been completed.
            }
            #endregion

            // Logo, Title & docs/discord links
            AudioManagerEditorUtil.Header("Audio Manager");
            
            DrawScriptSection();
            GUILayout.Space(2f);
            DrawManagementSection();
            GUILayout.Space(2f);
            

            // Mixers...
            if (file.objectReferenceValue && fileObj != null)
            {
                if (soundPrefab != null)
                {
                    DisplayMixers();
                }
            }

            GUILayout.Space(2f);

            #region Directories & Clips Buttons & Displays
            if (file.objectReferenceValue)
            {
                // Directories & Clips Buttons
                if (soundPrefab != null)
                {
                    DrawDirectories();
                }
                
                GUI.color = normalColour;

                GUILayout.Space(2.5f);

                // Clips Display
                #region Clips Display

                
                EditorGUILayout.BeginVertical("HelpBox");
                
                EditorGUI.indentLevel++;
                serializedObject.FindProperty("shouldShowClips").boolValue = EditorGUILayout.Foldout(serializedObject.FindProperty("shouldShowClips").boolValue, "Clips");
                EditorGUI.indentLevel--;
                
                
                if (serializedObject.FindProperty("shouldShowClips").boolValue && !AudioManagerScriptHelper.AreAllDirectoryStringsBlank(fileDirs) && !AudioManagerScriptHelper.AreDupDirectories(fileDirs))
                {
                    GUILayout.Space(5f);

                    lastTotal = AudioManagerScriptHelper.CheckAmount(fileDirs);
                    totalClipsInFile = AudioManagerScriptHelper.GetNumberOfClips(fileLib);

                    if (lastTotal > 0)
                    {
                        if (fileDirs.arraySize > 0 && (lastTotal > totalClipsInFile) || (lastTotal < totalClipsInFile))
                        {
                            // Init Lists
                            audioList = new List<AudioClip>();
                            audioStrings = new List<string>();

                            // Auto filling the lists 
                            AudioManagerScriptHelper.AddAudioClips(fileDirs, fileIsPopulated, audioList);
                            AudioManagerScriptHelper.AddStrings(audioList, audioStrings);

                            fileLib.ClearArray();

                            for (var i = 0; i < audioList.Count; i++)
                            {
                                fileLib.InsertArrayElementAtIndex(i);
                                fileLib.GetArrayElementAtIndex(i).FindPropertyRelative("key").stringValue = audioStrings[i];
                                fileLib.GetArrayElementAtIndex(i).FindPropertyRelative("value").objectReferenceValue = audioList[i];
                            }
                        }


                        if (fileDirs.arraySize > 0 && lastTotal.Equals(totalClipsInFile))
                            DisplayNames();
                        else
                            AudioManagerScriptHelper.HelpLabels(file, fileDirs, fileLib, fileIsPopulated, lastTotal);
                    }
                    else
                        EditorGUILayout.HelpBox("Cannot scan project due to the directories entered not containing any audio clips.", MessageType.Warning);
                }
                
                EditorGUILayout.EndVertical();

                #endregion
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("Please assign a Audio Manager File to use this asset.", MessageType.Info, true);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
            #endregion
            
            fileObj?.ApplyModifiedProperties();
            serializedObject.ApplyModifiedProperties();
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Referencing Setup Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */  
        
        /// <summary>
        /// Runs the Init setup for the manager if needed.
        /// </summary>
        private void FirstSetup()
        {
            cachedSource = audioManagerScript.gameObject.GetComponent<AudioSource>();
            
            // Adds an Audio Source to the gameObject this script is on if its not already there (used for previewing audio only) 
            // * Hide flags hides it from the inspector so you don't notice it there *
            if (cachedSource)
            {
                cachedSource.hideFlags = HideFlags.HideInInspector;
                cachedSource.playOnAwake = false;
            }
            else
            {
                audioManagerScript.gameObject.AddComponent<AudioSource>();
                cachedSource = audioManagerScript.gameObject.GetComponent<AudioSource>();
                cachedSource.hideFlags = HideFlags.HideInInspector;
                cachedSource.playOnAwake = false;
            }
        }
        
        
        /// <summary>
        /// Sets up all the references needed for the editor.
        /// </summary>
        private void ReferenceSetup()
        {
            file = serializedObject.FindProperty("audioManagerFile");
            lastFile = serializedObject.FindProperty("audioManagerFile");

            if (file.objectReferenceValue)
            {
                fileObj = new SerializedObject(file.objectReferenceValue);
                soundPrefab = fileObj.FindProperty("soundPrefab");
                audioMixerList = fileObj.FindProperty("audioMixer");
                fileDirs = fileObj.FindProperty("directory");
                fileLib = fileObj.FindProperty("library");
                fileIsPopulated = fileObj.FindProperty("isPopulated");
            }
            
            
            if (file.objectReferenceValue && audioMixerList.arraySize == 0)
                audioMixerList.InsertArrayElementAtIndex(0);
            
            if (fileObj != null)
                fileObj.Update();
            
            // Setup data asset is needed...
            var _asset = AssetDatabase.FindAssets("t:audiomanagersettings", null);

            if (_asset.Length > 0) return;
            
            var _editorScript = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)).Split('/');
            var _editorPath = string.Empty;

            for (int i = 0; i < _editorScript.Length; i++)
            {
                if (i.Equals(_editorScript.Length - 1)) continue;
                _editorPath += $"{_editorScript[i]}/";
            }
            
            AssetDatabase.Refresh();
        }

        
        /// <summary>
        /// Updates the default colours of the editor that are saved to return the editor to defaults when not changing.
        /// </summary>
        private void UpdateNormalEditorColours()
        {
            normalColour = GUI.color;
            normalBackgroundColour = GUI.backgroundColor;
            normalContentColour = GUI.contentColor;
        }


        private void CallUpdateDirectories()
        {
            AudioManagerScriptHelper.OnBaseDirectoryChanged(fileDirs);
            Repaint();
        }
        
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Draw Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */ 
        
        /// <summary>
        /// Draws the script section of the custom inspector.
        /// </summary>
        private void DrawScriptSection()
        {
            GUILayout.Space(4.5f);
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(1.5f);
            
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(target as AudioManager), typeof(AudioManager), false);
            GUI.enabled = true;
            
            GUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Draws the main management section of the custom inspector.
        /// </summary>
        private void DrawManagementSection()
        {
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(3.5f);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Manager Settings", EditorStyles.boldLabel, GUILayout.MaxWidth(120f));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2.5f);
            
            EditorGUILayout.BeginHorizontal();

            
            // Enable / Disable Static Instance Use...
#if !Use_CGAudioManager_Static && !USE_CG_AM_STATIC
            GUI.backgroundColor = AudioManagerEditorUtil.Green;
            if (GUILayout.Button("Enable Static Instance"))
            {
                if (!ScriptingDefineHandler.IsScriptingDefinePresent())
                    ScriptingDefineHandler.AddScriptingDefine("USE_CG_AM_STATIC", EditorUserBuildSettings.activeBuildTarget);

                AudioManagerEditorUtil.Settings.isUsingStatic = true;
            }
            GUI.backgroundColor = normalBackgroundColour;
#else
            GUI.backgroundColor = AudioManagerEditorUtil.Red;
            if (GUILayout.Button("Disable Static Instance"))

            {
                if (ScriptingDefineHandler.IsScriptingDefinePresent())
                {
                    ScriptingDefineHandler.RemoveScriptingDefine("Use_CGAudioManager_Static", EditorUserBuildSettings.activeBuildTarget);
                    ScriptingDefineHandler.RemoveScriptingDefine("USE_CG_AM_STATIC", EditorUserBuildSettings.activeBuildTarget);
                }
            }

            AudioManagerEditorUtil.Settings.isUsingStatic = false;

            GUI.backgroundColor = normalBackgroundColour;
#endif
            
            
            EditorGUILayout.EndHorizontal();
            

            // Audio Manager File (AMF) field
            EditorGUI.BeginChangeCheck();

            if (hasAmf)
            {
                EditorGUILayout.PropertyField(file, new GUIContent("Audio Manager File: "));
            }
            else
            {
                GUI.backgroundColor = AudioManagerEditorUtil.Yellow;
                
                if (GUILayout.Button("Generate Audio Manager File (Required)"))
                {
                    AssetDatabase.CreateAsset(CreateInstance(typeof(AudioManagerFile)), "Assets/Resources/Carter Games/Audio Manager/Audio Manager File.asset");
                    AssetDatabase.Refresh();
                    file.objectReferenceValue = (AudioManagerFile) AudioManagerEditorUtil.GetFile<AudioManagerFile>("t:audiomanagerfile");
                    hasAmf = file.objectReferenceValue != null;
                    Repaint();
                }

                GUI.backgroundColor = normalBackgroundColour;
            }
            


            if (EditorGUI.EndChangeCheck() && lastFile != file)
            {
                ReferenceSetup();
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }


            // if file exists
            if (file.objectReferenceValue && fileObj != null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(soundPrefab, new GUIContent("Sound Prefab"));
                EditorGUILayout.EndHorizontal();
                
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                }
            }

            EditorGUILayout.EndVertical();
        }
        
        
        /// <summary>
        /// Creates the display that is used to show all the mixers
        /// </summary>
        private void DisplayMixers()
        {
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(2f);
            
            EditorGUI.indentLevel++;
            serializedObject.FindProperty("shouldShowMixers").boolValue = EditorGUILayout.Foldout(serializedObject.FindProperty("shouldShowMixers").boolValue, "Mixers");
            EditorGUI.indentLevel--;
            
            
            if (serializedObject.FindProperty("shouldShowMixers").boolValue)
            {
                // Going through all the audio clips and making an element in the Inspector for them
                if (fileObj != null)
                {
                    if (file.objectReferenceValue && audioMixerList.arraySize > 0)
                    {
                        GUILayout.Space(2.5f);

                        for (var i = 0; i < audioMixerList.arraySize; i++)
                        {
                            // Starts the ordering
                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.PrefixLabel("Mixer ID: '" + i + "'");

                            audioMixerList.GetArrayElementAtIndex(i).objectReferenceValue =
                                (AudioMixerGroup)EditorGUILayout.ObjectField(
                                    audioMixerList.GetArrayElementAtIndex(i).objectReferenceValue,
                                    typeof(AudioMixerGroup),
                                    false);


                            if (i != audioMixerList.arraySize)
                            {
                                GUI.backgroundColor = AudioManagerEditorUtil.Green;

                                if (GUILayout.Button("+", GUILayout.Width(25)))
                                    audioMixerList.InsertArrayElementAtIndex(audioMixerList.arraySize);
                            }


                            if (!i.Equals(0))
                            {
                                GUI.backgroundColor = AudioManagerEditorUtil.Red;

                                if (GUILayout.Button("-", GUILayout.Width(25)))
                                    audioMixerList.DeleteArrayElementAtIndex(i);
                            }
                            else
                            {
                                GUI.backgroundColor = AudioManagerEditorUtil.Hidden;
                                GUILayout.Button("", GUILayout.Width(25));
                            }

                            // Ends the GUI ordering
                            EditorGUILayout.EndHorizontal();

                            GUI.backgroundColor = normalBackgroundColour;
                            GUI.contentColor = normalContentColour;
                        }
                    }
                    else if (file.objectReferenceValue && audioMixerList.arraySize <= 0)
                    {
                        audioMixerList.InsertArrayElementAtIndex(0);
                    }
                }
            }

            GUILayout.Space(2f);
            EditorGUILayout.EndVertical();
        }

        
        /// <summary>
        /// Draws the directories section of the custom inspector.
        /// </summary>
        private void DrawDirectories()
        {
            GUI.color = normalColour;

            // Directories Display
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(2f);

            EditorGUI.indentLevel++;
            serializedObject.FindProperty("shouldShowDir").boolValue = EditorGUILayout.Foldout(serializedObject.FindProperty("shouldShowDir").boolValue, "Directories");
            EditorGUI.indentLevel--;


            if (serializedObject.FindProperty("shouldShowDir").boolValue)
            {
                // controls the directories 
                if (fileDirs.arraySize > 0)
                {
                    AudioManagerScriptHelper.DirectoriesDisplay(fileDirs);
                }
                else
                {
                    fileDirs.InsertArrayElementAtIndex(0);
                    AudioManagerScriptHelper.AddToDirectories(fileDirs, AudioManagerEditorUtil.Settings.baseAudioScanPath);
                    AudioManagerScriptHelper.DirectoriesDisplay(fileDirs);
                }
            }

            GUILayout.Space(2f);
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Creates the display that is used to show all the clips with play/stop buttons next to them.
        /// </summary>
        private void DisplayNames()
        {
            // Used as a placeholder for the clip name each loop
            string _elementString;
            AudioClip _elementAudio;

            // Going through all the audio clips and making an element in the Inspector for them
            if (!file.objectReferenceValue || fileLib.arraySize <= 0) return;
            
            for (var i = 0; i < fileLib.arraySize; i++)
            {
                _elementString = fileLib.GetArrayElementAtIndex(i).FindPropertyRelative("key").stringValue;
                _elementAudio = (AudioClip) fileLib.GetArrayElementAtIndex(i).FindPropertyRelative("value").objectReferenceValue;

                // Starts the ordering
                EditorGUILayout.BeginHorizontal();

                // Changes the GUI colour to green for the buttons
                GUI.color = AudioManagerEditorUtil.Green;

                // If there are no clips playing it will show "preview clip" buttons for all elements
                if (!cachedSource.isPlaying)
                {
                    if (AudioManagerEditorUtil.PlayButton != null)
                    {
                        if (GUILayout.Button(AudioManagerEditorUtil.PlayButton, GUIStyle.none, GUILayout.Width(20), GUILayout.Height(20)))
                        {
                            cachedSource.clip = _elementAudio;
                            cachedSource.time = 0f;
                            cachedSource.Play();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("P", GUILayout.Width(20), GUILayout.Height(20)))
                        {
                            cachedSource.clip = _elementAudio;
                            cachedSource.time = 0f;
                            cachedSource.Play();
                        }
                    }
                }
                // if a clip is playing, the clip that is playing will have a "stop clip" button instead of "preview clip" 
                else if (cachedSource.clip.Equals(_elementAudio))
                {
                    GUI.color = AudioManagerEditorUtil.Red;

                    if (AudioManagerEditorUtil.StopButton != null)
                    {
                        if (GUILayout.Button(AudioManagerEditorUtil.StopButton, GUIStyle.none, GUILayout.Width(20), GUILayout.Height(20)))
                            cachedSource.Stop();
                    }
                    else
                    {
                        if (GUILayout.Button("S", GUILayout.Width(20), GUILayout.Height(20)))
                            cachedSource.Stop();
                    }
                }
                // This just ensures the rest of the elements keep a button next to them
                else
                {
                    if (AudioManagerEditorUtil.PlayButton != null)
                    {
                        if (GUILayout.Button(AudioManagerEditorUtil.PlayButton, GUIStyle.none, GUILayout.Width(20), GUILayout.Height(20)))
                        {
                            cachedSource.clip = _elementAudio;
                            cachedSource.PlayOneShot(_elementAudio);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("P", GUILayout.Width(20), GUILayout.Height(20)))
                        {
                            cachedSource.clip = _elementAudio;
                            cachedSource.PlayOneShot(_elementAudio);
                        }
                    }
                }

                // Resets the GUI colour
                GUI.color = normalColour;

                // Adds the text for the clip
                EditorGUILayout.TextArea(_elementString, GUILayout.ExpandWidth(true));

                // Ends the GUI ordering
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}