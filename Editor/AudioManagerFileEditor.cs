using UnityEditor;
using UnityEngine;

/*
 * 
 *  Audio Manager
 *							  
 *	Audio Manager File Editor
 *      The editor script for the Audio Manager Files, handles the custom inspector for any Audio Manager File.
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
    [CustomEditor(typeof(AudioManagerFile))]
    public class AudioManagerFileEditor : UnityEditor.Editor
    {
        private readonly Color32 amRedCol = new Color32(255, 150, 157, 255);
        private readonly string[] TabTitles = new string[2] {"Settings", "Library"};
        
        private SerializedProperty audioPrefab;
        private SerializedProperty isPopulated;
        private SerializedProperty audioMixers;
        private SerializedProperty directories;
        private SerializedProperty library;
        private SerializedProperty tabPos;
        
        private Color defaultContentCol;
        private Color normalBackgroundCol;
        
        
        private void Awake()
        {
            audioPrefab = serializedObject.FindProperty("soundPrefab");
            isPopulated = serializedObject.FindProperty("isPopulated");
            audioMixers = serializedObject.FindProperty("audioMixer");
            directories = serializedObject.FindProperty("directory");
            library = serializedObject.FindProperty("library");
            tabPos = serializedObject.FindProperty("tabPos");
            defaultContentCol = GUI.contentColor;
            normalBackgroundCol = GUI.backgroundColor;
        }


        public override void OnInspectorGUI()
        {
            AudioManagerEditorHelper.Header("Audio Manager File", false, normalBackgroundCol);
            RenderTabBar();
            RenderSettings(tabPos.intValue);
            RenderLibrary(tabPos.intValue);

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }


        private void RenderTabBar()
        {
            GUILayout.Space(5f);
            tabPos.intValue = GUILayout.Toolbar(tabPos.intValue, TabTitles);
            GUILayout.Space(5f);
        }


        private void RenderSettings(int pos)
        {
            if (!pos.Equals(0)) return;
            
            GUI.contentColor = amRedCol;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("File Settings", EditorStyles.boldLabel, GUILayout.MaxWidth(120f));
            EditorGUILayout.EndHorizontal();
            GUI.contentColor = defaultContentCol;
            
            EditorGUILayout.HelpBox("The audio prefab that is spawned in when you call for audio to be played.", MessageType.None);
            EditorGUILayout.PropertyField(audioPrefab, new GUIContent("Sound Prefab:"));
            
            GUILayout.Space(15f);
            
            GUI.contentColor = amRedCol;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("File Setup", EditorStyles.boldLabel, GUILayout.MaxWidth(120f));
            EditorGUILayout.EndHorizontal();
            GUI.contentColor = defaultContentCol;

            EditorGUILayout.HelpBox("Tells the Audio Manager that this file has run the directory setup & if it has audio from the directories scanned.", MessageType.None);
            EditorGUILayout.PropertyField(isPopulated);
            
            
            GUILayout.Space(10);
            
            
            GUI.contentColor = amRedCol;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mixers", EditorStyles.boldLabel, GUILayout.MaxWidth(120f));
            EditorGUILayout.EndHorizontal();
            GUI.contentColor = defaultContentCol;

            if (audioMixers.arraySize.Equals(0))
            {
                audioMixers.InsertArrayElementAtIndex(0);
                return;
            }
            
            for (var i = 0; i < audioMixers.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"#{(i+1).ToString()}", GUILayout.Width(TextWidth($"#{(i+1).ToString()} ")));
                EditorGUILayout.PropertyField(audioMixers.GetArrayElementAtIndex(i), GUIContent.none);
                
                if (GUILayout.Button("+", GUILayout.Width(25f)))
                    audioMixers.InsertArrayElementAtIndex(i + 1);

                if (GUILayout.Button("-", GUILayout.Width(25f)))
                    audioMixers.DeleteArrayElementAtIndex(i);

                EditorGUILayout.EndHorizontal();
            }    
            
            
            GUILayout.Space(10);
            
            GUI.contentColor = amRedCol;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Directories", EditorStyles.boldLabel, GUILayout.MaxWidth(120f));
            EditorGUILayout.EndHorizontal();
            GUI.contentColor = defaultContentCol;
            
            if (directories.arraySize.Equals(0))
            {
                if (GUILayout.Button("Add First Element"))
                {
                    directories.InsertArrayElementAtIndex(0);
                }
                
                return;
            }
            
            for (var i = 0; i < directories.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"#{(i+1).ToString()}", GUILayout.Width(TextWidth($"#{(i+1).ToString()} ")));
                EditorGUILayout.PropertyField(directories.GetArrayElementAtIndex(i), GUIContent.none);
                
                if (GUILayout.Button("+", GUILayout.Width(25f)))
                    directories.InsertArrayElementAtIndex(i + 1);

                if (GUILayout.Button("-", GUILayout.Width(25f)))
                    directories.DeleteArrayElementAtIndex(i);

                EditorGUILayout.EndHorizontal();
            }
        }


        private void RenderLibrary(int pos)
        {
            if (!pos.Equals(1)) return;
            
            GUI.contentColor = amRedCol;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Controls", EditorStyles.boldLabel, GUILayout.MaxWidth(120f));
            EditorGUILayout.EndHorizontal();
            GUI.contentColor = defaultContentCol;
            
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Library"))
            {
                library.ClearArray();
            }
            EditorGUILayout.EndHorizontal();
            
            GUI.contentColor = amRedCol;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Audio Library", EditorStyles.boldLabel, GUILayout.MaxWidth(120f));
            EditorGUILayout.EndHorizontal();
            GUI.contentColor = defaultContentCol;
            
            EditorGUILayout.HelpBox("Shows all the clips that are present in the library of this file.", MessageType.None);

            GUILayout.Space(5f);


            
            if (library.arraySize.Equals(0)) return;
            
            GUI.contentColor = amRedCol;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Clip Name", EditorStyles.boldLabel, GUILayout.Width(Screen.width / 2.25f));
            EditorGUILayout.LabelField("Audio Clip File", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            GUI.contentColor = defaultContentCol;
            
            GUI.enabled = false;
            
            for (var i = 0; i < library.arraySize; i++)
            {
                var _fileName = library.GetArrayElementAtIndex(i).FindPropertyRelative("key");
                var _fileClip = library.GetArrayElementAtIndex(i).FindPropertyRelative("value");
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_fileName, GUIContent.none, GUILayout.Width(Screen.width / 2.25f));
                
                EditorGUILayout.PropertyField(_fileClip, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            }
            
            GUI.enabled = true;
        }
        
        
        private static float TextWidth(string text)
        {
            return GUI.skin.label.CalcSize(new GUIContent(text)).x;
        }
    }
}