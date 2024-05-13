using CarterGames.Assets.AudioManager.Logging;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    /// <summary>
    /// Handles the custom editor for the inspector music track player.
    /// </summary>
    [CustomEditor(typeof(InspectorMusicTrackPlayer))]
    public sealed class InspectorMusicTrackPlayerEditor : UnityEditor.Editor
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static MixerSearchProvider mixerSearchProvider;
        private static TrackSearchProvider trackSearchProvider;
        private static TrackListContentsSearchProvider trackListContentsSearchProvider;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            
            GUILayout.Space(5f);
            
            UtilEditor.DrawMonoScriptSection(target as InspectorMusicTrackPlayer);
            
            GUILayout.Space(5f);

            EditorGUILayout.LabelField("Playlist Info", EditorStyles.boldLabel);
            UtilEditor.DrawHorizontalGUILine();
            
            EditorGUILayout.BeginVertical("HelpBox");
            GUILayout.Space(2.5f);

            if (string.IsNullOrEmpty(serializedObject.Fp("trackListId").stringValue))
            {
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Select Playlist"))
                {
                    trackSearchProvider ??= CreateInstance<TrackSearchProvider>();
                        
                    TrackSearchProvider.ToExclude.Clear();
                
                    TrackSearchProvider.OnSearchTreeSelectionMade.Clear();
                    TrackSearchProvider.OnSearchTreeSelectionMade.Add(SelectTrack);
                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), trackSearchProvider);
                }
                
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);

                if (!string.IsNullOrEmpty(serializedObject.Fp("trackListId").stringValue))
                {
                    EditorGUILayout.TextField("Playlist Id", UtilEditor.Library
                        .MusicTrackLookup[serializedObject.Fp("trackListId").stringValue].ListKey);
                }
                else
                {
                    EditorGUILayout.TextField(string.Empty);
                }

                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(Application.isPlaying);

                if (GUILayout.Button("Change List", GUILayout.Width(100)))
                {
                    trackSearchProvider ??= CreateInstance<TrackSearchProvider>();

                    TrackSearchProvider.ToExclude.Clear();
                    TrackSearchProvider.ToExclude.Add(serializedObject.Fp("trackListId").stringValue);

                    TrackSearchProvider.OnSearchTreeSelectionMade.Clear();
                    TrackSearchProvider.OnSearchTreeSelectionMade.Add(SelectTrack);
                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                        trackSearchProvider);
                }

                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                if (UtilEditor.Library.MusicTrackLookup.ContainsKey(serializedObject.Fp("trackListId").stringValue))
                {
                    if (UtilEditor.Library.MusicTrackLookup[serializedObject.Fp("trackListId").stringValue]
                            .GetTracksRaw().Count > 1)
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.PropertyField(serializedObject.Fp("firstTrackId"));
                        EditorGUI.EndDisabledGroup();
                        
                        
                        EditorGUI.BeginDisabledGroup(Application.isPlaying);
                        
                        if (GUILayout.Button("Change Track", GUILayout.Width(100)))
                        {
                            trackListContentsSearchProvider ??= CreateInstance<TrackListContentsSearchProvider>();

                            TrackListContentsSearchProvider.ToExclude.Clear();
                            TrackListContentsSearchProvider.TrackListId =
                                serializedObject.Fp("trackListId").stringValue;
                            TrackListContentsSearchProvider.ToExclude.Add(serializedObject.Fp("firstTrackId")
                                .stringValue);

                            TrackListContentsSearchProvider.OnSearchTreeSelectionMade.Clear();
                            TrackListContentsSearchProvider.OnSearchTreeSelectionMade.Add(SelectTrackListClip);
                            SearchWindow.Open(
                                new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                                trackListContentsSearchProvider);
                        }
                    }
                }
                
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
            }
            

            EditorGUILayout.PropertyField(serializedObject.Fp("playInstantly"));
            EditorGUILayout.PropertyField(serializedObject.Fp("volume"));


            if (string.IsNullOrEmpty(serializedObject.Fp("mixerGroupId").stringValue))
            {
                EditorGUI.BeginDisabledGroup(Application.isPlaying);
                
                if (GUILayout.Button("Select Mixer"))
                {
                    mixerSearchProvider ??= CreateInstance<MixerSearchProvider>();
                        
                    MixerSearchProvider.ToExclude.Clear();
                
                    MixerSearchProvider.OnSearchTreeSelectionMade.Clear();
                    MixerSearchProvider.OnSearchTreeSelectionMade.Add(SelectMixer);
                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), mixerSearchProvider);
                }
                
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
            
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(serializedObject.Fp("mixerGroupRef"));
                EditorGUI.EndDisabledGroup();
            
                EditorGUI.BeginDisabledGroup(Application.isPlaying);
                
                if (GUILayout.Button("Change Mixer", GUILayout.Width(100)))
                {
                    mixerSearchProvider ??= CreateInstance<MixerSearchProvider>();
                        
                    MixerSearchProvider.ToExclude.Clear();
                    MixerSearchProvider.ToExclude.Add(serializedObject.Fp("mixerGroupId").stringValue);
                
                    MixerSearchProvider.OnSearchTreeSelectionMade.Clear();
                    MixerSearchProvider.OnSearchTreeSelectionMade.Add(SelectMixer);
                    SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), mixerSearchProvider);
                }
                
                EditorGUI.EndDisabledGroup();
            
                EditorGUILayout.EndHorizontal();
            }
            
            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();

            GUILayout.Space(5f);
            
            EditorGUILayout.LabelField("In Transition", EditorStyles.boldLabel);
            UtilEditor.DrawHorizontalGUILine();

            EditorGUILayout.BeginVertical("HelpBox");
            
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            
            EditorGUILayout.PropertyField(serializedObject.Fp("inMusicTransition"));
            EditorGUILayout.PropertyField(serializedObject.Fp("inTransitionDuration"));
            
            EditorGUI.EndDisabledGroup();
            
            GUILayout.Space(2.5f);
            EditorGUILayout.EndVertical();
            
            if (EditorGUI.EndChangeCheck())
            {
                if (serializedObject.Fp("inTransitionDuration").floatValue < 0f)
                {
                    AmDebugLogger.Warning($"{AudioManagerErrorCode.InvalidMusicInspectorInput}\nTransition in cannot be a value below 0.");
                    serializedObject.Fp("inTransitionDuration").floatValue = 0f;
                }
                
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
            
            GUILayout.Space(15f);
            
            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
            UtilEditor.DrawHorizontalGUILine();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel++;
            serializedObject.Fp("showEvents").boolValue =
                EditorGUILayout.Foldout(serializedObject.Fp("showEvents").boolValue, "Show Events");
            EditorGUI.indentLevel--;
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
            
            if (serializedObject.Fp("showEvents").boolValue)
            {
                EditorGUILayout.PropertyField(serializedObject.Fp("onStarted"));
                EditorGUILayout.PropertyField(serializedObject.Fp("onChanged"));
                EditorGUILayout.PropertyField(serializedObject.Fp("onLooped"));
                EditorGUILayout.PropertyField(serializedObject.Fp("onCompleted"));
            }
            
            EditorGUI.EndDisabledGroup();
            
            if (!PerUserSettings.DeveloperDebugMode) return;
            
            EditorGUILayout.LabelField("DEVELOPER DEBUG", EditorStyles.boldLabel);
            UtilEditor.DrawHorizontalGUILine();
            
            base.OnInspectorGUI();
        }

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Sets the mixer when called based on the search provider.
        /// </summary>
        /// <param name="treeEntry">The entry selected.</param>
        private void SelectMixer(SearchTreeEntry treeEntry)
        {
            MixerSearchProvider.OnSearchTreeSelectionMade.Remove(SelectMixer);
            
            if (serializedObject.Fp("mixerGroupId").stringValue == treeEntry.userData.ToString()) return;
            serializedObject.Fp("mixerGroupId").stringValue = treeEntry.userData.ToString();
            serializedObject.Fp("mixerGroupRef").objectReferenceValue =
                UtilEditor.Library.GetMixer(serializedObject.Fp("mixerGroupId").stringValue);
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }


        /// <summary>
        /// Sets the track when called based on the search provider.
        /// </summary>
        /// <param name="treeEntry">The entry selected.</param>
        private void SelectTrack(SearchTreeEntry treeEntry)
        {
            TrackSearchProvider.OnSearchTreeSelectionMade.Remove(SelectTrack);
            
            if (serializedObject.Fp("trackListId").stringValue == treeEntry.userData.ToString()) return;
            serializedObject.Fp("trackListId").stringValue = treeEntry.userData.ToString();
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
        
        
        /// <summary>
        /// Sets the mixer when called based on the search provider.
        /// </summary>
        /// <param name="treeEntry">The entry selected.</param>
        private void SelectTrackListClip(SearchTreeEntry treeEntry)
        {
            TrackListContentsSearchProvider.OnSearchTreeSelectionMade.Remove(SelectTrackListClip);
            
            if (serializedObject.Fp("firstTrackId").stringValue == treeEntry.userData.ToString()) return;
            serializedObject.Fp("firstTrackId").stringValue = treeEntry.userData.ToString();
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
    }
}