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
using UnityEditor;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// The custom inspector for the music player script.
    /// </summary>
    [CustomEditor(typeof(MusicPlayer)), CanEditMultipleObjects]
    public class MusicPlayerEditor : UnityEditor.Editor
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private MusicPlayer player;
        private Color normalBgCol;
        
        private SerializedProperty musicTrack;
        private SerializedProperty mixer;
        private SerializedProperty timeToStartFrom;
        private SerializedProperty shouldLoop;
        private SerializedProperty playOnAwake;
        private SerializedProperty timeToLoopAt;
        private SerializedProperty showSource;
        private SerializedProperty volume;
        private SerializedProperty pitch;
        private SerializedProperty musicIntroTransition;
        private SerializedProperty transitionLength;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        

        private void OnEnable()
        {
            player = (MusicPlayer)target;
            
            musicTrack = serializedObject.FindProperty("musicTrack");
            mixer = serializedObject.FindProperty("mixer");
            timeToStartFrom = serializedObject.FindProperty("timeToStartFrom");
            shouldLoop = serializedObject.FindProperty("shouldLoop");
            timeToLoopAt = serializedObject.FindProperty("timeToLoopAt");
            showSource = serializedObject.FindProperty("showSource");
            volume = serializedObject.FindProperty("volume");
            pitch = serializedObject.FindProperty("pitch");
            playOnAwake = serializedObject.FindProperty("playOnAwake");
            musicIntroTransition = serializedObject.FindProperty("introTransition");
            transitionLength = serializedObject.FindProperty("transitionLength");

            normalBgCol = GUI.backgroundColor;
        }
        
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!player)
                player = (MusicPlayer)target;

            EditorGUI.BeginChangeCheck();
            
            AudioManagerEditorUtil.Header("Music Player");
            AudioSourceSetup();
            DrawScriptSection();
            
            GUILayout.Space(2.5f);
            DrawTrackSection();
            
            //
            //

            GUILayout.Space(2.5f);
            DrawFirstPlaySetup();
            GUILayout.Space(2.5f);
            
            //
            //

            DrawSourcesSection();

            if (!EditorGUI.EndChangeCheck()) return;
            serializedObject.ApplyModifiedProperties();
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Draws the script section in the custom inspector.
        /// </summary>
        private void DrawScriptSection()
        {
            GUILayout.Space(4.5f);
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(1.5f);
            
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(target as MusicPlayer), typeof(MusicPlayer), false);
            GUI.enabled = true;
            
            GUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Draws the track section of the custom inspector.
        /// </summary>
        private void DrawTrackSection()
        {
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(2.5f);
            
            EditorGUILayout.LabelField("Track Info", EditorStyles.boldLabel, GUILayout.MaxWidth(92f));

            GUILayout.Space(2.5f);
            
            EditorGUILayout.PropertyField(musicTrack, new GUIContent("Track To Play:"));
            EditorGUILayout.PropertyField(mixer, new GUIContent("Music Audio Mixer"));

            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
        }
        
        
        /// <summary>
        /// Draws the first setup section of the custom inspector.
        /// </summary>
        private void DrawFirstPlaySetup()
        {
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(2.5f);
            
            EditorGUILayout.LabelField("First Play Setup", EditorStyles.boldLabel, GUILayout.MaxWidth(125f));

            GUILayout.Space(2.5f);
            
            EditorGUILayout.PropertyField(playOnAwake, new GUIContent("Play On Awake:"));
            EditorGUILayout.PropertyField(musicIntroTransition, new GUIContent("Intro Transition:"));
            EditorGUILayout.PropertyField(transitionLength, new GUIContent("Transition Length:"));
            volume.floatValue = EditorGUILayout.Slider( new GUIContent("Track Volume:"), volume.floatValue, 0f, 1f);
            pitch.floatValue = EditorGUILayout.Slider( new GUIContent("Track Pitch:"), pitch.floatValue, 0f, 1f);
            EditorGUILayout.PropertyField(shouldLoop, new GUIContent("Should Loop Track:"));
            EditorGUILayout.PropertyField(timeToStartFrom, new GUIContent("Start Track At:"));
            EditorGUILayout.PropertyField(timeToLoopAt, new GUIContent("Loop Track At:"));
            
            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Draws the sources button in the custom inspector.
        /// </summary>
        private void DrawSourcesSection()
        {
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(2.5f);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (!showSource.boolValue)
            {
                GUI.backgroundColor = AudioManagerEditorUtil.Green;
                if (GUILayout.Button("Show Audio Sources", GUILayout.Width(140f)))
                {
                    player.GetComponents<AudioSource>()[0].hideFlags = HideFlags.None;
                    player.GetComponents<AudioSource>()[1].hideFlags = HideFlags.None;
                    showSource.boolValue = true;
                }
                GUI.backgroundColor = normalBgCol;
            }
            else
            {
                GUI.backgroundColor = AudioManagerEditorUtil.Red;
                if (GUILayout.Button("Hide Audio Sources", GUILayout.Width(140f)))
                {
                    player.GetComponents<AudioSource>()[0].hideFlags = HideFlags.HideInInspector;
                    player.GetComponents<AudioSource>()[1].hideFlags = HideFlags.HideInInspector;
                    showSource.boolValue = false;
                }
                GUI.backgroundColor = normalBgCol;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
        }
        
        
        /// <summary>
        /// Sets up the audio sources for the player to use.
        /// </summary>
        private void AudioSourceSetup()
        {
            // Adds an Audio Source to the gameObject this script is on if its not already there
            // * Hide flags hides it from the inspector so you don't notice it there *
            if (player.gameObject.GetComponent<AudioSource>()) return;
            
            player.gameObject.AddComponent<AudioSource>();
            player.gameObject.AddComponent<AudioSource>();
            player.gameObject.GetComponents<AudioSource>()[0].hideFlags = HideFlags.HideInInspector;
            player.gameObject.GetComponents<AudioSource>()[1].hideFlags = HideFlags.HideInInspector;
            player.gameObject.GetComponents<AudioSource>()[1].playOnAwake = false;
        }
    }
}