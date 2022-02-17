using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;
using System.IO;
using System.Linq;

/*
 * 
 *  Audio Manager
 *							  
 *	Audio Manager Editor
 *      The editor script for the Audio Manager, handles the custom inspector and the automation/storage of clips.
 *
 *  Warning:
 *	    Please refrain from editing this script as it will cause issues to the assets...
 *			
 *  Written by:
 *      Jonathan Carter
 *
 *  Published By:
 *      Carter Games
 *      E: hello@carter.games
 *      W: https://www.carter.games
 *		
 *  Version: 2.5.6
*	Last Updated: 09/02/2022 (d/m/y)							
 * 
 */

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Editor | The Audio Manager custom inspector editor script, should be placed in an /editor folder so to allow unity to make builds. 
    /// </summary>
    [CustomEditor(typeof(AudioManager)), CanEditMultipleObjects]
    public class AudioManagerEditor : UnityEditor.Editor
    {
        private const string DefaultBaseAudioScanPath = "/audio";
        private string BaseAudioFolderLocation = DefaultBaseAudioScanPath;

        private readonly string[] ScriptingDefines = new string[2] { "Use_CGAudioManager_Static", "USE_CG_AM_STATIC" };
        
        // Colours for the Editor Buttons
        private readonly Color32 greenCol = new Color32(41, 176, 97, 255);
        private readonly Color32 redCol = new Color32(190, 42, 42, 255);
        private readonly Color32 amRedCol = new Color32(255, 150, 157, 255);

        private Color normalColour;
        private Color normalBackgroundColour;
        private Color normalContentColour;

        private const string PlayIconLocation = "Play";
        private const string StopIconLocation = "Stop";
        private const string NoClipsInDirMessage = "No clips found in one of these directories, please check you have all directories spelt correctly:\n";

        private bool shouldShowMessage;                 // Defines whether or not to show a warning message on the inspector.

        private List<AudioClip> audioList;              // List of AudioClips used to add the audio to the library in the Audio Manager Script
        private List<string> audioStrings;              // List of Strings used to add the names of the audio clips to the library in the Audio Manager Script

        private AudioManager audioManagerScript;        // Reference to the Audio Manager Script that this script overrides the inspector for

        private string newPath = default;

        private bool showDirectories = false;           // Should the directories section be open?
        private bool showClips = true;                  // Should the clips section be open?
        private bool isSetup = false;                   // Has the initial setup been completed?

        private SerializedProperty file;
        private SerializedProperty lastFile;
        private SerializedObject fileObj;
        private SerializedProperty soundPrefab;
        private SerializedProperty audioMixerList;
        private SerializedProperty fileDirs;
        private SerializedProperty fileLib;
        private SerializedProperty fileIsPopulated;


        /// <summary>
        /// Assigns the script and sets the library up is it is null.
        /// </summary>
        private void OnEnable()
        {
            audioManagerScript = (AudioManager)target;            // References the Audio Manager Script

            ReferenceSetup();
            
            // Sets the boolean values up to what they were set to last.
            showDirectories = serializedObject.FindProperty("shouldShowDir").boolValue;
            showClips = serializedObject.FindProperty("shouldShowClips").boolValue;

            normalColour = GUI.color;
            normalBackgroundColour = GUI.backgroundColor;
            normalContentColour = GUI.contentColor;
        }

        
        /// <summary>
        /// OnInspectorGUI | Overrides the default inspector of the Audio Manager Script with this custom one.
        /// </summary>
        public override void OnInspectorGUI()
        {
            if (fileObj != null)
                fileObj.Update();
            
            serializedObject.Update();
            
            if (lastFile != file)
            {
                ReferenceSetup();
                serializedObject.Update();
                serializedObject.ApplyModifiedProperties();
            }
            

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
            AudioManagerEditorHelper.Header("Audio Manager", true, normalBackgroundColour);

            
            EditorGUILayout.BeginVertical("Box");
            
            GUILayout.Space(5f);
            
            GUI.contentColor = amRedCol;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Manager Settings", EditorStyles.boldLabel, GUILayout.MaxWidth(120f));
            EditorGUILayout.EndHorizontal();
            GUI.contentColor = normalContentColour;
            
            GUILayout.Space(5f);
            
            
            EditorGUILayout.BeginHorizontal();

            
            // Enable / Disable Static Instance Use...
#if !Use_CGAudioManager_Static && !USE_CG_AM_STATIC
            GUI.backgroundColor = greenCol;
            if (GUILayout.Button("Enable Static Instance"))
            {
                if (!IsScriptingDefinePresent(ScriptingDefines[0], EditorUserBuildSettings.activeBuildTarget)
                    && !IsScriptingDefinePresent(ScriptingDefines[1], EditorUserBuildSettings.activeBuildTarget))
                    AddScriptingDefine("USE_CG_AM_STATIC", EditorUserBuildSettings.activeBuildTarget);
            }
            GUI.backgroundColor = normalBackgroundColour;
#else
            GUI.backgroundColor = redCol;
            if (GUILayout.Button("Disable Static Instance"))

            {
                if (IsScriptingDefinePresent(ScriptingDefines[0], EditorUserBuildSettings.activeBuildTarget))
                    RemoveScriptingDefine("Use_CGAudioManager_Static", EditorUserBuildSettings.activeBuildTarget);
                
                if (IsScriptingDefinePresent(ScriptingDefines[1], EditorUserBuildSettings.activeBuildTarget))
                    RemoveScriptingDefine("USE_CG_AM_STATIC", EditorUserBuildSettings.activeBuildTarget);
            }
            GUI.backgroundColor = normalBackgroundColour;
#endif
            EditorGUILayout.EndHorizontal();
            

            #region Audio Manager File Assignment
            // Audio Manager File (AMF) field
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(file, new GUIContent("Audio Manager File: "));
            EditorGUILayout.EndHorizontal();
            #endregion

            #region Sound Prefab Area
            
            // if file exists
            if (file.objectReferenceValue && fileObj != null)
            {
                EditorGUILayout.BeginHorizontal();
                soundPrefab.objectReferenceValue = (GameObject) EditorGUILayout.ObjectField("Audio Prefab: ", soundPrefab.objectReferenceValue, typeof(GameObject), false);
                EditorGUILayout.EndHorizontal();
            }
            #endregion
            
            EditorGUILayout.EndVertical();
            
            
            
            GUILayout.Space(5f);
            
            

            EditorGUI.BeginChangeCheck();

            if (file.objectReferenceValue && fileObj != null)
            {
                if (soundPrefab != null)
                {
                    GUILayout.Space(10f);
                    DisplayMixers();
                    GUILayout.Space(10f);
                }
            }


            #region Directories & Clips Buttons & Displays
            if (file.objectReferenceValue)
            {
                // Directories & Clips Buttons
                if (soundPrefab != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    if (!showDirectories)
                    {
                        GUI.color = Color.cyan;
                        if (GUILayout.Button("Show Directories", GUILayout.Width(120)))
                        {
                            serializedObject.FindProperty("shouldShowDir").boolValue = !serializedObject.FindProperty("shouldShowDir").boolValue;
                            showDirectories = serializedObject.FindProperty("shouldShowDir").boolValue;
                        }
                    }
                    else
                    {
                        GUI.color = normalColour;
                        if (GUILayout.Button("Hide Directories", GUILayout.Width(120)))
                        {
                            serializedObject.FindProperty("shouldShowDir").boolValue = !serializedObject.FindProperty("shouldShowDir").boolValue;
                            showDirectories = serializedObject.FindProperty("shouldShowDir").boolValue;
                        }
                    }

                    if (!showClips)
                    {
                        GUI.color = Color.cyan;
                        if (GUILayout.Button("Show Clips", GUILayout.Width(95)))
                        {
                            serializedObject.FindProperty("shouldShowClips").boolValue = !serializedObject.FindProperty("shouldShowClips").boolValue;
                            showClips = serializedObject.FindProperty("shouldShowClips").boolValue;
                        }
                    }
                    else
                    {
                        GUI.color = normalColour;
                        if (GUILayout.Button("Hide Clips", GUILayout.Width(95)))
                        {
                            serializedObject.FindProperty("shouldShowClips").boolValue = !serializedObject.FindProperty("shouldShowClips").boolValue;
                            showClips = serializedObject.FindProperty("shouldShowClips").boolValue;
                        }
                    }

                    GUI.color = normalColour;
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();


                    // Directories Display
                    if (showDirectories)
                    {
                        EditorGUILayout.Space();

                        EditorGUILayout.BeginVertical("Box");

                        GUILayout.Space(5f);
            
                        GUI.contentColor = amRedCol;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Directories", EditorStyles.boldLabel, GUILayout.MaxWidth(120f));
                        EditorGUILayout.EndHorizontal();
                        GUI.contentColor = normalContentColour;
            
                        GUILayout.Space(5f);

                        // controls the directories 
                        if (fileDirs.arraySize > 0)
                        {
                            DirectoriesDisplay();
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("No directories on file, use the buttons below to add a new directory to scan.", MessageType.Info);
                            EditorGUILayout.Space();

                            EditorGUILayout.Space();
                            newPath = EditorGUILayout.TextField(new GUIContent("Path To Add:"), newPath);

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            GUI.color = greenCol;

                            if (GUILayout.Button("Continue", GUILayout.Width(80)))
                            {
                                if (fileDirs.arraySize > 0) goto SkipCont;
                                fileDirs.InsertArrayElementAtIndex(0);
                                AddToDirectories(newPath);
                            }
                            
                            SkipCont: ;

                            GUI.color = normalColour;
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUILayout.EndVertical();
                    }
                }

                GUILayout.Space(10f);


                // Clips Display
                if (showClips && !AreAllDirectoryStringsBlank() && !AreDupDirectories())
                {
                    EditorGUILayout.BeginVertical("Box");

                    GUILayout.Space(5f);
            
                    GUI.contentColor = amRedCol;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Clips", EditorStyles.boldLabel, GUILayout.MaxWidth(120f));
                    EditorGUILayout.EndHorizontal();
                    GUI.contentColor = normalContentColour;
            
                    GUILayout.Space(5f);

                    if (CheckAmount() > 0)
                    {
                        if (fileDirs.arraySize > 0 && (CheckAmount() > GetNumberOfClips()) || (CheckAmount() < GetNumberOfClips()))
                        {               
                            // Init Lists
                            audioList = new List<AudioClip>();
                            audioStrings = new List<string>();

                            // Auto filling the lists 
                            AddAudioClips();
                            AddStrings();

                            fileLib.ClearArray();

                            for (int i = 0; i < audioList.Count; i++)
                            {
                                fileLib.InsertArrayElementAtIndex(i);
                                fileLib.GetArrayElementAtIndex(i).FindPropertyRelative("key").stringValue = audioStrings[i];
                                fileLib.GetArrayElementAtIndex(i).FindPropertyRelative("value").objectReferenceValue = audioList[i];
                            }
                        }
                        
                        
                        if (fileDirs.arraySize > 0 && CheckAmount().Equals(GetNumberOfClips()))
                            DisplayNames();
                        else
                            HelpLabels();
                    }
                    else
                        EditorGUILayout.HelpBox("No audio clips found in your project.", MessageType.Warning);

                    EditorGUILayout.EndVertical();
                }
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

            if (fileObj != null)
                fileObj.ApplyModifiedProperties();
            
            serializedObject.ApplyModifiedProperties();
        }

        
        /// <summary>
        /// Runs the Init setup for the manager if needed....
        /// </summary>
        private void FirstSetup()
        {
            // Adds an Audio Source to the gameObject this script is on if its not already there (used for previewing audio only) 
            // * Hide flags hides it from the inspector so you don't notice it there *
            if (audioManagerScript.gameObject.GetComponent<AudioSource>())
            {
                audioManagerScript.gameObject.GetComponent<AudioSource>().hideFlags = HideFlags.HideInInspector;
                audioManagerScript.GetComponent<AudioSource>().playOnAwake = false;
            }
            else
            {
                audioManagerScript.gameObject.AddComponent<AudioSource>();
                audioManagerScript.gameObject.GetComponent<AudioSource>().hideFlags = HideFlags.HideInInspector;
                audioManagerScript.GetComponent<AudioSource>().playOnAwake = false;
            }
            
            
            var _asset = AssetDatabase.FindAssets("t:audiomanagersettingsdata", null);

            if (_asset.Length > 0)
            {
                var _path = AssetDatabase.GUIDToAssetPath(_asset[0]);
                var _loadedSettings =
                    (AudioManagerSettingsData)AssetDatabase.LoadAssetAtPath(_path, typeof(AudioManagerSettingsData));

                BaseAudioFolderLocation = _loadedSettings.baseAudioScanPath;
            }
            else
            {
                BaseAudioFolderLocation = DefaultBaseAudioScanPath;
            }


            // Makes the audio directory if it doesn't exist in your project
            // * This will not create a new folder if you already have an audio folder *
            // * As of V2 it will also create a new Audio Manager audioManagerFile if there isn't one in the audio folder *
            if (!Directory.Exists(Application.dataPath + BaseAudioFolderLocation))
            {
                AssetDatabase.CreateFolder("Assets", "Audio");

                if (!Directory.Exists(Application.dataPath + BaseAudioFolderLocation))
                {
                    AssetDatabase.CreateFolder("Assets" + BaseAudioFolderLocation, "Files");
                }

                AssetDatabase.CreateFolder("Assets" + BaseAudioFolderLocation, "Files");

                var _newAmf = ScriptableObject.CreateInstance<AudioManagerFile>();
                AssetDatabase.CreateAsset(_newAmf, "Assets" + BaseAudioFolderLocation + "/Files/Audio Manager File.asset");
                file.objectReferenceValue = (AudioManagerFile)AssetDatabase.LoadAssetAtPath("Assets" + BaseAudioFolderLocation + "/Files/Audio Manager File.asset", typeof(AudioManagerFile));
                _newAmf.directory = new List<string>();
                _newAmf.directory.Add("");
            }
            else if (((Directory.Exists(Application.dataPath + BaseAudioFolderLocation)) && (!Directory.Exists(Application.dataPath + BaseAudioFolderLocation + "/Files"))))
            {
                AssetDatabase.CreateFolder("Assets" + BaseAudioFolderLocation, "Files");
                var _newAmf = ScriptableObject.CreateInstance<AudioManagerFile>();
                AssetDatabase.CreateAsset(_newAmf, "Assets" + BaseAudioFolderLocation + "/Files/Audio Manager File.asset");
                file.objectReferenceValue = (AudioManagerFile)AssetDatabase.LoadAssetAtPath("Assets" + BaseAudioFolderLocation + "/Files/Audio Manager File.asset", typeof(AudioManagerFile));
                _newAmf.directory = new List<string>();
                _newAmf.directory.Add("");
            }
        }


        /// <summary>
        /// Checks to see how many files are found from the scan so it can be displayed.
        /// </summary>
        /// <returns>Int | The amount of clips that have been found.</returns>
        private int CheckAmount()
        {
            int _amount = default;
            var _allFiles = new List<string>();

            if (fileDirs == null) return 0;
            if (fileDirs.arraySize > 0)
            {
                for (int i = 0; i < fileDirs.arraySize; i++)
                {
                    if (Directory.Exists(Application.dataPath + BaseAudioFolderLocation + "/" + fileDirs.GetArrayElementAtIndex(i).stringValue))
                    {
                        // 2.3.1 - adds a range so it adds each directory to the asset 1 by 1
                        _allFiles.AddRange(new List<string>(Directory.GetFiles(Application.dataPath + BaseAudioFolderLocation + "/" + fileDirs.GetArrayElementAtIndex(i).stringValue)));
                    }
                    else
                    {
                        // !Warning Message - shown in the console should there not be a directory named what the user entered
                        _allFiles = new List<string>();
                        shouldShowMessage = true;
                    }
                }
            }

            // Checks to see if there is anything in the path, if its empty it will not run the rest of the code and instead put a message in the console
            if (_allFiles.Count <= 0) return _amount;

            foreach (var _thingy in _allFiles)
            {
                var _path = "Assets" + _thingy.Replace(Application.dataPath, "").Replace('\\', '/');

                if (AssetDatabase.LoadAssetAtPath(_path, typeof(AudioClip)))
                    ++_amount;
            }

            return _amount;
        }

        
        /// <summary>
        /// Adds all strings for the found clips to the AMF.
        /// </summary>
        private void AddStrings()
        {
            for (var i = 0; i < audioList.Count; i++)
            {
                if (audioList[i] == null)
                    audioList.Remove(audioList[i]);
            }

            var _ignored = 0;

            foreach (var _t in audioList)
            {
                if (_t.ToString().Contains("(UnityEngine.AudioClip)"))
                    audioStrings.Add(_t.ToString().Replace(" (UnityEngine.AudioClip)", ""));
                else
                    _ignored++;
            }

            if (_ignored > 0)
            {
                // This message should never show up, but its here just in-case
#if (!CarterGames_Assets_AudioManager_NoDebug)
                Debug.LogAssertion("* Audio Manager *: " + _ignored + " entries ignored, this is due to the files either been in sub directories or other files that are not Unity AudioClips.");
#endif
            }
        }

        
        /// <summary>
        /// Adds all the AudioClips to the AMF.
        /// </summary>
        private void AddAudioClips()
        {
            // Makes a new list the size of the amount of objects in the path
            // In V:2.3+ - Updated to allow for custom folders the hold audio clips (so users can organise their clips and still use the manager, default will just get all sounds in "/audio")
            var _allFiles = new List<string>();


            for (int i = 0; i < fileDirs.arraySize; i++)
            {
                if (fileDirs.GetArrayElementAtIndex(i).stringValue.Equals(""))
                    // 2.4.1 - fixed an issue where the directory order would break the asset finding files.
                    _allFiles.AddRange(new List<string>(Directory.GetFiles(Application.dataPath + BaseAudioFolderLocation)));
                else if (Directory.Exists(Application.dataPath + BaseAudioFolderLocation + "/" + fileDirs.GetArrayElementAtIndex(i).stringValue))
                    // 2.3.1 - adds a range so it adds each directory to the asset 1 by 1.
                    _allFiles.AddRange(new List<string>(Directory.GetFiles(Application.dataPath + BaseAudioFolderLocation + "/" + fileDirs.GetArrayElementAtIndex(i).stringValue)));
                else
                {
                    // !Warning Message - shown in the console should there not be a directory named what the user entered
#if (!CarterGames_Assets_AudioManager_NoDebug)
                    Debug.LogWarning("(*Audio Manager*): Path does not exist! please make sure you spelt your path correctly: " + Application.dataPath + BaseAudioFolderLocation + "/" + fileDirs.GetArrayElementAtIndex(i).stringValue);
#endif
                    shouldShowMessage = true;
                }
            }


            // Checks to see if there is anything in the path, if its empty it will not run the rest of the code and instead put a message in the console
            if (_allFiles.Any())
            {
                AudioClip _source;

                foreach (var _thingy in _allFiles)
                {
                    var _path = "Assets" + _thingy.Replace(Application.dataPath, "").Replace('\\', '/');

                    if (!AssetDatabase.LoadAssetAtPath(_path, typeof(AudioClip)))
                    {
                        continue;
                    }
                    
                    _source = (AudioClip)AssetDatabase.LoadAssetAtPath(_path, typeof(AudioClip));
                    audioList.Add(_source);
                }

                fileIsPopulated.boolValue = true;
            }
            else
            {
                // !Warning Message - shown in the console should there be no audio in the directory been scanned
#if (!CarterGames_Assets_AudioManager_NoDebug)
                Debug.LogWarning("(*Audio Manager*): Please ensure there are Audio files in the directory: " +
                                 Application.dataPath + BaseAudioFolderLocation);
#endif
            }
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
                GUI.color = greenCol;

                // If there are no clips playing it will show "preview clip" buttons for all elements
                if (!audioManagerScript.GetComponent<AudioSource>().isPlaying)
                {
                    if (Resources.Load<Texture2D>(PlayIconLocation))
                    {
                        if (GUILayout.Button(Resources.Load<Texture2D>(PlayIconLocation), GUIStyle.none, GUILayout.Width(20), GUILayout.Height(20)))
                        {
                            audioManagerScript.GetComponent<AudioSource>().clip = _elementAudio;
                            audioManagerScript.GetComponent<AudioSource>().time = 0f;
                            audioManagerScript.GetComponent<AudioSource>().Play();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("P", GUILayout.Width(20), GUILayout.Height(20)))
                        {
                            audioManagerScript.GetComponent<AudioSource>().clip = _elementAudio;
                            audioManagerScript.GetComponent<AudioSource>().time = 0f;
                            audioManagerScript.GetComponent<AudioSource>().Play();
                        }
                    }
                }
                // if a clip is playing, the clip that is playing will have a "stop clip" button instead of "preview clip" 
                else if (audioManagerScript.GetComponent<AudioSource>().clip.Equals(_elementAudio))
                {
                    GUI.color = redCol;

                    if (Resources.Load<Texture2D>(StopIconLocation))
                    {
                        if (GUILayout.Button(Resources.Load<Texture2D>(StopIconLocation), GUIStyle.none, GUILayout.Width(20), GUILayout.Height(20)))
                            audioManagerScript.GetComponent<AudioSource>().Stop();
                    }
                    else
                    {
                        if (GUILayout.Button("S", GUILayout.Width(20), GUILayout.Height(20)))
                            audioManagerScript.GetComponent<AudioSource>().Stop();
                    }
                }
                // This just ensures the rest of the elements keep a button next to them
                else
                {
                    if (Resources.Load<Texture2D>(PlayIconLocation))
                    {
                        if (GUILayout.Button(Resources.Load<Texture2D>(PlayIconLocation), GUIStyle.none, GUILayout.Width(20), GUILayout.Height(20)))
                        {
                            audioManagerScript.GetComponent<AudioSource>().clip = _elementAudio;
                            audioManagerScript.GetComponent<AudioSource>().PlayOneShot(_elementAudio);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("P", GUILayout.Width(20), GUILayout.Height(20)))
                        {
                            audioManagerScript.GetComponent<AudioSource>().clip = _elementAudio;
                            audioManagerScript.GetComponent<AudioSource>().PlayOneShot(_elementAudio);
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

        
        /// <summary>
        /// Creates the display that is used to show all the mixers
        /// </summary>
        private void DisplayMixers()
        {
            EditorGUILayout.BeginVertical("Box");

            // Going through all the audio clips and making an element in the Inspector for them
            if (fileObj != null)
            {
                if (file.objectReferenceValue && audioMixerList.arraySize > 0)
                {
                    GUILayout.Space(5f);

                    GUI.contentColor = amRedCol;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Mixers", EditorStyles.boldLabel, GUILayout.MaxWidth(120f));
                    EditorGUILayout.EndHorizontal();
                    GUI.contentColor = normalContentColour;

                    GUILayout.Space(5f);

                    for (var i = 0; i < audioMixerList.arraySize; i++)
                    {
                        // Starts the ordering
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.PrefixLabel("Mixer ID: '" + i + "'");

                        audioMixerList.GetArrayElementAtIndex(i).objectReferenceValue =
                            (AudioMixerGroup) EditorGUILayout.ObjectField(
                                audioMixerList.GetArrayElementAtIndex(i).objectReferenceValue, typeof(AudioMixerGroup),
                                false);


                        if (i != audioMixerList.arraySize)
                        {
                            GUI.color = greenCol;

                            if (GUILayout.Button("+", GUILayout.Width(25)))
                                audioMixerList.InsertArrayElementAtIndex(audioMixerList.arraySize);
                            
                            GUI.color = normalColour;
                        }
                        

                        if (!i.Equals(0))
                        {
                            GUI.color = Color.red;

                            if (GUILayout.Button("-", GUILayout.Width(25)))
                                audioMixerList.DeleteArrayElementAtIndex(i);
                            
                            GUI.color = normalColour;
                        }

                        // Ends the GUI ordering
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else if (file.objectReferenceValue && audioMixerList.arraySize <= 0)
                {
                    audioMixerList.InsertArrayElementAtIndex(0);
                }
            }

            EditorGUILayout.EndVertical();
        }

        
        /// <summary>
        /// Checks to see if there are no directories...
        /// </summary>
        /// <returns>Bool | Whether or not there is a directory in the list.</returns>
        private bool AreAllDirectoryStringsBlank()
        {
            var _check = 0;

            if (fileDirs == null || fileDirs.arraySize <= 1) return false;
            
            for (var i = 0; i < fileDirs.arraySize; i++)
            {
                if (fileDirs.GetArrayElementAtIndex(i).stringValue == "")
                    ++_check;
            }

            return _check.Equals(fileDirs.arraySize);

        }

        
        /// <summary>
        /// Checks to see if there are directories with the same name (avoid scanning when there is).
        /// </summary>
        /// <returns>Bool | Whether or not there is a duplicate directory in the list.</returns>
        private bool AreDupDirectories()
        {
            var _check = 0;

            if (fileDirs == null) return false;
            if (fileDirs.arraySize > 0)
            {
                if (fileDirs.arraySize < 2) return false;

                for (var i = 0; i < fileDirs.arraySize; i++)
                {
                    for (var j = 0; j < fileDirs.arraySize; j++)
                    {
                        // avoids checking the same position... as that would be true....
                        if (i.Equals(j)) continue;

                        string dir1, dir2;

                        dir1 = fileDirs.GetArrayElementAtIndex(i).stringValue.ToLower();
                        dir2 = fileDirs.GetArrayElementAtIndex(j).stringValue.ToLower();

                        dir1 = dir1.Replace("/", "");
                        dir2 = dir2.Replace("/", "");

                        if (dir1.Equals(dir2))
                        {
                            ++_check;
                        }
                    }
                }

                return _check > 0;
            }

            return false;
        }

        
        /// <summary>
        /// Adds the value inputted to both directories.
        /// </summary>
        /// <param name="value">String | The value to add.</param>
        private void AddToDirectories(string value)
        {
            if (!fileDirs.arraySize.Equals(1))
            {
                fileDirs.InsertArrayElementAtIndex(fileDirs.arraySize);
            }
            
            fileDirs.GetArrayElementAtIndex(fileDirs.arraySize - 1).stringValue = value;
        }

        
        /// <summary>
        /// Displays the directories if there are more than one in the AMF.
        /// </summary>
        private void DirectoriesDisplay()
        {
            if (fileDirs.arraySize <= 0) return;
            for (var i = 0; i < fileDirs.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                fileDirs.GetArrayElementAtIndex(i).stringValue = EditorGUILayout.TextField(new GUIContent("Path #" + (i + 1).ToString() + ": "), fileDirs.GetArrayElementAtIndex(i).stringValue);

                if (i != fileDirs.arraySize)
                {
                    GUI.color = greenCol;

                    if (GUILayout.Button("+", GUILayout.Width(25)))
                        fileDirs.InsertArrayElementAtIndex(i);
                }

                GUI.color = normalColour;

                if (!i.Equals(0))
                {
                    GUI.color = Color.red;

                    if (GUILayout.Button("-", GUILayout.Width(25)))
                        fileDirs.DeleteArrayElementAtIndex(fileDirs.arraySize - 1);
                }

                GUI.color = normalColour;
                EditorGUILayout.EndHorizontal();
            }
        }

        
        /// <summary>
        /// Audio Manager Editor Method | gets the number of clips currently in this instance of the Audio Manager.
        /// </summary>
        /// <returns>Int | number of clips in the AMF on this Audio Manager.</returns>
        private int GetNumberOfClips()
        {
            // 2.4.1 - fixed an issue where this if statement didn't fire where there was only 1 file, simple mistake xD, previous ">" now ">=".
            if (fileLib != null && fileLib.arraySize >= 1)
            {
                return fileLib.arraySize;
            }
            
            return 0;
        }



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
            var _asset = AssetDatabase.FindAssets("t:audiomanagersettingsdata", null);

            if (_asset.Length > 0) return;
            
            var _editorScript = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this)).Split('/');
            var _editorPath = string.Empty;

            for (int i = 0; i < _editorScript.Length; i++)
            {
                if (i.Equals(_editorScript.Length - 1)) continue;
                _editorPath += $"{_editorScript[i]}/";
            }
                
            AssetDatabase.CreateAsset(CreateInstance(typeof(AudioManagerSettingsData)), $"{_editorPath}Audio Manager Settings.asset");
            AssetDatabase.Refresh();
        }

        
        /// <summary>
        /// Shows a variety of help labels when stuff goes wrong, these just explain to the user what has happened and how they should go about fixing it.
        /// </summary>
        private void HelpLabels()
        {
            if (!file.objectReferenceValue || fileDirs.arraySize <= 0) return;
            
            if (fileIsPopulated.boolValue)
            {
                if (fileLib.arraySize > 0 && fileLib.arraySize != CheckAmount())
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();

                    string _errorString = null;

                    if (fileDirs.arraySize != 0)
                    {
                        _errorString = NoClipsInDirMessage;

                        for (var i = 0; i < fileDirs.arraySize; i++)
                        {
                            _errorString = _errorString + "assets" + BaseAudioFolderLocation + "/" + fileDirs.GetArrayElementAtIndex(i).stringValue + "\n";
                        }
                    }
                    else
                        _errorString = "No clips found in: " + "assets" + BaseAudioFolderLocation;

                    EditorGUILayout.HelpBox(_errorString, MessageType.Info, true);
                    EditorGUILayout.EndHorizontal();
                }
                else if (fileLib.arraySize != CheckAmount())
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();

                    string _errorString = null;

                    if (fileDirs.arraySize != 0)
                    {
                        _errorString = NoClipsInDirMessage;

                        for (int i = 0; i < fileDirs.arraySize; i++)
                        {
                            _errorString = _errorString + "assets/" + BaseAudioFolderLocation + "/" + fileDirs.GetArrayElementAtIndex(i).stringValue + "\n";
                        }
                    }
                    else
                        _errorString = "No clips found in: " + "assets/" + BaseAudioFolderLocation;

                    EditorGUILayout.HelpBox(_errorString, MessageType.Info, true);
                    EditorGUILayout.EndHorizontal();
                }
                else if (CheckAmount() == 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();

                    string _errorString = null;

                    if (fileDirs.arraySize != 0)
                    {
                        _errorString = NoClipsInDirMessage;

                        for (int i = 0; i < fileDirs.arraySize; i++)
                        {
                            _errorString = _errorString + "assets/" + BaseAudioFolderLocation + "/" + fileDirs.GetArrayElementAtIndex(i) + "\n";
                        }
                    }
                    else
                        _errorString = "No clips found in: " + "assets/" + BaseAudioFolderLocation;

                    EditorGUILayout.HelpBox(_errorString, MessageType.Info, true);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
            }
            else if (shouldShowMessage)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("There was a problem scanning, please check the console for more information", MessageType.Warning, true);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }
        
        
        

        
        
        
        
        // Helper stuff for scripting defines... (Makes it so the code can add a define and remove it without effecting the others in the project)
        // Patch in 2.5.1: Fixed an issue where this code would remove all other defines on addition or removal..
        private string GetScriptingDefines(BuildTarget buildTarget) 
        {
            var group = BuildPipeline.GetBuildTargetGroup(buildTarget);
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        }

        private void SetScriptingDefines(string scriptingDefines, BuildTarget buildTarget) 
        {
            var group = BuildPipeline.GetBuildTargetGroup(buildTarget);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, scriptingDefines);
        }

        private string[] GetScriptingDefinesCollection(BuildTarget buildTarget) 
        {
            string scriptingDefines = GetScriptingDefines(buildTarget);
            string[] separateScriptingDefines = scriptingDefines.Split(';');
            return separateScriptingDefines;
        }
        
        private bool IsScriptingDefinePresent(string scriptingDefine, BuildTarget buildTarget) 
        {
            string[] scriptingDefines = GetScriptingDefinesCollection(buildTarget);
            return scriptingDefines.Contains(scriptingDefine);
        }

        public void AddScriptingDefine(string define, params BuildTarget[] buildTargets)
        {
            if (buildTargets == null) return;

            foreach (BuildTarget buildTarget in buildTargets) 
            {
                if (IsScriptingDefinePresent(define, buildTarget)) continue;
                
                string scriptingDefines = GetScriptingDefines(buildTarget) + ";" + define;
                SetScriptingDefines(scriptingDefines, buildTarget);
            }
        }

        private void RemoveScriptingDefine(string define, params BuildTarget[] buildTargets)
        {
            if (buildTargets == null) return;

            foreach (BuildTarget buildTarget in buildTargets)
            {
                if (!IsScriptingDefinePresent(define, buildTarget)) continue;
                
                List<string> scriptingDefines = GetScriptingDefinesCollection(buildTarget).ToList();
                int removeIndex = scriptingDefines.FindIndex(item => item == define);
                scriptingDefines.RemoveAt(removeIndex);
                string updatedScriptingDefines = string.Join(";", scriptingDefines);
                SetScriptingDefines(updatedScriptingDefines, buildTarget);
            }
        }
    }
}