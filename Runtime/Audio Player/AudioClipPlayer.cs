/*
 * Copyright (c) 2018-Present Carter Games
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

using System.Collections;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    /// <summary>
    /// The class that plays the audio from the manager at runtime.
    /// </summary>
    [RequireComponent(typeof(AudioSource)), AddComponentMenu("Carter Games/Audio Manager/Audio Clip Player")]
    public class AudioClipPlayer : MonoBehaviour
    {
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Fields
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private AudioManager am;
        private AudioSource source;
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Unity Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        private void OnEnable()
        {
            source = GetComponentInChildren<AudioSource>(true);
            StartCoroutine(Co_FrameDelayedUpdateRoutine());
        }

        private void OnDisable()
        {
            transform.SetParent(AudioPool.Parent);
            StopAllCoroutines();
        }
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Methods
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Disables the clip entered once it has completed a play-through.
        /// </summary>
        /// <param name="time">Float | The amount of the to wait before disabling the object.</param>
        public void Cleanup(float time)
        {
            if (!am)
#if Use_CGAudioManager_Static || USE_CG_AM_STATIC
                am = AudioManager.instance;
#else
                am = FindObjectOfType<AudioManager>();
#endif
            
            StartCoroutine(Co_CleanupCo(time));
        }
        
        
        /// <summary>
        /// Runs the delayed update loop when called.
        /// </summary>
        private void DelayedUpdate()
        {
            if (source.isPlaying) return;
            RemovalLogic();
        }
        
        /// <summary>
        /// Runs the removal logic when called.
        /// </summary>
        private void RemovalLogic()
        {
            transform.SetParent(AudioPool.Parent);
            AudioPool.Return(source);
            gameObject.SetActive(false);
        }
        
        
        /* ─────────────────────────────────────────────────────────────────────────────────────────────────────────────
        |   Coroutines
        ───────────────────────────────────────────────────────────────────────────────────────────────────────────── */
        
        /// <summary>
        /// Runs an update loop with a delay to be more performant.
        /// </summary>
        private IEnumerator Co_FrameDelayedUpdateRoutine()
        {
            while (gameObject.activeInHierarchy)
            {
                for (var i = 0; i < 30; i++)
                    yield return new WaitForEndOfFrame();

                DelayedUpdate();
            }
        }

        
        /// <summary>
        /// Cleans up the clip player when completed.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private IEnumerator Co_CleanupCo(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            RemovalLogic();
        }
    }
}