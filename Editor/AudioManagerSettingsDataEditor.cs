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
 *  Version: 2.5.2
 *	Last Updated: 07/08/2021 (d/m/y)							
 * 
 */

namespace CarterGames.Assets.AudioManager.Editor
{
    [CustomEditor(typeof(AudioManagerSettingsData))]
    public class AudioManagerSettingsDataEditor : UnityEditor.Editor
    {
        private readonly Color32 amRedCol = new Color32(255, 150, 157, 255);
        private Color defautlCol;
        
        private SerializedProperty scanDirectory;


        private void OnEnable()
        {
            scanDirectory = serializedObject.FindProperty("baseAudioScanPath");
            defautlCol = GUI.contentColor;
        }


        public override void OnInspectorGUI()
        {
            GUILayout.Space(5f);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Shows either the Carter Games Logo or an alternative for if the icon is deleted/not included when you import the package
            // Note: if you are using an older version of the asset, the directory/name of the logo may not match this and therefore will display the text title only
            if (Resources.Load<Texture2D>("LogoAM"))
            {
                if (GUILayout.Button(Resources.Load<Texture2D>("LogoAM"), GUIStyle.none, GUILayout.Width(50), GUILayout.Height(50)))
                {
                    GUI.FocusControl(null);
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);
            
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Global Settings Data", EditorStyles.boldLabel, GUILayout.Width(AudioManagerEditor.TextWidth("Global Settings Data   ")));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            
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