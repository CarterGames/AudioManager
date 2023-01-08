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

using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// The custom inspector for the audio player script.
    /// </summary>
    [CustomEditor(typeof(AudioPlayer)), CanEditMultipleObjects]
    public class AudioPlayerEditor : UnityEditor.Editor
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private AudioPlayer player;
        private Color normalBgCol;

        private SerializedProperty file;
        private SerializedProperty mixer;
        private SerializedProperty clipsToPlay;
        private SerializedProperty scrollPos;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private void OnEnable()
        {
            player = (AudioPlayer)target;
            
            file = serializedObject.FindProperty("audioManagerFile");
            mixer = serializedObject.FindProperty("mixer");
            clipsToPlay = serializedObject.FindProperty("clipsToPlay");
            scrollPos = serializedObject.FindProperty("scrollPos");

            normalBgCol = GUI.backgroundColor;
        }
        

        public override void OnInspectorGUI()
        {
            AudioManagerEditorUtil.Header("Audio Player");
            
            AudioSourceSetup();

            DrawScriptSection();
            GUILayout.Space(2.5f);
            
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(2.5f);
            
            DrawOptions();
            
            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();

            if (file.objectReferenceValue != null)
            {
                GUILayout.Space(2.5f);

                EditorGUILayout.BeginVertical("HelpBox");
                GUILayout.Space(2.5f);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Clips", EditorStyles.boldLabel, GUILayout.MaxWidth(92f));
                EditorGUILayout.EndHorizontal();
                
                GUILayout.Space(2.5f);

                if (clipsToPlay.arraySize > 0)
                {
                    scrollPos.vector2Value = EditorGUILayout.BeginScrollView(scrollPos.vector2Value, GUILayout.MaxHeight(400), GUILayout.MinHeight(0));
                    
                    for (var i = 0; i < clipsToPlay.arraySize; i++)
                        DrawClip(i);

                    EditorGUILayout.EndScrollView();
                }
                else
                {
                    clipsToPlay.InsertArrayElementAtIndex(0);
                    clipsToPlay.GetArrayElementAtIndex(0).FindPropertyRelative("show").boolValue = true;
                }
                
                GUILayout.Space(2.5f);
                EditorGUILayout.EndVertical();
            }
            
            
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Runs the setup for the audio source.
        /// </summary>
        private void AudioSourceSetup()
        {
            // Adds an Audio Source to the gameObject this script is on if its not already there (used for previewing audio only) 
            // * Hide flags hides it from the inspector so you don't notice it there *
            if (player.gameObject.GetComponent<AudioSource>()) return;
            
            player.gameObject.AddComponent<AudioSource>();
            player.gameObject.GetComponent<AudioSource>().hideFlags = HideFlags.HideInInspector;
        }
        

        /// <summary>
        /// Adds an element to the arrays for a new clip to be played.
        /// </summary>
        /// <param name="i">The element for the.</param>
        private void AddNewElement(int i)
        {
            clipsToPlay.InsertArrayElementAtIndex(i);
            clipsToPlay.GetArrayElementAtIndex(i).FindPropertyRelative("clipName").stringValue = string.Empty;
            clipsToPlay.GetArrayElementAtIndex(i).FindPropertyRelative("show").boolValue = true;
        }

        
        /// <summary>
        /// Removes an element from the arrays, deleting a clip from the list to be played.
        /// </summary>
        /// <param name="i">The element to remove.</param>
        private void RemoveElement(int i)
        {
            clipsToPlay.DeleteArrayElementAtIndex(i);
            clipsToPlay.serializedObject.ApplyModifiedProperties();
            clipsToPlay.serializedObject.Update();
        }
        
        
        /// <summary>
        /// Draws the script section of the custom inspector.
        /// </summary>
        private void DrawScriptSection()
        {
            GUILayout.Space(4.5f);
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(1.5f);
            
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(target as AudioPlayer), typeof(AudioPlayer), false);
            GUI.enabled = true;
            
            GUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
        }


        private void DrawOptions()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel, GUILayout.MaxWidth(92f));
            EditorGUILayout.EndHorizontal();

            // Audio Manager File (AMF) field
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Audio Manager File:", GUILayout.MaxWidth(140f));
            file.objectReferenceValue = (AudioManagerFile) EditorGUILayout.ObjectField(file.objectReferenceValue, typeof(AudioManagerFile), false);
            EditorGUILayout.EndHorizontal();

            if (file.objectReferenceValue == null) return;
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Audio Mixer:", GUILayout.MaxWidth(140f));
            mixer.objectReferenceValue = (AudioMixerGroup)EditorGUILayout.ObjectField(mixer.objectReferenceValue, typeof(AudioMixerGroup), false);
            EditorGUILayout.EndHorizontal();
        }
        
        
        /// <summary>
        /// Draws a clip setup in the custom inspector.
        /// </summary>
        /// <param name="index">The index of the clip to show.</param>
        private void DrawClip(int index)
        {
            // References...
            if (clipsToPlay.GetArrayElementAtIndex(index).serializedObject == null) return;

            var show = clipsToPlay.GetArrayElementAtIndex(index).FindPropertyRelative("show");
            var clipName = clipsToPlay.GetArrayElementAtIndex(index).FindPropertyRelative("clipName");
            var volume = clipsToPlay.GetArrayElementAtIndex(index).FindPropertyRelative("volume");
            var pitch = clipsToPlay.GetArrayElementAtIndex(index).FindPropertyRelative("pitch");
            var showOptional = clipsToPlay.GetArrayElementAtIndex(index).FindPropertyRelative("showOptional");
            var fromTime = clipsToPlay.GetArrayElementAtIndex(index).FindPropertyRelative("fromTime");
            var clipDelay = clipsToPlay.GetArrayElementAtIndex(index).FindPropertyRelative("clipDelay");

            //
            //

            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(2.5f);

            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();

            if (clipName.stringValue != "")
                show.boolValue = EditorGUILayout.Foldout(show.boolValue, clipName.stringValue);
            else
                show.boolValue = EditorGUILayout.Foldout(show.boolValue, "New Clip...");

            EditorGUI.indentLevel--;

            GUI.backgroundColor = AudioManagerEditorUtil.Green;
            if (GUILayout.Button("+", GUILayout.Width(30f)))
            {
                AddNewElement(clipsToPlay.arraySize);
            }

            GUI.backgroundColor = normalBgCol;

            if (!index.Equals(0))
            {
                GUI.backgroundColor = AudioManagerEditorUtil.Red;
                if (GUILayout.Button("-", GUILayout.Width(30f)))
                {
                    RemoveElement(index);
                    return;
                }

                GUI.backgroundColor = normalBgCol;
            }
            else
            {
                GUI.backgroundColor = AudioManagerEditorUtil.Hidden;
                GUILayout.Button(" ", GUILayout.Width(30f));
                GUI.backgroundColor = normalBgCol;
            }

            EditorGUILayout.EndHorizontal();


            if (show.boolValue)
            {
                EditorGUILayout.BeginVertical();
                GUILayout.Space(7.5f);

                EditorGUILayout.PropertyField(clipName);
                EditorGUILayout.PropertyField(volume);
                EditorGUILayout.PropertyField(pitch);


                EditorGUILayout.Space();

                EditorGUI.indentLevel++;
                showOptional.boolValue =
                    EditorGUILayout.Foldout(showOptional.boolValue, "Optional", EditorStyles.boldFont);
                EditorGUI.indentLevel--;

                if (showOptional.boolValue)
                {
                    EditorGUILayout.PropertyField(fromTime);
                    EditorGUILayout.PropertyField(clipDelay);
                }

                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
        }
    }
}