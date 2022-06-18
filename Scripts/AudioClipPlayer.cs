/*
 * 
 *  Audio Manager
 *							  
 *	Audio Clip Player Script
 *      A clean up script that disables audio clip instances when the clip has finished playing for reuse.
 *			
 *  Written by:
 *      Jonathan Carter
 *
 *  Published By:
 *      Carter Games
 *      E: hello@carter.games
 *      W: https://www.carter.games
 *		
 *  Version: 2.5.8
 *	Last Updated: 18/06/2022 (d/m/y)										
 * 
 */

using System.Collections;
using UnityEngine;

namespace CarterGames.Assets.AudioManager
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioClipPlayer : MonoBehaviour
    {
        private AudioManager am;
        private AudioSource source;
        

        private void OnEnable()
        {
            source = GetComponentInChildren<AudioSource>(true);
            StartCoroutine(Co_FrameDelayedUpdateRoutine());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
        

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
        
        
        private IEnumerator Co_FrameDelayedUpdateRoutine()
        {
            while (true)
            {
                for (var i = 0; i < 30; i++)
                    yield return new WaitForEndOfFrame();

                DelayedUpdate();
            }
        }


        private void DelayedUpdate()
        {
            if (source.isPlaying) return;
            RemovalLogic();
        }
        
        
        private IEnumerator Co_CleanupCo(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            RemovalLogic();
        }
        
        
        private void RemovalLogic()
        {
            AudioPool.Return(source);
            gameObject.SetActive(false);
        }
    }
}