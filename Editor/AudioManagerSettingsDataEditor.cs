using UnityEditor;
using UnityEngine;

/*
 * 
 *  Audio Manager
 *							  
 *	Audio Manager Settings Data Editor
 *      The editor script for the audio settings, just to make it look more appealing.
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
    [CustomEditor(typeof(AudioManagerSettingsData))]
    public class AudioManagerSettingsDataEditor : UnityEditor.Editor
    {
        private readonly Color32 amRedCol = new Color32(255, 150, 157, 255);
        private Color defautlCol;
        private Color normalBgCol;
        
        private SerializedProperty scanDirectory;


        private void OnEnable()
        {
            scanDirectory = serializedObject.FindProperty("baseAudioScanPath");
            defautlCol = GUI.contentColor;
            normalBgCol = GUI.backgroundColor;
        }


        public override void OnInspectorGUI()
        {
            GUILayout.Space(5f);
            
            AudioManagerEditorHelper.Header("Global Settings Data", false, normalBgCol);
            
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Open Editor", GUILayout.Width(85f)))
            {
                AudioManagerSettings.OnShowWindow();
            }
            GUI.backgroundColor = defautlCol;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            
            GUI.contentColor = amRedCol;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Global Settings", EditorStyles.boldLabel, GUILayout.MaxWidth(120f));
            EditorGUILayout.EndHorizontal();
            GUI.contentColor = defautlCol;

            EditorGUILayout.PropertyField(scanDirectory);

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
    }
}