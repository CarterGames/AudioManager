using System;
using UnityEditor;
using UnityEngine;

/*
 * 
 *  Audio Manager
 *							  
 *	Audio Manager Settings
 *      Handles any settings specific for the Audio Manager asset that are global changes...
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
    public class AudioManagerSettings : EditorWindow
    {
        // Tools for the deselecting of panels
        public Rect deselectWindow;
        private readonly Color32 amRedCol = new Color32(255, 150, 157, 255);



        [MenuItem("Tools/Audio Manager | CG/Global Settings", priority = 2)]
        public static void OnShowWindow()
        {
            GetWindow<AudioManagerSettings>("Audio Manager Settings");
        }
        


        private void OnGUI()
        {
            deselectWindow = new Rect(0, 0, position.width, position.height);
            
            HeaderDisplay();

            
            var _asset = AssetDatabase.FindAssets("t:audiomanagersettingsdata", null);

            if (_asset != null && _asset.Length > 0)
            {
                GUI.contentColor = amRedCol;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Base Audio Scan Directory", EditorStyles.boldLabel,
                    GUILayout.MaxWidth(200f));
                EditorGUILayout.EndHorizontal();
                GUI.contentColor = Color.white;
                EditorGUILayout.HelpBox(
                    "This will be from your /Assets/... directory or equivalent for the platform you are developing on. The default is YourProjectAssetsFolder/Audio which is written here as /audio.",
                    MessageType.None);



                var _path = AssetDatabase.GUIDToAssetPath(_asset[0]);
                var _loadedSettings =
                    (AudioManagerSettingsData) AssetDatabase.LoadAssetAtPath(_path, typeof(AudioManagerSettingsData));


                EditorGUI.BeginChangeCheck();
                _loadedSettings.baseAudioScanPath = EditorGUILayout.TextField(_loadedSettings.baseAudioScanPath);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(_loadedSettings);
                    AssetDatabase.SaveAssets();
                }
            }

            // defines the min/max size of the editor window.
            SetMinMaxWindowSize();

            // Makes it so you can deselect elements in the window by adding a button the size of the window that you can't see under everything
            //make sure the following code is at the very end of OnGUI Function
            if (GUI.Button(deselectWindow, "", GUIStyle.none))
            {
                GUI.FocusControl(null);
            }
        }
        
        
        
        /// <summary>
        /// Defines the min and max size for the editor window
        /// </summary>
        private void SetMinMaxWindowSize()
        {
            EditorWindow editorWindow = this;
            editorWindow.minSize = new Vector2(400f, 300f);
            editorWindow.maxSize = new Vector2(400f, 300f);
        }



        /// <summary>
        /// Shows the header info including logo, asset name and documentation/discord buttons.
        /// </summary>
        private void HeaderDisplay()
        {
            GUILayout.Space(10f);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Shows either the Carter Games Logo or an alternative for if the icon is deleted/not included when you import the package
            // Note: if you are using an older version of the asset, the directory/name of the logo may not match this and therefore will display the text title only
            if (Resources.Load<Texture2D>("Carter Games/Audio Manager/LogoAM"))
            {
                if (GUILayout.Button(Resources.Load<Texture2D>("Carter Games/Audio Manager/LogoAM"), GUIStyle.none,
                    GUILayout.Width(50), GUILayout.Height(50)))
                {
                    GUI.FocusControl(null);
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);

            // Label that shows the name of the script / tool & the Version number for user reference sake.
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Audio Manager Settings", EditorStyles.boldLabel,
                GUILayout.Width(TextWidth("Audio Manager Settings  ")));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Version: 2.5.6", GUILayout.Width(TextWidth("Version 2.5.6  ")));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }


        // JTools Bit
        private float TextWidth(string text)
        {
            return GUI.skin.label.CalcSize(new GUIContent(text)).x;
        }
    }
}