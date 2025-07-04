﻿/*
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

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace CarterGames.Shared.AudioManager.Editor
{
    /// <summary>
    /// Handles the setup of the asset index for runtime references to scriptable objects used for the asset.
    /// </summary>
    public sealed class AssetIndexHandler : IPreprocessBuildWithReport, IAssetEditorInitialize
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        private static readonly string AssetFilter = $"t:{nameof(AmDataAsset)}";

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   IAssetEditorInitialize Implementation
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */

        /// <summary>
        /// Defines the order that this initializer run at.
        /// </summary>
        public int InitializeOrder => 1;

        
        /// <summary>
        /// Runs when the asset initialize flow is used.
        /// </summary>
        public void OnEditorInitialized()
        {
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
            
            UpdateIndex();
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   IPreprocessBuildWithReport Implementation
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// The order this script is processed in, in this case its the default.
        /// </summary>
        public int callbackOrder => 0;
        
        
        /// <summary>
        /// Runs before a build is executed.
        /// </summary>
        /// <param name="report">The report about the build (I don't need it, but its a param for the method).</param>
        public void OnPreprocessBuild(BuildReport report)
        {
            UpdateIndex();
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Initializes the event subscription needed for this to work in editor.
        /// </summary>
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
        }


        /// <summary>
        /// Runs when the editor has updated.
        /// </summary>
        private static void OnEditorUpdate()
        {
            // If the user is about to enter play-mode, update the index, otherwise leave it be. 
            if (!EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying) return;
            UpdateIndex();
        }


        /// <summary>
        /// Updates the index with all the save manager asset scriptable objects in the project.
        /// </summary>
        [MenuItem("Tools/Carter Games/Audio Manager/Update Asset Index", priority = 17)]
        public static void UpdateIndex()
        {
            var foundAssets = new List<AmDataAsset>();
            var asset = AssetDatabase.FindAssets(AssetFilter, null);
            
            if (asset == null || asset.Length <= 0) return;

            foreach (var assetInstance in asset)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetInstance);
                var assetObj = (AmDataAsset) AssetDatabase.LoadAssetAtPath(assetPath, typeof(AmDataAsset));
                
                // Doesn't include editor only or the index itself.
                if (assetObj == null) continue;
                if (assetObj is AmDataAssetIndex) continue;
                if (assetObj is AmEditorOnlyDataAsset) continue;
                foundAssets.Add((AmDataAsset) AssetDatabase.LoadAssetAtPath(assetPath, typeof(AmDataAsset)));
            }

            var indexProp = ScriptableRef.GetAssetDef<AmDataAssetIndex>().ObjectRef;
            
            RemoveNullReferences(indexProp);
            UpdateIndexReferences(foundAssets ,indexProp);
            
            indexProp.ApplyModifiedProperties();
            indexProp.Update();
        }


        private static void RemoveNullReferences(SerializedObject indexProp)
        {
            for (var i = 0; i < indexProp.Fp("assets").Fpr("list").arraySize; i++)
            {
                var entry = indexProp.Fp("assets").Fpr("list").GetIndex(i);
                var jIndexAdjustment = 0;

                for (var j = 0; j < entry.Fpr("value").arraySize; j++)
                {
                    if (entry.Fpr("value").GetIndex(j - jIndexAdjustment).objectReferenceValue != null) continue;
                    entry.Fpr("value").DeleteIndex(j);
                    jIndexAdjustment++;
                }
            }
        }


        private static void UpdateIndexReferences(IReadOnlyList<AmDataAsset> foundAssets, SerializedObject indexProp)
        {
            indexProp.Fp("assets").Fpr("list").ClearArray();
            
            for (var i = 0; i < foundAssets.Count; i++)
            {
                for (var j = 0; j < indexProp.Fp("assets").Fpr("list").arraySize; j++)
                {
                    var entry = indexProp.Fp("assets").Fpr("list").GetIndex(j);
                    
                    if (entry.Fpr("key").stringValue.Equals(foundAssets[i].GetType().ToString()))
                    {
                        for (var k = 0; k < entry.Fpr("value").arraySize; k++)
                        {
                            if (entry.Fpr("value").GetIndex(k).objectReferenceValue == foundAssets[i]) goto AlreadyExists;
                        }
                        
                        entry.Fpr("value").InsertIndex(entry.Fpr("value").arraySize);
                        entry.Fpr("value").GetIndex(entry.Fpr("value").arraySize - 1).objectReferenceValue = foundAssets[i];
                        goto AlreadyExists;
                    }
                }
                
                indexProp.Fp("assets").Fpr("list").InsertIndex(indexProp.Fp("assets").Fpr("list").arraySize);
                indexProp.Fp("assets").Fpr("list").GetIndex(indexProp.Fp("assets").Fpr("list").arraySize - 1).Fpr("key").stringValue = foundAssets[i].GetType().ToString();
                
                if (indexProp.Fp("assets").Fpr("list").GetIndex(indexProp.Fp("assets").Fpr("list").arraySize - 1).Fpr("value").arraySize > 0)
                {
                    indexProp.Fp("assets").Fpr("list").GetIndex(indexProp.Fp("assets").Fpr("list").arraySize - 1)
                        .Fpr("value").ClearArray();
                }
                
                indexProp.Fp("assets").Fpr("list").GetIndex(indexProp.Fp("assets").Fpr("list").arraySize - 1).Fpr("value").InsertIndex(0);
                indexProp.Fp("assets").Fpr("list").GetIndex(indexProp.Fp("assets").Fpr("list").arraySize - 1)
                    .Fpr("value").GetIndex(0).objectReferenceValue = foundAssets[i];

                AlreadyExists: ;
            } 
        }
    }
}