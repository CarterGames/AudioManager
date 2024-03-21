/*
 * Copyright (c) 2024 Carter Games
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
 * FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using CarterGames.Common;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// Handles accessing stuff in the do not destroy scene for the asset.
    /// </summary>
    public static class DoNotDestroyHandler
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private static Transform cacheBaseParent;
        private static Ref cacheRef;
        private static MusicRoutineHandler cacheMusicRoutineHandler;
        private static MusicSourceHandler cacheMusicSourceHandler;
        private static Transform cacheAudioParent;
        private static Transform cacheMusicParent;

        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Properties
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Gets if the handler is initialized.
        /// </summary>
        private static bool IsInitialized { get; set; }
        
        
        /// <summary>
        /// The base parent of the do not destroy setup.
        /// </summary>
        public static Transform BaseParent
        {
            get
            {
                if (cacheBaseParent != null) return cacheBaseParent;
                SetupParent();
                return cacheBaseParent;
            }
        }
        
        
        /// <summary>
        /// The ref class to use in the scene.
        /// </summary>
        public static Ref RefClass
        {
            get
            {
                if (cacheRef != null) return cacheRef;
                SetupParent();
                cacheRef = BaseParent.GetComponent<Ref>();
                return cacheRef;
            }
        }
        
        
        /// <summary>
        /// The ref class to use in the scene.
        /// </summary>
        public static MusicRoutineHandler MusicRoutineHandler
        {
            get
            {
                if (cacheMusicRoutineHandler != null) return cacheMusicRoutineHandler;
                SetupParent();
                cacheMusicRoutineHandler = MusicParent.GetComponentInChildren<MusicRoutineHandler>(true);
                return cacheMusicRoutineHandler;
            }
        }
        
        
        /// <summary>
        /// The ref class to use in the scene.
        /// </summary>
        public static MusicSourceHandler MusicSourceHandler
        {
            get
            {
                if (cacheMusicSourceHandler != null) return cacheMusicSourceHandler;
                SetupParent();
                cacheMusicSourceHandler = MusicParent.GetComponentInChildren<MusicSourceHandler>(true);
                return cacheMusicSourceHandler;
            }
        }
        
        
        /// <summary>
        /// The audio parent for pooling objects.
        /// </summary>
        public static Transform AudioParent
        {
            get
            {
                if (cacheAudioParent != null) return cacheAudioParent;
                SetupParent();
                return cacheAudioParent;
            }
        }
        
        
        /// <summary>
        /// The music parent for pooling objects.
        /// </summary>
        public static Transform MusicParent
        {
            get
            {
                if (cacheMusicParent != null) return cacheMusicParent;
                SetupParent();
                return cacheMusicParent;
            }
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Initializes the class when called.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (cacheBaseParent != null) return;
            SetupParent();
        }


        /// <summary>
        /// Sets up the parents when called.
        /// </summary>
        private static void SetupParent()
        {
            if (cacheBaseParent != null) return;
            
            var obj = new GameObject("Audio Manager");
            cacheBaseParent = obj.transform;
            cacheBaseParent.gameObject.AddComponent<Ref>();
            
            var audioPoolParent = new GameObject("Audio Clip | Object Pool");
            var musicPoolParent = new GameObject("Music Components");
                
            audioPoolParent.transform.SetParent(cacheBaseParent);
            musicPoolParent.transform.SetParent(cacheBaseParent);
            
            cacheAudioParent = audioPoolParent.transform;
            cacheMusicParent = musicPoolParent.transform;

            var routineParent = new GameObject("Music Routines");
            routineParent.transform.SetParent(cacheMusicParent);
            
            var sourcesParent = new GameObject("Music Sources");
            sourcesParent.transform.SetParent(cacheMusicParent);
            
            routineParent.gameObject.AddComponent<MusicRoutineHandler>();
            sourcesParent.gameObject.AddComponent<MusicSourceHandler>();
            
            MusicSourceHandler.Initialize();
                
            Object.DontDestroyOnLoad(cacheBaseParent.gameObject);
        }
    }
}