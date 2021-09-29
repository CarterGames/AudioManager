using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;

/*
 * 
 *  Audio Manager
 *							  
 *	Audio Player Editor
 *      The editor script for the Audio Player, handles the custom inspector for the player.
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
    [CustomEditor(typeof(AudioPlayer)), CanEditMultipleObjects]
    public class AudioPlayerEditor : UnityEditor.Editor
    {
        private readonly Color32 greenCol = new Color32(41, 176, 97, 255);
        private readonly Color32 redCol = new Color32(190, 42, 42, 255);
        private readonly Color32 amRedCol = new Color32(255, 150, 157, 255);
        
        private AudioPlayer player;

        private SerializedProperty file;
        private SerializedProperty mixer;
        private SerializedProperty clipStrings;
        private SerializedProperty clipVolume;
        private SerializedProperty clipPitch;
        private SerializedProperty clipTimes;
        private SerializedProperty clipDelays;
        private SerializedProperty dropDownBools;
        private SerializedProperty dropDownOptionals;
        
        
        
        private void OnEnable()
        {
            player = (AudioPlayer)target;
            
            file = serializedObject.FindProperty("audioManagerFile");
            mixer = serializedObject.FindProperty("mixer");
            clipStrings = serializedObject.FindProperty("clipsToPlay");
            clipVolume = serializedObject.FindProperty("clipsVolume");
            clipPitch = serializedObject.FindProperty("clipsPitch");
            clipTimes = serializedObject.FindProperty("clipsFromTime");
            clipDelays = serializedObject.FindProperty("clipsWithDelay");
            dropDownBools = serializedObject.FindProperty("dropDowns");
            dropDownOptionals = serializedObject.FindProperty("dropDownsOptional");
        }

   
        
        public override void OnInspectorGUI()
        {
            HeaderDisplay();
            
            AudioSourceSetup();

            EditorGUILayout.BeginVertical("Box");
            GUILayout.Space(5f);

            GUI.contentColor = amRedCol;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Audio Info", EditorStyles.boldLabel, GUILayout.MaxWidth(92f));
            EditorGUILayout.EndHorizontal();
            GUI.contentColor = Color.white;

            GUILayout.Space(5f);

            // Audio Manager File (AMF) field
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Audio Manager File:", GUILayout.MaxWidth(140f));
            file.objectReferenceValue = (AudioManagerFile) EditorGUILayout.ObjectField(file.objectReferenceValue, typeof(AudioManagerFile), false);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Audio Mixer:", GUILayout.MaxWidth(140f));
            mixer.objectReferenceValue = (AudioMixerGroup) EditorGUILayout.ObjectField(mixer.objectReferenceValue, typeof(AudioMixerGroup), false);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);

            EditorGUILayout.EndVertical();

            if (file.objectReferenceValue != null)
            {

                GUILayout.Space(5f);

                EditorGUILayout.BeginVertical("Box");
                GUILayout.Space(5f);
                GUI.contentColor = amRedCol;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Clips", EditorStyles.boldLabel, GUILayout.MaxWidth(92f));
                EditorGUILayout.EndHorizontal();
                GUI.contentColor = Color.white;
                GUILayout.Space(5f);


                GUILayout.Space(5f);
                EditorGUILayout.EndVertical();


                if (clipStrings.arraySize > 0)
                {
                    for (int i = 0; i < clipStrings.arraySize; i++)
                    {
                        if (clipStrings.GetArrayElementAtIndex(i).stringValue != "")
                            dropDownBools.GetArrayElementAtIndex(i).boolValue =
                                EditorGUILayout.Foldout(
                                    dropDownBools.GetArrayElementAtIndex(i).boolValue,
                                    clipStrings.GetArrayElementAtIndex(i).stringValue);
                        else
                            dropDownBools.GetArrayElementAtIndex(i).boolValue =
                                EditorGUILayout.Foldout(
                                    dropDownBools.GetArrayElementAtIndex(i).boolValue, "New Clip...");

                        if (dropDownBools.GetArrayElementAtIndex(i).boolValue)
                        {
                            EditorGUILayout.BeginVertical("Box");

                            clipStrings.GetArrayElementAtIndex(i).stringValue =
                                EditorGUILayout.TextField("Clip Name:",
                                    clipStrings.GetArrayElementAtIndex(i).stringValue);


                            clipVolume.GetArrayElementAtIndex(i).floatValue =
                                EditorGUILayout.Slider("Volume:", clipVolume.GetArrayElementAtIndex(i).floatValue, 0f,
                                    1f);

                            clipPitch.GetArrayElementAtIndex(i).floatValue =
                                EditorGUILayout.FloatField("Pitch:", clipPitch.GetArrayElementAtIndex(i).floatValue);

                            EditorGUILayout.Space();


                            dropDownOptionals.GetArrayElementAtIndex(i).boolValue = EditorGUILayout.Foldout(
                                dropDownOptionals.GetArrayElementAtIndex(i).boolValue, "Optional",
                                EditorStyles.boldFont);

                            if (dropDownOptionals.GetArrayElementAtIndex(i).boolValue)
                            {
                                clipTimes.GetArrayElementAtIndex(i).floatValue =
                                    EditorGUILayout.FloatField("Play From Time:",
                                        clipTimes.GetArrayElementAtIndex(i).floatValue);

                                clipDelays.GetArrayElementAtIndex(i).floatValue =
                                    EditorGUILayout.FloatField("Play With Delay:",
                                        clipDelays.GetArrayElementAtIndex(i).floatValue);
                            }


                            EditorGUILayout.Space();

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();

                            GUI.backgroundColor = greenCol;
                            if (GUILayout.Button("+", GUILayout.Width(30f)))
                            {
                                AddNewElement(clipStrings.arraySize);
                            }

                            GUI.backgroundColor = Color.white;

                            if (!i.Equals(0))
                            {
                                GUI.backgroundColor = redCol;
                                if (GUILayout.Button("-", GUILayout.Width(30f)))
                                {
                                    RemoveElement(i);
                                }

                                GUI.backgroundColor = Color.white;
                            }

                            GUILayout.FlexibleSpace();
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.EndVertical();
                        }
                    }
                }
                else
                {
                    clipStrings.InsertArrayElementAtIndex(0);
                    clipVolume.InsertArrayElementAtIndex(0);
                    clipPitch.InsertArrayElementAtIndex(0);
                    clipTimes.InsertArrayElementAtIndex(0);
                    clipDelays.InsertArrayElementAtIndex(0);
                    dropDownBools.InsertArrayElementAtIndex(0);
                    dropDownOptionals.InsertArrayElementAtIndex(0);

                    clipVolume.GetArrayElementAtIndex(0).floatValue = 1f;
                    clipPitch.GetArrayElementAtIndex(0).floatValue = 1f;
                    dropDownBools.GetArrayElementAtIndex(0).boolValue = true;
                }
            }



            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
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
            EditorGUILayout.LabelField("Audio Player", EditorStyles.boldLabel, GUILayout.Width(TextWidth("Audio Player   ")));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Version: 2.5.2", GUILayout.Width(TextWidth("Version 2.5.2  ")));
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

        
        
        
        /// <summary>
        /// Runs the setup for the audio source....
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
        /// Adds an element to the arrays for a new clip to be played...
        /// </summary>
        /// <param name="i">Int | The element for the </param>
        private void AddNewElement(int i)
        {
            clipStrings.InsertArrayElementAtIndex(i);
            clipVolume.InsertArrayElementAtIndex(i);
            clipPitch.InsertArrayElementAtIndex(i);
            clipTimes.InsertArrayElementAtIndex(i);
            clipDelays.InsertArrayElementAtIndex(i);
            dropDownBools.InsertArrayElementAtIndex(i);
            dropDownOptionals.InsertArrayElementAtIndex(i);

            clipStrings.GetArrayElementAtIndex(i).stringValue = string.Empty;
            clipVolume.GetArrayElementAtIndex(i).floatValue = 1f;
            clipPitch.GetArrayElementAtIndex(i).floatValue = 1f;
            dropDownBools.GetArrayElementAtIndex(i).boolValue = true;
        }



        /// <summary>
        /// Removes an element from the arrays, deleting a clip from the list to be played...
        /// </summary>
        /// <param name="i">Int | The element to remove...</param>
        private void RemoveElement(int i)
        {
            clipStrings.DeleteArrayElementAtIndex(i);
            clipVolume.DeleteArrayElementAtIndex(i);
            clipPitch.DeleteArrayElementAtIndex(i);
            clipTimes.DeleteArrayElementAtIndex(i);
            clipDelays.DeleteArrayElementAtIndex(i);
            dropDownBools.DeleteArrayElementAtIndex(i);
            dropDownOptionals.DeleteArrayElementAtIndex(i);
        }
        
        
        // JTools Bit
        private float TextWidth(string text)
        {
            return GUI.skin.label.CalcSize(new GUIContent(text)).x;
        }
    }
}