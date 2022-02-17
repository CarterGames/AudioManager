using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;

/*
 * 
 *  Audio Manager
 *							  
 *	Music Player Editor
 *      Handles the custom inspector for the music player script....
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
    [CustomEditor(typeof(MusicPlayer)), CanEditMultipleObjects]
    public class MusicPlayerEditor : UnityEditor.Editor
    {
        private readonly Color32 greenCol = new Color32(41, 176, 97, 255);
        private readonly Color32 redCol = new Color32(190, 42, 42, 255);
        private readonly Color32 amRedCol = new Color32(255, 150, 157, 255);
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
        
        
        /// <summary>
        /// Assigns the script and does any setup needed.
        /// </summary>
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
        
        
        /// <summary>
        /// Overrides the default inspector of the Music Player Script with this custom one.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!player)
                player = (MusicPlayer)target;

            AudioManagerEditorHelper.Header("Music Player", true, normalBgCol);
            AudioSourceSetup();


            EditorGUILayout.BeginVertical("Box");
            GUILayout.Space(5f);

            GUI.contentColor = amRedCol;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Track Info", EditorStyles.boldLabel, GUILayout.MaxWidth(92f));
            EditorGUILayout.EndHorizontal();
            GUI.contentColor = Color.white;

            GUILayout.Space(5f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Track To Play:", GUILayout.MaxWidth(140f));
            EditorGUILayout.PropertyField(musicTrack, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Music Audio Mixer:", GUILayout.MaxWidth(140f));
            EditorGUILayout.PropertyField(mixer, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical("Box");
            
            GUILayout.Space(5f);

            GUI.contentColor = amRedCol;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("First Play Setup", EditorStyles.boldLabel, GUILayout.MaxWidth(125f));
            EditorGUILayout.EndHorizontal();
            GUI.contentColor = Color.white;
            
            GUILayout.Space(5f);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Play On Awake:", GUILayout.MaxWidth(140f));
            EditorGUILayout.PropertyField(playOnAwake, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Intro Transition:", GUILayout.MaxWidth(140f));
            EditorGUILayout.PropertyField(musicIntroTransition, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Transition Length:", GUILayout.MaxWidth(140f));
            EditorGUILayout.PropertyField(transitionLength, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Track Volume:", GUILayout.MaxWidth(140f));
            volume.floatValue = EditorGUILayout.Slider(volume.floatValue, 0f, 1f);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Track Pitch:", GUILayout.MaxWidth(140f));
            EditorGUILayout.PropertyField(pitch, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Should Loop Track:", GUILayout.MaxWidth(140f));
            EditorGUILayout.PropertyField(shouldLoop, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Start Track At:", GUILayout.MaxWidth(140f));
            EditorGUILayout.PropertyField(timeToStartFrom, GUIContent.none);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Loop Track At:", GUILayout.MaxWidth(140f));
            EditorGUILayout.PropertyField(timeToLoopAt, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(7.5f);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (!showSource.boolValue)
            {
                GUI.backgroundColor = greenCol;
                if (GUILayout.Button("Show Audio Sources", GUILayout.Width(140f)))
                {
                    player.GetComponents<AudioSource>()[0].hideFlags = HideFlags.None;
                    player.GetComponents<AudioSource>()[1].hideFlags = HideFlags.None;
                    showSource.boolValue = true;
                }
                GUI.backgroundColor = Color.white;
            }
            else
            {
                GUI.backgroundColor = redCol;
                if (GUILayout.Button("Hide Audio Sources", GUILayout.Width(140f)))
                {
                    player.GetComponents<AudioSource>()[0].hideFlags = HideFlags.HideInInspector;
                    player.GetComponents<AudioSource>()[1].hideFlags = HideFlags.HideInInspector;
                    showSource.boolValue = false;
                }
                GUI.backgroundColor = Color.white;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);
            EditorGUILayout.EndVertical();


            serializedObject.ApplyModifiedProperties();
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

            // Label that shows the name of the script / tool & the Version number for user reference sake.
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Music Player", EditorStyles.boldLabel, GUILayout.Width(TextWidth("Music Player   ")));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Version: 2.5.4", GUILayout.Width(TextWidth("Version 2.5.4  ")));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2.5f);

            // Links to the docs and discord server for the user to access quickly if needed.
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Docs", GUILayout.Width(45f)))
            {
                Application.OpenURL("https://carter.games/audiomanager");
            }
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("Discord", GUILayout.Width(65f)))
            {
                Application.OpenURL("https://carter.games/discord");
            }
            GUI.backgroundColor = redCol;
            if (GUILayout.Button("Report Issue", GUILayout.Width(100f)))
            {
                Application.OpenURL("https://carter.games/report");
            }
            GUI.backgroundColor = Color.white;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(10f);
        }


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
        
        
        
        // JTools Bit
        private float TextWidth(string text)
        {
            return GUI.skin.label.CalcSize(new GUIContent(text)).x;
        }
    }
}