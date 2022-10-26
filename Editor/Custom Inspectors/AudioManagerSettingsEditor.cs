using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// The custom inspector for the audio manager settings asset.
    /// </summary>
    [CustomEditor(typeof(AudioManagerSettings))]
    public class AudioManagerSettingsEditor : UnityEditor.Editor
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private SerializedProperty baseAudioScanPath;
        private SerializedProperty isUsingStatic;
        private SerializedProperty showDebugMessages;
        private Color defaultBackgroundColor;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private void OnEnable()
        {
            baseAudioScanPath = serializedObject.FindProperty("baseAudioScanPath");
            isUsingStatic = serializedObject.FindProperty("isUsingStatic");
            showDebugMessages = serializedObject.FindProperty("showDebugMessages");

            defaultBackgroundColor = GUI.backgroundColor;
        }
        

        public override void OnInspectorGUI()
        {
            AudioManagerEditorUtil.SettingsHeader();
            DrawScriptSection();
            DrawOptions();
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Draws the script section of the custom inspector.
        /// </summary>
        private void DrawScriptSection()
        {
            GUILayout.Space(4.5f);
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(1.5f);
            
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromScriptableObject(target as AudioManagerSettings), typeof(AudioManagerSettings), false);
            GUI.enabled = true;

            DrawButton();
            
            GUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        /// Draws the options for the custom inspector.
        /// </summary>
        private void DrawOptions()
        {
            GUILayout.Space(4.5f);
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(1.5f);
            
            GUI.enabled = false;
            EditorGUILayout.PropertyField(baseAudioScanPath);
            EditorGUILayout.PropertyField(isUsingStatic);
            EditorGUILayout.PropertyField(showDebugMessages);
            GUI.enabled = true;
            
            GUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
        }
        
        
        /// <summary>
        /// Draws the edit settings button in the custom inspector.
        /// </summary>
        private void DrawButton()
        {
            GUILayout.Space(2f);

            GUI.backgroundColor = AudioManagerEditorUtil.Green;
            if (GUILayout.Button("Edit Settings", GUILayout.Height(25)))
            {
                SettingsService.OpenProjectSettings("Project/Carter Games/Audio Manager");
            }
            GUI.backgroundColor = defaultBackgroundColor;
        }
    }
}