/*
 * Copyright (c) 2025 Carter Games
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