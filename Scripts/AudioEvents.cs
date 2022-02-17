using System;
using UnityEngine;

/*
 * 
 *  Audio Manager
 *							  
 *	Audio Events
 *      Holds the global events the manager can call.
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

namespace CarterGames.Assets.AudioManager
{
    public class AudioEvents : MonoBehaviour
    {
        /// <summary>
        /// Called when a clip starts playing...
        /// </summary>
        public static event Action OnClipStart;
        
        /// <summary>
        /// Called when a clip finishes playing...
        /// </summary>
        public static event Action OnClipEnd;


        private void OnEnable() => OnClipStart?.Invoke();

        private void OnDisable() => OnClipEnd?.Invoke();
    }
}