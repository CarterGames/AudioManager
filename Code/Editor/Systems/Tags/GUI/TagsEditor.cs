/*
 * Audio Manager (3.x)
 * Copyright (c) Carter Games
 *
 * This program is free software: you can redistribute it and/or modify it under the terms of the
 * GNU General Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version. 
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
 *
 * You should have received a copy of the GNU General Public License along with this program.
 * If not, see <https://www.gnu.org/licenses/>. 
 */

using CarterGames.Shared.AudioManager.Editor;
using UnityEditor;
using UnityEngine;

namespace CarterGames.Assets.AudioManager.Editor
{
    public class TagsEditor : EditorWindow
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static Vector2 ScrollPos
        {
            get => (Vector2) PerUserSettingsEditor.GetOrCreateValue<Vector2>("tags_editor", PerUserSettingType.PlayerPref, Vector2.zero);
            set => PerUserSettingsEditor.SetValue<Vector2>("tags_editor", PerUserSettingType.PlayerPref, value);
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Menu Items
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        public static void ShowWindow()
        {
            var window = GetWindow<TagsEditor>();

            window.titleContent = new GUIContent("Audio Library (Tags)",
                EditorGUIUtility.IconContent("d_FilterByLabel@2x").image);

            window.minSize = new Vector2(500f, 300f);
            window.maxSize = new Vector2(950f, 750f);
            window.ShowPopup();
        }


        private void OnGUI()
        {
            EditorGUILayout.BeginVertical("HelpBox");
            EditorGUILayout.Space(1.5f);
            
            EditorGUILayout.LabelField("Tags", EditorStyles.boldLabel);
            UtilEditor.DrawHorizontalGUILine();
            
            EditorGUILayout.HelpBox("Create & manage tags for any clips in the library here.", MessageType.Info);
            
            UtilEditor.DrawHorizontalGUILine();

            ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos);
            
            DrawListView(ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef
                .Fp("tags"));
            
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.Space(1.5f);
            EditorGUILayout.EndVertical();
        }


        private void DrawListView(SerializedProperty arrayProp)
        {
            EditorGUI.BeginChangeCheck();
            
            for (var i = 0; i < arrayProp.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(arrayProp.GetIndex(i));

                if (GUILayout.Button("-", GUILayout.Width(25)))
                {
                    arrayProp.DeleteIndex(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            
            if (GUILayout.Button("+"))
            {
                arrayProp.InsertIndex(arrayProp.arraySize);
                arrayProp.GetIndex(arrayProp.arraySize - 1).stringValue = string.Empty;
            }

            if (EditorGUI.EndChangeCheck())
            {
                ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.ApplyModifiedProperties();
                ScriptableRef.GetAssetDef<AudioLibrary>().ObjectRef.Update();
            }
        }
    }
}