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
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// A helper class to save space on the audio manager editor class a little.
    /// </summary>
    public static class AudioManagerScriptHelper
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private static bool shouldUpdateDirectories;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Checks to see how many files are found from the scan so it can be displayed.
        /// </summary>
        /// <returns>Int | The amount of clips that have been found.</returns>
        public static int CheckAmount(SerializedProperty fileDirs)
        {
            var _amount = 0;
            var _allFiles = new List<string>();

            if (fileDirs == null) return 0;
            
            if (fileDirs.arraySize > 0)
            {
                for (var i = 0; i < fileDirs.arraySize; i++)
                {
                    if (Directory.Exists(Application.dataPath + fileDirs.GetArrayElementAtIndex(i).stringValue.Replace("Assets", "")))
                    {
                        // 2.3.1 - adds a range so it adds each directory to the asset 1 by 1
                        _allFiles.AddRange(new List<string>(Directory.GetFiles(Application.dataPath + fileDirs.GetArrayElementAtIndex(i).stringValue.Replace("Assets", ""))));
                    }
                    else
                    {
                        _allFiles = new List<string>();
                    }
                }
            }

            
            // Checks to see if there is anything in the path, if its empty it will not run the rest of the code and instead put a message in the console
            if (_allFiles.Count <= 0)
            {
                return _amount;
            }
            
            foreach (var _thingy in _allFiles)
            {
                var _path = _thingy.Replace(Application.dataPath, "Assets").Replace('\\', '/');
                
                if (AssetDatabase.LoadAssetAtPath(_path, typeof(AudioClip)))
                    ++_amount;
            }
            
            return _amount;
        }
        
                        
        /// <summary>
        /// Audio Manager Editor Method | gets the number of clips currently in this instance of the Audio Manager.
        /// </summary>
        /// <returns>Int | number of clips in the AMF on this Audio Manager.</returns>
        public static int GetNumberOfClips(SerializedProperty fileLib)
        {
            // 2.4.1 - fixed an issue where this if statement didn't fire where there was only 1 file, simple mistake xD, previous ">" now ">=".
            if (fileLib != null && fileLib.arraySize >= 1)
                return fileLib.arraySize;

            return 0;
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Directory Management Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Checks to see if there are no directories...
        /// </summary>
        /// <returns>Bool | Whether or not there is a directory in the list.</returns>
        public static bool AreAllDirectoryStringsBlank(SerializedProperty fileDirs)
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
        public static bool AreDupDirectories(SerializedProperty fileDirs)
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
        public static void AddToDirectories(SerializedProperty fileDirs, string value)
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
        public static void DirectoriesDisplay(SerializedProperty fileDirs)
        {
            var defaultCol = GUI.backgroundColor;
            var options = DirectorySelectHelper.GetDirectoriesFromBase(shouldUpdateDirectories);
            shouldUpdateDirectories = false;
            
            if (fileDirs.arraySize <= 0) return;
            for (var i = 0; i < fileDirs.arraySize; i++)
            {
                if (!options.Contains(fileDirs.GetArrayElementAtIndex(i).stringValue))
                {
                    AmLog.Warning($"Directory not valid with current base scan path: \"{fileDirs.GetArrayElementAtIndex(i).stringValue}\". The directory will be removed!");
                    fileDirs.DeleteArrayElementAtIndex(i);
                    continue;
                }
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Path #{i + 1}: ",  GUILayout.Width(AudioManagerEditorUtil.TextWidth($"Path #{i + 1}: ")));
                
                EditorGUI.BeginChangeCheck();
                fileDirs.GetArrayElementAtIndex(i).stringValue = DirectorySelectHelper.ConvertIntToDir(EditorGUILayout.Popup(DirectorySelectHelper.ConvertStringToIndex(fileDirs.GetArrayElementAtIndex(i).stringValue, options), options.ToArray()), options);
                if (EditorGUI.EndChangeCheck())
                {
                    shouldUpdateDirectories = true;
                    fileDirs.serializedObject.ApplyModifiedProperties();
                    fileDirs.serializedObject.Update();
                }
                
                
                if (i != fileDirs.arraySize)
                {
                    GUI.backgroundColor = AudioManagerEditorUtil.Green;

                    EditorGUI.BeginChangeCheck();
                    if (GUILayout.Button("+", GUILayout.Width(25)))
                        fileDirs.InsertArrayElementAtIndex(i);
                    if (EditorGUI.EndChangeCheck())
                    {
                        shouldUpdateDirectories = true;
                        fileDirs.serializedObject.ApplyModifiedProperties();
                        fileDirs.serializedObject.Update();
                    }
                }

                GUI.backgroundColor = defaultCol;

                if (!i.Equals(0))
                {
                    GUI.backgroundColor = AudioManagerEditorUtil.Red;

                    EditorGUI.BeginChangeCheck();
                    if (GUILayout.Button("-", GUILayout.Width(25)))
                        fileDirs.DeleteArrayElementAtIndex(i);
                    if (EditorGUI.EndChangeCheck())
                    {
                        shouldUpdateDirectories = true;
                        fileDirs.serializedObject.ApplyModifiedProperties();
                        fileDirs.serializedObject.Update();
                    }
                }
                else
                {
                    GUI.backgroundColor = AudioManagerEditorUtil.Hidden;
                    GUILayout.Button("", GUILayout.Width(25));
                }

                GUI.backgroundColor = defaultCol;
                EditorGUILayout.EndHorizontal();
            }
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Clip Management Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Adds all strings for the found clips to the AMF.
        /// </summary>
        public static void AddStrings(List<AudioClip> audioList, List<string> audioStrings)
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

            if (_ignored <= 0) return;
            
            // This message should never show up, but its here just in-case
            if (AudioManagerEditorUtil.Settings.showDebugMessages)
                AmLog.Warning($"{_ignored} entries ignored, this is due to the files either been in sub directories or other files that are not Unity AudioClips.");
        }

        
        /// <summary>
        /// Adds all the AudioClips to the AMF.
        /// </summary>
        public static void AddAudioClips(SerializedProperty fileDirs, SerializedProperty fileIsPopulated, List<AudioClip> audioList)
        {
            // Makes a new list the size of the amount of objects in the path
            // In V:2.3+ - Updated to allow for custom folders the hold audio clips (so users can organise their clips and still use the manager, default will just get all sounds in "/audio")
            var _allFiles = new List<string>();


            for (int i = 0; i < fileDirs.arraySize; i++)
            {
                if (Directory.Exists(Application.dataPath + fileDirs.GetArrayElementAtIndex(i).stringValue.Replace("Assets", "")))
                    // 2.3.1 - adds a range so it adds each directory to the asset 1 by 1.
                    _allFiles.AddRange(new List<string>(Directory.GetFiles(Application.dataPath + fileDirs.GetArrayElementAtIndex(i).stringValue.Replace("Assets", ""))));
                else
                {
                    // !Warning Message - shown in the console should there not be a directory named what the user entered
                    if (AudioManagerEditorUtil.Settings.showDebugMessages)
                        AmLog.Warning($"Path does not exist! please make sure you spelt your path correctly: {Application.dataPath + fileDirs.GetArrayElementAtIndex(i).stringValue.Replace("Assets", "")}");
                }
            }


            // Checks to see if there is anything in the path, if its empty it will not run the rest of the code and instead put a message in the console
            if (_allFiles.Any())
            {
                AudioClip source;

                foreach (var _thingy in _allFiles)
                {
                    var _path = "Assets" + _thingy.Replace(Application.dataPath, "").Replace('\\', '/');

                    if (!AssetDatabase.LoadAssetAtPath(_path, typeof(AudioClip)))
                    {
                        continue;
                    }

                    source = (AudioClip)AssetDatabase.LoadAssetAtPath(_path, typeof(AudioClip));
                    audioList.Add(source);
                }

                fileIsPopulated.boolValue = true;
            }
            else
            {
                // !Warning Message - shown in the console should there be no audio in the directory been scanned
                if (AudioManagerEditorUtil.Settings.showDebugMessages)
                    AmLog.Warning($"Please ensure there are Audio files in the directory: {Application.dataPath + AudioManagerEditorUtil.Settings.baseAudioScanPath}");
            }
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Help Label Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Shows a variety of help labels when stuff goes wrong, these just explain to the user what has happened and how they should go about fixing it.
        /// </summary>
        public static void HelpLabels(SerializedProperty file, SerializedProperty fileDirs, SerializedProperty fileLib, SerializedProperty fileIsPopulated, int lastTotal)
        {
            if (!file.objectReferenceValue || fileDirs.arraySize <= 0) return;
            if (!fileIsPopulated.boolValue) return;

            
            if (fileLib.arraySize > 0 && fileLib.arraySize != lastTotal)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                
                var _errorString = string.Empty;
                
                if (fileDirs.arraySize != 0)
                {
                    _errorString = "No clips found in one of these directories, please check there is audio in the following directory:\n";

                    for (var i = 0; i < fileDirs.arraySize; i++)
                        _errorString = _errorString + "assets" + AudioManagerEditorUtil.Settings.baseAudioScanPath + "/" + fileDirs.GetArrayElementAtIndex(i).stringValue + "\n";
                }
                else
                    _errorString = "No clips found in: " + "assets" + AudioManagerEditorUtil.Settings.baseAudioScanPath;

                EditorGUILayout.HelpBox(_errorString, MessageType.Info, true);
                EditorGUILayout.EndHorizontal();
            }
            else if (fileLib.arraySize != lastTotal)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();

                string _errorString;

                if (fileDirs.arraySize != 0)
                {
                    _errorString = "No clips found in one of these directories, please check there is audio in the following directory:\n";

                    for (var i = 0; i < fileDirs.arraySize; i++)
                        _errorString = _errorString + "assets/" + AudioManagerEditorUtil.Settings.baseAudioScanPath + "/" + fileDirs.GetArrayElementAtIndex(i).stringValue + "\n";
                }
                else
                    _errorString = "No clips found in: " + "assets/" + AudioManagerEditorUtil.Settings.baseAudioScanPath;

                EditorGUILayout.HelpBox(_errorString, MessageType.Info, true);
                EditorGUILayout.EndHorizontal();
            }
            else if (lastTotal.Equals(0))
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();

                string _errorString;

                if (fileDirs.arraySize != 0)
                {
                    _errorString = "No clips found in one of these directories, please check there is audio in the following directory:\n";

                    for (var i = 0; i < fileDirs.arraySize; i++)
                        _errorString = _errorString + "assets/" + AudioManagerEditorUtil.Settings.baseAudioScanPath + "/" + fileDirs.GetArrayElementAtIndex(i) + "\n";
                }
                else
                    _errorString = "No clips found in: " + "assets/" + AudioManagerEditorUtil.Settings.baseAudioScanPath;

                EditorGUILayout.HelpBox(_errorString, MessageType.Info, true);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }

        
        /// <summary>
        /// Updates the directories in the audio manager file selected when the base directory is changed (removes any invalid directories from the base change).
        /// </summary>
        public static void OnBaseDirectoryChanged(SerializedProperty fileDir)
        {
            DirectorySelectHelper.RefreshAllDirectories();
            var valid = DirectorySelectHelper.GetDirectoriesFromBase(true);
            
            if (fileDir == null) return;
            if (fileDir.arraySize <= 0) return;
            
            for (var i = fileDir.arraySize - 1; i >= 0; i--)
            {
                if (valid.Contains(fileDir.GetArrayElementAtIndex(i).stringValue)) continue;
                fileDir.DeleteArrayElementAtIndex(i);
            }

            fileDir.serializedObject.ApplyModifiedProperties();
            fileDir.serializedObject.Update();
        }
    }
}